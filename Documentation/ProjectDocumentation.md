# 📚 Library Management System

**Complete Project Documentation**

---

**University:** [University Name]  
**Faculty:** [Faculty Name]  
**Student Name:** [Student Name]  
**Supervisor:** [Supervisor Name]  
**Academic Year:** 2025 - 2026  

---

## Table of Contents

### Part I: Introduction
1. [Project Overview](#1-project-overview)
2. [System Architecture](#2-system-architecture)

### Part II: User Interface & Features
3. [Main Menu](#3-main-menu)
4. [Book Management](#4-book-management)
5. [Author Management](#5-author-management)
6. [Publisher Management](#6-publisher-management)
7. [Category Management](#7-category-management)
8. [Member Management](#8-member-management)
9. [Loan Management](#9-loan-management)
10. [Reports & Statistics](#10-reports--statistics)

### Part III: Technical Architecture
11. [Database Design](#11-database-design)
12. [Entity Relationship Diagram](#12-entity-relationship-diagram)
13. [Database Tables](#13-database-tables)

### Part IV: Security & Data Integrity
14. [Security Features](#14-security-features)

### Part V: Conclusion
15. [Conclusion & Future Enhancements](#15-conclusion--future-enhancements)

---

# Part I: Introduction

## 1. Project Overview

The **Library Management System** is a comprehensive desktop application developed using **C# and .NET 6** with **Windows Forms** to modernize the operations of university and public libraries. It serves as a centralized platform for librarians to manage their core assets—books, members, and circulation records—efficiently and accurately.

### Project Goals:
- Replace traditional manual logging systems
- Minimize human error in library operations
- Speed up the borrowing and return processes
- Provide instant access to book availability status
- Generate comprehensive reports and statistics

### Technologies Used:
- **Programming Language:** C# (.NET 6)
- **UI Framework:** Windows Forms
- **Database:** SQLite
- **Architecture Pattern:** Repository Pattern
- **Data Access:** ADO.NET with Microsoft.Data.Sqlite

### Key Features:
✅ Complete CRUD operations for all entities  
✅ Multi-copy book management  
✅ Automatic loan tracking  
✅ Foreign key constraints enforcement  
✅ Transaction-based operations  
✅ Data integrity protection  
✅ Input validation  
✅ Real-time reports and statistics  

---

## 2. System Architecture

The system follows a **three-layer architecture**:

### 1. Presentation Layer (Forms/)
- User interface components
- Input validation
- User interaction handling

### 2. Business Logic Layer (Data/)
- Repository pattern implementation
- Database operations
- Transaction management

### 3. Data Layer (Models/)
- Entity definitions
- Data models

---

# Part II: User Interface & Features

## 3. Main Menu

The main menu provides a clean, intuitive interface with large, accessible buttons for navigating to different modules of the system.

![Main Menu](MainMenu.png)
*Figure 1: Main Menu - Central navigation hub with Arabic interface*

### Features:
- **إدارة الكتب** (Book Management)
- **إدارة الأعضاء** (Member Management)
- **إدارة الإعارات** (Loan Management)
- **التقارير** (Reports)
- **إدارة المؤلفين** (Author Management)
- **إدارة الناشرين** (Publisher Management)
- **إدارة الفئات** (Category Management)

---

## 4. Book Management

The Book Management module is the central repository for the library's collection, allowing detailed cataloging and inventory control.

![Book Management](Books.png)
*Figure 2: Book Management Interface - Add, update, and delete books with multi-copy support*

### Key Functionalities:

#### Add Books:
- Enter ISBN (unique identifier)
- Book title (required)
- Publication year
- Edition information
- Select publisher from dropdown
- Select author from dropdown
- Select category from dropdown
- Specify number of copies to create

#### Update Books:
- Modify book information
- Update publisher, author, or category associations
- Cannot modify ISBN (primary key)

#### Delete Books:
- Cascading delete removes all copies and associated loans
- Maintains data integrity

#### Features:
- **Multi-Copy Support:** Create multiple physical copies of the same book
- **Relationship Management:** Link books to publishers, authors, and categories
- **Search & Filter:** DataGridView with sortable columns
- **Validation:** ISBN uniqueness check, required field validation

---

## 5. Author Management

This module maintains a comprehensive database of authors, ensuring that book records are linked to valid, existing author profiles.

![Author Management](Authors.png)
*Figure 3: Author Management - Manage author information and biographies*

### Features:

#### Author Information:
- **Author ID:** Auto-generated unique identifier
- **Name:** Author's full name (required)
- **Biography:** Detailed author background
- **Birth Date:** Author's date of birth

#### Operations:
- **Add:** Create new author profiles
- **Update:** Modify existing author information
- **Delete:** Remove authors (protected if linked to books)
- **Clear:** Reset form fields

#### Delete Protection:
⚠️ Cannot delete authors who have books in the library. This prevents orphaned book records and maintains referential integrity.

---

## 6. Publisher Management

Tracks publishing houses to facilitate book acquisition and inventory organization.

![Publisher Management](Publishers.png)
*Figure 4: Publisher Management - Track publisher details and contact information*

### Publisher Information:

#### Fields:
- **Publisher ID:** Auto-generated unique identifier
- **Name:** Publisher name (required)
- **Address:** Publisher's physical address
- **Contact Info:** Email or phone contact

#### Operations:
- **Add:** Register new publishers
- **Update:** Modify publisher details
- **Delete:** Remove publishers (protected if linked to books)
- **Clear:** Reset form

#### Delete Protection:
⚠️ Cannot delete publishers who have books in the library catalog.

---

## 7. Category Management

Organizes books into genres and categories for easier browsing and classification.

![Category Management](Categories.png)
*Figure 5: Category Management - Organize books by genre and category*

### Features:

#### Category Information:
- **Category ID:** Auto-generated unique identifier
- **Name:** Category name (e.g., Fantasy, Science Fiction, Mystery)

#### Pre-loaded Categories:
1. Fantasy
2. Science Fiction
3. Mystery
4. Horror
5. Adventure

#### Operations:
- **Add:** Create new categories
- **Update:** Modify category names
- **Delete:** Remove categories (protected if linked to books)
- **Clear:** Reset form

#### Delete Protection:
⚠️ Cannot delete categories that are assigned to books in the library.

---

## 8. Member Management

Manages the library's patron database, storing contact information and membership status.

![Member Management](Members.png)
*Figure 6: Member Management - Register and manage library members*

### Member Information:

#### Fields:
- **Member ID:** Auto-generated unique identifier
- **First Name:** Member's first name (required)
- **Last Name:** Member's last name (required)
- **Email:** Email address (validated)
- **Phone:** Contact phone number
- **Address:** Physical address
- **Join Date:** Automatically set to current date

#### Operations:
- **Add:** Register new members
- **Update:** Modify member information
- **Delete:** Remove members (protected if they have active loans)
- **Clear:** Reset form

#### Email Validation:
✅ Basic email format validation (must contain @ and .)

#### Delete Protection:
⚠️ Cannot delete members with active (unreturned) loans. Returned loans are automatically deleted when the member is removed.

---

## 9. Loan Management

The circulation engine of the system, handling the complete check-out and check-in processes.

![Loan Management](Loans.png)
*Figure 7: Loan Management - Issue and return books with automatic tracking*

### Loan Workflow:

#### Issue Loan:
1. Select member from dropdown
2. Select available book from dropdown (only books with available copies shown)
3. Set issue date (default: today)
4. Set due date (default: 14 days from issue date)
5. Click "Issue Loan"
6. System automatically:
   - Selects first available copy
   - Creates loan record
   - Updates copy status to "Loaned"

#### Return Book:
1. Select loan from the grid
2. Click "Return"
3. System automatically:
   - Sets return date
   - Updates copy status to "Available"
   - Marks loan as "Returned"

#### Features:
- **Automatic Copy Selection:** System picks the first available copy
- **Status Tracking:** Active vs. Returned loans
- **Date Management:** Issue date, due date, return date
- **Add Copy Button:** Quick utility to add copies for testing

#### Loan Information Displayed:
- Loan ID
- Member name
- Book title
- Copy ID
- Issue date
- Due date
- Return date (or "Active" if not returned)

---

## 10. Reports & Statistics

Provides visual insights into library operations to aid in decision-making.

![Reports Dashboard](Reports.png)
*Figure 8: Reports & Statistics - Real-time analytics and key performance indicators*

### Key Performance Indicators:

#### 1. Total Books (Blue)
- Count of unique book titles in the library
- Current value: 5 books

#### 2. Total Members (Green)
- Count of registered library members
- Current value: 5 members

#### 3. Active Loans (Orange)
- Count of currently unreturned loans
- Current value: 2 active loans

#### 4. Overdue Loans (Red)
- Count of loans past their due date
- Current value: 2 overdue loans

### Benefits:
- **Real-time Data:** Statistics update automatically
- **Visual Design:** Color-coded cards for quick recognition
- **Decision Support:** Helps librarians identify issues quickly

---

# Part III: Technical Architecture

## 11. Database Design

The system uses **SQLite** as its database engine, providing:
- Lightweight, serverless architecture
- Zero configuration required
- Cross-platform compatibility
- ACID compliance
- Full SQL support

### Database Features:
✅ **Foreign Key Constraints:** Enabled to enforce referential integrity  
✅ **Auto-increment Primary Keys:** For all ID fields  
✅ **Check Constraints:** For status fields (e.g., Available, Loaned, Lost)  
✅ **Default Values:** For join dates and timestamps  
✅ **NOT NULL Constraints:** For required fields  

---

## 12. Entity Relationship Diagram

The following ERD illustrates the complete database structure with all 9 tables and their relationships:

![Entity Relationship Diagram](ERD.png)
*Figure 9: Complete ERD showing all tables, relationships, and cardinalities*

### Relationship Summary:

#### One-to-Many Relationships:
- **PUBLISHER → BOOK:** One publisher can publish many books
- **BOOK → BOOK_COPY:** One book can have many physical copies
- **BOOK_COPY → LOAN:** One copy can be loaned multiple times (over time)
- **MEMBER → LOAN:** One member can have multiple loans

#### Many-to-Many Relationships:
- **BOOK ↔ AUTHOR:** Books can have multiple authors, authors can write multiple books (via BOOK_AUTHOR junction table)
- **BOOK ↔ CATEGORY:** Books can belong to multiple categories, categories can contain multiple books (via BOOK_CATEGORY junction table)

---

## 13. Database Tables

### Complete Schema (9 Tables)

#### 1. PUBLISHER Table
```sql
CREATE TABLE PUBLISHER (
    Publisher_ID INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Address TEXT,
    Contact_Info TEXT
);
```
**Purpose:** Stores publishing house information  
**Primary Key:** Publisher_ID  
**Required Fields:** Name  

---

#### 2. AUTHOR Table
```sql
CREATE TABLE AUTHOR (
    Author_ID INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Biography TEXT,
    Birth_Date TEXT
);
```
**Purpose:** Stores author information and biographies  
**Primary Key:** Author_ID  
**Required Fields:** Name  

---

#### 3. CATEGORY Table
```sql
CREATE TABLE CATEGORY (
    Category_ID INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL
);
```
**Purpose:** Stores book categories/genres  
**Primary Key:** Category_ID  
**Required Fields:** Name  

---

#### 4. BOOK Table
```sql
CREATE TABLE BOOK (
    ISBN TEXT PRIMARY KEY,
    Title TEXT NOT NULL,
    Publication_Year INTEGER,
    Edition TEXT,
    Publisher_ID INTEGER,
    FOREIGN KEY (Publisher_ID) REFERENCES PUBLISHER(Publisher_ID)
);
```
**Purpose:** Stores core book information  
**Primary Key:** ISBN (unique book identifier)  
**Foreign Keys:** Publisher_ID → PUBLISHER  
**Required Fields:** ISBN, Title  

---

#### 5. BOOK_AUTHOR Table (Junction)
```sql
CREATE TABLE BOOK_AUTHOR (
    Book_ISBN TEXT,
    Author_ID INTEGER,
    PRIMARY KEY (Book_ISBN, Author_ID),
    FOREIGN KEY (Book_ISBN) REFERENCES BOOK(ISBN),
    FOREIGN KEY (Author_ID) REFERENCES AUTHOR(Author_ID)
);
```
**Purpose:** Many-to-many relationship between books and authors  
**Composite Primary Key:** (Book_ISBN, Author_ID)  
**Foreign Keys:** Book_ISBN → BOOK, Author_ID → AUTHOR  

---

#### 6. BOOK_CATEGORY Table (Junction)
```sql
CREATE TABLE BOOK_CATEGORY (
    Book_ISBN TEXT,
    Category_ID INTEGER,
    PRIMARY KEY (Book_ISBN, Category_ID),
    FOREIGN KEY (Book_ISBN) REFERENCES BOOK(ISBN),
    FOREIGN KEY (Category_ID) REFERENCES CATEGORY(Category_ID)
);
```
**Purpose:** Many-to-many relationship between books and categories  
**Composite Primary Key:** (Book_ISBN, Category_ID)  
**Foreign Keys:** Book_ISBN → BOOK, Category_ID → CATEGORY  

---

#### 7. BOOK_COPY Table
```sql
CREATE TABLE BOOK_COPY (
    Copy_ID INTEGER PRIMARY KEY AUTOINCREMENT,
    ISBN TEXT NOT NULL,
    Status TEXT CHECK (Status IN ('Available', 'Loaned', 'Lost')),
    Shelf_Location TEXT,
    FOREIGN KEY (ISBN) REFERENCES BOOK(ISBN)
);
```
**Purpose:** Tracks individual physical copies of books  
**Primary Key:** Copy_ID  
**Foreign Keys:** ISBN → BOOK  
**Required Fields:** Copy_ID, ISBN  
**Check Constraint:** Status must be 'Available', 'Loaned', or 'Lost'  

---

#### 8. MEMBER Table
```sql
CREATE TABLE MEMBER (
    Member_ID INTEGER PRIMARY KEY AUTOINCREMENT,
    First_Name TEXT NOT NULL,
    Last_Name TEXT NOT NULL,
    Email TEXT,
    Phone TEXT,
    Address TEXT,
    Join_Date TEXT DEFAULT CURRENT_DATE
);
```
**Purpose:** Stores library member information  
**Primary Key:** Member_ID  
**Required Fields:** First_Name, Last_Name  
**Default Value:** Join_Date = current date  

---

#### 9. LOAN Table
```sql
CREATE TABLE LOAN (
    Loan_ID INTEGER PRIMARY KEY AUTOINCREMENT,
    Book_Copy_ID INTEGER NOT NULL,
    Member_ID INTEGER NOT NULL,
    Loan_Date TEXT NOT NULL,
    Due_Date TEXT NOT NULL,
    Return_Date TEXT,
    Status TEXT,
    FOREIGN KEY (Book_Copy_ID) REFERENCES BOOK_COPY(Copy_ID),
    FOREIGN KEY (Member_ID) REFERENCES MEMBER(Member_ID)
);
```
**Purpose:** Tracks book loans and returns  
**Primary Key:** Loan_ID  
**Foreign Keys:** Book_Copy_ID → BOOK_COPY, Member_ID → MEMBER  
**Required Fields:** Book_Copy_ID, Member_ID, Loan_Date, Due_Date  

---

# Part IV: Security & Data Integrity

## 14. Security Features

### 1. Foreign Key Enforcement
```csharp
public static string ConnectionString = "Data Source=LibraryDB.sqlite;Foreign Keys=True;";
```
SQLite foreign key constraints are **explicitly enabled** to prevent:
- Invalid references between tables
- Orphaned records
- Data inconsistencies

### 2. Delete Protection

The system implements intelligent delete protection to maintain data integrity:

#### Members:
```csharp
// Cannot delete members with active loans
if (activeLoans > 0) {
    throw new InvalidOperationException(
        $"Cannot delete member. They have {activeLoans} active loan(s). 
         Please return all books first."
    );
}
```
✅ Returned loans are automatically deleted  
⚠️ Active loans prevent member deletion  

#### Authors:
```csharp
// Cannot delete authors with books
if (bookCount > 0) {
    throw new InvalidOperationException(
        $"Cannot delete author. They have {bookCount} book(s) in the library. 
         Please remove the books first."
    );
}
```

#### Publishers:
```csharp
// Cannot delete publishers with books
if (bookCount > 0) {
    throw new InvalidOperationException(
        $"Cannot delete publisher. They have {bookCount} book(s) in the library. 
         Please remove the books first."
    );
}
```

#### Categories:
```csharp
// Cannot delete categories with books
if (bookCount > 0) {
    throw new InvalidOperationException(
        $"Cannot delete category. It has {bookCount} book(s) in the library. 
         Please remove the books or change their category first."
    );
}
```

### 3. Transaction-Based Operations

All multi-step operations use database transactions:

```csharp
using (var transaction = connection.BeginTransaction())
{
    try
    {
        // Multiple database operations
        // ...
        transaction.Commit();
    }
    catch
    {
        transaction.Rollback();
        throw;
    }
}
```

**Benefits:**
- **Atomicity:** All operations succeed or all fail
- **Consistency:** Database remains in valid state
- **Isolation:** Concurrent operations don't interfere
- **Durability:** Committed changes are permanent

### 4. Input Validation

#### Email Validation:
```csharp
if (!string.IsNullOrWhiteSpace(txtEmail.Text))
{
    if (!txtEmail.Text.Contains("@") || !txtEmail.Text.Contains("."))
    {
        MessageBox.Show("Please enter a valid email address.");
        return false;
    }
}
```

#### Required Field Validation:
- ISBN, Title for books
- First Name, Last Name for members
- Name for authors, publishers, categories

### 5. Cascading Deletes

When deleting a book, the system automatically:
1. Deletes all loans for the book's copies
2. Deletes all physical copies
3. Deletes author associations (BOOK_AUTHOR)
4. Deletes category associations (BOOK_CATEGORY)
5. Finally deletes the book record

This ensures no orphaned records remain in the database.

---

# Part V: Conclusion

## 15. Conclusion & Future Enhancements

### Project Summary

The **Library Management System** successfully achieves its primary objectives:

✅ **Automation:** Eliminates manual record-keeping  
✅ **Efficiency:** Speeds up borrowing and return processes  
✅ **Accuracy:** Minimizes human error through validation  
✅ **Integrity:** Maintains data consistency through constraints  
✅ **Insights:** Provides real-time reports and statistics  

### Technical Achievements

1. **Clean Architecture:** Separation of concerns with Repository pattern
2. **Data Integrity:** Foreign key constraints and transaction management
3. **User Experience:** Intuitive Arabic interface with clear workflows
4. **Scalability:** Modular design allows easy feature additions
5. **Reliability:** Comprehensive error handling and validation

### System Statistics

- **9 Database Tables:** Fully normalized schema
- **8 User Interfaces:** Complete CRUD operations
- **7 Modules:** Books, Authors, Publishers, Categories, Members, Loans, Reports
- **4 Security Features:** Foreign keys, delete protection, transactions, validation

---

**Prepared by:** [Student Name]  
**For:** [University Name]  
**Academic Year:** 2025 - 2026

