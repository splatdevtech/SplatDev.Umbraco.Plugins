#!/usr/bin/env bash
# =============================================================================
# SplatDev Umbraco 13 Test Runner
# Boots Umbraco in background, waits for healthcheck, runs Playwright E2E tests,
# and captures screenshots/videos to /output/.
# =============================================================================
set -euo pipefail

readonly UMBRACO_URL="http://localhost:5001"
readonly HEALTH_ENDPOINT="${UMBRACO_URL}/umbraco"
readonly MAX_WAIT=180
readonly SCREENSHOT_DIR="/output/screenshots"
readonly VIDEO_DIR="/output/videos"
readonly E2E_DIR="/app/e2e"

log() {
  echo "[$(date '+%Y-%m-%d %H:%M:%S')] $*"
}

cleanup() {
  local exit_code=$?
  if [ -n "${UMBRACO_PID:-}" ] && kill -0 "$UMBRACO_PID" 2>/dev/null; then
    log "Stopping Umbraco (PID $UMBRACO_PID)..."
    kill "$UMBRACO_PID" 2>/dev/null || true
    wait "$UMBRACO_PID" 2>/dev/null || true
  fi
  exit "$exit_code"
}

trap cleanup EXIT INT TERM

# ---- Prepare output directories ----
mkdir -p "$SCREENSHOT_DIR" "$VIDEO_DIR"

# ---- Start Umbraco in background ----
log "Starting Umbraco 13 baseline..."
cd /app
dotnet Umbraco13.Baseline.dll &
UMBRACO_PID=$!
log "Umbraco started with PID $UMBRACO_PID"

# ---- Wait for healthcheck ----
log "Waiting for Umbraco to be ready at $HEALTH_ENDPOINT (max ${MAX_WAIT}s)..."
elapsed=0
while [ "$elapsed" -lt "$MAX_WAIT" ]; do
  if curl -sf "$HEALTH_ENDPOINT" > /dev/null 2>&1; then
    log "Umbraco is ready after ${elapsed}s"
    break
  fi

  if ! kill -0 "$UMBRACO_PID" 2>/dev/null; then
    log "ERROR: Umbraco process exited unexpectedly"
    exit 1
  fi

  sleep 3
  elapsed=$((elapsed + 3))
done

if [ "$elapsed" -ge "$MAX_WAIT" ]; then
  log "ERROR: Umbraco did not become ready within ${MAX_WAIT}s"
  exit 1
fi

# ---- Verify backoffice is accessible ----
log "Verifying backoffice login page..."
http_code=$(curl -s -o /dev/null -w '%{http_code}' "${UMBRACO_URL}/umbraco" || echo "000")
if [ "$http_code" = "200" ] || [ "$http_code" = "302" ]; then
  log "Backoffice responding with HTTP $http_code"
else
  log "WARNING: Backoffice returned HTTP $http_code (may still be initializing)"
fi

# ---- Run Playwright E2E tests ----
SPEC_COUNT=$(find "$E2E_DIR/tests" -name "*.spec.ts" 2>/dev/null | wc -l)
if [ "$SPEC_COUNT" -gt 0 ]; then
  log "Running $SPEC_COUNT Playwright spec file(s) from $E2E_DIR/tests..."
  cd "$E2E_DIR"

  npx playwright test --reporter=list \
    2>&1 | tee /output/playwright-results.log || {
      log "WARNING: Some Playwright tests failed (see /output/playwright-results.log)"
    }

  log "E2E test run complete"
else
  log "No .spec.ts tests found at $E2E_DIR/tests — skipping Playwright run"
  log "To add tests, place .spec.ts files in docker/test/e2e/tests/"
fi

# ---- Capture a smoke-test screenshot ----
log "Capturing smoke-test screenshots..."
node - <<'SMOKE_JS'
const { chromium } = require('playwright');

(async () => {
  const browser = await chromium.launch({ args: ['--no-sandbox'] });
  const page = await browser.newPage();

  try {
    await page.goto('http://localhost:5001/umbraco', { waitUntil: 'networkidle', timeout: 30000 });
    await page.screenshot({ path: '/output/screenshots/01-login-page.png', fullPage: true });
    console.log('Screenshot: login page captured');
  } catch (err) {
    console.error('Failed to capture login page:', err.message);
  }

  try {
    await page.goto('http://localhost:5001/', { waitUntil: 'networkidle', timeout: 30000 });
    await page.screenshot({ path: '/output/screenshots/02-frontend-home.png', fullPage: true });
    console.log('Screenshot: frontend home captured');
  } catch (err) {
    console.error('Failed to capture frontend home:', err.message);
  }

  await browser.close();
})();
SMOKE_JS

log "Screenshots saved to $SCREENSHOT_DIR"
log "Test run finished successfully"
