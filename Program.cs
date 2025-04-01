using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddValidatorsFromAssemblyContaining<UserValidator>();

var app = builder.Build();

// In-memory user dictionary
var users = new ConcurrentDictionary<int, User>();

// Prepopulate users (optional)
users.TryAdd(1, new User { ID = 1, UserName = "Alice", UserAge = 30 });
users.TryAdd(2, new User { ID = 2, UserName = "Bob", UserAge = 25 });
users.TryAdd(3, new User { ID = 3, UserName = "Charlie", UserAge = 35 });

app.MapGet("/", () => "Welcome to the User API! Use /users to access user data.");

// GET: Retrieve all users
app.MapGet("/users", () =>
{
    return Results.Ok(users.Values);
});

// GET: Retrieve a user by ID
app.MapGet("/users/{id:int}", (int id) =>
{
    if (users.TryGetValue(id, out var user))
    {
        return Results.Ok(user);
    }
    return Results.NotFound("User not found");
});

// POST: Add a new user
app.MapPost("/users", (User newUser, IValidator<User> validator) =>
{
    var validationResult = validator.Validate(newUser);
    if (!validationResult.IsValid)
    {
        return Results.BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
    }

    if (users.TryAdd(newUser.ID, newUser))
    {
        return Results.Created($"/users/{newUser.ID}", newUser);
    }
    return Results.BadRequest("User ID already exists");
});

// PUT: Update an existing user
app.MapPut("/users/{id:int}", (int id, User updatedUser, IValidator<User> validator) =>
{
    var validationResult = validator.Validate(updatedUser);
    if (!validationResult.IsValid)
    {
        return Results.BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
    }

    if (users.TryGetValue(id, out var existingUser))
    {
        existingUser.UserName = updatedUser.UserName;
        existingUser.UserAge = updatedUser.UserAge;
        return Results.Ok(existingUser);
    }
    return Results.NotFound("User not found");
});

// DELETE: Remove a user by ID
app.MapDelete("/users/{id:int}", (int id) =>
{
    if (users.TryRemove(id, out _))
    {
        return Results.Ok("User removed successfully");
    }
    return Results.NotFound("User not found");
});

app.Run();

