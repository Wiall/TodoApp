# TodoApp

A full-stack To-Do application inspired by Microsoft To Do.

## Features

* User registration and login
* JWT authentication with refresh tokens
* Create, view, edit and delete tasks
* Create, view, edit and delete categories
* Assign categories to tasks
* Search and filter tasks
* Task pagination
* Angular frontend

## Technologies

### Backend

* .NET 8
* ASP.NET Core Web API
* Entity Framework Core
* ASP.NET Core Identity
* Microsoft SQL Server
* JWT
* FluentValidation

### Frontend

* Angular
* TypeScript
* Bootstrap

## Architecture

The backend is organized into the following layers:

* **API** - controllers and HTTP endpoints
* **BLL** - business logic, services, DTOs and validation
* **Domain** - entities and repository contracts (interfaces layer)
* **DAL** - Entity Framework Core, repositories and database configuration

## Project Structure

```text
TodoApp/
├── TodoApp.API/
├── TodoApp.BLL/
├── TodoApp.DAL/
├── TodoApp.Domain/
├── todo-app-ui/
├── TodoApp.sln
├── README.md
└── .gitignore
```

## Getting Started

### Backend

1. Configure the SQL Server connection string in `appsettings.json`.
2. Configure the JWT settings and provide the JWT key through User Secrets or environment variables.
3. Apply database migrations:

```bash
dotnet ef database update --project TodoApp.DAL --startup-project TodoApp.API
```

4. Run the API:

```bash
dotnet run --project TodoApp.API
```

### Frontend

Navigate to the frontend directory:

```bash
cd todo-app-ui
```

Install dependencies:

```bash
npm install
```

Run the development server:

```bash
ng serve
```
