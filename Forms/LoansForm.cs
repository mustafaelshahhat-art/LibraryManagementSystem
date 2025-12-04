using System;
using System.Drawing;
using System.Windows.Forms;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using System.Collections.Generic;
using System.Linq;

namespace LibraryManagementSystem
{
    public class LoansForm : Form
    {
        private DataGridView dgvLoans;
        private ComboBox cmbMembers, cmbBooks;
        private DateTimePicker dtpIssue, dtpDue;
        private Button btnIssue, btnReturn, btnRefresh, btnAddCopy;
        private LoanRepository _repository;
        private MemberRepository _memberRepo;
        private BookRepository _bookRepo; // To get books for adding copies

        public LoansForm()
        {
            _repository = new LoanRepository();
            _memberRepo = new MemberRepository();
            _bookRepo = new BookRepository();
            InitializeUI();
            LoadData();
        }

        private void InitializeUI()
        {
            this.Text = "Manage Loans";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;

            TableLayoutPanel mainLayout = new TableLayoutPanel();
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.RowCount = 2;
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 250F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainLayout.Padding = new Padding(10);

            GroupBox inputGroup = new GroupBox();
            inputGroup.Text = "Issue New Loan";
            inputGroup.Dock = DockStyle.Fill;
            inputGroup.Font = new Font("Segoe UI", 10);

            TableLayoutPanel inputLayout = new TableLayoutPanel();
            inputLayout.Dock = DockStyle.Fill;
            inputLayout.ColumnCount = 4;
            inputLayout.RowCount = 4;
            inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            inputLayout.Padding = new Padding(10);
            
            // Row 0
            inputLayout.Controls.Add(CreateLabel("Member:"), 0, 0);
            cmbMembers = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            inputLayout.Controls.Add(cmbMembers, 1, 0);

            inputLayout.Controls.Add(CreateLabel("Available Book:"), 2, 0);
            cmbBooks = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            inputLayout.Controls.Add(cmbBooks, 3, 0);

            // Row 1
            inputLayout.Controls.Add(CreateLabel("Issue Date:"), 0, 1);
            dtpIssue = new DateTimePicker { Dock = DockStyle.Fill, Format = DateTimePickerFormat.Short };
            inputLayout.Controls.Add(dtpIssue, 1, 1);

            inputLayout.Controls.Add(CreateLabel("Due Date:"), 2, 1);
            dtpDue = new DateTimePicker { Dock = DockStyle.Fill, Format = DateTimePickerFormat.Short, Value = DateTime.Now.AddDays(14) };
            inputLayout.Controls.Add(dtpDue, 3, 1);

            // Row 2 - Buttons
            FlowLayoutPanel buttonPanel = new FlowLayoutPanel();
            buttonPanel.Dock = DockStyle.Fill;
            buttonPanel.FlowDirection = FlowDirection.LeftToRight;
            buttonPanel.AutoSize = true;
            buttonPanel.Padding = new Padding(0, 20, 0, 0);

            btnIssue = CreateButton("Issue Loan", Color.SeaGreen);
            btnReturn = CreateButton("Return Book", Color.Orange);
            btnRefresh = CreateButton("Refresh", Color.Gray);
            btnAddCopy = CreateButton("Add Copy (Test)", Color.CornflowerBlue); // Helper to add copies quickly

            btnIssue.Click += BtnIssue_Click;
            btnReturn.Click += BtnReturn_Click;
            btnRefresh.Click += (s, e) => LoadData();
            btnAddCopy.Click += BtnAddCopy_Click;

            buttonPanel.Controls.Add(btnIssue);
            buttonPanel.Controls.Add(btnReturn);
            buttonPanel.Controls.Add(btnRefresh);
            buttonPanel.Controls.Add(btnAddCopy);

            inputLayout.Controls.Add(buttonPanel, 0, 2);
            inputLayout.SetColumnSpan(buttonPanel, 4);

            inputGroup.Controls.Add(inputLayout);

            dgvLoans = new DataGridView();
            dgvLoans.Dock = DockStyle.Fill;
            dgvLoans.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvLoans.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvLoans.MultiSelect = false;
            dgvLoans.ReadOnly = true;
            dgvLoans.AllowUserToAddRows = false;
            dgvLoans.BackgroundColor = Color.White;
            dgvLoans.BorderStyle = BorderStyle.None;
            dgvLoans.RowHeadersVisible = false;

            mainLayout.Controls.Add(inputGroup, 0, 0);
            mainLayout.Controls.Add(dgvLoans, 0, 1);
            this.Controls.Add(mainLayout);
        }

