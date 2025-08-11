# WhiteListing-Backend

## Overview

**WhiteListing-Backend** provides secure whitelisting and access control, with a unique focus on modern authentication.  
The project’s core feature is its **custom integration of ASP.NET Core Identity with Supabase (PostgreSQL)**—replacing the typical SQL Server backend.  
This allows you to fully leverage .NET Identity’s user, role, and claim management on Supabase, a flexible cloud database.

---

## Key Features

- **Custom Identity Provider for Supabase:**  
  ASP.NET Core Identity, commonly designed for SQL Server, is customized here to use Supabase/PostgreSQL for all user and role management.
- **Whitelisting API:**  
  Manage lists of users/entities with privileged access.
- **JWT Authentication:**  
  Issue and validate tokens for secure, stateless API access.

- **RESTful Design:**  
  Easily integrate with frontends or third-party services.

---

## Technology Stack

- **.NET 6+/ASP.NET Core**
- **Supabase (PostgreSQL)**
- **ASP.NET Core Identity (custom provider)**
- **Entity Framework Core**
- **JWT Authentication**
- **Dotenv (.env) for configuration**

---

## Getting Started

### Prerequisites

- [.NET 6+ SDK](https://dotnet.microsoft.com/download)
- [Supabase project](https://supabase.com/) (with PostgreSQL database)
- [Git](https://git-scm.com/)
- **No database migrations required:** All tables are managed via Supabase.

### Installation & Setup

1. **Clone the repository:**
   ```bash
   git clone https://github.com/Erickthamara/WhiteListing-Backend.git
   cd WhiteListing-Backend
   ```

2. **Configure environment variables:**
   - Copy `.env.example` to `.env`
   - Fill in your Supabase connection string, JWT secret, and other settings.

3. **Restore dependencies:**
   ```bash
   dotnet restore
   ```

4. **Run the server:**
   ```bash
   dotnet run
   ```

---

## Usage

### Authentication

- **JWT-based authentication.**
- User accounts, roles, and claims are stored in Supabase via the custom Identity provider.
- Register, login, and manage accounts using standard .NET Identity endpoints.

### Whitelist Management API

| Method | Endpoint                   | Description                          |
|--------|----------------------------|--------------------------------------|
| GET    | `/api/whitelist`           | List all whitelisted entities        |
| POST   | `/api/whitelist`           | Add to whitelist                     |
| PUT    | `/api/whitelist/{id}`      | Update whitelisted entity            |
| DELETE | `/api/whitelist/{id}`      | Remove from whitelist                |

> **All endpoints require JWT authentication.**

---

## Project Structure

```
WhiteListing-Backend/
│
├── Controllers/            # API endpoints
├── Models/                 # Data models (Identity, Whitelist, etc.)
├── Services/               # Business logic/services
├── Data/                   # Custom Identity storage, DbContext
├── Middleware/             # Auth, logging, etc.
└── .env                    # Environment variables
```

---

## Custom Identity Integration

This project customizes ASP.NET Core Identity to use Supabase/PostgreSQL instead of the default SQL Server.  
For details, see:

- [Custom Identity Provider Sample (Microsoft Docs)](https://github.com/dotnet/AspNetCore.Docs/tree/7db6835f0bbb75dfdedf31474736dac9b8ae9e82/aspnetcore/security/authentication/identity-custom-storage-providers/sample/CustomIdentityProviderSample)
- `Data/` folder for custom stores and user management.

---

## Contributing

1. Fork this repo
2. Create your branch (`git checkout -b feature/your-feature`)
3. Commit and push your changes
4. Open a Pull Request

---

## License

Distributed under the MIT License. See `LICENSE` for details.

---

## Contact

- **Author:** [Erickthamara](https://github.com/Erickthamara)
- **Issues:** [GitHub Issues](https://github.com/Erickthamara/WhiteListing-Backend/issues)

---

## Acknowledgements

- [.NET](https://dotnet.microsoft.com/)
- [Supabase](https://supabase.com/)
- [Custom Identity Provider Sample](https://github.com/dotnet/AspNetCore.Docs/tree/7db6835f0bbb75dfdedf31474736dac9b8ae9e82/aspnetcore/security/authentication/identity-custom-storage-providers/sample/CustomIdentityProviderSample)
