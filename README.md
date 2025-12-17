GrowFlow_Phoenix

Phoenix is a .NET 9 Web API designed as a tenant-scoped interface for GrowFlow customers. It provides CRUD operations for Employees, Customers, and Orders, and can synchronize with a mock regulatory traceability API, Leviathan.

Features

CRUD API for Employees (core) and Customers / Orders (bonus).

Background service to sync Leviathan data periodically.

Tenant-scoped: each Phoenix instance serves a single GrowFlow business.

SQLite persistence for local snapshot of Leviathan data.

Swagger/OpenAPI documentation auto-generated for testing.

Getting Started
Prerequisites

.NET 9 SDK

SQLite
 (included via EF Core)

Postman or browser for testing API endpoints

Running the API

Clone the repository:

git clone https://github.com/SvilenPavlov/GrowFlow_Phoenix.git
cd GrowFlow_Phoenix


Run the API:

dotnet run


The API will start at https://localhost:7105 (check console for exact port).

Swagger UI available at https://localhost:7105/swagger for interactive testing.

The database phoenix.db will be auto-created on startup if it doesn’t exist.

Configuration

Leviathan API credentials are stored in appsettings.json:

"LeviathanCreds": {
  "ApiUser": "CHALLENGEUSER",
  "ApiKey": "CHALLENGEKEY"
}


Background sync interval: every 5 minutes (configurable in LeviathanSyncService).

Project Structure

Controllers/ – API endpoints for Employees, Customers, Orders

Data/ – EF Core DbContext and models

Services/ – Business logic and Leviathan client integration

Program.cs – DI registration, DB setup, Swagger configuration

Notes

Currently supports snapshotting Leviathan data locally to allow tenant-scoped views.

Designed to handle potential Leviathan inconsistencies, including missing fields or delayed updates.

SQLite is used for simplicity; can be switched to SQL Server for production.
