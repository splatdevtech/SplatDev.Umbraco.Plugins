# Evidence Capture Infrastructure

Evidence capture for SplatDev Umbraco plugin testing uses Playwright + Chromium running inside Ubuntu-based SDK Docker images (`mcr.microsoft.com/dotnet/sdk:10.0` and `8.0` — Ubuntu Noble with glibc). This resolves the Alpine glibc incompatibility with Chromium.

## Directory Layout

```
docker/test/
├── e2e/                          # Shared Playwright suite (baked into both images)
│   ├── playwright.config.ts      # Central config: screenshot=on, video=on
│   ├── package.json              # @playwright/test dependency
│   ├── tests/                    # .spec.ts test files (add per-plugin tests here)
│   └── scripts/
│       ├── capture-screenshots.mjs   # Batch screenshot capture (all 60+ plugins)
│       └── install-screenshots.mjs  # Copies screenshots into each plugin's README
│
├── u17/                          # Umbraco 17 (net10.0, port 5000)
│   ├── Dockerfile
│   └── scripts/
│       └── test-runner.sh        # Boot + wait + run + smoke screenshots
│
├── u13/                          # Umbraco 13 (net8.0, port 5001)
│   ├── Dockerfile
│   └── scripts/
│       └── test-runner.sh
│
└── output/                       # Bind-mount point for CI evidence (gitkeep only)
    ├── screenshots/              # Smoke + capture-screenshots.mjs output
    ├── videos/                   # Retained for manual use (Playwright uses test-results/)
    └── test-results/             # Playwright per-test artifacts (screenshot + video)
```

## Output Structure (at Runtime)

When `test-runner.sh` executes inside the container, artifacts land under `/output/`:

```
/output/
├── screenshots/
│   ├── 01-login-page.png                   # Smoke: Umbraco login screen
│   ├── 02-frontend-home.png                # Smoke: frontend homepage
│   ├── {plugin}-backoffice.png             # From capture-screenshots.mjs
│   ├── {plugin}-frontend.png               # From capture-screenshots.mjs
│   └── capture-manifest.json               # Capture results summary
│
├── test-results/                            # Playwright per-test artifacts
│   └── {test-title}-{hash}/
│       ├── test-failed-1.png               # Auto screenshot on failure
│       ├── video.webm                       # Full-test video recording
│       └── trace.zip                        # On first retry (trace: on-first-retry)
│
├── playwright-report/                       # HTML report (npx playwright show-report)
├── playwright-results.json                  # Machine-readable results
└── playwright-results.log                   # Raw test-runner.sh stdout
```

## Playwright Configuration

`docker/test/e2e/playwright.config.ts` controls evidence capture for all `.spec.ts` tests:

| Setting | Value | Effect |
|---------|-------|--------|
| `screenshot` | `'on'` | Screenshot every test pass AND fail |
| `video` | `'on'` | Record full video for every test |
| `trace` | `'on-first-retry'` | Captures trace on flaky tests |
| `outputDir` | `/output/test-results` | Per-test artifacts directory |
| `reporter` | html + list + json | HTML report + machine-readable JSON |

## Environment Variables

Both test-runner.sh scripts honour these env vars:

| Variable | Default | Description |
|----------|---------|-------------|
| `UMBRACO_URL` | `http://localhost:5000` (U17) or `:5001` (U13) | Base URL for Playwright and healthcheck |
| `PLAYWRIGHT_OUTPUT_DIR` | `/output/test-results` | Override Playwright artifact root |

Override at container start:
```bash
docker run -e UMBRACO_URL=http://host.docker.internal:5000 ...
```

## Running the Test Containers

### Umbraco 17 (net10.0)

```bash
# Build
docker build -f docker/test/u17/Dockerfile -t splatdev-umbraco-u17-test .

# Run: boot Umbraco + smoke screenshots
docker run --rm \
  -v $(pwd)/evidence/u17:/output \
  splatdev-umbraco-u17-test \
  /app/test-runner.sh

# Run: batch screenshot capture (all plugins)
docker run --rm \
  -v $(pwd)/evidence/u17:/output \
  splatdev-umbraco-u17-test \
  bash -c "dotnet UmbracoBaseline.dll & sleep 90 && node /app/e2e/scripts/capture-screenshots.mjs http://localhost:5000 /output/screenshots"
```

### Umbraco 13 (net8.0)

```bash
# Build
docker build -f docker/test/u13/Dockerfile -t splatdev-umbraco-u13-test .

# Run: boot Umbraco + smoke screenshots
docker run --rm \
  -v $(pwd)/evidence/u13:/output \
  splatdev-umbraco-u13-test \
  /app/test-runner.sh
```

## Adding Plugin Tests

Place `.spec.ts` files in `docker/test/e2e/tests/`. They are automatically COPY'd into the image and picked up by the test-runner.

```typescript
// docker/test/e2e/tests/backups.spec.ts
import { test, expect } from '@playwright/test';

test('Backups dashboard loads', async ({ page }) => {
  await page.goto('/umbraco');
  // login ...
  await page.goto('/umbraco#/backups');
  await expect(page.locator('.backups-dashboard')).toBeVisible();
  // Playwright auto-screenshots this on pass (screenshot: 'on')
});
```

The test title becomes the output directory name:  
`/output/test-results/backups-dashboard-loads-{hash}/`

## Collecting Evidence for SPL Issues

After a test run, copy `/output/` off the container:
```bash
docker cp <container-id>:/output ./evidence/$(date +%Y%m%d-%H%M%S)
```

Or use a bind mount (`-v`) as shown above. Attach the `screenshots/` and `test-results/` contents to the relevant SPL issue.

## Why Ubuntu SDK Images (Not Alpine)

Playwright's Chromium requires glibc. Alpine uses musl libc which is incompatible. The `mcr.microsoft.com/dotnet/sdk:10.0` and `sdk:8.0` images are based on Ubuntu Noble (24.04) and have glibc, so Chromium installs and runs without emulation layers.
