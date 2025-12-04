using System;
using Microsoft.Data.Sqlite;

namespace LibraryManagementSystem.Data
{
    public static class DatabaseHelper
    {
        // SQLite connection string
        // The database file 'LibraryDB.sqlite' will be created in the application's execution directory.
        public static string DbPath = "LibraryDB.sqlite";

        public static void InitializeDatabase()
        {
            bool isNew = !System.IO.File.Exists(DbPath);

            using (var connection = GetConnection())
            {
                connection.Open();
                
                if (isNew)
                {
                    try
                    {
                        // Try to load schema from multiple possible locations
                        string schemaPath = "schema.sql";
                        if (!System.IO.File.Exists(schemaPath))
                        {
                            // Try Documentation folder
                            schemaPath = System.IO.Path.Combine("Documentation", "schema.sql");
                        }
                        
                        if (System.IO.File.Exists(schemaPath))
                        {
                            string schema = System.IO.File.ReadAllText(schemaPath);
                            using (var command = new SqliteCommand(schema, connection))
                            {
                                command.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            throw new System.IO.FileNotFoundException("schema.sql not found. Please ensure schema.sql exists in the project root or Documentation folder.");
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Failed to initialize database schema: " + ex.Message, ex);
                    }
                }
                
                SeedData(connection);
            }
        }

        private static void SeedData(SqliteConnection connection)
        {
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // Check if authors exist
                    long authorCount = 0;
                    using (var cmd = new SqliteCommand("SELECT COUNT(*) FROM AUTHOR", connection, transaction))
                    {
                        object? result = cmd.ExecuteScalar();
                        authorCount = result != null ? (long)result : 0;
                    }

                    System.Diagnostics.Debug.WriteLine($"[DEBUG] Author Count: {authorCount}");

                    System.Diagnostics.Debug.WriteLine($"[DEBUG] Seeding data started. Author count before seed: {authorCount}");

                    if (authorCount == 0)
                    {
                        System.Diagnostics.Debug.WriteLine("[DEBUG] Inserting authors...");
                        // Authors
                        var authors = new[]
                        {
                            "('J.K. Rowling', 'British author, best known for the Harry Potter series.')",
                            "('George R.R. Martin', 'American novelist and short story writer, A Song of Ice and Fire.')",
                            "('J.R.R. Tolkien', 'English writer, poet, philologist, and academic, The Lord of the Rings.')",
                            "('Agatha Christie', 'English writer known for her sixty-six detective novels.')",
                            "('Stephen King', 'American author of horror, supernatural fiction, suspense, crime, science-fiction, and fantasy novels.')"
                        };
                        int authorInserted = 0;
                        foreach (var a in authors)
                        {
                            new SqliteCommand($"INSERT INTO AUTHOR (Name, Biography) VALUES {a}", connection, transaction).ExecuteNonQuery();
                            authorInserted++;
                        }
                        System.Diagnostics.Debug.WriteLine($"[DEBUG] {authorInserted} authors inserted");

                        System.Diagnostics.Debug.WriteLine("[DEBUG] Inserting publishers...");
                        var publishers = new[]
                        {
                            "('Bloomsbury', 'London, UK', 'contact@bloomsbury.com')",
                            "('Bantam Books', 'New York, USA', 'info@bantam.com')",
                            "('Allen & Unwin', 'Sydney, Australia', 'support@allenandunwin.com')",
                            "('HarperCollins', 'New York, USA', 'help@harpercollins.com')",
                            "('Scribner', 'New York, USA', 'contact@scribner.com')"
                        };
                        foreach (var p in publishers)
                        {
                            new SqliteCommand($"INSERT INTO PUBLISHER (Name, Address, Contact_Info) VALUES {p}", connection, transaction).ExecuteNonQuery();
                        }

                        System.Diagnostics.Debug.WriteLine("[DEBUG] Inserting categories...");
                        var categories = new[]
                        {
                            "('Fantasy')",
                            "('Science Fiction')",
                            "('Mystery')",
                            "('Horror')",
                            "('Adventure')"
                        };
                        foreach (var c in categories)
                        {
                            new SqliteCommand($"INSERT INTO CATEGORY (Name) VALUES {c}", connection, transaction).ExecuteNonQuery();
                        }

                        // Members
                        var members = new[]
                        {
                            "('John', 'Doe', 'john.doe@email.com', '555-0101', '2023-01-15')",
                            "('Jane', 'Smith', 'jane.smith@email.com', '555-0102', '2023-02-20')",
                            "('Alice', 'Johnson', 'alice.j@email.com', '555-0103', '2023-03-10')",
                            "('Bob', 'Wilson', 'bob.w@email.com', '555-0104', '2023-04-05')",
                            "('Eva', 'Brown', 'eva.b@email.com', '555-0105', '2023-05-12')"
                        };
                        foreach (var m in members)
                        {
                            new SqliteCommand($"INSERT INTO MEMBER (First_Name, Last_Name, Email, Phone, Join_Date) VALUES {m}", connection, transaction).ExecuteNonQuery();
                        }

                        // Books
                        // Book 1
                        new SqliteCommand("INSERT INTO BOOK (ISBN, Title, Publication_Year, Edition, Publisher_ID) VALUES ('978-0747532743', 'Harry Potter and the Philosopher''s Stone', 1997, '1st', 1)", connection, transaction).ExecuteNonQuery();
                        new SqliteCommand("INSERT INTO BOOK_AUTHOR (Book_ISBN, Author_ID) VALUES ('978-0747532743', 1)", connection, transaction).ExecuteNonQuery();
                        new SqliteCommand("INSERT INTO BOOK_CATEGORY (Book_ISBN, Category_ID) VALUES ('978-0747532743', 1)", connection, transaction).ExecuteNonQuery();
                        new SqliteCommand("INSERT INTO BOOK_COPY (ISBN, Status, Shelf_Location) VALUES ('978-0747532743', 'Available', 'Fantasy-01')", connection, transaction).ExecuteNonQuery();

                        // Book 2
                        new SqliteCommand("INSERT INTO BOOK (ISBN, Title, Publication_Year, Edition, Publisher_ID) VALUES ('978-0553103540', 'A Game of Thrones', 1996, '1st', 2)", connection, transaction).ExecuteNonQuery();
                        new SqliteCommand("INSERT INTO BOOK_AUTHOR (Book_ISBN, Author_ID) VALUES ('978-0553103540', 2)", connection, transaction).ExecuteNonQuery();
                        new SqliteCommand("INSERT INTO BOOK_CATEGORY (Book_ISBN, Category_ID) VALUES ('978-0553103540', 1)", connection, transaction).ExecuteNonQuery();
                        new SqliteCommand("INSERT INTO BOOK_COPY (ISBN, Status, Shelf_Location) VALUES ('978-0553103540', 'Available', 'Fantasy-02')", connection, transaction).ExecuteNonQuery();

                        // Book 3
                        new SqliteCommand("INSERT INTO BOOK (ISBN, Title, Publication_Year, Edition, Publisher_ID) VALUES ('978-0618640157', 'The Fellowship of the Ring', 1954, '1st', 3)", connection, transaction).ExecuteNonQuery();
                        new SqliteCommand("INSERT INTO BOOK_AUTHOR (Book_ISBN, Author_ID) VALUES ('978-0618640157', 3)", connection, transaction).ExecuteNonQuery();
                        new SqliteCommand("INSERT INTO BOOK_CATEGORY (Book_ISBN, Category_ID) VALUES ('978-0618640157', 1)", connection, transaction).ExecuteNonQuery();
                        new SqliteCommand("INSERT INTO BOOK_COPY (ISBN, Status, Shelf_Location) VALUES ('978-0618640157', 'Available', 'Fantasy-03')", connection, transaction).ExecuteNonQuery();

                        // Book 4
                        new SqliteCommand("INSERT INTO BOOK (ISBN, Title, Publication_Year, Edition, Publisher_ID) VALUES ('978-0007119318', 'Murder on the Orient Express', 1934, '1st', 4)", connection, transaction).ExecuteNonQuery();
                        new SqliteCommand("INSERT INTO BOOK_AUTHOR (Book_ISBN, Author_ID) VALUES ('978-0007119318', 4)", connection, transaction).ExecuteNonQuery();
                        new SqliteCommand("INSERT INTO BOOK_CATEGORY (Book_ISBN, Category_ID) VALUES ('978-0007119318', 3)", connection, transaction).ExecuteNonQuery();
                        new SqliteCommand("INSERT INTO BOOK_COPY (ISBN, Status, Shelf_Location) VALUES ('978-0007119318', 'Available', 'Mystery-01')", connection, transaction).ExecuteNonQuery();

                        // Book 5
                        new SqliteCommand("INSERT INTO BOOK (ISBN, Title, Publication_Year, Edition, Publisher_ID) VALUES ('978-1501142970', 'It', 1986, '1st', 5)", connection, transaction).ExecuteNonQuery();
                        new SqliteCommand("INSERT INTO BOOK_AUTHOR (Book_ISBN, Author_ID) VALUES ('978-1501142970', 5)", connection, transaction).ExecuteNonQuery();
                        new SqliteCommand("INSERT INTO BOOK_CATEGORY (Book_ISBN, Category_ID) VALUES ('978-1501142970', 4)", connection, transaction).ExecuteNonQuery();
                        new SqliteCommand("INSERT INTO BOOK_COPY (ISBN, Status, Shelf_Location) VALUES ('978-1501142970', 'Available', 'Horror-01')", connection, transaction).ExecuteNonQuery();
                    }

                    // Check if loans exist
                    long loanCount = 0;
                    using (var cmd = new SqliteCommand("SELECT COUNT(*) FROM LOAN", connection, transaction))
                    {
                        loanCount = (long)cmd.ExecuteScalar();
                    }

                    if (loanCount == 0 && authorCount == 0) // Only add loans if we just added books (to ensure IDs match) OR check if books exist
                    {
                        // Safer to only add loans if we know the IDs are valid. 
                        // Assuming IDs 1-3 exist for Members and Copies if we just seeded them.
                        // If authorCount == 0, we just seeded everything.
                        
                        if (authorCount == 0) 
                        {
                            // Loans
                            new SqliteCommand("INSERT INTO LOAN (Book_Copy_ID, Member_ID, Loan_Date, Due_Date, Status) VALUES (1, 1, '2023-10-01', '2023-10-15', 'Active')", connection, transaction).ExecuteNonQuery();
                            new SqliteCommand("UPDATE BOOK_COPY SET Status = 'Loaned' WHERE Copy_ID = 1", connection, transaction).ExecuteNonQuery();

                            new SqliteCommand("INSERT INTO LOAN (Book_Copy_ID, Member_ID, Loan_Date, Due_Date, Return_Date, Status) VALUES (2, 2, '2023-09-01', '2023-09-15', '2023-09-10', 'Returned')", connection, transaction).ExecuteNonQuery();
                            
                            new SqliteCommand("INSERT INTO LOAN (Book_Copy_ID, Member_ID, Loan_Date, Due_Date, Status) VALUES (3, 3, '2023-08-01', '2023-08-15', 'Active')", connection, transaction).ExecuteNonQuery();
                            new SqliteCommand("UPDATE BOOK_COPY SET Status = 'Loaned' WHERE Copy_ID = 3", connection, transaction).ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                    System.Diagnostics.Debug.WriteLine("[DEBUG] Seed data committed successfully");
                }
                catch (Exception seedEx)
                {
                    System.Diagnostics.Debug.WriteLine($"[ERROR] Seed data failed: {seedEx.Message}");
                    System.Diagnostics.Debug.WriteLine($"[ERROR] Stack trace: {seedEx.StackTrace}");
                    transaction.Rollback();
                    throw;
                }
            }
        }
        public static string ConnectionString = "Data Source=LibraryDB.sqlite;Foreign Keys=True;";

        public static SqliteConnection GetConnection()
        {
            return new SqliteConnection(ConnectionString);
        }
    }
}
