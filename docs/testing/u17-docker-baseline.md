# Umbraco 17 Docker Test Baseline

Self-contained Docker image that runs an Umbraco 17 site with all 80+ SplatDev plugins installed via local project references.

## Overview

All SplatDev plugins dual-target `net8.0` and `net10.0`. This baseline builds the `net10.0` target and installs every plugin into a fresh Umbraco 17.4.2 site using SQLite, so no external database is required.

The Docker build is a three-stage pipeline:

| Stage | Base Image | Purpose |
|-------|-----------|---------|
| `frontend-build` | `node:22-bookworm-slim` | Builds Vite/Lit frontend clients for 33 plugins |
| `dotnet-build` | `dotnet/sdk:10.0` | Builds all plugins and publishes the Umbraco baseline |
| `runtime` | `dotnet/aspnet:10.0` | Runs the site with Playwright + Chromium for E2E |

## Quick Start

### Build the image

```bash
docker build -f docker/test/u17/Dockerfile -t splatdev-test-u17 .
```

### Run Umbraco

```bash
docker run -p 5000:5000 splatdev-test-u17
```

Access the backoffice at `http://localhost:5000/umbraco`.

### Default credentials

| Field | Value |
|-------|-------|
| Email | `admin@splatdev.local` |
| Password | `SplatDev2024!` |

### Run E2E tests

```bash
docker run \
  -v $(pwd)/docker/test/e2e:/app/e2e:ro \
  -v $(pwd)/output:/output \
  splatdev-test-u17 \
  /app/test-runner.sh
```

This boots Umbraco, waits for it to be ready, runs Playwright tests from `/app/e2e/`, and saves screenshots and videos to `/output/`.

## Architecture

### Plugin references

Since NuGet packages are not yet published (pending SPL-1735), the baseline uses `<ProjectReference>` entries pointing to each plugin's `.csproj` file. The generated `UmbracoBaseline.csproj` contains references to all plugin projects organized by category:

- Core/shared libraries (10 projects)
- Messaging providers (8 projects)
- Payment providers (5 projects)
- Umbraco infrastructure (11 projects)
- Umbraco plugins (68 projects)
- Nested plugins: FormBuilder.Extension, Schema2Yaml, Yaml2Schema
- Tools (4 projects)
- Themes (9 projects)

### Frontend builds

33 plugins have Vite/Lit frontend clients in `client/` directories. The `frontend-build` stage runs `npm ci && npx vite build` for each, outputting compiled assets to `App_Plugins/<name>/dist/`.

### Database

Uses SQLite for self-contained testing. Connection string:

```
Data Source=|DataDirectory|/umbraco_baseline.db;Cache=Shared;Foreign Keys=True;Pooling=True
```

Umbraco runs unattended install on first boot, creating the admin user automatically.

### Excluded projects

| Project | Reason |
|---------|--------|
| `SplatDev.Api.Common.ApiVersioning` | Only targets `net8.0` |
| `SplatDev.Umbraco.Plugins.FormBuilder/FormBuilder` | Reference implementation, targets `net9.0` |
| `SplatDev.Umbraco.Plugins.BackupManager` | Nested git repo, managed separately |
| All `*.Tests` projects | Not needed in runtime baseline |

## Test Runner

`docker/test/u17/scripts/test-runner.sh` automates the full test flow:

1. Starts Umbraco in the background
2. Polls the healthcheck endpoint (max 180s timeout)
3. Runs Playwright E2E tests from `/app/e2e/` (if present)
4. Captures smoke-test screenshots (login page, frontend home)
5. Saves all artifacts to `/output/`

### Output directories

| Path | Content |
|------|---------|
| `/output/screenshots/` | E2E and smoke-test screenshots |
| `/output/videos/` | Playwright video recordings |
| `/output/playwright-results.log` | Full test output |

## Healthcheck

The container includes a Docker HEALTHCHECK that polls `http://localhost:5000/umbraco` every 15 seconds with a 60-second start period.

## Adding E2E Tests

Place Playwright test files in `docker/test/e2e/`. The test runner mounts this directory at `/app/e2e/` and runs `npx playwright test` against the running Umbraco instance.

Example test structure:

```
docker/test/e2e/
  playwright.config.ts
  tests/
    backoffice-login.spec.ts
    plugin-dashboards.spec.ts
```
