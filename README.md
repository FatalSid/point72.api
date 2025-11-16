# Point72 Word Inversion API

A RESTful API built with ASP.NET Core 8 that inverts words in sentences and persists the results in a SQL Server database. The API tracks request counts and timestamps for each unique sentence.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server Express 2019](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or later (free edition)
- SQL Server Management Studio (SSMS) - Optional but recommended for database management

## Project Setup

### 1. Clone the Repository

```bash
git clone <repository-url>
cd point72.api
```

### 2. Database Setup

This application uses SQL Server Express as the database. Follow these steps to set it up:

#### Option A: Using Entity Framework Migrations (Recommended)

1. **Update the connection string** in `appsettings.json` and `appsettings.Development.json`:
   
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=Point72Api;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False;"
   }
   ```
   
   > **Note:** Replace `localhost\\SQLEXPRESS` with your SQL Server instance name if different.

2. **Install EF Core Tools** (if not already installed):
   
   ```bash
   dotnet tool install --global dotnet-ef
   ```

3. **Apply database migrations**:
   
   ```bash
   cd point72.api
   dotnet ef database update
   ```

   This will create the `Point72Api` database and the `InversionRecords` table with appropriate indexes.

#### Option B: Manual SQL Script Execution

If you prefer to create the database manually:

1. **Connect to SQL Server** using SQLCMD:
   
   ```bash
   sqlcmd -S localhost\SQLEXPRESS -E
   ```

2. **Create the database** (optional, if using a dedicated database):
   
   ```sql
   CREATE DATABASE Point72Api;
   GO
   USE Point72Api;
   GO
   ```

3. **Create the table and indexes**:
   
   ```sql
   CREATE TABLE InversionRecords (
       Id INT IDENTITY(1,1) PRIMARY KEY,
       Request NVARCHAR(2000) NOT NULL,
       Response NVARCHAR(2000) NOT NULL,
       CreatedAt DATETIME2 NOT NULL,
       RequestCount INT NOT NULL,
       LastUpdatedAt DATETIME2 NOT NULL
   );
   GO
   
   CREATE INDEX IX_InversionRecords_CreatedAt ON InversionRecords(CreatedAt);
   GO
   
   CREATE INDEX IX_InversionRecords_Request_Response ON InversionRecords(Request, Response);
   GO
   ```

### 3. Restore Dependencies

```bash
dotnet restore
```

### 4. Run the Application

```bash
dotnet run
```

The application will start and listen on:
- **HTTP**: http://localhost:5262
- **HTTPS**: https://localhost:7062

## Using the API

Access the **Swagger UI** in your browser to explore and test the API endpoints:

- [https://localhost:7062/swagger](https://localhost:7062/swagger)
- [http://localhost:5262/swagger](http://localhost:5262/swagger)

### API Endpoints

#### 1. **POST** `/api/WordInversion/invert`
Inverts the words in a given sentence and stores the result.

- **Request Body**:
  ```json
  {
    "sentence": "your sentence here"
  }
  ```

- **Response**:
  ```json
  {
    "id": 1,
    "request": "your sentence here",
    "response": "ruoy ecnetnes ereh",
    "createdAt": "2024-01-15T10:30:00Z",
    "requestCount": 1,
    "lastUpdatedAt": "2024-01-15T10:30:00Z"
  }
  ```

- **Behavior**: If the same sentence is submitted multiple times, it updates the `requestCount` and `lastUpdatedAt` instead of creating a new record.

#### 2. **GET** `/api/WordInversion/all`
Retrieves all inversion records from the database, ordered by most recently updated.

- **Response**: Array of inversion records

#### 3. **GET** `/api/WordInversion/search?word={word}`
Searches for records containing the specified word in either the request or response.

- **Query Parameter**: `word` (e.g., `/api/WordInversion/search?word=hello`)
- **Response**: Array of matching inversion records

#### 4. **GET** `/health`
Health check endpoint for monitoring application and database connectivity.

## Database Schema

### InversionRecords Table

| Column | Type | Description |
|--------|------|-------------|
| `Id` | INT (Identity) | Primary key, auto-incremented |
| `Request` | NVARCHAR(2000) | Original sentence input |
| `Response` | NVARCHAR(2000) | Sentence with inverted words |
| `CreatedAt` | DATETIME2 | UTC timestamp of first creation |
| `RequestCount` | INT | Number of times this sentence was requested |
| `LastUpdatedAt` | DATETIME2 | UTC timestamp of last update |

**Indexes**:
- `IX_InversionRecords_CreatedAt` - For efficient date-based queries
- `IX_InversionRecords_Request_Response` - For fast text search operations

## Configuration

### Connection Strings

Update the connection string in `appsettings.json` or `appsettings.Development.json` based on your SQL Server setup:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER\\INSTANCE;Database=Point72Api;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False;"
  }
}
```

**Connection String Parameters**:
- `Server`: Your SQL Server instance (e.g., `localhost\SQLEXPRESS`)
- `Database`: Database name (default: `Point72Api`, but can use `master` for testing)
- `Trusted_Connection=True`: Uses Windows Authentication
- `TrustServerCertificate=True`: Trusts the SQL Server certificate
- `Encrypt=False`: Disables encryption for local development

## Project Structure

```
point72.api/
??? Controllers/          # API endpoint controllers
??? Data/                # Database context
??? DTOs/                # Data Transfer Objects
??? Migrations/          # EF Core migrations
??? Models/              # Entity models
??? Repositories/        # Data access layer
??? Services/            # Business logic layer
??? appsettings.json     # Application configuration
??? Program.cs           # Application entry point
```

## Architecture

The application follows clean architecture principles:

- **Controllers**: Handle HTTP requests and responses
- **Services**: Contain business logic for word inversion
- **Repositories**: Manage database operations
- **Models**: Define database entities
- **DTOs**: Define API contracts

## Technologies Used

- **ASP.NET Core 8**: Web framework
- **Entity Framework Core 8**: ORM for database access
- **SQL Server Express 2019**: Database
- **Swagger/OpenAPI**: API documentation
- **Dependency Injection**: Built-in IoC container

## Troubleshooting

### Connection Issues

If you encounter SSL/Certificate errors:
- Ensure `TrustServerCertificate=True` and `Encrypt=False` are in your connection string
- Verify your SQL Server instance name using SQL Server Configuration Manager

### Database Not Found

- Run `dotnet ef database update` to create the database
- Or manually create the database using the SQL scripts provided above

### Port Already in Use

Modify the ports in `Properties/launchSettings.json` if the default ports are occupied.

## Development Notes

- The application uses UTC timestamps for all date/time fields
- Duplicate sentence detection is case-sensitive
- Maximum sentence length is 2000 characters

