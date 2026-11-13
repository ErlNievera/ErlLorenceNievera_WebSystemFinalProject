# Student Grade Management System

A web-based academic grade management system built using **ASP.NET Core Web API, ASP.NET Core MVC, C#, JavaScript, and SQL Server**.

The system provides separate experiences for faculty members and students. Faculty members can manage student grades, while students can securely view their own academic records.

## Features

### Faculty
- View student grade records.
- Add new grades.
- Update existing grades.
- Delete grade records.
- View available students.
- Automatic server-side grade remarks calculation.

### Students
- Secure student login.
- View personal student information.
- View their own academic grades.
- Dynamically load grades using JavaScript Fetch API.
- Receive clear messages when records are unavailable.

### Security
- ASP.NET Core Identity authentication.
- Role-based authorization for Faculty and Student accounts.
- JWT authentication for API requests.
- Students can only access their own academic records.
- Grade ownership is determined using authenticated account information.
- Server-side validation of grade values and remarks.
- JWT tokens are not exposed to browser JavaScript or stored in local storage.

## Grade Remarks

| Grade Range | Remarks |
|---|---|
| 95–100 | CMS |
| 90–94 | SP |
| 85–89 | SAP |
| 80–84 | GP |
| 75–79 | EP |
| 0–74 | BEP |

Remarks are calculated on the server. Client-submitted remarks are not trusted.

## Technologies Used

- C#
- ASP.NET Core Web API
- ASP.NET Core MVC
- JavaScript (Fetch API, async/await)
- HTML and CSS
- JSON
- SQL Server
- Entity Framework Core
- ASP.NET Core Identity
- JWT Authentication
- Postman

## Requirements

- Visual Studio 2022 or a compatible .NET development environment
- .NET 10 SDK
- SQL Server 2019 or a compatible SQL Server version
- SQL Server Management Studio (SSMS)
- Postman (for API testing)

## Database Setup

The project uses a SQL Server database named `StudentGradeManagementDB`.

The database is distributed separately as a **BACPAC file**. It is not included in this public GitHub repository because it contains student, faculty, and authentication records.

### Obtain the Database

Request the database backup from the project author through an authorized private file-sharing link.

Do not upload the BACPAC file to this public repository.

### Import the Database

1. Open SQL Server Management Studio.
2. Connect to your SQL Server instance.
3. Right-click **Databases**.
4. Select **Import Data-tier Application**.
5. Browse for the supplied `StudentGradeManagementDB.bacpac` file.
6. Set the database name to `StudentGradeManagementDB`.
7. Complete the import process.
8. Verify that the database and its tables were created successfully.

The BACPAC includes the academic tables and ASP.NET Core Identity tables, including existing account records and role assignments.

## Application Configuration

Sensitive settings must be configured separately on each development computer using .NET User Secrets.

Do not commit database credentials, JWT signing keys, or passwords to GitHub.

### Configure API User Secrets

Open a terminal in the solution directory and run the following commands:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=YOUR_SQL_SERVER;Database=StudentGradeManagementDB;Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=True" --project ".\StudentGradeManagement.Api\StudentGradeManagement.Api.csproj"

dotnet user-secrets set "Jwt:Key" "YOUR-OWN-SECURE-JWT-SIGNING-KEY" --project ".\StudentGradeManagement.Api\StudentGradeManagement.Api.csproj"

dotnet user-secrets set "DemoAccounts:InitialPassword" "YOUR-DEMO-ACCOUNT-PASSWORD" --project ".\StudentGradeManagement.Api\StudentGradeManagement.Api.csproj"

dotnet user-secrets set "DemoAccounts:ResetExistingPasswords" "false" --project ".\StudentGradeManagement.Api\StudentGradeManagement.Api.csproj"
```

Replace the placeholders with your own configuration.

For Windows Authentication, ensure your Windows account has permission to access the SQL Server database.

### Important Notes

- The API demo account seeder runs in the Development environment.
- Existing account passwords are not reset when `DemoAccounts:ResetExistingPasswords` is set to `false`.
- The imported database contains existing Identity password hashes and role assignments.
- Keep the database records and account mappings intact when using the supplied database.
- Never use production credentials or expose sensitive configuration in source control.

### Configure the MVC Application

Ensure that the MVC application's configured API base URL points to the API address:

```text
http://localhost:5221/
```

Check the `ApiSettings:BaseUrl` configuration before running the MVC project.

## Running the Application

The configured development addresses are:

| Application | Address |
|---|---|
| Web API | http://localhost:5221 |
| API Swagger | http://localhost:5221/swagger |
| MVC Website | http://localhost:5018 |
| MVC HTTPS | https://localhost:7144 |

### Start the API

From the solution directory:

```powershell
dotnet run --project .\StudentGradeManagement.Api
```

Run the API using the Development environment so the configured development seeding process can execute.

### Start the MVC Application

Open another terminal:

```powershell
dotnet run --project .\StudentGradeManagement.Mvc
```

Open the MVC website using its configured local address.

The MVC application communicates with the API through its configured HTTP client.

## API Endpoints

### Authentication

| Method | Endpoint | Purpose |
|---|---|---|
| POST | `/api/Auth/login` | Authenticate a user |

### Faculty

| Method | Endpoint | Purpose |
|---|---|---|
| GET | `/api/Grades` | Retrieve grades |
| GET | `/api/Grades/{id}` | Retrieve a grade |
| POST | `/api/Grades` | Create a grade |
| PUT | `/api/Grades/{id}` | Update a grade |
| DELETE | `/api/Grades/{id}` | Delete a grade |
| GET | `/api/Grades/students` | Retrieve students |

### Student

| Method | Endpoint | Purpose |
|---|---|---|
| GET | `/api/Grades/my` | Retrieve authenticated student's grades |
| GET | `/api/Grades/my/profile` | Retrieve authenticated student's profile |

## Testing

API endpoints and security behavior are tested using Postman.

Tests include:

- Successful and unsuccessful authentication.
- Faculty grade CRUD operations.
- Grade validation for values outside 0–100.
- Automatic remarks calculation.
- Duplicate grade prevention.
- Student access restrictions.
- Student ownership verification.
- Unauthorized and forbidden requests.
- Student profile and grade retrieval.

## Author

**Erl Lorence Nievera**

Student Grade Management System — Web System Final Project
