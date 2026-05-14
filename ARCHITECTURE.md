# Architecture Guiding Principles

- Azure-first design: prefer Platform as a Service (PaaS) components (App Service, Functions, Storage, SQL Azure, Redis) over IaaS VMs where feasible.
- Data & integration: centralize data strategy (SQL Server / Azure SQL, distributed cache, messaging with Service Bus or Event Grid).
- API design: consistent REST API patterns, versioning, OpenAPI docs, and clear contract boundaries via clean architecture.
- Asynchrony & scalability: favor async patterns, event-driven messaging, and bounded contexts to improve throughput and resilience.
- Infrastructure as Code: manage infrastructure with Bicep (or Terraform) and keep IaC in an infra/ folder with environment-scoped modules.
- Environment separation: dev, staging, prod with separate configurations and secrets managed via Key Vault and managed identities.
- CI/CD: automated builds, tests, and deployments using GitHub Actions or Azure DevOps, with clear rollback strategies.
- Security & compliance: integrate Azure AD/OAuth/OpenID Connect, secret management via Key Vault, and secure by default configurations.
- Observability: Instrument services with Application Insights, structured logging, tracing, and health checks.
- Governance: align with business goals, avoid premature microservices, and focus on maintainable, observable systems.

This document should be used as a baseline for future architecture decisions and updates.

API Versioning: Centralized pattern
- We will introduce a shared ApiVersioningExtensions library (SplatDev.Api.Common.ApiVersioning) to standardize AddSplatApiVersioning across ASP.NET Core Web APIs.
- This reduces duplication, ensures consistent behavior, and simplifies future changes to API surface and explorer integration.
