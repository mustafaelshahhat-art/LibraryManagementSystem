using System;
using System.Drawing;
using System.Windows.Forms;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using System.Collections.Generic;
using System.Linq;

namespace LibraryManagementSystem
{
    public class BooksForm : Form
    {
        private DataGridView dgvBooks;
        private TextBox txtISBN, txtTitle, txtYear, txtEdition;
        private ComboBox cmbPublisher;
        private NumericUpDown numCopies;
        private ComboBox cmbAuthors;
        private ComboBox cmbCategories; // New Category ComboBox
        private Button btnAdd, btnUpdate, btnDelete, btnClear;
        private BookRepository _repository;
        private PublisherRepository _pubRepo;
        private AuthorRepository _authRepo;
        private CategoryRepository _catRepo; // New Category Repository

        public BooksForm()
        {
            _repository = new BookRepository();
            _pubRepo = new PublisherRepository();
            _authRepo = new AuthorRepository();
            _catRepo = new CategoryRepository(); // Initialize
            InitializeUI();
            LoadData();
        }

        private void InitializeUI()
        {
            this.Text = "Manage Books";
            this.Size = new Size(1100, 800); // Increased height slightly
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;

            TableLayoutPanel mainLayout = new TableLayoutPanel();
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.RowCount = 2;
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainLayout.Padding = new Padding(10);

            GroupBox inputGroup = new GroupBox();
            inputGroup.Text = "Book Details";
            inputGroup.Dock = DockStyle.Fill;
            inputGroup.Font = new Font("Segoe UI", 10);
            inputGroup.AutoSize = true;
            inputGroup.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            
            TableLayoutPanel inputLayout = new TableLayoutPanel();
            inputLayout.Dock = DockStyle.Fill;
            inputLayout.ColumnCount = 4;
            inputLayout.RowCount = 6; // Increased rows
            inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            inputLayout.Padding = new Padding(10);
            inputLayout.AutoSize = true;
            inputLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            // Row 0
            inputLayout.Controls.Add(CreateLabel("ISBN:"), 0, 0);
            txtISBN = CreateTextBox();
            inputLayout.Controls.Add(txtISBN, 1, 0);

            inputLayout.Controls.Add(CreateLabel("Title:"), 2, 0);
            txtTitle = CreateTextBox();
            inputLayout.Controls.Add(txtTitle, 3, 0);

            // Row 1
            inputLayout.Controls.Add(CreateLabel("Year:"), 0, 1);
            txtYear = CreateTextBox();
            inputLayout.Controls.Add(txtYear, 1, 1);

            inputLayout.Controls.Add(CreateLabel("Edition:"), 2, 1);
            txtEdition = CreateTextBox();
            inputLayout.Controls.Add(txtEdition, 3, 1);

            // Row 2
            inputLayout.Controls.Add(CreateLabel("Publisher:"), 0, 2);
            cmbPublisher = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            inputLayout.Controls.Add(cmbPublisher, 1, 2);

            inputLayout.Controls.Add(CreateLabel("Copies:"), 2, 2);
            numCopies = new NumericUpDown { Dock = DockStyle.Fill, Minimum = 1, Maximum = 100, Value = 1 };
            inputLayout.Controls.Add(numCopies, 3, 2);

            // Row 3 - Authors
            inputLayout.Controls.Add(CreateLabel("Author:"), 0, 3);
            cmbAuthors = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            inputLayout.Controls.Add(cmbAuthors, 1, 3);

            // Row 4 - Categories (New)
            inputLayout.Controls.Add(CreateLabel("Category:"), 0, 4);
            cmbCategories = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            inputLayout.Controls.Add(cmbCategories, 1, 4);

            // Row Styles
            inputLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // 0
            inputLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // 1
            inputLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // 2
            inputLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // 3
            inputLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // 4
            inputLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // 5 - Buttons

            // Row 5 - Buttons
            FlowLayoutPanel buttonPanel = new FlowLayoutPanel();
            buttonPanel.Dock = DockStyle.Fill;
            buttonPanel.FlowDirection = FlowDirection.LeftToRight;
            buttonPanel.AutoSize = true;
            buttonPanel.Padding = new Padding(0, 10, 0, 0);
            
            btnAdd = CreateButton("Add", Color.SeaGreen);
            btnUpdate = CreateButton("Update", Color.Orange);
            btnDelete = CreateButton("Delete", Color.IndianRed);
            btnClear = CreateButton("Clear", Color.Gray);

            btnAdd.Click += BtnAdd_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnDelete.Click += BtnDelete_Click;
            btnClear.Click += (s, e) => ClearInputs();

            buttonPanel.Controls.Add(btnAdd);
            buttonPanel.Controls.Add(btnUpdate);
            buttonPanel.Controls.Add(btnDelete);
            buttonPanel.Controls.Add(btnClear);

            inputLayout.Controls.Add(buttonPanel, 0, 5);
            inputLayout.SetColumnSpan(buttonPanel, 4);

            inputGroup.Controls.Add(inputLayout);

            dgvBooks = new DataGridView();
            dgvBooks.Dock = DockStyle.Fill;
            dgvBooks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvBooks.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvBooks.MultiSelect = false;
            dgvBooks.ReadOnly = true;
            dgvBooks.AllowUserToAddRows = false;
            dgvBooks.BackgroundColor = Color.White;
            dgvBooks.RowHeadersVisible = false;
            dgvBooks.SelectionChanged += DgvBooks_SelectionChanged;

            mainLayout.Controls.Add(inputGroup, 0, 0);
            mainLayout.Controls.Add(dgvBooks, 0, 1);
            this.Controls.Add(mainLayout);
        }

