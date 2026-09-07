# Parking Management

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white)
![EF Core](https://img.shields.io/badge/EF%20Core-9.0-512BD4)
![Tests](https://img.shields.io/badge/tests-51%20passing-brightgreen)
![License](https://img.shields.io/badge/license-MIT-blue)

The domain and application core of a parking-management system, built in C# / .NET 8 to
practice **Domain-Driven Design** and **Clean Architecture** with a thorough automated test suite.

The solution models parking locations and the vehicles parked in them: letting vehicles in
(respecting slot capacity), letting them out, and calculating parking fees. It is organised as
independent, dependency-ordered layers so that the business rules stay isolated from
infrastructure concerns such as the database.

> **Scope note:** This repository contains the layered core (Domain, Application, Infrastructure)
> and its tests. It is a library-and-tests project, not a runnable end-user application — there is
> no UI or hosting entry point. The focus is on clean separation of concerns and test coverage.

## Architecture

The project follows the classic Clean Architecture dependency rule: dependencies point *inwards*,
and the Domain layer knows nothing about the outer layers.

```
┌─────────────────────────────────────────────┐
│  Infrastructure        (EF Core, SQL Server) │  ← depends on Domain
│  ┌───────────────────────────────────────┐  │
│  │  Application   (services, DTOs, mapping)│  │  ← depends on Domain
│  │  ┌─────────────────────────────────┐   │  │
│  │  │  Domain   (entities, rules)     │   │  │  ← depends on nothing
│  │  └─────────────────────────────────┘   │  │
│  └───────────────────────────────────────┘  │
└─────────────────────────────────────────────┘
```

| Layer | Project | Responsibility |
|-------|---------|----------------|
| **Domain** | `ParkingManagement.Domain` | Entities and business rules. `ParkingLocation` (aggregate root) and `Vehicle`, plus the repository interface. No external dependencies. |
| **Application** | `ParkingManagement.Application` | Use-case orchestration via `ParkingLocationService`, DTOs, and entity ↔ DTO mapping. |
| **Infrastructure** | `ParkingManagement.Infrastructure` | EF Core `DbContext` and the repository implementation (SQL Server provider). |

## Domain rules

- A `ParkingLocation` has a fixed number of slots; `LetVehicleIn` throws when the location is full.
- `LetVehicleOut` removes the vehicle and returns the fee for the stay.
- Fees are charged per started hour — any partial hour is rounded **up** (`Math.Ceiling`) and
  multiplied by the location's tariff rate.

## Tech stack

- **.NET 8** / C#
- **Entity Framework Core 9** (SQL Server provider; EF Core InMemory for tests)
- **xUnit** — test framework
- **Moq** — mocking dependencies at the service boundary
- **FluentAssertions** — expressive assertions

## Project structure

```
ParkingManagement/
├── ParkingManagement.Domain/               # Entities, aggregate, repository interface
├── ParkingManagement.Application/          # Services, DTOs, mapping
├── ParkingManagement.Infrastructure/       # EF Core DbContext + repository
├── ParkingManagement.Domain.Tests/         # Domain rule tests
├── ParkingManagement.Application.Tests/    # Service + mapping tests (Moq)
└── ParkingManagement.Infrastructure.Tests/ # Repository tests (EF Core InMemory)
```

## Getting started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Build

```bash
git clone https://github.com/<your-username>/ParkingManagement.git
cd ParkingManagement
dotnet build
```

### Run the tests

```bash
dotnet test
```

All 51 tests should pass. The Infrastructure tests use EF Core's InMemory provider, so **no SQL
Server instance is required** to build or test the project.

## Testing

The suite covers each layer independently:

- **Domain** — capacity limits, fee calculation (exact hours, partial-hour rounding, long stays,
  zero-tariff, varying rates), and vehicle add/remove behaviour.
- **Application** — `ParkingLocationService` orchestration with a mocked repository, and full
  DTO ↔ entity mapping round-trips.
- **Infrastructure** — the repository against an in-memory database, including filtering,
  eager-loading of vehicles, and `OnDelete(SetNull)` behaviour when a location is removed.

## Possible improvements

Ideas for extending the project further:

- Add input validation / value objects (e.g. a structured `Address` and license-plate format checks).
- Expose the application core through an ASP.NET Core Web API or a desktop/web front-end.
- Introduce persistence configuration and EF Core migrations for a real SQL Server database.
- Add CI (GitHub Actions) to build and run the tests on every push.

## License

Released under the [MIT License](LICENSE).