        private Label CreateLabel(string text)
        {
            return new Label { Text = text, Anchor = AnchorStyles.Left | AnchorStyles.Right, AutoSize = true, TextAlign = ContentAlignment.MiddleLeft };
        }

        private Button CreateButton(string text, Color color)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.BackColor = color;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btn.Size = new Size(120, 35);
            btn.Margin = new Padding(0, 0, 10, 0);
            btn.Cursor = Cursors.Hand;
            return btn;
        }

        private void LoadData()
        {
            try
            {
                // Load Loans
                var loans = _repository.GetAllLoans();
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Loaded {loans.Count} loans");
                dgvLoans.DataSource = null;
                dgvLoans.DataSource = loans.Select(l => new 
                { 
                    l.LoanId, 
                    Member = l.Member.FirstName + " " + l.Member.LastName, 
                    Book = l.BookCopy.Book.Title, 
                    CopyID = l.CopyId,
                    l.IssueDate, 
                    l.DueDate, 
                    ReturnDate = l.ReturnDate.HasValue ? l.ReturnDate.Value.ToShortDateString() : "Active" 
                }).ToList();

                // Load Members
                var members = _memberRepo.GetAllMembers();
                cmbMembers.DataSource = new BindingSource(members, null);
                cmbMembers.DisplayMember = "FirstName";
                cmbMembers.ValueMember = "MemberId";

                // Load Available Books (not copies)
                var books = _repository.GetAvailableBooks();
                cmbBooks.DataSource = new BindingSource(books, null);
                cmbBooks.DisplayMember = "Title";
                cmbBooks.ValueMember = "ISBN";
                
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Loaded {members.Count} members and {books.Count} available books");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR] LoadData failed: {ex.Message}\n{ex.StackTrace}");
                MessageBox.Show("Error loading data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnIssue_Click(object sender, EventArgs e)
        {
            if (cmbMembers.SelectedItem == null || cmbBooks.SelectedItem == null)
            {
                MessageBox.Show("Please select a member and a book.");
                return;
            }

            try
            {
                int memberId = (int)cmbMembers.SelectedValue;
                string isbn = cmbBooks.SelectedValue.ToString();
                
                // Get first available copy for this book
                int? copyId = _repository.GetFirstAvailableCopyId(isbn);
                
                if (!copyId.HasValue)
                {
                    MessageBox.Show("No available copies for this book!");
                    return;
                }
                
                _repository.IssueLoan(copyId.Value, memberId, dtpIssue.Value, dtpDue.Value);
                MessageBox.Show("Loan issued successfully!");
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error issuing loan: " + ex.Message);
            }
        }

        private void BtnReturn_Click(object sender, EventArgs e)
        {
            if (dgvLoans.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select a loan to return.");
                return;
            }

            var row = dgvLoans.SelectedRows[0];
            string status = row.Cells["ReturnDate"].Value.ToString();
            if (status != "Active")
            {
                MessageBox.Show("This book is already returned.");
                return;
            }

            int loanId = (int)row.Cells["LoanId"].Value;

            try
            {
                _repository.ReturnLoan(loanId, DateTime.Now);
                MessageBox.Show("Book returned successfully!");
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error returning book: " + ex.Message);
            }
        }

        private void BtnAddCopy_Click(object sender, EventArgs e)
        {
            // Simple dialog to add a copy for an ISBN
            string isbn = Microsoft.VisualBasic.Interaction.InputBox("Enter ISBN to add copy for:", "Add Copy", "");
            if (!string.IsNullOrWhiteSpace(isbn))
            {
                try
                {
                    _repository.AddCopy(isbn, "Shelf A1");
                    MessageBox.Show("Copy added!");
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error adding copy: " + ex.Message);
                }
            }
        }
    }
}


