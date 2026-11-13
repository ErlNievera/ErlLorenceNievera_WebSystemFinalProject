# Student Grade Management System

A web-based academic grade management system developed using **ASP.NET Core Web API, ASP.NET Core MVC, C#, JavaScript, and Microsoft SQL Server**.

The system provides separate portals for faculty members and students. Faculty members can manage student academic grade records, while students can securely access their own academic information and grades.

## Features

### Faculty Portal

- View student grade records.
- Add new grade records.
- Update existing grade records.
- Delete grade records.
- View available students.
- Automatically calculate grade remarks on the server.

### Student Portal

- Secure student login.
- View personal student information.
- View personal academic grade records.
- Dynamically retrieve grades using the JavaScript Fetch API.
- Display appropriate messages when academic records are unavailable.

### Security

- ASP.NET Core Identity authentication.
- Role-based authorization for Faculty and Student accounts.
- JWT authentication for API requests.
- Authenticated student ownership verification.
- Students can only access their own academic records.
- Server-side validation of grade values.
- Server-side calculation of grade remarks.
- JWT tokens are handled by the MVC application's server-side code rather than exposed to browser JavaScript.

## Grade Remarks

| Grade Range | Remarks |
|---|---|
| 95–100 | CMS |
| 90–94 | SP |
| 85–89 | SAP |
| 80–84 | GP |
| 75–79 | EP |
| 0–74 | BEP |

Grade remarks are calculated on the server. Client-submitted remarks are not trusted.

## Technologies Used

- C#
- ASP.NET Core Web API
- ASP.NET Core MVC
- JavaScript (Fetch API and async/await)
- HTML5
- CSS3
- Bootstrap
- JSON
- Microsoft SQL Server
- Entity Framework Core
- ASP.NET Core Identity
- JWT Authentication
- Postman
- Git and GitHub

## Project Requirements

- Visual Studio 2022 or a compatible .NET development environment
- .NET 10 SDK
- Microsoft SQL Server 2019 or a compatible SQL Server version
- SQL Server Management Studio (SSMS)
- Git
- Postman

## Project Structure

```text
ErlLorenceNievera_WebSystemFinalProject/
│
├── StudentGradeManagement.Api/
│   ├── Controllers/
│   └── ...
│
├── StudentGradeManagement.Application/
│   └── ...
│
├── StudentGradeManagement.Domain/
│   └── ...
│
├── StudentGradeManagement.Infrastructure/
│   └── ...
│
├── StudentGradeManagement.Mvc/
│   ├── Controllers/
│   ├── Views/
│   ├── wwwroot/
│   └── ...
│
├── Postman/
│   └── StudentGradeManagement.postman_collection.json
│
├── ErlLorenceNievera_WebSystemFinalProject.sln
├── README.md
└── .gitignore
```

## Installation and Setup

### 1. Clone the Repository

Open PowerShell or a terminal and execute:

```powershell
git clone https://github.com/ErlNievera/ErlLorenceNievera_WebSystemFinalProject.git

cd ErlLorenceNievera_WebSystemFinalProject
```

### 2. Restore Dependencies

Restore the NuGet packages required by the solution:

```powershell
dotnet restore .\ErlLorenceNievera_WebSystemFinalProject.sln
```

### 3. Build the Solution

Build all projects:

```powershell
dotnet build .\ErlLorenceNievera_WebSystemFinalProject.sln
```

Make sure the solution builds successfully before proceeding.

## Database Setup

The application uses a Microsoft SQL Server database named:

```text
StudentGradeManagementDB
```

The database is distributed separately as a BACPAC file. It is not included in the public GitHub repository because it contains academic records and authentication data.

### Obtain the Database File

Request the following file through the private file-sharing link provided by the project author:

```text
StudentGradeManagementDB.bacpac
```

**Security reminder:** Do not upload the database backup to the public GitHub repository. It may contain personal academic information and authentication records.

### Import the Database Using SSMS

