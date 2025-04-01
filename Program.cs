using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// In-memory user list
var users = new List<User>{
    new User { ID = 1, UserName = "Alice", UserAge = 30 },
    new User { ID = 2, UserName = "Bob", UserAge = 25 },
    new User { ID = 3, UserName = "Charlie", UserAge = 35 }
};

app.MapGet("/", () => "Welcome to the User API! Use /users to access user data.");

// GET: Retrieve all users or a user by ID
app.MapGet("/users", () =>
{
    return Results.Ok(users);
});

app.MapGet("/users/{id:int}", (int id) =>
{
    var user = users.FirstOrDefault(u => u.ID == id);
    return user is not null ? Results.Ok(user) : Results.NotFound("User not found");
});

// POST: Add a new user
app.MapPost("/users", (User newUser) =>
{
    users.Add(newUser);
    return Results.Created($"/users/{newUser.ID}", newUser);
});

// PUT: Update an existing user
app.MapPut("/users/{id:int}", (int id, User updatedUser) =>
{
    var user = users.FirstOrDefault(u => u.ID == id);
    if (user is null)
    {
        return Results.NotFound("User not found");
    }

    user.UserName = updatedUser.UserName;
    user.UserAge = updatedUser.UserAge;
    return Results.Ok(user);
});

// DELETE: Remove a user by ID
app.MapDelete("/users/{id:int}", (int id) =>
{
    var user = users.FirstOrDefault(u => u.ID == id);
    if (user is null)
    {
        return Results.NotFound("User not found");
    }

    users.Remove(user);
    return Results.Ok("User removed successfully");
});

app.Run();

