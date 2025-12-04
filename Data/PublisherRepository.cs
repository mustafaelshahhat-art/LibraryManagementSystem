using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Data
{
    public class PublisherRepository
    {
        public List<Publisher> GetAllPublishers()
        {
            List<Publisher> publishers = new List<Publisher>();
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = "SELECT Publisher_ID, Name, Address, Contact_Info FROM PUBLISHER";
                using (var command = new SqliteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            publishers.Add(new Publisher
                            {
                                PublisherId = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Address = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                ContactInfo = reader.IsDBNull(3) ? "" : reader.GetString(3)
                            });
                        }
                    }
                }
            }
            return publishers;
        }

        public void AddPublisher(Publisher publisher)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = "INSERT INTO PUBLISHER (Name, Address, Contact_Info) VALUES (@Name, @Address, @Contact)";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", publisher.Name);
                    command.Parameters.AddWithValue("@Address", (object)publisher.Address ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Contact", (object)publisher.ContactInfo ?? DBNull.Value);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdatePublisher(Publisher publisher)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = "UPDATE PUBLISHER SET Name = @Name, Address = @Address, Contact_Info = @Contact WHERE Publisher_ID = @Id";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", publisher.PublisherId);
                    command.Parameters.AddWithValue("@Name", publisher.Name);
                    command.Parameters.AddWithValue("@Address", (object)publisher.Address ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Contact", (object)publisher.ContactInfo ?? DBNull.Value);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeletePublisher(int id)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Check if publisher has books
                        string checkQuery = "SELECT COUNT(*) FROM BOOK WHERE Publisher_ID = @Id";
                        using (var cmd = new SqliteCommand(checkQuery, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@Id", id);
                            long bookCount = (long)cmd.ExecuteScalar();
                            
                            if (bookCount > 0)
                            {
                                throw new InvalidOperationException($"Cannot delete publisher. They have {bookCount} book(s) in the library. Please remove the books first.");
                            }
                        }

                        // Delete publisher
                        string deleteQuery = "DELETE FROM PUBLISHER WHERE Publisher_ID = @Id";
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
