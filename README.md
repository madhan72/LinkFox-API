# LinkFox-API

🦊 LinkFox – .NET Core WebAPI

A production-ready LinkFox URL Shortener built with .NET Core 8, Clean Architecture, Entity Framework Core, and Redis Cache.
Supports URL shortening, redirect with click tracking, and analytics.

📦 Prerequisites

Before running this project, ensure you have the following installed:

.NET 8 SDK

SQL Server

Git

⚙️ Project Setup
1. Clone the Repository
git clone https://github.com/yourusername/linkfox.git
cd linkfox

2. Configure Database Connection

Update appsettings.json in WebApi project:

"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=LinkFoxDb;User Id=sa;Password=YourPassword;"
} 

3. Apply Database Migrations

Database was designed as Create if not available or use existing one.

4. Run the Application
dotnet run --project WebApi


The API will be available at:
👉 https://localhost:7161
👉 http://localhost:5190

🛠 Features

🔗 Shorten URL with custom alias or auto-generated Base62 shortcode

↩️ Redirect from short URL to long URL

📊 Analytics API – track clicks, devices, referrer, country, city

✅ Unit & Integration Tests (XUnit, Moq, FluentAssertions)

🏗 Clean Architecture (Domain, Application, Infrastructure, WebApi)

📚 API Endpoints
Method	Endpoint	Description
POST	/api/urls	Create short URL
GET	/r/{shortCode}	Redirect to long URL
GET	/api/urls	List URLs (pagination + sort)
GET	/api/urls/{shortCode}/analytics	Get analytics for a URL

🧪 Running Tests

Navigate to test project(s):

dotnet test

Projects under tests/ include:

Application.Tests → Service layer unit tests

Infrastructure.Tests → Repository & EF Core tests

WebApi.Tests → API integration tests


📌 Tech Stack

.NET Core 8

Entity Framework Core

SQL Server

XUnit / Moq / FluentAssertions (testing)

Clean Architecture
