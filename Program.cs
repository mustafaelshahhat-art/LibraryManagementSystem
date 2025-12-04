using System;
using System.Windows.Forms;
using LibraryManagementSystem.Data;

namespace LibraryManagementSystem;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        try
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            // Initialize Database and Seed Data
            DatabaseHelper.InitializeDatabase();

            Application.Run(new LibraryManagementSystem.Forms.MainMenuForm());
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Fatal Error: {ex.Message}\n\n{ex.StackTrace}", "Application Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Environment.Exit(1);
        }
    }    
}