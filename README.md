# TaskManager

TaskManager is a .NET 8 Web API solution organized with a clean layered structure:

- `TaskManager.Api`: API host (ASP.NET Core, Swagger, DI setup)
- `TaskManager.Application`: application/service layer (currently scaffolded)
- `TaskManager.Domain`: domain entities and enums
- `TaskManager.Infrastructure`: EF Core, PostgreSQL provider, DbContext, migrations

## Tech Stack

- .NET 8
- ASP.NET Core Web API (Minimal hosting model)
- Entity Framework Core 8
- PostgreSQL (`Npgsql.EntityFrameworkCore.PostgreSQL`)
- Swagger / OpenAPI

## Solution Structure

```text
TaskManager.sln
TaskManager.Api/
TaskManager.Application/
TaskManager.Domain/
TaskManager.Infrastructure/
```

## Domain Model (Current)

- `User`
  - `Id`, `FirstName`, `LastName`, `Email`, `PasswordHash`, `Role` (list of `Role_`)
- `Task`
  - `Task_Id`, `Title`, `Description`, `CreatedAt`, `CompletedAt`, `DueDate`, `Priority`, `Status`, `UserId`, `CategoryId`
- `Category`
  - `Category_Id`, `CategoryName`, `UserId`

Enums:
- `Priority_`: `Low`, `Medium`, `High`
- `Status_`: `NotStarted`, `InProgress`, `Completed`, `OnHold`
- `Role_`: `User`, `Admin`

## Prerequisites

- .NET SDK 8.x
- PostgreSQL running locally or remotely
- Optional (for migrations): EF Core CLI tools

Install EF CLI tools once:

```bash
dotnet tool install --global dotnet-ef
```

## Configuration

Connection string is read from:

- `TaskManager.Api/appsettings.json`
- key: `ConnectionStrings:DefaultConnection`

Example format:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=TaskManager;Username=postgres;Password=your_password"
}
```

For local development, prefer environment variables or user-secrets instead of storing credentials directly in tracked files.

## Restore and Build

From solution root:

```bash
dotnet restore
dotnet build
```

## Database Migrations

A migration already exists in `TaskManager.Infrastructure/Migrations` (`InitialCreate`).

Apply migrations to database from solution root:

```bash
dotnet ef database update \
  --project TaskManager.Infrastructure \
  --startup-project TaskManager.Api
```

Create a new migration:

```bash
dotnet ef migrations add <MigrationName> \
  --project TaskManager.Infrastructure \
  --startup-project TaskManager.Api
```

## Run the API

From solution root:

```bash
dotnet run --project TaskManager.Api
```

Swagger UI is available in development mode at:

- `https://localhost:<port>/swagger`

## Current API Status

Infrastructure and database wiring are configured in `Program.cs`, but endpoint mappings are not yet added. The next step is to define feature endpoints (for example: auth, users, tasks, categories).