        private Label CreateLabel(string text)
        {
            return new Label { Text = text, Anchor = AnchorStyles.Left | AnchorStyles.Right, AutoSize = true, TextAlign = ContentAlignment.MiddleLeft };
        }

        private TextBox CreateTextBox()
        {
            return new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right };
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
            btn.Size = new Size(100, 35);
            btn.Margin = new Padding(0, 0, 10, 0);
            btn.Cursor = Cursors.Hand;
            return btn;
        }

        private void LoadData()
        {
            try
            {
                // Load Publishers
                var publishers = _pubRepo.GetAllPublishers();
                cmbPublisher.DataSource = new BindingSource(publishers, null);
                cmbPublisher.DisplayMember = "Name";
                cmbPublisher.ValueMember = "PublisherId";
                cmbPublisher.SelectedIndex = -1;

                // Load Authors
                var authors = _authRepo.GetAllAuthors();
                cmbAuthors.DataSource = new BindingSource(authors, null);
                cmbAuthors.DisplayMember = "Name";
                cmbAuthors.ValueMember = "AuthorId";
                cmbAuthors.SelectedIndex = -1;

                // Load Categories
                var categories = _catRepo.GetAllCategories();
                cmbCategories.DataSource = new BindingSource(categories, null);
                cmbCategories.DisplayMember = "Name";
                cmbCategories.ValueMember = "CategoryId";
                cmbCategories.SelectedIndex = -1;

                // Load Books
                var books = _repository.GetAllBooks();
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Loaded {books.Count} books from repository");
                
                if (books.Count == 0)
                {
                    MessageBox.Show("No books found in the database. Please add books first.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                
                dgvBooks.DataSource = null;
                dgvBooks.DataSource = books.Select(b => new 
                { 
                    b.ISBN, 
                    b.Title, 
                    Year = b.PublicationYear, 
                    b.Edition, 
                    Publisher = b.Publisher?.Name ?? "N/A",
                    Category = b.Categories.FirstOrDefault()?.Name ?? "N/A" // Show first category
                }).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR] LoadData failed: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[ERROR] Stack trace: {ex.StackTrace}");
                MessageBox.Show("Error loading data: " + ex.Message + "\n\nStack: " + ex.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvBooks_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvBooks.SelectedRows.Count > 0)
            {
                var row = dgvBooks.SelectedRows[0];
                
                // Read values from cells
                txtISBN.Text = row.Cells["ISBN"].Value?.ToString() ?? "";
                txtTitle.Text = row.Cells["Title"].Value?.ToString() ?? "";
                txtYear.Text = row.Cells["Year"].Value?.ToString() ?? "";
                txtEdition.Text = row.Cells["Edition"].Value?.ToString() ?? "";
                
                // Find publisher by name
                string publisherName = row.Cells["Publisher"].Value?.ToString() ?? "";
                if (!string.IsNullOrEmpty(publisherName) && publisherName != "N/A")
                {
                    for (int i = 0; i < cmbPublisher.Items.Count; i++)
                    {
                        var pub = (Publisher)cmbPublisher.Items[i];
                        if (pub.Name == publisherName)
                        {
                            cmbPublisher.SelectedIndex = i;
                            break;
                        }
                    }
                }
                else
                {
                    cmbPublisher.SelectedIndex = -1;
                }

                // Find category by name
                string categoryName = row.Cells["Category"].Value?.ToString() ?? "";
                if (!string.IsNullOrEmpty(categoryName) && categoryName != "N/A")
                {
                    for (int i = 0; i < cmbCategories.Items.Count; i++)
                    {
                        var cat = (Category)cmbCategories.Items[i];
                        if (cat.Name == categoryName)
                        {
                            cmbCategories.SelectedIndex = i;
                            break;
                        }
                    }
                }
                else
                {
                    cmbCategories.SelectedIndex = -1;
                }
                
                txtISBN.Enabled = false;

                // Clear author selection (as we don't fetch it in the grid view currently)
                cmbAuthors.SelectedIndex = -1;
            }
        }

        private void ClearInputs()
        {
            txtISBN.Text = "";
            txtTitle.Text = "";
            txtYear.Text = "";
            txtEdition.Text = "";
            cmbPublisher.SelectedIndex = -1;
            cmbAuthors.SelectedIndex = -1;
            cmbCategories.SelectedIndex = -1;
            txtISBN.Enabled = true;
            dgvBooks.ClearSelection();
        }

        private bool ValidateInputs(out int year, out int pubId)
        {
            year = 0;
            pubId = 0;

            if (string.IsNullOrWhiteSpace(txtISBN.Text))
            {
                MessageBox.Show("ISBN is required.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Title is required.");
                return false;
            }
            if (!int.TryParse(txtYear.Text, out year))
            {
                MessageBox.Show("Year must be a valid number.");
                return false;
            }
            if (cmbPublisher.SelectedValue == null)
            {
                MessageBox.Show("Please select a publisher.");
                return false;
            }
            pubId = (int)cmbPublisher.SelectedValue;
            return true;
        }

        private List<int> GetSelectedAuthorIds()
        {
            List<int> ids = new List<int>();
            if (cmbAuthors.SelectedValue != null)
            {
                ids.Add((int)cmbAuthors.SelectedValue);
            }
            return ids;
        }

        private List<int> GetSelectedCategoryIds()
        {
            List<int> ids = new List<int>();
            if (cmbCategories.SelectedValue != null)
            {
                ids.Add((int)cmbCategories.SelectedValue);
            }
            return ids;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs(out int year, out int pubId)) return;

            try
            {
                Book book = new Book
                {
                    ISBN = txtISBN.Text,
                    Title = txtTitle.Text,
                    PublicationYear = year,
                    Edition = txtEdition.Text,
                    PublisherId = pubId
                };
                _repository.AddBook(book, GetSelectedAuthorIds(), GetSelectedCategoryIds(), (int)numCopies.Value);
                MessageBox.Show("Book added successfully!");
                LoadData();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding book: " + ex.Message);
            }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs(out int year, out int pubId)) return;

            try
            {
                Book book = new Book
                {
                    ISBN = txtISBN.Text,
                    Title = txtTitle.Text,
                    PublicationYear = year,
                    Edition = txtEdition.Text,
                    PublisherId = pubId
                };
                _repository.UpdateBook(book, GetSelectedAuthorIds(), GetSelectedCategoryIds());
                MessageBox.Show("Book updated successfully!");
                LoadData();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating book: " + ex.Message);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtISBN.Text))
            {
                MessageBox.Show("Select a book to delete.");
                return;
            }

            if (MessageBox.Show("Are you sure you want to delete this book?", "Confirm Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    _repository.DeleteBook(txtISBN.Text);
                    MessageBox.Show("Book deleted successfully!");
                    LoadData();
                    ClearInputs();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting book: " + ex.Message);
                }
            }
        }
    }
}
