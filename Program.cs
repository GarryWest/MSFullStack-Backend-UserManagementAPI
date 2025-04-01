using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Collections.Concurrent;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;


var builder = WebApplication.CreateBuilder(args);

// Add authentication services
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "yourIssuer", // Replace with your issuer
            ValidAudience = "yourAudience", // Replace with your audience
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes("SuperSecureKey_256BitLength_AAAAAAA")) // Replace with your secret key
        };
    });

// Add services to the container.
builder.Services.AddValidatorsFromAssemblyContaining<UserValidator>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireClaim("Role", "Admin"));
});

// Add logging services
builder.Logging.ClearProviders();
builder.Logging.AddConsole(); // Adds console logging; you can add other providers like File, Debug, etc.


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Use developer exception page in development mode
}
else
{
    app.UseExceptionHandler("/error"); // Use a custom error handler in production mode
    app.UseHsts(); // Use HTTP Strict Transport Security (HSTS) in production mode
}

// Use authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// In-memory user dictionary
var users = new ConcurrentDictionary<int, User>();

// Prepopulate users (optional)
users.TryAdd(1, new User { ID = 1, UserName = "Alice", UserAge = 30 });
users.TryAdd(2, new User { ID = 2, UserName = "Bob", UserAge = 25 });
users.TryAdd(3, new User { ID = 3, UserName = "Charlie", UserAge = 35 });

// Register the error-handling middleware
app.UseMiddleware<ErrorHandlingMiddleware>();

// Middleware to log requests and responses
// This middleware logs the incoming HTTP request and the outgoing HTTP response
app.Use(async (context, next) =>
{
    // Log the incoming HTTP request
    Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");

    await next.Invoke();

    // Log the outgoing HTTP response
    Console.WriteLine($"Response: {context.Response.StatusCode}");
});

// Test endpoint to trigger an exception
app.MapGet("/throw", () => { throw new Exception("This is a test exception!"); });

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

Console.WriteLine("JWT Token: " + GenerateJwtToken());

app.MapGet("/secured", () => "This is a secured endpoint. You are authenticated!")
.RequireAuthorization(); // Require authentication for this endpoint


app.Run();



string GenerateJwtToken()
{
    // var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("yourSecretKey"));
    var securityKey = new SymmetricSecurityKey(
    System.Text.Encoding.UTF8.GetBytes("SuperSecureKey_256BitLength_AAAAAAA")); // Example 32-character key

    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var claims = new[]
    {
        new Claim(ClaimTypes.Name, "UserName"),
        new Claim(ClaimTypes.Role, "UserRole")
    };

    var token = new JwtSecurityToken(
        issuer: "yourIssuer",
        audience: "yourAudience",
        claims: claims,
        expires: DateTime.Now.AddMinutes(30),
        signingCredentials: credentials);

    return new JwtSecurityTokenHandler().WriteToken(token);
}
