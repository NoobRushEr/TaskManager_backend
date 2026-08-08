# TaskManager

TaskManager is a full-stack task management application with a .NET 8 Web API, PostgreSQL persistence, JWT authentication, and an Angular 19 frontend.

The backend is organized with a layered architecture:

- `TaskManager.Api`: ASP.NET Core API host, controllers, Swagger, CORS, JWT authentication, and dependency injection.
- `TaskManager.Application`: DTOs, service interfaces, application services, validation, and user-claim helpers.
- `TaskManager.Domain`: core entities and enums.
- `TaskManager.Infrastructure`: Entity Framework Core `DbContext`, repositories, PostgreSQL mappings, JWT token generation, and migrations.
- `TaskManager.Frontend`: Angular client for login, registration, protected task listing, auth guard, JWT interceptor, and API services.

## Features

- User registration and login with BCrypt password hashing.
- JWT bearer authentication with role claims.
- User roles: `User` and `Admin`.
- Task CRUD with owner/admin authorization checks.
- Task status workflow validation.
- Soft delete and admin-only restore for tasks.
- Dashboard/task statistics by status, priority, and category.
- Category and user management endpoints.
- Angular login/register screens and protected task list route.
- Swagger UI in development.

## Tech Stack

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core 8
- PostgreSQL via `Npgsql.EntityFrameworkCore.PostgreSQL`
- FluentValidation
- BCrypt.Net
- JWT bearer authentication
- Swagger / OpenAPI
- Angular 19
- RxJS

## Repository Structure

```text
TaskManager.sln
TaskManager.Api/
TaskManager.Application/
TaskManager.Domain/
TaskManager.Infrastructure/
TaskManager.Frontend/
```

## Domain Model

`User`

- `Id`
- `FirstName`
- `LastName`
- `Email`
- `PasswordHash`
- `Roles`
- `Tasks`
- `Categories`

`TaskItem`

- `Task_Id`
- `Title`
- `Description`
- `CreatedAt`
- `CompletedAt`
- `DueDate`
- `Priority`
- `Status`
- `IsDeleted`
- `DeletedAt`
- `UserId`
- `CategoryId`

`Category`

- `CategoryId`
- `CategoryName`
- `UserId`
- `Tasks`

Enums:

- `Priority_`: `Low`, `Medium`, `High`
- `Status_`: `NotStarted`, `InProgress`, `Completed`, `OnHold`
- `Role_`: `User`, `Admin`

## Prerequisites

- .NET SDK 8.x
- Node.js and npm
- PostgreSQL
- Optional EF Core CLI:

```bash
dotnet tool install --global dotnet-ef
```

## Backend Configuration

The API reads the database connection string from `TaskManager.Api/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=TaskManager;Username=postgres;Password=your_password"
}
```

For local development, use user secrets or environment variables for real credentials instead of committing passwords.

The API development launch profiles are:

- HTTP: `http://localhost:5115`
- HTTPS: `https://localhost:7285`

Swagger is available in development at `/swagger`.

## Database Setup

From the solution root, apply the existing migrations:

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

## Run the Backend

From the solution root:

```bash
dotnet restore
dotnet build
dotnet run --project TaskManager.Api
```

## Run the Frontend

The Angular app expects the API at `http://localhost:5115/api`, configured in:

```text
TaskManager.Frontend/src/environments/environment.ts
```

From the frontend project:

```bash
cd TaskManager.Frontend
npm install
npm start
```

The frontend runs at:

```text
http://localhost:4200
```

The API CORS policy currently allows `http://localhost:4200`.

## API Overview

Base URL:

```text
http://localhost:5115/api
```

### Auth

```text
POST /Auth/register
POST /Auth/login
```

Register request:

```json
{
  "firstName": "Jane",
  "lastName": "Doe",
  "email": "jane@example.com",
  "password": "password123"
}
```

Login request:

```json
{
  "email": "jane@example.com",
  "password": "password123"
}
```

Successful login returns a JWT token and expiration. Send protected requests with:

```text
Authorization: Bearer <token>
```

### Tasks

Most task endpoints require `Admin` or `User`. Admin-only endpoints are marked below.

