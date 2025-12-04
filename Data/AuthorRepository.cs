using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Data
{
    public class AuthorRepository
    {
        public List<Author> GetAllAuthors()
        {
            List<Author> authors = new List<Author>();
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = "SELECT Author_ID, Name, Biography, Birth_Date FROM AUTHOR";
                using (var command = new SqliteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            authors.Add(new Author
                            {
                                AuthorId = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Biography = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                BirthDate = reader.IsDBNull(3) ? "" : reader.GetString(3)
                            });
                        }
                    }
                }
            }
            return authors;
        }

        public void AddAuthor(Author author)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = "INSERT INTO AUTHOR (Name, Biography, Birth_Date) VALUES (@Name, @Bio, @BirthDate)";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", author.Name);
                    command.Parameters.AddWithValue("@Bio", (object)author.Biography ?? DBNull.Value);
                    command.Parameters.AddWithValue("@BirthDate", (object)author.BirthDate ?? DBNull.Value);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateAuthor(Author author)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = "UPDATE AUTHOR SET Name = @Name, Biography = @Bio, Birth_Date = @BirthDate WHERE Author_ID = @Id";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", author.AuthorId);
                    command.Parameters.AddWithValue("@Name", author.Name);
                    command.Parameters.AddWithValue("@Bio", (object)author.Biography ?? DBNull.Value);
                    command.Parameters.AddWithValue("@BirthDate", (object)author.BirthDate ?? DBNull.Value);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteAuthor(int id)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Check if author has books
                        string checkQuery = "SELECT COUNT(*) FROM BOOK_AUTHOR WHERE Author_ID = @Id";
                        using (var cmd = new SqliteCommand(checkQuery, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@Id", id);
                            long bookCount = (long)cmd.ExecuteScalar();
                            
                            if (bookCount > 0)
                            {
                                throw new InvalidOperationException($"Cannot delete author. They have {bookCount} book(s) in the library. Please remove the books first.");
                            }
                        }

                        // Delete author
                        string deleteQuery = "DELETE FROM AUTHOR WHERE Author_ID = @Id";
                        using (var cmd = new SqliteCommand(deleteQuery, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@Id", id);
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
