using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Data
{
    public class MemberRepository
    {
        public List<Member> GetAllMembers()
        {
            List<Member> members = new List<Member>();
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = "SELECT Member_ID, First_Name, Last_Name, Email, Phone, Address, Join_Date FROM MEMBER";
                
                using (var command = new SqliteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Member member = new Member
                            {
                                MemberId = reader.GetInt32(0),
                                FirstName = reader.GetString(1),
                                LastName = reader.GetString(2),
                                Email = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                Phone = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                Address = reader.IsDBNull(5) ? "" : reader.GetString(5),
                                JoinDate = DateTime.Parse(reader.GetString(6))
                            };
                            members.Add(member);
                        }
                    }
                }
            }
            return members;
        }

        public void AddMember(Member member)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = @"
                    INSERT INTO MEMBER (First_Name, Last_Name, Email, Phone, Address, Join_Date)
                    VALUES (@FirstName, @LastName, @Email, @Phone, @Address, @JoinDate)";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FirstName", member.FirstName);
                    command.Parameters.AddWithValue("@LastName", member.LastName);
                    command.Parameters.AddWithValue("@Email", (object)member.Email ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Phone", (object)member.Phone ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Address", (object)member.Address ?? DBNull.Value);
                    command.Parameters.AddWithValue("@JoinDate", member.JoinDate.ToString("yyyy-MM-dd"));
                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateMember(Member member)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = @"
                    UPDATE MEMBER 
                    SET First_Name = @FirstName, 
                        Last_Name = @LastName, 
                        Email = @Email, 
                        Phone = @Phone, 
                        Address = @Address
                    WHERE Member_ID = @MemberId";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MemberId", member.MemberId);
                    command.Parameters.AddWithValue("@FirstName", member.FirstName);
                    command.Parameters.AddWithValue("@LastName", member.LastName);
                    command.Parameters.AddWithValue("@Email", (object)member.Email ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Phone", (object)member.Phone ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Address", (object)member.Address ?? DBNull.Value);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteMember(int memberId)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 1. Check for active loans
                        string checkActiveLoansQuery = @"
                            SELECT COUNT(*) 
                            FROM LOAN 
                            WHERE Member_ID = @MemberId AND Return_Date IS NULL";
                        
                        using (var cmd = new SqliteCommand(checkActiveLoansQuery, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@MemberId", memberId);
                            long activeLoans = (long)cmd.ExecuteScalar();
                            
                            if (activeLoans > 0)
                            {
                                throw new InvalidOperationException($"Cannot delete member. They have {activeLoans} active loan(s). Please return all books first.");
                            }
                        }

                        // 2. Delete returned loans (history) for this member
                        string deleteLoansQuery = "DELETE FROM LOAN WHERE Member_ID = @MemberId";
                        using (var cmd = new SqliteCommand(deleteLoansQuery, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@MemberId", memberId);
                            cmd.ExecuteNonQuery();
                        }

                        // 3. Delete the member
                        string deleteMemberQuery = "DELETE FROM MEMBER WHERE Member_ID = @MemberId";
                        using (var cmd = new SqliteCommand(deleteMemberQuery, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@MemberId", memberId);
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
