Invoice Classifier Backend
Backend for the Invoice Classifier project – an expense tracking and invoice scanning system using OCR. Built with ASP.NET Core and connected to a SQLite database. Includes JWT-based authentication, invoice field extraction, and REST API for managing user expenses.
Key Features
User registration and login with JWT

OCR text extraction from invoices

Categorized expense management

Data stored using SQLite
User-level data separation – each user sees only their own expenses

Technologies
ASP.NET Core 8

Entity Framework Core

SQLite

Tesseract OCR / Magick.NET

JWT Authentication

RESTful API

Running Locally
Clone the repository:

bash
Copy
Edit
git clone https://github.com/shirzohar/invoice-classifier-backend.git
cd invoice-classifier-backend
Configure environment variables:

Create a file named appsettings.Development.json or set environment variables:

json
Copy
Edit
{
  "Jwt": {
    "Key": "your_jwt_secret_key",
    "Issuer": "your_issuer"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=users.db"
  }
}
Apply database migrations:

bash
Copy
Edit
dotnet ef database update
Run the server:

bash
Copy
Edit
dotnet run