1. Open SQL Server Management Studio.
2. Connect to your SQL Server instance.
3. Right-click **Databases**.
4. Select **Import Data-tier Application**.
5. Browse and select `StudentGradeManagementDB.bacpac`.
6. Set the database name to `StudentGradeManagementDB`.
7. Complete the import process.
8. Verify that the database and its tables were created successfully.

The supplied BACPAC is intended to include the academic tables and ASP.NET Core Identity tables, including existing account records and role assignments.

## Application Configuration

Sensitive configuration values must be stored using .NET User Secrets rather than committed to GitHub.

### 1. Configure API User Secrets

Run the following commands from the solution directory:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=YOUR_SQL_SERVER;Database=StudentGradeManagementDB;Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=True" --project ".\StudentGradeManagement.Api\StudentGradeManagement.Api.csproj"

dotnet user-secrets set "Jwt:Key" "YOUR-OWN-SECURE-JWT-SIGNING-KEY" --project ".\StudentGradeManagement.Api\StudentGradeManagement.Api.csproj"

dotnet user-secrets set "DemoAccounts:InitialPassword" "YOUR-DEMO-ACCOUNT-PASSWORD" --project ".\StudentGradeManagement.Api\StudentGradeManagement.Api.csproj"

dotnet user-secrets set "DemoAccounts:ResetExistingPasswords" "false" --project ".\StudentGradeManagement.Api\StudentGradeManagement.Api.csproj"
```

Replace the placeholders with your actual SQL Server instance name and secure configuration values.

For Windows Authentication, ensure that the Windows account running the API has permission to access the database.

**Important:** The API project must have User Secrets configured through its project configuration for these commands to work.

### 2. Demo Account Configuration

The project uses development account seeding under the Development environment.

Important notes:

- Demo account seeding runs when the API is running in the Development environment.
- Existing account passwords are not reset when `DemoAccounts:ResetExistingPasswords` is set to `false`.
- The supplied database contains existing ASP.NET Core Identity account records and password hashes.
- Preserve the existing account records and their student/faculty mappings.
- Existing accounts require their corresponding valid passwords to authenticate.
- The project author must provide authorized demonstration credentials privately, or the instructor must configure accounts through an approved setup procedure.

### 3. Configure the MVC API Base URL

The MVC application's API base URL is configured in:

```text
StudentGradeManagement.Mvc/appsettings.json
```

The default development API address is:

```text
http://localhost:5221/
```

Update this value if the API runs at a different address.

**Security reminder:** Never commit database passwords, JWT signing keys, demo passwords, or other sensitive credentials to GitHub.

## Running the Application

Start the Web API before starting the MVC application.

### 1. Run the Web API

Open a terminal in the solution directory:

```powershell
dotnet run --project .\StudentGradeManagement.Api
```

Run the API in the Development environment when development account seeding is required.

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
| POST | `/api/Grades` | Create a grade record |
| PUT | `/api/Grades/{id}` | Update a grade record |
| DELETE | `/api/Grades/{id}` | Delete a grade record |
| GET | `/api/Grades/students` | Retrieve available students |

### Student Endpoints

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/Grades/my` | Retrieve the authenticated student's grades |
| GET | `/api/Grades/my/profile` | Retrieve the authenticated student's profile |

## API Testing with Postman

An exported Postman collection is included in the repository.

**Collection file:**

```text
Postman/StudentGradeManagement.postman_collection.json
```

### Import the Collection

1. Open Postman.
2. Select **Import**.
3. Choose `StudentGradeManagement.postman_collection.json`.
4. Import the collection.
5. Configure the request URLs and authentication credentials for your local environment.
6. Execute the requests to test the API endpoints.

### Testing Coverage

The documented API testing coverage includes:

- Successful and unsuccessful authentication.
- Faculty grade CRUD operations.
- Grade validation for values outside the 0–100 range.
- Rejection of decimal grade values.
- Automatic grade remarks calculation.
- Duplicate grade prevention.
- Student access restrictions.
- Student ownership verification.
- Unauthorized and forbidden requests.
- Student profile retrieval.
- Student grade retrieval.

## Author

**Erl Lorence Nievera**

Student Grade Management System  
Web System Final Project
