using Microsoft.EntityFrameworkCore;
using LMS.API.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configure the DbContext. "DefaultConnection" can be found in appsettings.json.
builder.Services.AddDbContext<LmsContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
