using LeaveManagement.API.Data;
using LeaveManagement.API.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add this line:
builder.Services.AddScoped<IBusinessDaysCalculator, BusinessDaysCalculator>();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();

    try
    {
        context.Database.EnsureCreated();

        Console.WriteLine("Starting database seeding...");
        DbSeeder.SeedData(context);
        Console.WriteLine("Database seeding completed!");

        Console.WriteLine($"Employees: {context.Employees.Count()}");
        Console.WriteLine($"Holidays: {context.Holidays.Count()}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"ERROR during seeding: {ex.Message}");
    }
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// THIS IS THE CRITICAL LINE YOU WERE MISSING!
app.MapControllers();

app.Run();