using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using UberSystem.Api.Authentication.Extensions;
using UberSystem.Domain.Entities;
using UberSystem.Domain.Interfaces.Services;
using UberSystem.Infrastructure;
using UberSystem.Service;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<UberSystemDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("UberSystemDatabase")));



// Register IUserService
builder.Services.AddScoped<IUserService, UserService>();
// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1",
        Title = "UberSystem.Api.Authentication",
        Description = "An ASP.NET Core Web API for managing customers",
        TermsOfService = new Uri("https://lms-hcmuni.fpt.edu.vn/course/view.php?id=2110"),
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Contact",
            Url = new Uri("https://lms-hcmuni.fpt.edu.vn/course/view.php?id=2110")
        },
        License = new Microsoft.OpenApi.Models.OpenApiLicense
        {
            Name = "License",
            Url = new Uri("https://lms-hcmuni.fpt.edu.vn/course/view.php?id=2110")
        }
    });
    // Set the comments path for the Swagger JSON and UI.
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    opt.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    //Add bearer
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'"
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
});
var connectionString = builder.Configuration.GetConnectionString("Default");
var configuration = builder.Configuration;

//DI services
builder.Services.AddDatabase(configuration).AddServices();
//builder.Services.AddAutoMapperFramework();

var app = builder.Build();

// Configure the HTTP request pipeline.
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