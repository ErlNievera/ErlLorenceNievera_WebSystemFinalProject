# Student Grade Management System

A web-based academic grade management system built with **ASP.NET Core Web API, ASP.NET Core MVC, C#, JavaScript, and SQL Server**.

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

## Grade Remarks

| Grade Range | Remarks    |
| ----------- | ---------- |
| 95–100      | CMS        |
| 90–94       | SP         |
| 85–89       | SAP        |
| 80–84       | GP         |
| 75–79       | EP         |
| 0–74        | BEP/FAILED |

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
├── database/
│   └── StudentGradeManagementDB_WS_FINAL_PROJECT.sql
├── Postman/
│   └── [Exported Postman collection]
└── README.md
```

## Requirements

- Visual Studio 2022 or a compatible .NET development environment
- .NET SDK 10
- SQL Server
- SQL Server Management Studio (SSMS)
- Postman

## Database Setup

1. Open SQL Server Management Studio.
2. Connect to your SQL Server instance using the appropriate authentication method.
3. Open the database SQL script.
4. Execute the script to create and populate `StudentGradeManagementDB`.
5. Verify that the database and its tables were created successfully.

Identity-related tables are managed separately through the application's Entity Framework Core Identity configuration and migrations.

## Application Configuration

Before running the application:

1. Configure the SQL Server connection string in the appropriate configuration or secure development settings.
2. Configure the JWT authentication settings.
3. Ensure that the API and MVC projects use the correct API base URL.
4. Apply the required Entity Framework Core migrations.

**Security note:** Never commit database passwords, JWT signing keys, or other credentials to GitHub. Use User Secrets or environment variables for sensitive configuration.

## Running the Application

The configured development addresses are:

| Application | Address                                                        |
| ----------- | -------------------------------------------------------------- |
| Web API     | [http://localhost:5221](http://localhost:5221)                 |
| API Swagger | [http://localhost:5221/swagger](http://localhost:5221/swagger) |
| MVC Website | [http://localhost:5018](http://localhost:5018)                 |
| MVC HTTPS   | [https://localhost:7144](https://localhost:7144)               |

Start the API first, then start the MVC application.

The MVC application communicates with the API through its configured HTTP client.

## API Endpoints

### Authentication

`POST /api/Auth/login`

### Faculty

| Method | Endpoint               | Purpose           |
| ------ | ---------------------- | ----------------- |
| GET    | `/api/Grades`          | Retrieve grades   |
| GET    | `/api/Grades/{id}`     | Retrieve a grade  |
| POST   | `/api/Grades`          | Create a grade    |
| PUT    | `/api/Grades/{id}`     | Update a grade    |
| DELETE | `/api/Grades/{id}`     | Delete a grade    |
| GET    | `/api/Grades/students` | Retrieve students |

### Student

| Method | Endpoint                 | Purpose                                  |
| ------ | ------------------------ | ---------------------------------------- |
| GET    | `/api/Grades/my`         | Retrieve authenticated student's grades  |
| GET    | `/api/Grades/my/profile` | Retrieve authenticated student's profile |

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

See the Postman collection and test screenshots included in the repository.

## Author

**Erl Lorence Nievera**

Student Grade Management System — Web System Final Project