```text
GET    /Task                         Admin only
GET    /Task/paged?page=1&pageSize=10 Admin only
GET    /Task/{id}                    Admin only
POST   /Task
PUT    /Task/{task_id}
DELETE /Task/{task_id}
GET    /Task/user/{userId}
GET    /Task/tasks?includeDeleted=false
PUT    /Task/update-status
GET    /Task/dashboard
GET    /Task/tasks-count
GET    /Task/tasks-by-status/{status}
GET    /Task/tasks-count-by-category
DELETE /Task/soft-delete/{task_id}
PUT    /Task/restore/{task_id}       Admin only
```

Create task request:

```json
{
  "title": "Finish README",
  "description": "Update project documentation",
  "dueDate": "2026-06-30T00:00:00Z",
  "priority": "High",
  "status": "NotStarted",
  "categoryId": 1
}
```

Task status values:

```text
NotStarted, InProgress, Completed, OnHold
```

Task priority values:

```text
Low, Medium, High
```

### Categories

These endpoints are currently public in the API code.

```text
GET    /Category
GET    /Category/{id}
POST   /Category
PUT    /Category/{id}
DELETE /Category/{id}
GET    /Category/{categoryId}/tasks
```

Create category request:

```json
{
  "categoryName": "Work",
  "userId": 1
}
```

### Users

These endpoints are currently public in the API code.

```text
GET    /User
GET    /User/{id}
PUT    /User/{id}
DELETE /User/{id}
GET    /User/{id}/categories
GET    /User/{user_id}/task/{taskId}
```

## Caching Architecture (Cache-Aside Decorators)

To optimize read performance and enforce separation of concerns, the application implements the **Cache-Aside Pattern** at the Service Layer using the **Decorator Pattern**:

- **Task Caching (`CachingTaskServiceDecorator`)**:
  - Caches task lookups by ID and user task lists.
  - Automatically evicts cached queries when tasks are created, updated, status-modified, deleted, or soft-deleted.
  - Leaves dashboard/analytical queries un-cached for custom extension.
- **Category Caching (`CachingCategoryServiceDecorator`)**:
  - Caches category lookups by ID and the complete category lists.
  - Automatically invalidates category lists and individual entries on write operations.

### Configuration
Caching is powered by ASP.NET Core's built-in `IMemoryCache`. Services are registered in `Program.cs` as follows:
```csharp
builder.Services.AddMemoryCache();

builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<ICategoryService>(provider => 
    new CachingCategoryServiceDecorator(
        provider.GetRequiredService<CategoryService>(),
        provider.GetRequiredService<IMemoryCache>()
    ));

builder.Services.AddScoped<TaskService>();
builder.Services.AddScoped<ITaskService>(provider => 
    new CachingTaskServiceDecorator(
        provider.GetRequiredService<TaskService>(),
        provider.GetRequiredService<IMemoryCache>()
    ));
```

## Background Processing (Soft-Delete Purge)

To maintain database efficiency and clean up storage, the application implements a background cleanup mechanism:

- **Soft-Delete Retention**: Deleted tasks are flagged as `IsDeleted = true` and keep a timestamp of when they were deleted (`DeletedAt`).
- **Purge Worker (`SoftDeletePurgeWorker`)**: 
  - Runs periodically in the background as a hosted `BackgroundService`.
  - Implements the modern, thread-safe, drift-free `.NET` `PeriodicTimer` for scheduled execution intervals (e.g. 24 hours).
  - Automatically queries and permanently purges soft-deleted tasks that have exceeded a **30-day retention period**.
  - Safely wraps operations in custom try-catch scopes to prevent background database connection errors from crashing the main API host.

### Configuration
The background worker is registered as a hosted service in `Program.cs`:
```csharp
builder.Services.AddHostedService<SoftDeletePurgeWorker>();
```

## Frontend Routes

```text
/login
/register
/tasks      Protected by authGuard
```

The frontend stores the JWT token and current user response in `localStorage`. The JWT interceptor attaches the token to API requests.

## Tests

Backend:

```bash
dotnet build
```

Frontend:

```bash
cd TaskManager.Frontend
npm test
```

## Current Notes

- The JWT signing key is hard-coded in the API and infrastructure code. Move it to configuration or user secrets before production use.
- The checked-in `appsettings.json` contains a local PostgreSQL password. Replace it locally and avoid committing real credentials.
- `CategoryController` and `UserController` do not currently require authorization attributes.
- Soft-deleted tasks are filtered globally by EF Core; `GET /Task/tasks?includeDeleted=true` explicitly ignores that filter for the current user's task list.
