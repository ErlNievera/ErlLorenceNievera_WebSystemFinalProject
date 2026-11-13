# Student Grade Management System

A web-based academic grade management system developed using **ASP.NET Core Web API, ASP.NET Core MVC, C#, JavaScript, and Microsoft SQL Server**.

The system provides separate experiences for faculty members and students. Faculty members can manage student academic grades, while students can securely access their personal academic records.

## Features

### Faculty Portal
- View student grade records.
- Add new grade records.
- Update existing grades.
- Delete grade records.
- View available students.
- Automatic server-side grade remarks calculation.

### Student Portal
- Secure student login.
- View personal student information.
- View personal academic grades.
- Dynamically retrieve grades using JavaScript Fetch API.
- Display appropriate messages when records are unavailable.

### Security
- ASP.NET Core Identity authentication.
- Role-based authorization for Faculty and Student accounts.
- JWT authentication for API requests.
- Authenticated student ownership verification.
- Students can only access their own academic records.
- Server-side grade validation.
- Server-side grade remarks calculation.
- JWT tokens are handled server-side by the MVC application rather than exposed to browser JavaScript.

## Grade Remarks

| Grade Range | Remarks |
|---|---|
| 95–100 | CMS |
| 90–94 | SP |
| 85–89 | SAP |
| 80–84 | GP |
| 75–79 | EP |
| 0–74 | BEP |

Grade remarks are calculated by the server. Client-submitted remarks are not trusted.

## Technologies Used

- C#
- ASP.NET Core Web API
- ASP.NET Core MVC
- JavaScript (Fetch API, async/await)
- HTML and CSS
- JSON
- Microsoft SQL Server
- Entity Framework Core
- ASP.NET Core Identity
- JWT Authentication
- Postman

## Project Requirements

- Visual Studio 2022 or a compatible .NET development environment
- .NET 10 SDK
- Microsoft SQL Server 2019 or a compatible SQL Server version
- SQL Server Management Studio (SSMS)
- Git
- Postman (for API testing)

## Project Structure

```text
ErlLorenceNievera_WebSystemFinalProject/
│
├── StudentGradeManagement.Api/
├── StudentGradeManagement.Application/
├── StudentGradeManagement.Domain/
├── StudentGradeManagement.Infrastructure/
├── StudentGradeManagement.Mvc/
│
├── ErlLorenceNievera_WebSystemFinalProject.sln
├── README.md
└── .gitignore
```

## Installation and Setup

### 1. Clone the Repository

Open a terminal and execute:

```powershell
git clone https://github.com/ErlNievera/ErlLorenceNievera_WebSystemFinalProject.git

cd ErlLorenceNievera_WebSystemFinalProject
```

### 2. Restore and Build the Solution

```powershell
dotnet restore .\ErlLorenceNievera_WebSystemFinalProject.sln

dotnet build .\ErlLorenceNievera_WebSystemFinalProject.sln
```

Make sure the solution builds successfully before proceeding.

## Database Setup

The application uses a SQL Server database named:

```text
StudentGradeManagementDB
```

The database is distributed separately as a BACPAC file. It is not included in the public GitHub repository because it contains academic records and authentication data.

### Obtain the Database Backup

Request the `StudentGradeManagementDB.bacpac` file through the private file-sharing link provided by the project author.

Do not upload the database backup to the public GitHub repository.

### Import the Database Using SSMS

1. Open SQL Server Management Studio.
2. Connect to your SQL Server instance.
3. Right-click **Databases**.
4. Select **Import Data-tier Application**.
5. Browse and select `StudentGradeManagementDB.bacpac`.
6. Set the database name to `StudentGradeManagementDB`.
7. Complete the import process.
8. Verify that the database tables were created successfully.

The supplied BACPAC includes the academic tables and ASP.NET Core Identity tables, including existing account records and role assignments.

## Application Configuration

Sensitive configuration must be stored using .NET User Secrets rather than committed to GitHub.

### 1. Configure API User Secrets

Run the following commands from the solution directory:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=YOUR_SQL_SERVER;Database=StudentGradeManagementDB;Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=True" --project ".\StudentGradeManagement.Api\StudentGradeManagement.Api.csproj"

dotnet user-secrets set "Jwt:Key" "YOUR-OWN-SECURE-JWT-SIGNING-KEY" --project ".\StudentGradeManagement.Api\StudentGradeManagement.Api.csproj"

dotnet user-secrets set "DemoAccounts:InitialPassword" "YOUR-DEMO-ACCOUNT-PASSWORD" --project ".\StudentGradeManagement.Api\StudentGradeManagement.Api.csproj"

dotnet user-secrets set "DemoAccounts:ResetExistingPasswords" "false" --project ".\StudentGradeManagement.Api\StudentGradeManagement.Api.csproj"
```

Replace the placeholders with the correct SQL Server instance name and your own secure configuration values.

For Windows Authentication, ensure that the Windows account running the application has permission to access the database.

### 2. Important Account Configuration Notes

- Demo account seeding runs when the API is running in the Development environment.
- Existing account passwords are not reset when `DemoAccounts:ResetExistingPasswords` is set to `false`.
- The supplied database contains existing Identity account records and password hashes.
- Preserve the existing account records and their student/faculty mappings.
- Use the appropriate existing demo account credentials when testing the application.

### 3. Verify MVC API Configuration

The MVC application's API base URL is configured in:

```text
StudentGradeManagement.Mvc/appsettings.json
```

Default development API address:

```text
http://localhost:5221/
```

Update this value if the API is running at a different address.

**Security Reminder:** Never commit database passwords, JWT signing keys, demo passwords, or other sensitive credentials to GitHub.

## Running the Application

Start the Web API before starting the MVC application.

### 1. Run the Web API

Open a terminal in the solution directory:

```powershell
dotnet run --project .\StudentGradeManagement.Api
```

Ensure the API runs in the Development environment so the development account seeding process can execute.

API address:

```text
http://localhost:5221
```

Swagger documentation:

```text
http://localhost:5221/swagger
```

### 2. Run the MVC Application

Open a second terminal in the solution directory:

```powershell
dotnet run --project .\StudentGradeManagement.Mvc
```

MVC development address:

```text
http://localhost:5018
```

MVC HTTPS address:

```text
https://localhost:7144
```

The MVC application communicates with the Web API through its configured HTTP client.

## API Endpoints

### Authentication

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/Auth/login` | Authenticate a user |

### Faculty Endpoints

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/Grades` | Retrieve grade records |
| GET | `/api/Grades/{id}` | Retrieve a specific grade |
| POST | `/api/Grades` | Create a grade |
| PUT | `/api/Grades/{id}` | Update a grade |
| DELETE | `/api/Grades/{id}` | Delete a grade |
| GET | `/api/Grades/students` | Retrieve available students |

### Student Endpoints

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/Grades/my` | Retrieve authenticated student's grades |
| GET | `/api/Grades/my/profile` | Retrieve authenticated student's profile |

## Testing

API endpoints and security behavior are tested using Postman.

Testing includes:

- Successful and unsuccessful authentication.
- Faculty grade CRUD operations.
- Grade validation for values outside 0–100.
- Automatic grade remarks calculation.
- Duplicate grade prevention.
- Student access restrictions.
- Student ownership verification.
- Unauthorized and forbidden requests.
- Student profile and grade retrieval.

## Author

**Erl Lorence Nievera**

Student Grade Management System  
Web System Final Project
