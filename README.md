# DataService  
A **.NET 8 Web API** that provides a data retrieval service with **caching, file storage, and database support**.  
The service follows a **layered architecture**, implements **repository, service, and factory design patterns**, and uses **JWT authentication with role-based authorization** for secure access.  

---

## Features
- 🔐 **Authentication & Authorization**  
  - JWT-based authentication.  
  - Role-based authorization (`Admin`, `User`).  

- 📦 **Data Management**  
  - `User` can fetch data by ID.  
  - `Admin` can insert and update data.  
  - Data is stored via multiple storage providers: cache, file system, and database.  

- ⚡ **Architecture & Design Patterns**  
  - Repository pattern for data access.  
  - Service layer abstraction for business logic.  
  - **Factory Design Pattern** for selecting a storage provider dynamically (`Cache`, `FileStorage`, `Database`).  
  - AutoMapper for DTO ↔ Entity mapping.  

- 🛡️ **Security**  
  - HTTPS support (dev cert).  
  - JWT token validation (issuer, audience, signing key).  

- 📖 **API Documentation**  
  - Integrated **Swagger UI** (`/swagger`) in development.  

---

## Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)  
- SQL Server (or LocalDB)  
- [Postman](https://www.postman.com/downloads/) for testing  

---

## Setup
1. Clone the repository:
   ```bash
   git clone https://github.com/your-username/dataservice.git
   cd dataservice
   ```

2. Update **connection string** in `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DbConnectionString": "Server=(localdb)\\mssqllocaldb;Database=DataServiceDb;Trusted_Connection=True;"
   }
   ```

3. Run the project:
   ```bash
   dotnet run --project DataService.Api
   ```

4. Access the API:
   - Swagger UI: [http://localhost:5296/swagger](http://localhost:5296/swagger)  
   - HTTPS endpoint: [https://localhost:7252](https://localhost:7252)  

---

## Authentication & Roles
### Login (Get JWT Token)
**Request**
```http
POST /auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "admin123"
}
```

**Response**
```json
{
  "token": "<jwt-token-here>"
}
```

### Usage
Include the token in the **Authorization** header for all protected requests:
```
Authorization: Bearer <jwt-token-here>
```

### Roles
- **User**
  - Can fetch data: `GET /data/{id}`  
- **Admin**
  - Can insert data: `POST /data`  
  - Can update data: `PUT /data/{id}`  
  - Can also fetch data  

---

## Example API Calls

### Insert Data (Admin only)
**Request**
```http
POST /data
Authorization: Bearer <admin-token>
Content-Type: application/json

"Sample data content"
```

**Response**
```http
201 Created
"4d7d7c6e-b8c3-4b9c-85e1-6b354e9b6c33"
```

---

### Fetch Data (User or Admin)
**Request**
```http
GET /data/4d7d7c6e-b8c3-4b9c-85e1-6b354e9b6c33
Authorization: Bearer <user-token>
```

**Response**
```json
{
  "id": "4d7d7c6e-b8c3-4b9c-85e1-6b354e9b6c33",
  "value": "Sample data content"
}
```

---

### Update Data (Admin only)
**Request**
```http
PUT /data/4d7d7c6e-b8c3-4b9c-85e1-6b354e9b6c33
Authorization: Bearer <admin-token>
Content-Type: application/json

"Updated data content"
```

**Response**
```http
204 No Content
```

---

## Postman Testing
A **Postman collection** and environment are provided in the `postman/` folder.  

### Import into Postman
1. Import `postman/DataService.postman_collection.json`.  
2. Import `postman/DataService.postman_environment.json`.  
3. Select the environment in Postman.  
4. Run the following flow:
   1. `POST /auth/login` as **Admin** → store token.  
   2. `POST /data` (Admin only) → insert data, get ID.  
   3. `GET /data/{id}` as **User** → fetch inserted data.  
   4. `PUT /data/{id}` as **Admin** → update data.  

👉 Alternatively, you can simply **run the entire Postman collection** to execute all steps automatically in sequence (acts as an automated test).  

---

## Project Structure
```
DataService/
│
├── DataService.Api/               # API project (controllers, middleware)
├── DataService.Application/       # Application layer (services, DTOs, mappings)
├── DataService.Infrastructure/    # Infrastructure (repositories, storage providers, factory)
├── DataService.Models/            # Domain entities
├── postman/                       # Postman collection & environment JSONs
└── README.md
```

---

## Examiner Notes
- The project can be run via **Visual Studio** or by executing the built `.exe`.  
- Development HTTPS certificate may need to be trusted (see Postman settings).  
- All test scenarios are reproducible using the included Postman collection.  
- The **Factory Design Pattern** is implemented in the `StorageProviderFactory` to dynamically select between **Cache**, **File Storage**, and **Database** providers.
