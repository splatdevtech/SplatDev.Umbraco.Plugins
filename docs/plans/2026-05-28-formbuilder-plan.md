# FormBuilder — Implementation Plan

**Date:** 2026-05-28
**Issue:** [SPL-1656](/SPL/issues/SPL-1656)

## Phase 1: Foundation ✅

- [x] Design spec document
- [x] NPoco entities (Form, FormField, DropdownValue, Workflow, EmailTemplate, Status)
- [x] FluentMigrator migration (M001 creates 6 tables)
- [x] FormRepository (CRUD + field ordering)
- [x] FormSubmissionValidator
- [x] Workflow engine (EmailWorkflow, SendNotification)

## Phase 2: API ✅

- [x] FormBuilderApiController (CRUD: list, get, create, update, delete, duplicate, field types)
- [x] FormSubmissionController (public submit with CSRF)
- [x] EmailService with IOptions<T> configuration
- [x] 16 xUnit controller tests

## Phase 3: Backoffice UI v17 ✅

- [x] Vite + Lit 3 client setup
- [x] Form list dashboard
- [x] Form editor dashboard
- [x] Welcome dashboard
- [x] umbraco-package.json with section + dashboard extensions

## Phase 4: Documentation ✅

- [x] Design spec (`docs/specs/2026-05-28-formbuilder-design.md`)
- [x] README.md (architecture, API, configuration)
- [x] This plan document

## Phase 5: Remaining

- [ ] v13 AngularJS backoffice (package.manifest, controllers, views)
- [ ] Submission storage entity + migration (FormBuilderSubmissions table)
- [ ] Submission list + export (CSV) endpoints
- [ ] E2E Playwright tests
- [ ] NuGet packaging verification
- [ ] Integration guide

## Phase 6: Polish

- [ ] Multi-language support (en + es)
- [ ] Recaptcha field type implementation
- [ ] File upload field type implementation
- [ ] Conditional field logic (v2)
- [ ] Multi-page forms (v2)
