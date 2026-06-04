#!/usr/bin/env bash
# =============================================================================
# Playwright screenshot runner for SplatDev Umbraco plugin evidence capture.
#
# Waits for both Umbraco 13 (port 5001) and Umbraco 17 (port 5000) to be
# healthy on the Docker network, then captures per-plugin dashboard screenshots
# via the shared capture-plugin-evidence.mjs script.
#
# Environment variables:
#   U13_URL              - Umbraco 13 base URL (default: http://umbraco13:5001)
#   U17_URL              - Umbraco 17 base URL (default: http://umbraco17:5000)
#   UMBRACO_ADMIN_USER   - Backoffice admin email (default: admin@splatdev.com)
#   UMBRACO_ADMIN_PASS   - Backoffice admin password (default: SplatDev123!)
#   SKIP_U13             - Set to "1" to skip Umbraco 13 pass
#   SKIP_U17             - Set to "1" to skip Umbraco 17 pass
#   MAX_WAIT             - Max seconds to wait for each Umbraco (default: 300)
# =============================================================================
set -euo pipefail

U13_URL="${U13_URL:-http://umbraco13:5001}"
U17_URL="${U17_URL:-http://umbraco17:5000}"
UMBRACO_ADMIN_USER="${UMBRACO_ADMIN_USER:-admin@splatdev.com}"
UMBRACO_ADMIN_PASS="${UMBRACO_ADMIN_PASS:-SplatDev123!}"
SKIP_U13="${SKIP_U13:-0}"
SKIP_U17="${SKIP_U17:-0}"
MAX_WAIT="${MAX_WAIT:-300}"

E2E_SCRIPTS="/app/e2e/scripts"
OUTPUT_BASE="/output/screenshots"

log() {
  echo "[$(date '+%Y-%m-%d %H:%M:%S')] $*"
}

wait_for_umbraco() {
  local name="$1"
  local url="$2"
  local health_path="${3:-/umbraco}"
  local elapsed=0

  log "Waiting for $name to be healthy at ${url}${health_path} (max ${MAX_WAIT}s)..."
  while [ "$elapsed" -lt "$MAX_WAIT" ]; do
    if curl -sf --max-time 5 "${url}${health_path}" > /dev/null 2>&1; then
      log "$name is ready after ${elapsed}s"
      return 0
    fi
    sleep 5
    elapsed=$((elapsed + 5))
  done

  log "ERROR: $name did not become healthy within ${MAX_WAIT}s"
  return 1
}

capture_pass() {
  local label="$1"   # u13 or u17
  local url="$2"
  local out_dir="${OUTPUT_BASE}/${label}"

  log "--- Starting ${label} screenshot pass ---"
  log "Base URL: $url"
  log "Output:   $out_dir"

  mkdir -p "$out_dir"

  UMBRACO_ADMIN_USER="$UMBRACO_ADMIN_USER" \
  UMBRACO_ADMIN_PASS="$UMBRACO_ADMIN_PASS" \
  node "${E2E_SCRIPTS}/capture-plugin-evidence.mjs" \
    "$url" \
    "$out_dir" \
    "$label" \
    && log "--- ${label} pass complete ---" \
    || { log "WARNING: ${label} pass finished with errors (partial screenshots may exist)"; }
}

# ---- Wait for services ----
[ "$SKIP_U13" = "1" ] || wait_for_umbraco "Umbraco 13" "$U13_URL"
[ "$SKIP_U17" = "1" ] || wait_for_umbraco "Umbraco 17" "$U17_URL"

# ---- Extra buffer for SPA initialization ----
log "Giving Umbraco instances 10s extra for SPA initialization..."
sleep 10

# ---- Capture screenshots ----
[ "$SKIP_U13" = "1" ] || capture_pass "u13" "$U13_URL"
[ "$SKIP_U17" = "1" ] || capture_pass "u17" "$U17_URL"

log "=== Screenshot capture complete. Output: $OUTPUT_BASE ==="
