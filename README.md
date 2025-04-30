## 🧠 BusyMatch Backend

Backend for the BusyMatch project – an expense tracking and invoice scanning system using OCR. Built with ASP.NET Core and connected to a SQLite database. Includes JWT-based authentication, invoice field extraction, and REST API for managing user expenses.

### 🚀 Key Features

- ✅ User registration and login with JWT
- 📄 OCR text extraction from invoices
- 💰 Categorized expense management
- 📂 Data stored using SQLite
- 🔐 User-level data separation – each user sees only their own expenses

---

### 🛠️ Technologies

- ASP.NET Core 8
- Entity Framework Core
- SQLite
- Tesseract OCR / Magick.NET
- JWT Authentication
- RESTful API

---

### ▶️ Running Locally

1. **Clone the repository:**

```bash
git clone https://github.com/shirzohar/busymatch-backend.git
cd busymatch-backend
```

2. **Configure environment variables:**

Create a file named `appsettings.Development.json` or set environment variables:

```json
{
  "Jwt": {
    "Key": "your_jwt_secret_key",
    "Issuer": "your_issuer"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=users.db"
  }
}
```

3. **Apply database migrations:**

```bash
dotnet ef database update
```

4. **Run the server:**

```bash
dotnet run
```

---

### 📂 Project Structure

```plaintext
├── Controllers/           # Controllers for authentication, invoices, and expenses
├── Data/                  # EF Core DbContext
├── Models/                # Data models / database schema
├── Services/              # OCR logic and invoice parsing
├── appsettings.json       # Configuration file
├── Program.cs             # Main entry point
```

---

### 🧪 Testing

There are currently no automated tests. You can manually test the API using Postman or curl.

---

### 📌 Notes

- OCR functionality requires the presence of `tessdata` files in a valid directory.
- Data is stored in a local SQLite database (`users.db`).
- Make sure to use secure JWT keys in production.

---

### 📬 Author

**Shir Zohar** – [GitHub](https://github.com/shirzohar)

