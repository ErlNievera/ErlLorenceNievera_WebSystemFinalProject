
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentGradeManagement.Infrastructure.Identity;
using StudentGradeManagement.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// 1. DATABASE CONNECTION
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Database connection string 'DefaultConnection' was not found.");

// 2. IDENTITY DATABASE CONTEXT
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// 3. ACADEMIC DATABASE CONTEXT
builder.Services.AddDbContext<AcademicDbContext>(options =>
    options.UseSqlServer(connectionString));

// 4. ASP.NET CORE IDENTITY
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = true;

        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// 5. API SERVICES
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 6. SEED IDENTITY ROLES
await using (var scope = app.Services.CreateAsyncScope())
{
    var roleManager = scope.ServiceProvider
        .GetRequiredService<RoleManager<IdentityRole>>();

    await RoleSeeder.SeedRolesAsync(roleManager);
}

// 7. HTTP PIPELINE
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
