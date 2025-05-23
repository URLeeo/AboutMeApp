# AboutMe API (CV API)

A RESTful API system to manage user portfolios, CVs, educations, certificates, and achievements. Built with ASP.NET Core and Entity Framework.

---

// Technologies Used

- ASP.NET Core 8
- Entity Framework Core
- C#
- MsSQL
- AutoMapper
- JWT Authentication
- Swagger UI
- Docker (optional)

---

// Features (CRUD for)

- **Authentication**
  - Register, Login, Refresh Token (JWT)
- **Certificates**
  - Add, View, Edit, Delete certificates
- **Educations**
  - Add, View, Edit, Delete education records
- **Experiences**
  - Manage user experience history
- **User Roles**
  - Assign and control user roles
- **Social Media**
  - Add/remove LinkedIn, GitHub, etc.
- **Templates**
  - Manage CV layout templates
- **User Profiles**
  - Create and update user general information
- **Search Endpoints**
  - Search within each entity (certificates, education, etc.)
- **Public CV Links**
  - Shareable URLs like `cv.me/username`

---

// Project Structure

The project is structured using Clean Architecture principles:

/src
├── Core
│ ├── AboutMeApp.Application # Business logic, use cases, services
│ ├── AboutMeApp.Common # Shared utilities and constants
│ └── AboutMeApp.Domain # Domain entities and interfaces
│
├── Infrastructure
│ ├── AboutMeApp.Infrastructure # Infrastructure-level logic
│ └── AboutMeApp.Persistence # Database access, EF Core, migrations
│
└── Presentation
└── AboutMeApp.WebAPI # ASP.NET Core Web API layer (Controllers, Middleware)

test/ # Unit and integration tests

---

// Swagger UI Demo

Here is a preview of the API endpoints using Swagger UI:

![API_Preview](https://github.com/user-attachments/assets/6cc2a6c9-8975-431a-932c-b3797eaf7df2)

> You can test every endpoint directly from Swagger by providing JWT tokens.

---

// Getting Started

```bash
# 1. Clone the repository
git clone https://github.com/URLeeo/AboutMeApp

# 2. Navigate to project folder
cd "project_name"

# 3. Set your DB connection string in appsettings.json or .env

# 4. Run the API
dotnet run


# 2. Navigate to project folder
cd aboutmeapi

# 3. Set your DB connection string in appsettings.json or .env

# 4. Run the API
dotnet run

