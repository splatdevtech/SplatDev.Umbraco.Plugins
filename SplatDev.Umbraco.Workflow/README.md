# SplatDev.Umbraco.Workflow

> **Status: In Development**

A drop-in workflow engine for Umbraco CMS providing state machines, transitions, history tracking, assignments, and a backoffice management UI.

## Planned Architecture

| Sub-project | Description |
|---|---|
| `Core` | Workflow engine contracts and state machine logic |
| `Persistence` | NPoco-backed repositories for workflow state |
| `Api` | Umbraco API controllers for workflow operations |
| `Backoffice` | AngularJS backoffice dashboard (Umbraco 13) |
| `Backoffice.V17` | Lit-based backoffice dashboard (Umbraco 17) |
| `Themes` | CSS tokens and styling |

## Planned Features

- Configurable state machine with custom states and transitions
- Workflow history and audit trail
- User/role-based task assignments
- Pizza-delivery style progress charts
- Queue management UI
- Configuration editor dashboard

## Target

- Umbraco 13 / .NET 8 (initial release)
- Umbraco 17 / .NET 10 (planned)

## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
