
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TeamTaskTracking.Web.Data;
using TeamTaskTracking.Web.Components;
using TeamTaskTracking.Web.Models;

var builder = WebApplication.CreateBuilder(args);

// เชื่อมต่อ MySQL
var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySQL(connectionString)
);

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/login";
    });
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Creates the schema only when the connected database is empty (for example,
// the first Railway deployment). Existing local tables and data are preserved.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.EnsureCreatedAsync();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute(
    "/not-found",
    createScopeForStatusCodePages: true
);

app.UseHttpsRedirection();

app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapGet("/db-check", async (AppDbContext db) =>
{
    try
    {
        bool connected = await db.Database.CanConnectAsync();

        return connected
            ? Results.Ok("MySQL Connected Successfully!")
            : Results.Problem("Cannot connect to MySQL");
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.MapPost("/account/register", async (HttpContext context, AppDbContext db, IPasswordHasher<User> passwordHasher) =>
{
    var form = await context.Request.ReadFormAsync();
    var fullName = form["FullName"].ToString().Trim();
    var email = form["Email"].ToString().Trim().ToLowerInvariant();
    var password = form["Password"].ToString();
    var confirmPassword = form["ConfirmPassword"].ToString();

    if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email) ||
        password.Length < 8 || password != confirmPassword)
    {
        return Results.Redirect("/register?error=invalid");
    }

    if (await db.Users.AnyAsync(user => user.Email == email))
    {
        return Results.Redirect("/register?error=email-exists");
    }

    var user = new User { FullName = fullName, Email = email };
    user.PasswordHash = passwordHasher.HashPassword(user, password);

    db.Users.Add(user);
    await db.SaveChangesAsync();

    return Results.Redirect("/login?registered=true");
});

app.MapPost("/account/login", async (HttpContext context, AppDbContext db, IPasswordHasher<User> passwordHasher) =>
{
    var form = await context.Request.ReadFormAsync();
    var email = form["Email"].ToString().Trim().ToLowerInvariant();
    var password = form["Password"].ToString();
    var user = await db.Users.SingleOrDefaultAsync(item => item.Email == email);

    if (user is null || passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password) == PasswordVerificationResult.Failed)
    {
        return Results.Redirect("/login?error=invalid");
    }

    var claims = new[]
    {
        new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
        new Claim(ClaimTypes.Name, user.FullName),
        new Claim(ClaimTypes.Email, user.Email)
    };
    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

    return Results.Redirect("/");
});

app.MapPost("/account/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/login");
});

app.Run();
