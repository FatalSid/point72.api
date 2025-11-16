# Point72 Word Inversion API

This project is a simple ASP.NET Core Web API that inverts words in a sentence and stores the results.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

## How to Run

1.  Open a terminal or command prompt.
2.  Navigate to the `point72.api` directory:
    ```sh
    cd point72.api
    ```
3.  Restore the project dependencies:
    ```sh
    dotnet restore
    ```
4.  Run the project:
    ```sh
    dotnet run
    ```
5.  The application will start and listen on `http://localhost:5262` and `https://localhost:7062`.

## Using the API

Once the application is running, you can access the Swagger UI in your browser to explore and test the API endpoints:

-   **HTTP**: [http://localhost:5262/swagger](http://localhost:5262/swagger)
-   **HTTPS**: [https://localhost:7062/swagger](https://localhost:7062/swagger)

## Database

This project uses an in-memory database provided by Entity Framework Core. This means:
- No database setup is required.
- The data is not persisted between application runs. Every time you restart the application, the database will be empty.
- I can add any other database provider (e.g., SQL Server, SQLite) if needed. I am not working on my laptop rather on a very old setup so let me know if needed.


- ## API Endpoints

-   **`POST /api/wordinversion/invert`**
    -   Inverts the words in a given sentence.
    -   If the same sentence is sent multiple times, it updates a counter and the last updated timestamp instead of creating a new entry.
    -   **Request body**: `{ "sentence": "your sentence here" }`
    -   **Response**: The inversion record, including the original request, the inverted response, the creation timestamp, the request count, and the last updated timestamp.

-   **`GET /api/wordinversion/all`**
    -   Retrieves all the inversion records from the database.

-   **`GET /api/wordinversion/search`**
    -   Searches for records by a specific word in either the request or the response.
    -   **Query parameter**: `word` (e.g., `/api/wordinversion/search?word=hello`)

