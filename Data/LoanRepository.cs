using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Data
{
    public class LoanRepository
    {
        public List<Loan> GetAllLoans()
        {
            List<Loan> loans = new List<Loan>();
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = @"
                    SELECT l.Loan_ID, l.Book_Copy_ID, l.Member_ID, l.Loan_Date, l.Due_Date, l.Return_Date,
                           m.First_Name, m.Last_Name,
                           bc.ISBN, b.Title
                    FROM LOAN l
                    JOIN MEMBER m ON l.Member_ID = m.Member_ID
                    JOIN BOOK_COPY bc ON l.Book_Copy_ID = bc.Copy_ID
                    JOIN BOOK b ON bc.ISBN = b.ISBN";
                
                using (var command = new SqliteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Loan loan = new Loan
                            {
                                LoanId = reader.GetInt32(0),
                                CopyId = reader.GetInt32(1),
                                MemberId = reader.GetInt32(2),
                                IssueDate = DateTime.Parse(reader.GetString(3)),
                                DueDate = DateTime.Parse(reader.GetString(4)),
                                ReturnDate = reader.IsDBNull(5) ? (DateTime?)null : DateTime.Parse(reader.GetString(5)),
                                
                                Member = new Member
                                {
                                    MemberId = reader.GetInt32(2),
                                    FirstName = reader.GetString(6),
                                    LastName = reader.GetString(7)
                                },
                                BookCopy = new BookCopy
                                {
                                    CopyId = reader.GetInt32(1),
                                    ISBN = reader.GetString(8),
                                    Book = new Book { Title = reader.GetString(9) }
                                }
                            };
                            loans.Add(loan);
                        }
                    }
                }
            }
            return loans;
        }

        public void IssueLoan(int copyId, int memberId, DateTime issueDate, DateTime dueDate)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 1. Insert Loan
                        string insertQuery = @"
                            INSERT INTO LOAN (Book_Copy_ID, Member_ID, Loan_Date, Due_Date, Status)
                            VALUES (@CopyId, @MemberId, @IssueDate, @DueDate, 'Active')";
                        
                        using (var cmd = new SqliteCommand(insertQuery, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@CopyId", copyId);
                            cmd.Parameters.AddWithValue("@MemberId", memberId);
                            cmd.Parameters.AddWithValue("@IssueDate", issueDate.ToString("yyyy-MM-dd"));
                            cmd.Parameters.AddWithValue("@DueDate", dueDate.ToString("yyyy-MM-dd"));
                            cmd.ExecuteNonQuery();
                        }

                        // 2. Update Copy Status
                        string updateCopyQuery = "UPDATE BOOK_COPY SET Status = 'Loaned' WHERE Copy_ID = @CopyId";
                        using (var cmd = new SqliteCommand(updateCopyQuery, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@CopyId", copyId);
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public void ReturnLoan(int loanId, DateTime returnDate)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Get Copy ID first
                        int copyId = 0;
                        string getCopyQuery = "SELECT Book_Copy_ID FROM LOAN WHERE Loan_ID = @LoanId";
                        using (var cmd = new SqliteCommand(getCopyQuery, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@LoanId", loanId);
                            object result = cmd.ExecuteScalar();
                            if (result != null) copyId = Convert.ToInt32(result);
                        }

                        // 1. Update Loan
                        string updateLoanQuery = "UPDATE LOAN SET Return_Date = @ReturnDate WHERE Loan_ID = @LoanId";
                        using (var cmd = new SqliteCommand(updateLoanQuery, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@LoanId", loanId);
                            cmd.Parameters.AddWithValue("@ReturnDate", returnDate.ToString("yyyy-MM-dd"));
                            cmd.ExecuteNonQuery();
                        }

                        // 2. Update Copy Status
                        if (copyId > 0)
                        {
                            string updateCopyQuery = "UPDATE BOOK_COPY SET Status = 'Available' WHERE Copy_ID = @CopyId";
                            using (var cmd = new SqliteCommand(updateCopyQuery, connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@CopyId", copyId);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        // Helper to get available books (not copies) for UI
        public List<Book> GetAvailableBooks()
        {
            List<Book> books = new List<Book>();
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = @"
                    SELECT DISTINCT b.ISBN, b.Title 
                    FROM BOOK b
                    JOIN BOOK_COPY bc ON b.ISBN = bc.ISBN
                    WHERE bc.Status = 'Available'
                    ORDER BY b.Title";

                using (var command = new SqliteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            books.Add(new Book
                            {
                                ISBN = reader.GetString(0),
                                Title = reader.GetString(1)
                            });
                        }
                    }
                }
            }
            return books;
        }

        // Helper to get first available copy for a book
        public int? GetFirstAvailableCopyId(string isbn)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = @"
                    SELECT Copy_ID 
                    FROM BOOK_COPY 
                    WHERE ISBN = @ISBN AND Status = 'Available'
                    LIMIT 1";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ISBN", isbn);
                    var result = command.ExecuteScalar();
                    return result != null ? (int?)Convert.ToInt32(result) : null;
                }
            }
        }

        // Helper to get available copies for UI
        public List<BookCopy> GetAvailableCopies()
        {
            List<BookCopy> copies = new List<BookCopy>();
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = @"
                    SELECT bc.Copy_ID, bc.ISBN, b.Title 
                    FROM BOOK_COPY bc
                    JOIN BOOK b ON bc.ISBN = b.ISBN
                    WHERE bc.Status = 'Available'";

                using (var command = new SqliteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            copies.Add(new BookCopy
                            {
                                CopyId = reader.GetInt32(0),
                                ISBN = reader.GetString(1),
                                Book = new Book { Title = reader.GetString(2) }
                            });
                        }
                    }
                }
            }
            return copies;
        }
        
        // Helper to add a copy (since we don't have a UI for it yet, we might need it)
        public void AddCopy(string isbn, string shelfLocation)
        {
             using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = "INSERT INTO BOOK_COPY (ISBN, Status, Shelf_Location) VALUES (@ISBN, 'Available', @Location)";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ISBN", isbn);
                    command.Parameters.AddWithValue("@Location", shelfLocation);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
