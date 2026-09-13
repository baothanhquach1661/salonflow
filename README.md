# SalonFlow

SalonFlow is a cloud-native appointment scheduling and waitlist management system for salons.

## Overview

SalonFlow is designed to help salons manage scheduled appointments and walk-in customers through one centralized system.

The project begins as a modular monolith so that the business logic remains easy to understand, test, deploy, and maintain. It is also built as a production-minded backend and DevOps case study using ASP.NET Core and Microsoft Azure.

## Current Status

SalonFlow is currently in the initial project setup phase.

* [x] Public GitHub repository created
* [x] .NET 10 SDK pinned with `global.json`
* [x] Modular solution structure created
* [x] Unit and integration test projects created
* [x] Central build quality rules configured
* [x] Local HTTPS API startup verified
* [ ] First business feature implemented
* [ ] Continuous integration workflow configured
* [ ] Azure infrastructure deployed

## Architecture

SalonFlow uses a modular monolith with Clean Architecture principles.

| Project                      | Responsibility                                                                                                                |
| ---------------------------- | ----------------------------------------------------------------------------------------------------------------------------- |
| `SalonFlow.Domain`           | Contains business entities, value objects, and core business rules. It has no dependency on infrastructure or web frameworks. |
| `SalonFlow.Application`      | Contains application use cases, DTOs, interfaces, and orchestration logic. It depends on Domain.                              |
| `SalonFlow.Infrastructure`   | Implements database access and external service integrations such as EF Core, Azure SQL, and Azure Service Bus.               |
| `SalonFlow.Api`              | Hosts the ASP.NET Core Web API, controllers, middleware, dependency injection, and application startup.                       |
| `SalonFlow.UnitTests`        | Tests business rules and application logic without external infrastructure.                                                   |
| `SalonFlow.IntegrationTests` | Tests the API, database integration, and other infrastructure components working together.                                    |

Dependencies point inward toward the business layer. The Domain project does not depend on the API, database, Azure SDKs, or other infrastructure.

## Repository Structure

| Path                    | Purpose                                                         |
| ----------------------- | --------------------------------------------------------------- |
| `src/`                  | Production application source code                              |
| `tests/`                | Automated unit and integration tests                            |
| `infra/`                | Azure Bicep templates and infrastructure configuration          |
| `docs/`                 | Architecture decisions, diagrams, and operational documentation |
| `SalonFlow.slnx`        | .NET solution containing all application and test projects      |
| `Directory.Build.props` | Shared build and code-quality configuration                     |
| `global.json`           | Required .NET SDK version and roll-forward policy               |

## Technology Roadmap

| Area                   | Technology                             | Status             |
| ---------------------- | -------------------------------------- | ------------------ |
| Backend                | .NET 10 and ASP.NET Core Web API       | Foundation created |
| API style              | Controller-based REST API and OpenAPI  | Foundation created |
| Data access            | Entity Framework Core                  | Planned            |
| Database               | Azure SQL                              | Planned            |
| Testing                | xUnit unit and integration tests       | Projects created   |
| Containers             | Docker                                 | Planned            |
| Hosting                | Azure Container Apps                   | Planned            |
| Infrastructure as Code | Bicep                                  | Planned            |
| Messaging              | Azure Service Bus                      | Planned            |
| CI/CD                  | GitHub Actions with Azure OIDC         | Planned            |
| Observability          | OpenTelemetry and Application Insights | Planned            |

## Prerequisites

Install the following tools before working with SalonFlow:

* Git
* .NET SDK 10.0.400 or a compatible 10.0.4xx patch

Docker and Azure tooling will be required in later development phases.

## Local Development

Clone the repository:

```bash
git clone https://github.com/baothanhquach1661/salonflow.git
cd salonflow
```

Restore dependencies:

```bash
dotnet restore SalonFlow.slnx
```

Build the solution:

```bash
dotnet build SalonFlow.slnx --no-restore
```

Run all tests:

```bash
dotnet test SalonFlow.slnx --no-build
```

The test projects are currently scaffolded. Real tests will be added together with the first business features.

Run the API locally:

```bash
dotnet run --project src/SalonFlow.Api/SalonFlow.Api.csproj --launch-profile https
```

Use the HTTPS address printed in the terminal to access the local API.

## Roadmap

1. Model the core salon appointment and waitlist domain.
2. Implement appointment and walk-in queue use cases.
3. Add EF Core persistence and local database development.
4. Connect the application to Azure SQL.
5. Add meaningful unit and integration tests.
6. Containerize the API with Docker.
7. Define Azure infrastructure using Bicep.
8. Deploy the API to Azure Container Apps.
9. Add asynchronous messaging with Azure Service Bus.
10. Build CI/CD workflows using GitHub Actions and OIDC.
11. Add distributed tracing, metrics, logging, and Application Insights.
