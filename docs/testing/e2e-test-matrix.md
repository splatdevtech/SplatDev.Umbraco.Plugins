# E2E Test Matrix for Umbraco Baselines

## Overview
This document contains the results of running Playwright E2E tests against the U13 and U17 Docker baselines for the SplatDev Umbraco plugins.

## Test Plan
1. Build the U13 baseline Docker image (`splatdev-test-u13`)
2. Build the U17 baseline Docker image (`splatdev-test-u17`)
3. Run a smoke test suite against each baseline:
   - Verify the Umbraco login page loads
   - Verify login with default credentials works
   - Verify access to the Umbraco dashboard
4. Document pass/fail results for each baseline.

## Baselines
- **U13 Baseline**: Umbraco 13 with .NET 8.0
- **U17 Baseline**: Umbraco 17 with .NET 10.0

## Test Results

### U13 Baseline (Umbraco 13)
| Test Case | Status | Notes |
|-----------|--------|-------|
| Build Docker Image | ❌ Failed | Build process was taking too long (>2 hours) and was cancelled. Need to optimize Dockerfile or increase allocated time. |
| Smoke Test - Login Page Loads | ⏭️ Skipped | Dependent on successful image build |
| Smoke Test - Login with Default Credentials | ⏭️ Skipped | Dependent on successful image build |
| Smoke Test - Dashboard Access | ⏭️ Skipped | Dependent on successful image build |

### U17 Baseline (Umbraco 17)
| Test Case | Status | Notes |
|-----------|--------|-------|
| Build Docker Image | ❌ Failed | Build process was taking too long (>2 hours) and was cancelled. Need to optimize Dockerfile or increase allocated time. |
| Smoke Test - Login Page Loads | ⏭️ Skipped | Dependent on successful image build |
| Smoke Test - Login with Default Credentials | ⏭️ Skipped | Dependent on successful image build |
| Smoke Test - Dashboard Access | ⏭️ Skipped | Dependent on successful image build |

## Next Steps
1. Optimize Docker build process:
   - Consider using Docker BuildKit for faster builds
   - Review and optimize the frontend build process in Dockerfile
   - Consider caching strategies for npm and NuGet packages
2. Allocate more time for builds in CI/CD pipeline
3. Consider creating a simpler test baseline for faster iteration
4. Once images can be built successfully, execute the smoke test suite

## Conclusion
The E2E test matrix could not be completed due to Docker build timeouts. The build process for both U13 and U17 baselines was taking excessively long (over 2 hours) and had to be cancelled. Optimization of the build process is required before E2E testing can proceed.

## References
- [U17 Docker Baseline Documentation](u17-docker-baseline.md)
- [Issue SPL-1842-C: E2E test matrix](https://paperclip.ing/801b4864-f9fa-47ba-8379-908dbdf45c8e/issues/SPL-1856)
