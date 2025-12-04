using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Data
{
    public class BookRepository
    {
        public List<Book> GetAllBooks()
        {
            List<Book> books = new List<Book>();
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                // Updated query to include Category
                string query = @"
                    SELECT b.ISBN, b.Title, b.Publication_Year, b.Edition, 
                           p.Publisher_ID, p.Name as PublisherName,
                           c.Category_ID, c.Name as CategoryName
                    FROM BOOK b
                    LEFT JOIN PUBLISHER p ON b.Publisher_ID = p.Publisher_ID
                    LEFT JOIN BOOK_CATEGORY bc ON b.ISBN = bc.Book_ISBN
                    LEFT JOIN CATEGORY c ON bc.Category_ID = c.Category_ID
                    GROUP BY b.ISBN"; // Group by ISBN to avoid duplicates if multiple categories (taking first one for now)
                
                using (var command = new SqliteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Book book = new Book
                            {
                                ISBN = reader.GetString(0),
                                Title = reader.GetString(1),
                                PublicationYear = reader.IsDBNull(2) ? 0 : reader.GetInt32(2),
                                Edition = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                PublisherId = reader.IsDBNull(4) ? 0 : reader.GetInt32(4),
                                Publisher = new Publisher 
                                { 
                                    PublisherId = reader.IsDBNull(4) ? 0 : reader.GetInt32(4),
                                    Name = reader.IsDBNull(5) ? "Unknown" : reader.GetString(5)
                                }
                            };

                            // Add the first category found (for display purposes in simple UI)
                            if (!reader.IsDBNull(6))
                            {
                                book.Categories.Add(new Category 
                                { 
                                    CategoryId = reader.GetInt32(6),
                                    Name = reader.GetString(7)
                                });
                            }

                            books.Add(book);
                        }
                    }
                }
                
                System.Diagnostics.Debug.WriteLine($"[DEBUG] GetAllBooks returned {books.Count} books");
            }
            return books;
        }

        public void AddBook(Book book, List<int> authorIds, List<int> categoryIds, int numberOfCopies)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string query = @"
                            INSERT INTO BOOK (ISBN, Title, Publication_Year, Edition, Publisher_ID)
                            VALUES (@ISBN, @Title, @Year, @Edition, @PublisherId)";

                        using (var command = new SqliteCommand(query, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@ISBN", book.ISBN);
                            command.Parameters.AddWithValue("@Title", book.Title);
                            command.Parameters.AddWithValue("@Year", book.PublicationYear);
                            command.Parameters.AddWithValue("@Edition", (object)book.Edition ?? DBNull.Value);
                            command.Parameters.AddWithValue("@PublisherId", book.PublisherId);
                            command.ExecuteNonQuery();
                        }

                        // Insert Authors
                        foreach (int authorId in authorIds)
                        {
                            string authQuery = "INSERT INTO BOOK_AUTHOR (Book_ISBN, Author_ID) VALUES (@ISBN, @AuthorId)";
                            using (var cmd = new SqliteCommand(authQuery, connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@ISBN", book.ISBN);
                                cmd.Parameters.AddWithValue("@AuthorId", authorId);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        // Insert Categories
                        foreach (int catId in categoryIds)
                        {
                            string catQuery = "INSERT INTO BOOK_CATEGORY (Book_ISBN, Category_ID) VALUES (@ISBN, @CatId)";
                            using (var cmd = new SqliteCommand(catQuery, connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@ISBN", book.ISBN);
                                cmd.Parameters.AddWithValue("@CatId", catId);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        // Insert Copies
                        for (int i = 0; i < numberOfCopies; i++)
                        {
                            string copyQuery = "INSERT INTO BOOK_COPY (ISBN, Status, Shelf_Location) VALUES (@ISBN, 'Available', 'Main Stack')";
                            using (var cmd = new SqliteCommand(copyQuery, connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@ISBN", book.ISBN);
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

        public void UpdateBook(Book book, List<int> authorIds, List<int> categoryIds)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string query = @"
                            UPDATE BOOK 
                            SET Title = @Title, 
                                Publication_Year = @Year, 
                                Edition = @Edition, 
                                Publisher_ID = @PublisherId
                            WHERE ISBN = @ISBN";

                        using (var command = new SqliteCommand(query, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@ISBN", book.ISBN);
                            command.Parameters.AddWithValue("@Title", book.Title);
                            command.Parameters.AddWithValue("@Year", book.PublicationYear);
                            command.Parameters.AddWithValue("@Edition", (object)book.Edition ?? DBNull.Value);
                            command.Parameters.AddWithValue("@PublisherId", book.PublisherId);
                            command.ExecuteNonQuery();
                        }

                        // Update Authors
                        string deleteAuth = "DELETE FROM BOOK_AUTHOR WHERE Book_ISBN = @ISBN";
                        using (var cmd = new SqliteCommand(deleteAuth, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@ISBN", book.ISBN);
                            cmd.ExecuteNonQuery();
                        }

                        foreach (int authorId in authorIds)
                        {
                            string authQuery = "INSERT INTO BOOK_AUTHOR (Book_ISBN, Author_ID) VALUES (@ISBN, @AuthorId)";
                            using (var cmd = new SqliteCommand(authQuery, connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@ISBN", book.ISBN);
                                cmd.Parameters.AddWithValue("@AuthorId", authorId);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        // Update Categories
                        string deleteCat = "DELETE FROM BOOK_CATEGORY WHERE Book_ISBN = @ISBN";
                        using (var cmd = new SqliteCommand(deleteCat, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@ISBN", book.ISBN);
                            cmd.ExecuteNonQuery();
                        }

                        foreach (int catId in categoryIds)
                        {
                            string catQuery = "INSERT INTO BOOK_CATEGORY (Book_ISBN, Category_ID) VALUES (@ISBN, @CatId)";
                            using (var cmd = new SqliteCommand(catQuery, connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@ISBN", book.ISBN);
                                cmd.Parameters.AddWithValue("@CatId", catId);
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



        public void DeleteBook(string isbn)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 1. Delete loans for this book's copies
                        string deleteLoanQuery = @"
                            DELETE FROM LOAN 
                            WHERE Book_Copy_ID IN (SELECT Copy_ID FROM BOOK_COPY WHERE ISBN = @ISBN)";
                        using (var cmd = new SqliteCommand(deleteLoanQuery, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@ISBN", isbn);
                            cmd.ExecuteNonQuery();
                        }

                        // 2. Delete book copies
                        string deleteCopiesQuery = "DELETE FROM BOOK_COPY WHERE ISBN = @ISBN";
                        using (var cmd = new SqliteCommand(deleteCopiesQuery, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@ISBN", isbn);
                            cmd.ExecuteNonQuery();
                        }

                        // 3. Delete book-author relationships
                        string deleteAuthorsQuery = "DELETE FROM BOOK_AUTHOR WHERE Book_ISBN = @ISBN";
                        using (var cmd = new SqliteCommand(deleteAuthorsQuery, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@ISBN", isbn);
                            cmd.ExecuteNonQuery();
                        }

                        // 4. Delete book-category relationships
                        string deleteCategoriesQuery = "DELETE FROM BOOK_CATEGORY WHERE Book_ISBN = @ISBN";
                        using (var cmd = new SqliteCommand(deleteCategoriesQuery, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@ISBN", isbn);
                            cmd.ExecuteNonQuery();
                        }

                        // 5. Finally, delete the book itself
                        string deleteBookQuery = "DELETE FROM BOOK WHERE ISBN = @ISBN";
                        using (var cmd = new SqliteCommand(deleteBookQuery, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@ISBN", isbn);
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
    }
}
