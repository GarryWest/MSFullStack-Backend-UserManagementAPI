Here is a detailed `README.md` for your `UserManagementAPI` project:

---

# UserManagementAPI

## Overview

The **UserManagementAPI** is a `.NET` minimal API built with **ASP.NET Core 9.0**. It provides CRUD operations for managing users, incorporates validation for user data, logging, error handling, and token-based authentication and authorization.

---

## Features

- **CRUD Operations**:
  - `GET`: Retrieve all users or a user by ID.
  - `POST`: Add a new user.
  - `PUT`: Update an existing user's details.
  - `DELETE`: Remove a user by ID.
- **Validation**:

  - Ensures all user data is valid (e.g., non-empty `UserName`, positive `ID`, reasonable `UserAge`).

- **Error Handling**:

  - Catches unhandled exceptions and returns consistent JSON error responses.

- **Logging**:

  - Logs HTTP requests and responses to the console.

- **Token-Based Authentication**:
  - Validates JWT tokens for secure access to protected endpoints.
  - Example: Access `/secured` endpoint only with a valid token.

---

## Technology Stack

- **Language**: C#
- **Framework**: ASP.NET Core 9.0
- **Dependencies**:
  - [FluentValidation.AspNetCore](https://github.com/FluentValidation/FluentValidation): For user input validation.
  - [Microsoft.AspNetCore.Authentication.JwtBearer](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/jwt): For JWT authentication.

---

## Installation

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download/dotnet) (Version 9.0 or higher)
- A text editor (e.g., Visual Studio Code)
- Postman or REST Client in VS Code for testing

### Steps

1. Clone the repository:
   ```bash
   git clone <repository-url>
   cd UserManagementAPI
   ```
2. Restore dependencies:
   ```bash
   dotnet restore
   ```
3. Run the application:
   ```bash
   dotnet run
   ```

---

## Project Structure

```
UserManagementAPI/
├── UserManagementAPI.csproj          # Project configuration
├── Program.cs                        # Entry point of the application
├── User.cs                           # User model definition
├── UserValidator.cs                  # User data validation rules
├── ErrorHandlingMiddleware.cs        # Middleware for handling exceptions
└── README.md                         # Project documentation
└── requests.http                     # API tests
```

---

## Usage

### Endpoints

| Endpoint          | HTTP Method | Description                       | Authentication Required |
| ----------------- | ----------- | --------------------------------- | ----------------------- |
| `/users`          | `GET`       | Retrieve all users                | No                      |
| `/users/{id:int}` | `GET`       | Retrieve a user by ID             | No                      |
| `/users`          | `POST`      | Add a new user                    | No                      |
| `/users/{id:int}` | `PUT`       | Update an existing user's details | No                      |
| `/users/{id:int}` | `DELETE`    | Remove a user by ID               | No                      |
| `/secured`        | `GET`       | Access secured endpoint           | Yes                     |

---

## Token-Based Authentication

### Generate JWT Token

To generate a token, call the `GenerateJwtToken` method:

```csharp
string GenerateJwtToken()
```

Use the token in the `Authorization` header for secured endpoints:

```
Authorization: Bearer <your-jwt-token>
```

### Secured Endpoint Example

```http
GET http://localhost:5000/secured
Authorization: Bearer <your-valid-jwt-token>
```

---

## Validation Rules

### User Data

- `ID`: Must be greater than 0.
- `UserName`: Must be between 3 and 50 characters.
- `UserAge`: Must be between 0 and 120.

Example of invalid data:

```json
{
  "ID": -1,
  "UserName": "",
  "UserAge": 150
}
```

---

## Error Handling

Unhandled exceptions are caught and returned as JSON:

```json
{
  "error": "Internal server error."
}
```

---

## Logging

Logs incoming HTTP requests and outgoing responses:

```plaintext
Request: GET /users
Response: 200
```

---

Here's the updated **Testing Section** for your `README.md` that describes the provided `requests.http` file:

---

## Testing

This section provides examples of how to test the API using a `requests.http` file with the [REST Client extension](https://marketplace.visualstudio.com/items?itemName=humao.rest-client) in Visual Studio Code. The API has endpoints for CRUD operations, error handling, and secure access via token validation.

### `requests.http` Content

#### 1. **Retrieve All Users**

```http
GET http://localhost:5170/users
```

**Description**: Retrieves a list of all users.

#### 2. **Retrieve a Specific User by ID**

```http
GET http://localhost:5170/users/1
```

**Description**: Fetches details of a user with the specified ID (e.g., `1`).

#### 3. **Add a New User**

```http
POST http://localhost:5170/users
Content-Type: application/json

{
  "ID": 4,
  "UserName": "John Doe",
  "UserAge": 30
}
```

**Description**: Adds a new user with ID `4`, name `John Doe`, and age `30`.

#### 4. **Update an Existing User**

```http
PUT http://localhost:5170/users/4
Content-Type: application/json

{
  "ID": 4,
  "UserName": "John Smith",
  "UserAge": 35
}
```

**Description**: Updates the user with ID `4`, changing the name to `John Smith` and age to `35`.

#### 5. **Delete a User**

```http
DELETE http://localhost:5170/users/4
```

**Description**: Removes the user with ID `4`.

#### 6. **Error Handling Example**

```http
GET http://localhost:5170/throw
```

**Description**: Triggers a server-side exception to test the error-handling middleware.

#### 7. **Access Secured Endpoint with a Valid Token**

```http
GET http://localhost:5170/secured
Authorization: Bearer <valid_token_here>
```

**Description**: Tests access to the `/secured` endpoint using a valid JWT token.

#### 8. **Access Secured Endpoint with an Invalid Token**

```http
GET http://localhost:5170/secured
Authorization: Bearer <invalid_token_here>
```

**Description**: Tests access to the `/secured` endpoint using an invalid JWT token. Should return `401 Unauthorized`.

#### 9. **Access Secured Endpoint Without a Token**

```http
GET http://localhost:5170/secured
```

**Description**: Tests access to the `/secured` endpoint without providing any token. Should return `401 Unauthorized`.

---

### How to Run Tests

1. Create a file named `requests.http` in the root of your project.
2. Copy and paste the above content into the file.
3. Install the [REST Client extension](https://marketplace.visualstudio.com/items?itemName=humao.rest-client) in Visual Studio Code.
4. Click "Send Request" above each HTTP request in the file to test the respective endpoint.
5. Verify the responses (e.g., correct data, error codes).

---

### Example Responses

- **Successful `GET` Request:**

  ```json
  [
    { "ID": 1, "UserName": "Alice", "UserAge": 30 },
    { "ID": 2, "UserName": "Bob", "UserAge": 25 },
    { "ID": 3, "UserName": "Charlie", "UserAge": 35 }
  ]
  ```

- **Invalid Token Response:**

  ```json
  { "error": "Unauthorized" }
  ```

- **Error Handling Example Response:**
  ```json
  { "error": "Internal server error." }
  ```

---

Use the [REST Client extension](https://marketplace.visualstudio.com/items?itemName=humao.rest-client) in VS Code.

---

## License

This project is licensed under the MIT License. See `LICENSE` for details.

---

Let me know if you'd like additional sections in the README or assistance with any part of your project!
