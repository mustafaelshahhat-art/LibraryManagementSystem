using System;
using System.Drawing;
using System.Windows.Forms;
using LibraryManagementSystem.Data;
using System.Linq;

namespace LibraryManagementSystem
{
    public class ReportsForm : Form
    {
        private LoanRepository _loanRepo;
        private BookRepository _bookRepo;
        private MemberRepository _memberRepo;

        public ReportsForm()
        {
            _loanRepo = new LoanRepository();
            _bookRepo = new BookRepository();
            _memberRepo = new MemberRepository();
            InitializeUI();
        }

        private void InitializeUI()
        {
            this.Text = "Library Reports";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;

            TableLayoutPanel mainLayout = new TableLayoutPanel();
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.ColumnCount = 2;
            mainLayout.RowCount = 2;
            mainLayout.Padding = new Padding(20);
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

            // Fetch Data
            int totalBooks = _bookRepo.GetAllBooks().Count;
            int totalMembers = _memberRepo.GetAllMembers().Count;
            var loans = _loanRepo.GetAllLoans();
            int activeLoans = loans.Count(l => l.ReturnDate == null);
            int overdueLoans = loans.Count(l => l.ReturnDate == null && l.DueDate < DateTime.Now);

            // Add Cards
            mainLayout.Controls.Add(CreateReportCard("Total Books", totalBooks.ToString(), Color.CornflowerBlue), 0, 0);
            mainLayout.Controls.Add(CreateReportCard("Total Members", totalMembers.ToString(), Color.SeaGreen), 1, 0);
            mainLayout.Controls.Add(CreateReportCard("Active Loans", activeLoans.ToString(), Color.Orange), 0, 1);
            mainLayout.Controls.Add(CreateReportCard("Overdue Loans", overdueLoans.ToString(), Color.IndianRed), 1, 1);

            this.Controls.Add(mainLayout);
        }

        private Panel CreateReportCard(string title, string value, Color color)
        {
            Panel panel = new Panel();
            panel.Dock = DockStyle.Fill;
            panel.Margin = new Padding(10);
            panel.BackColor = Color.White;
            panel.Padding = new Padding(20);
            
            // Border/Shadow effect (simple)
            panel.BorderStyle = BorderStyle.FixedSingle;

            Label lblTitle = new Label();
            lblTitle.Text = title;
            lblTitle.Font = new Font("Segoe UI", 14, FontStyle.Regular);
            lblTitle.ForeColor = Color.Gray;
            lblTitle.Dock = DockStyle.Top;
            lblTitle.Height = 40; // Fixed height to ensure space
            lblTitle.AutoSize = false;
            lblTitle.TextAlign = ContentAlignment.BottomCenter; // Align bottom of top area

            Label lblValue = new Label();
            lblValue.Text = value;
            lblValue.Font = new Font("Segoe UI", 36, FontStyle.Bold);
            lblValue.ForeColor = color;
            lblValue.Dock = DockStyle.Fill;
            lblValue.TextAlign = ContentAlignment.MiddleCenter;
            lblValue.AutoSize = false; // Ensure it fills

            // Add Value first (so it's at the bottom of z-order stack, but Dock order is reverse in some contexts, 
            // actually usually last added is top priority for Dock. 
            // We want Title to take Top space first, then Value fills rest.
            // So Title should be added LAST (top of z-order).
            panel.Controls.Add(lblValue);
            panel.Controls.Add(lblTitle);

            return panel;
        }
    }
}
