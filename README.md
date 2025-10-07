# 🗒️ NotinLite

**NotinLite** is a lightweight, minimalistic **note and reminder management application** built using **.NET**.  
It’s designed for speed, simplicity, and low resource usage — ideal for developers or teams who want a fast way to store, retrieve, and manage notes or reminders without the bloat of full-scale productivity suites.

---

## 🧭 About

Unlike heavy note-taking systems, **NotinLite** focuses on the essentials:
- Capture quick notes and reminders instantly  
- Keep data local and lightweight (SQLite by default)  
- Simple UI, fast backend, and minimal dependencies  

It’s built using the **Model–View–Controller (MVC)** architecture to ensure separation of concerns, modularity, and maintainability.

---

## ⚙️ Tech Stack

| Layer | Technology | Purpose |
|--------|-------------|----------|
| **Framework** | ASP.NET MVC / .NET 8 | Web application framework |
| **Language** | C# | Backend logic and domain development |
| **Database** | SQLite / SQL Server | Persistent storage for notes and reminders |
| **ORM** | Entity Framework Core | Database operations and migrations |
| **Frontend** | Razor Views, HTML5, CSS3, JavaScript | User interface for managing notes |
| **Logging** | Serilog / Built-in Logging | Application and error tracking |
| **Documentation** | Swagger (if API-enabled) | API exploration and testing |
| **Version Control** | Git + GitHub | Repository and collaboration management |

---

## 🚀 Features

- ✍️ Create, edit, and delete notes  
- ⏰ Add lightweight reminders or notifications  
- 🔍 Search and filter notes efficiently  
- 💾 Data persistence via SQLite or SQL Server  
- 🔐 Optional user authentication for private notes  
- 🧩 Extensible structure for future modules (tags, categories, attachments)

---

## 🧱 Architecture Overview

**NotinLite** follows a clean MVC structure:

- **Models** → Represent notes, reminders, and metadata  
- **Views** → Provide a simple and responsive UI  
- **Controllers** → Handle requests, logic, and data flow between models and views  
- **Database Context** → Manages persistence through Entity Framework Core  

This design makes it easy to scale and integrate additional modules like:
- Cloud sync  
- User authentication  
- RESTful APIs  

---

## ⚡ Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- Visual Studio / JetBrains Rider
- SQLite (or SQL Server if configured)

### Installation

```bash
git clone https://github.com/BenitoJD/NotinLite.git
cd NotinLite
```
Running the App

Open the solution file in Visual Studio (NotinLite.sln)

Restore dependencies

dotnet restore


Run the application

dotnet run


Navigate to https://localhost:5001 (or the port displayed in console)
