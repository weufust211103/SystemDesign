using Microsoft.EntityFrameworkCore;
using UberSystem.Api.Customer.Extensions;
using UberSystem.Domain.Interfaces.Services;
using UberSystem.Domain.Interfaces;
using UberSystem.Infrastructure;
using UberSystem.Service;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddAuthorization();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<DbFactory>(); // Register DbFactory
builder.Services.AddDbContext<UberSystemDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// Register AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<Func<UberSystemDbContext>>(provider => () => provider.GetService<UberSystemDbContext>());
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

