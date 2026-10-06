# SPL-4413 FormBuilder click-through QA

**Run date:** 2026-10-06 01:04 UTC
**Environment:** `https://u17-sandbox.splatdev.tech/umbraco`
**Host tested:** `u17-sandbox.splatdev.tech` (the required Umbraco 17 sandbox, not the corporate site)
**Result:** Blocked at QA-0 authentication gate; no plugin pass/fail claim made.

## Reconnaissance

A direct request to the exact backoffice URL returned HTTP 200 with the Umbraco backoffice shell:

- URL: `https://u17-sandbox.splatdev.tech/umbraco`
- Response content type: `text/html; charset=utf-8`
- Observed title: `Umbraco`
- Observed shell marker: `<base href="/umbraco/">`

The returned page was unauthenticated and did not expose a FormBuilder section, route, or plugin UI. Per the issue evidence rules, this generic shell is disqualified as feature evidence.

## Required artefacts / stop reason

1. **Installed and visible:** Not verifiable. No authenticated backoffice session was available, so the FormBuilder section could not be inspected.
2. **Primary workflow and persistence:** Not run. No plugin route or UI could be reached.
3. **Error path:** Not run. No plugin form could be reached.
4. **Browser/plugin evidence:** No qualifying plugin screenshot exists. A screenshot of the generic unauthenticated shell would be misleading and is intentionally not attached.

## Clear condition

Provide an authenticated backoffice session for the Umbraco 17 sandbox (SPL-4409 / QA-0 access gate), then rerun the click-through at the exact host above and capture the required plugin-specific URL, observed success/error strings, and screenshots.
