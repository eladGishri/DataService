# DataService
A .NET Web API that provides a data retrieval service while using caching, file storage, and a database.  
The service follows a layered architecture with good design patterns, security mechanisms, and role-based authorization.

## Features
- **Layered architecture**: Separation of concerns between controllers, services, infrastructure, and DTOs.  
- **JWT Authentication & Authorization**: Secure endpoints with role-based access (Admin/User).  
- **Storage abstraction**: Data can be stored in Cache, File Storage, or Database.  
- **Factory Design Pattern**: The storage provider is selected dynamically using a factory.  
- **Entity Framework Core**: Database persistence with SQL Server.  
- **Swagger UI**: Interactive API documentation with JWT support.  
- **Caching**: Speeds up data retrieval.  

## Project Structure
```
DataService.Api/             # Web API project (controllers, startup, etc.)
DataService.Application/     # Application services, DTOs, mapping
DataService.Infrastructure/  # EF Core, repositories, storage providers
DataService.Models/          # Domain entities
```

## Setup

1. Clone the repository and open the solution in Visual Studio or JetBrains Rider.
2. Update the database connection string in **appsettings.json** under `"ConnectionStrings": { "DbConnectionString": "..." }`.
3. Configure the file storage base directory in **appsettings.json**:
   ```json
   "FileStorage": {
     "BaseDirectory": "C:\\Temp\\DataServiceFileStorage"
   }
   ```
   > ⚠️ You must change `BaseDirectory` to a folder that exists on your local machine.
4. Build the solution (`dotnet build`).
5. Run the application (`dotnet run` or launch from Visual Studio).
6. Open Swagger UI at `http://localhost:<port>/swagger`.

## Authentication & Authorization

The API uses **JWT authentication**:  

- **Login endpoint**: `POST /auth/login`  
  - Example request body:  
    ```json
    {
      "username": "admin",
      "password": "admin123"
    }
    ```
  - Response: `{ "token": "<JWT_TOKEN>" }`

- **Roles**:
  - `Admin`: Can insert and update data (`POST /data`, `PUT /data/{id}`)
  - `User`: Can only fetch data (`GET /data/{id}`)

Add the token to Postman or Swagger:  
```
Authorization: Bearer <JWT_TOKEN>
```

## Example Workflow

1. Login as **Admin** → Get JWT token  
2. Use token to `POST /data` (insert new record)  
3. Copy the generated GUID id  
4. `GET /data/{id}` with Admin or User role  
5. Update record with `PUT /data/{id}` (Admin only)  

## Postman Testing

A **Postman collection** and environment file are included in the repository (`/postman`).  

- Import them into Postman.  
- Login first to retrieve the JWT.  
- Environment variables (`{{baseUrl}}`, `{{token}}`) are already set up.  
- You can **run the entire collection** to automatically test authentication, data insert, retrieval, and update.  

## Design Patterns Used

- **Factory Pattern**: Used to select the correct storage provider (`CacheStorageProvider`, `FileStorageProvider`, `DatabaseStorageProvider`).  
- **Repository Pattern**: Abstracts database access.  
- **DTOs and AutoMapper**: Decouple entities from API contracts.  

## Security

- JWT Bearer Authentication with role-based policies  
- HTTPS redirection (enabled in production)  
- CORS policy with explicit allowed origins  

## Requirements

- .NET 8 SDK  
- SQL Server (for database provider)  
- Postman (for testing)  

