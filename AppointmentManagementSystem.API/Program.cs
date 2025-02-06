using AppointmentManagementSystem.API.Data;
using AppointmentManagementSystem.API.Mappings;
using AppointmentManagementSystem.API.Repositories;
using AppointmentManagementSystem.API.Services.Interfaces;
using AppointmentManagementSystem.API.Services;
using Microsoft.EntityFrameworkCore;
using AppointmentManagementSystem.API.Models.Entities;
using Microsoft.AspNetCore.Identity;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using AppointmentManagementSystem.API.Repositories.Interfaces;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Doctor Appointment API", Version = "v1" });
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                },
                Scheme = "Oauth2",
                Name = JwtBearerDefaults.AuthenticationScheme,
                In = ParameterLocation.Header
            },
            new List<string>()
        }
        
    });
});

builder.Services.AddDbContext<AppointmentManagementSystemDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("AppointmentManagementSystemConnectionString")));

builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

// Register the UserRepository
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Register the UserService
builder.Services.AddScoped<IUserService, UserService>();

// Register the DoctorRepository
builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
// Register the DoctorService
builder.Services.AddScoped<IDoctorService, DoctorService>();


// Register the AppointmentRepository
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();

// Register the AppointmentService
builder.Services.AddScoped<IAppointmentService, AppointmentService>();



// Register the IPasswordHasher
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// Configure JWT authentication
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            AuthenticationType = "Jwt",
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],      
            ValidAudiences = new[] { builder.Configuration["Jwt:Audience"] },
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

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
