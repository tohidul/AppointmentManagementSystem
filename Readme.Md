# Appointment Management System

## Overview
The Appointment Management System is a .NET 8 application designed to manage appointments, users, and doctors. It provides a RESTful API for creating, updating, and managing appointments, users, and doctors.

## Features
- User registration and login
- Doctor registration and management
- Appointment scheduling and management
- Validation of appointment dates
- AutoMapper for object mapping
- Unit tests for controllers, services, and repositories

## Technologies
- .NET 8
- ASP.NET Core
- Entity Framework Core
- AutoMapper
- nUnit

## Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server

### Installation
1. Clone the repository:
https://github.com/tohidul/AppointmentManagementSystem
2. Navigate to the project directory:
cd AppointmentManagementSystem
3. Restore the dependencies:
dotnet restore


### Configuration
1. Update the connection string and jwt configs in `appsettings.json`:
    ` "ConnectionStrings": {
    "AppointmentManagementSystemConnectionString": "Server=<your server>;Database=DoctorAppointment;Trusted_Connection=True;TrustServerCertificate=true;uid=<db_user_>;password=<dbpassword>"
  },
  "Jwt": 
    "Key": "<secretKey>",
    "Issuer": "<Issuer>",
    "Audience": "<Audience>"
  }`


### Database Migration
1. Apply the database migrations:
    `dotnet ef database update`

    
### Running the Application
1. Run the application:
    `dotnet run --project AppointmentManagementSystem.API`


### Running Tests
1. Navigate to the test project directory:
    `cd AppointmentManagementSystem.Test`
2. Run the tests:
    `dotnet test`


## Project Structure
- `AppointmentManagementSystem.API`: Main API project
- `AppointmentManagementSystem.Test`: Unit test project
- `Controllers`: API controllers
- `Services`: Business logic implementations
- `Repositories`: Data access implementations
- `Models`: Data models, DTOs, and view models
- `Mappings`: AutoMapper profiles
- `ValidationAttributes`: Custom validation attributes
- `CustomActionFilters`: Custom action filters



