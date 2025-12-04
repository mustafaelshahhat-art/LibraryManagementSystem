using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Data
{
    public class CategoryRepository
    {
        public List<Category> GetAllCategories()
        {
            List<Category> categories = new List<Category>();
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = "SELECT Category_ID, Name FROM CATEGORY";
                using (var command = new SqliteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            categories.Add(new Category
                            {
                                CategoryId = reader.GetInt32(0),
                                Name = reader.GetString(1)
                            });
                        }
                    }
                }
            }
            return categories;
        }

        public void AddCategory(Category category)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = "INSERT INTO CATEGORY (Name) VALUES (@Name)";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", category.Name);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateCategory(Category category)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = "UPDATE CATEGORY SET Name = @Name WHERE Category_ID = @Id";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", category.CategoryId);
                    command.Parameters.AddWithValue("@Name", category.Name);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteCategory(int id)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Check if category has books
                        string checkQuery = "SELECT COUNT(*) FROM BOOK_CATEGORY WHERE Category_ID = @Id";
                        using (var cmd = new SqliteCommand(checkQuery, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@Id", id);
                            long bookCount = (long)cmd.ExecuteScalar();
                            
                            if (bookCount > 0)
                            {
                                throw new InvalidOperationException($"Cannot delete category. It has {bookCount} book(s) in the library. Please remove the books or change their category first.");
                            }
                        }

                        // Delete category
                        string deleteQuery = "DELETE FROM CATEGORY WHERE Category_ID = @Id";
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
