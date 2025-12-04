# 📚 Library Management System

A comprehensive Windows Forms application for managing library operations built with C# and .NET 6.

## 🎯 Features

### Core Functionality
- **Book Management**: Add, update, delete books with multi-copy support
- **Author Management**: Manage author information
- **Publisher Management**: Track publisher details
- **Category Management**: Manage book categories
- **Member Management**: Register and manage library members
- **Loan System**: Issue and return books with automatic copy selection
- **Reports**: View library statistics and analytics

### Technical Highlights
- ✅ Clean architecture with separation of concerns
- ✅ Repository pattern for data access
- ✅ SQLite database for lightweight storage
- ✅ Automatic database initialization with sample data
- ✅ Modern, responsive UI design
- ✅ Transaction-based operations for data integrity
- ✅ Foreign key constraints enforcement
- ✅ Data integrity protection (prevent orphaned records)

## 📁 Project Structure

```
LibraryManagementSystem/
├── Forms/                      # UI Layer
│   ├── MainMenuForm.cs        # Main menu
│   ├── BooksForm.cs           # Book management
│   ├── AuthorsForm.cs         # Author management
│   ├── PublishersForm.cs      # Publisher management
│   ├── CategoriesForm.cs      # Category management
│   ├── MembersForm.cs         # Member management
│   ├── LoansForm.cs           # Loan management
│   └── ReportsForm.cs         # Reports and statistics
├── Data/                       # Data Access Layer
│   ├── DatabaseHelper.cs      # Database initialization & seeding
│   ├── BookRepository.cs      # Book data operations
│   ├── AuthorRepository.cs    # Author data operations
│   ├── PublisherRepository.cs # Publisher data operations
│   ├── CategoryRepository.cs  # Category data operations
│   ├── MemberRepository.cs    # Member data operations
│   └── LoanRepository.cs      # Loan data operations
├── Models/                     # Domain Models
│   └── Entities.cs            # All entity classes
├── Documentation/              # Project Documentation
│   ├── ProjectDocumentation.html
│   ├── ProjectDocumentation.md
│   ├── schema.sql
│   └── (Images)
├── LibraryDB.sqlite           # SQLite database file
└── Program.cs                  # Application entry point
```

## 🚀 Getting Started

### Prerequisites
- .NET 6.0 SDK or later
- Windows OS

### Installation

1. **Clone or download the project**
   ```bash
   cd LibraryManagementSystem
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Build the project**
   ```bash
   dotnet build
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

### First Run
On first launch, the application will:
- Create the SQLite database (`LibraryDB.sqlite`)
- Execute the schema from `schema.sql`
- Populate sample data (5 books, 5 authors, 5 publishers, 5 members, 3 loans)

## 💾 Database Schema

### Tables
- **BOOK**: Book information (ISBN, Title, Year, Edition, Publisher)
- **AUTHOR**: Author details (Name, Biography, Birth Date)
- **PUBLISHER**: Publisher information (Name, Address, Contact)
- **MEMBER**: Library member data (Name, Email, Phone, Join Date)
- **BOOK_COPY**: Physical book copies (Copy ID, ISBN, Status, Location)
- **LOAN**: Loan records (Loan ID, Copy ID, Member ID, Dates, Status)
- **BOOK_AUTHOR**: Many-to-many relationship between books and authors
- **CATEGORY**: Book categories
- **BOOK_CATEGORY**: Many-to-many relationship between books and categories

## 🎨 User Interface

### Main Menu
- Clean, grid-based navigation
- Large, accessible buttons
- Intuitive layout

### Forms
- Consistent design across all forms
- DataGridView for data display
- Input validation
- Clear error messages

## 🔧 Key Features Explained

### Book Management
- Add books with single author selection
- Specify number of copies to create
- Update book information
- Delete books (cascading delete of copies and loans)

### Loan System
- Select available books only (books with available copies)
- Automatic copy selection (system picks first available copy)
- Track loan status (Active/Returned)
- Return books with automatic status update

### Data Seeding
- Automatic on first run
- Granular checks to prevent duplicates
- Transaction-based for data integrity

## 📦 Dependencies

```xml
<PackageReference Include="Microsoft.Data.Sqlite" Version="10.0.0" />
<PackageReference Include="System.Data.SqlClient" Version="4.9.0" />
```

## 🛠️ Development

### Reset Database
To reset the database with fresh sample data:
```bash
del LibraryDB.sqlite
dotnet run
```

### Build for Release
```bash
dotnet build -c Release
```

## 📝 Notes

- **ISBN**: Must be unique (Primary Key)
- **Loan Duration**: Default 14 days
- **Copy Status**: Automatically managed (Available/Loaned)
- **Cascading Deletes**: Deleting a book removes all related copies and loans
- **Delete Protection**:
  - Cannot delete members with active loans
  - Cannot delete authors with existing books
  - Cannot delete publishers with existing books
  - Cannot delete categories with existing books
- **Email Validation**: Basic validation for email format

## 🐛 Known Issues

- Nullable reference type warnings (non-critical, does not affect functionality)

## 📄 License

This project is for educational purposes.

## 👨‍💻 Author

Developed as a comprehensive library management solution using modern C# practices.

---

**Version**: 1.0  
**Last Updated**: December 2025
