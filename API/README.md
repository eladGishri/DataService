# DataService
A .NET Web API that provides a data retrieval service while using caching, file storage, and a database. 
The service follows a layered architecture with good design patterns and security mechanisms.

## Features
- **JWT Authentication** with role-based authorization (`Admin`, `User`).
- **Factory Design Pattern** for dynamically choosing the storage provider (Cache, File, Database).
- **Entity Framework Core** for database interactions.
- **AutoMapper** for mapping between DTOs and entities.
- **Swagger/OpenAPI** documentation with JWT authentication support.
- **CORS Policy** to allow cross-origin requests from specific clients.

## Architecture
The project follows a layered architecture:
- **Controllers**: Handle HTTP requests and responses.
- **Services**: Contain the business logic (`DataService`).
- **Storage Providers**: Implement different storage strategies (Cache, File, Database).
- **Factory**: Decides which storage provider to use at runtime.
- **Infrastructure**: Database context, repositories, and storage implementations.

## Setup

1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/DataService.git
   cd DataService
   ```

2. Configure the database connection in **`appsettings.json`** under `"ConnectionStrings:DbConnectionString"`.

3. Configure file storage in **`appsettings.json`**:
   ```json
   "FileStorage": {
     "BaseDirectory": "C:\\Temp\\DataServiceFileStorage"
   }
   ```
   > ⚠️ You must update the `BaseDirectory` to a valid path on your local machine where files can be stored.

4. Run database migrations (if applicable) or let EF Core auto-create the database.

5. Run the application:
   ```bash
   dotnet run
   ```

6. Access Swagger UI at:
   ```
   https://localhost:7252/swagger
   ```

## Testing with Postman

A **Postman Collection** and **Environment** are provided inside the `postman/` folder.  

Steps:
1. Import both files into Postman.
2. Authenticate by sending a `POST` request to `/auth/login` with valid credentials to receive a JWT token.
3. The token will be stored in the Postman environment and automatically added to subsequent requests.
4. You can run the **entire collection** to automatically test authentication and data access end-to-end.

## Security
- Passwords are not stored in plain text (implementation assumes proper hashing).
- JWT tokens are validated with signing keys, issuer, and audience.
- Role-based policies enforce authorization on sensitive endpoints.

## License
This project is licensed under the MIT License.
