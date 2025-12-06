# 📚 Library Management System

A comprehensive Windows Forms application for managing library operations built with **C# and .NET 6**.

![.NET](https://img.shields.io/badge/.NET-6.0-512BD4?style=flat&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-10.0-239120?style=flat&logo=csharp)
![SQLite](https://img.shields.io/badge/SQLite-3-003B57?style=flat&logo=sqlite)
![Windows Forms](https://img.shields.io/badge/Windows%20Forms-Desktop-0078D4?style=flat&logo=windows)

---

## 📄 Project Documentation

📥 **[View Full Project Documentation (PDF)](Documentation/Library%20Management%20System%20-%20Project%20Documentation.pdf)**

---

## 🎯 Features

### Core Functionality
| Feature | Description |
|---------|-------------|
| 📖 **Book Management** | Add, update, delete books with multi-copy support |
| ✍️ **Author Management** | Manage author information and biographies |
| 🏢 **Publisher Management** | Track publisher details and contacts |
| 🏷️ **Category Management** | Organize books by categories |
| 👥 **Member Management** | Register and manage library members |
| 🔄 **Loan System** | Issue and return books with automatic tracking |
| 📊 **Reports** | View library statistics and analytics |

### Technical Highlights
- ✅ Clean architecture with separation of concerns
- ✅ Repository pattern for data access
- ✅ SQLite database for lightweight storage
- ✅ Automatic database initialization with sample data
- ✅ Modern, responsive UI design
- ✅ Transaction-based operations for data integrity
- ✅ Foreign key constraints enforcement

---

## 📁 Project Structure

```
LibraryManagementSystem/
├── 📂 Forms/                    # UI Layer
│   ├── MainMenuForm.cs         # Main navigation menu
│   ├── BooksForm.cs            # Book management
│   ├── AuthorsForm.cs          # Author management
│   ├── PublishersForm.cs       # Publisher management
│   ├── CategoriesForm.cs       # Category management
│   ├── MembersForm.cs          # Member management
│   ├── LoansForm.cs            # Loan transactions
│   └── ReportsForm.cs          # Reports & statistics
│
├── 📂 Data/                     # Data Access Layer
│   ├── DatabaseHelper.cs       # Database initialization
│   ├── BookRepository.cs       # Book CRUD operations
│   ├── AuthorRepository.cs     # Author CRUD operations
│   ├── PublisherRepository.cs  # Publisher CRUD operations
│   ├── CategoryRepository.cs   # Category CRUD operations
│   ├── MemberRepository.cs     # Member CRUD operations
│   └── LoanRepository.cs       # Loan CRUD operations
│
├── 📂 Models/                   # Domain Models
│   └── Entities.cs             # All entity classes
│
├── 📂 Documentation/            # Project Documentation
│   └── Library Management System - Project Documentation.pdf
│
├── 📄 LibraryDB.sqlite          # SQLite database
├── 📄 schema.sql                # Database schema
├── 📄 Program.cs                # Entry point
└── 📄 README.md                 # This file
```

---

## 🚀 Getting Started

### Prerequisites
- ✅ .NET 6.0 SDK or later
- ✅ Windows OS
- ✅ Visual Studio 2022 (recommended)

### Installation

```bash
# 1. Clone the repository
git clone https://github.com/mustafaelshahhat-art/LibraryManagementSystem.git

# 2. Navigate to project directory
cd LibraryManagementSystem

# 3. Restore dependencies
dotnet restore

# 4. Build the project
dotnet build

# 5. Run the application
dotnet run
```

### First Run
On first launch, the application will automatically:
- ✅ Create the SQLite database (`LibraryDB.sqlite`)
- ✅ Execute the schema from `schema.sql`
- ✅ Populate sample data

---

## 💾 Database Schema

### Entity Relationship
| Table | Description |
|-------|-------------|
| `PUBLISHER` | Publisher information |
| `AUTHOR` | Author details |
| `CATEGORY` | Book categories |
| `BOOK` | Book information (ISBN as PK) |
| `BOOK_AUTHOR` | Many-to-many: Books ↔ Authors |
| `BOOK_CATEGORY` | Many-to-many: Books ↔ Categories |
| `BOOK_COPY` | Physical book copies |
| `MEMBER` | Library members |
| `LOAN` | Loan transactions |

### Relationships
```
PUBLISHER ──1:N──► BOOK
AUTHOR    ◄──M:N──► BOOK (via BOOK_AUTHOR)
CATEGORY  ◄──M:N──► BOOK (via BOOK_CATEGORY)
BOOK      ──1:N──► BOOK_COPY
BOOK_COPY ──1:N──► LOAN
MEMBER    ──1:N──► LOAN
```

---

## 📦 Dependencies

```xml
<PackageReference Include="Microsoft.Data.Sqlite" Version="10.0.0" />
<PackageReference Include="System.Data.SqlClient" Version="4.9.0" />
```

---

## 🛠️ Development

### Reset Database
```bash
del LibraryDB.sqlite
dotnet run
```

### Build for Release
```bash
dotnet build -c Release
dotnet publish -c Release -o ./publish
```

---

## 📝 Business Rules

| Rule | Description |
|------|-------------|
| 📖 ISBN | Must be unique (Primary Key) |
| ⏰ Loan Duration | Default 14 days |
| 📚 Copy Status | Automatically managed (Available/Loaned/Lost) |
| 🗑️ Cascading | Deleting a book removes related copies and loans |
| 🛡️ Protection | Cannot delete members with active loans |

---

## 📄 Documentation

Full project documentation is available in the `Documentation/` folder:
- 📄 **[Project Documentation (PDF)](Documentation/Library%20Management%20System%20-%20Project%20Documentation.pdf)** - Complete documentation with ERD, screenshots, and technical details

---

## 👨‍💻 Author

**Mustafa Elshahhat**  
GitHub: [@mustafaelshahhat-art](https://github.com/mustafaelshahhat-art)

---

## 📄 License

This project is for educational purposes - Database Systems 1 Course Project.

---

**Version**: 1.0  
**Last Updated**: December 2024

⭐ Star this repo if you find it helpful!
