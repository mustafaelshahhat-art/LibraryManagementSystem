using System;
using System.Collections.Generic;

namespace LibraryManagementSystem.Models
{
    public class Author
    {
        public int AuthorId { get; set; }
        public string Name { get; set; }
        public string Biography { get; set; }
        public string BirthDate { get; set; }
    }

    public class Publisher
    {
        public int PublisherId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string ContactInfo { get; set; }
    }

    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
    }

    public class Book
    {
        public string ISBN { get; set; }
        public string Title { get; set; }
        public int PublicationYear { get; set; }
        public string Edition { get; set; }
        public int PublisherId { get; set; }
        
        // Navigation properties
        public Publisher Publisher { get; set; }
        public List<Author> Authors { get; set; } = new List<Author>();
        public List<Category> Categories { get; set; } = new List<Category>();
    }

    public class BookCopy
    {
        public int CopyId { get; set; }
        public string ISBN { get; set; }
        public string Status { get; set; } // Available, Loaned, Lost
        public string ShelfLocation { get; set; }

        // Navigation property
        public Book Book { get; set; }
        
        public string BookTitleWithId => $"{Book?.Title ?? ISBN} (Copy {CopyId})";
    }

    public class Member
    {
        public int MemberId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public DateTime JoinDate { get; set; }
    }

    public class Loan
    {
        public int LoanId { get; set; }
        public int CopyId { get; set; }
        public int MemberId { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        // Navigation properties
        public BookCopy BookCopy { get; set; }
        public Member Member { get; set; }
    }
}
