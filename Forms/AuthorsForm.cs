using System;
using System.Drawing;
using System.Windows.Forms;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem
{
    public class AuthorsForm : Form
    {
        private DataGridView dgvAuthors;
        private TextBox txtName, txtBio, txtId, txtBirthDate;
        private Button btnAdd, btnUpdate, btnDelete, btnClear;
        private AuthorRepository _repository;

        public AuthorsForm()
        {
            _repository = new AuthorRepository();
            InitializeUI();
            LoadData();
        }

        private void InitializeUI()
        {
            this.Text = "Manage Authors";
            this.Size = new Size(900, 850);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;

            TableLayoutPanel mainLayout = new TableLayoutPanel();
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.RowCount = 2;
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainLayout.Padding = new Padding(10);

            GroupBox inputGroup = new GroupBox();
            inputGroup.Text = "Author Details";
            inputGroup.Dock = DockStyle.Fill;
            inputGroup.Font = new Font("Segoe UI", 10);
            inputGroup.AutoSize = true;
            inputGroup.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            TableLayoutPanel inputLayout = new TableLayoutPanel();
            inputLayout.Dock = DockStyle.Fill;
            inputLayout.ColumnCount = 2;
            inputLayout.RowCount = 4;
            inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));
            inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            inputLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            inputLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            inputLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            inputLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            inputLayout.Padding = new Padding(10);
            inputLayout.AutoSize = true;
            inputLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            // ID
            inputLayout.Controls.Add(new Label { Text = "ID:", AutoSize = true }, 0, 0);
            txtId = new TextBox { ReadOnly = true, PlaceholderText = "Auto-Generated", Dock = DockStyle.Fill };
            inputLayout.Controls.Add(txtId, 1, 0);

            // Name
            inputLayout.Controls.Add(new Label { Text = "Name:", AutoSize = true }, 0, 1);
            txtName = new TextBox { Dock = DockStyle.Fill };
            inputLayout.Controls.Add(txtName, 1, 1);

            // Bio
            inputLayout.Controls.Add(new Label { Text = "Bio:", AutoSize = true }, 0, 2);
            txtBio = new TextBox { Dock = DockStyle.Fill, Multiline = true, Height = 50 };
            inputLayout.Controls.Add(txtBio, 1, 2);

            // Birth Date
            inputLayout.Controls.Add(new Label { Text = "Birth Date:", AutoSize = true }, 0, 3);
            txtBirthDate = new TextBox { Dock = DockStyle.Fill, PlaceholderText = "YYYY-MM-DD" };
            inputLayout.Controls.Add(txtBirthDate, 1, 3);

            // Buttons
            FlowLayoutPanel buttonPanel = new FlowLayoutPanel();
            buttonPanel.Dock = DockStyle.Fill;
            buttonPanel.AutoSize = true;
            
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

            inputLayout.Controls.Add(buttonPanel, 0, 4);
            inputLayout.SetColumnSpan(buttonPanel, 2);

            inputGroup.Controls.Add(inputLayout);

            dgvAuthors = new DataGridView();
            dgvAuthors.Dock = DockStyle.Fill;
            dgvAuthors.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvAuthors.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAuthors.MultiSelect = false;
            dgvAuthors.ReadOnly = true;
            dgvAuthors.AllowUserToAddRows = false;
            dgvAuthors.BackgroundColor = Color.White;
            dgvAuthors.RowHeadersVisible = false;
            dgvAuthors.SelectionChanged += DgvAuthors_SelectionChanged;
            dgvAuthors.Margin = new Padding(0, 10, 0, 0);

            mainLayout.Controls.Add(inputGroup, 0, 0);
            mainLayout.Controls.Add(dgvAuthors, 0, 1);
            this.Controls.Add(mainLayout);
        }

        private Button CreateButton(string text, Color color)
        {
            return new Button
            {
                Text = text,
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Size = new Size(100, 35),
                Margin = new Padding(0, 0, 10, 0),
                Cursor = Cursors.Hand
            };
        }

        private void LoadData()
        {
            try
            {
                var authors = _repository.GetAllAuthors();
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Loaded {authors.Count} authors");
                
                dgvAuthors.DataSource = null;
                dgvAuthors.DataSource = authors;
                
                if (authors.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("[DEBUG] No authors found in database");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR] LoadData failed: {ex.Message}");
                MessageBox.Show("Error loading authors: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvAuthors_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvAuthors.SelectedRows.Count > 0)
            {
                var author = (Author)dgvAuthors.SelectedRows[0].DataBoundItem;
                txtId.Text = author.AuthorId.ToString();
                txtName.Text = author.Name;
                txtBio.Text = author.Biography;
                txtBirthDate.Text = author.BirthDate;
            }
        }

        private void ClearInputs()
        {
            txtId.Text = "";
            txtName.Text = "";
            txtBio.Text = "";
            txtBirthDate.Text = "";
            dgvAuthors.ClearSelection();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("Please enter author name", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                _repository.AddAuthor(new Author { Name = txtName.Text, Biography = txtBio.Text, BirthDate = txtBirthDate.Text });
                MessageBox.Show("Author added successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding author: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtId.Text) || !int.TryParse(txtId.Text, out int id))
                {
                    MessageBox.Show("Please select an author to update", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("Please enter author name", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                _repository.UpdateAuthor(new Author { AuthorId = id, Name = txtName.Text, Biography = txtBio.Text, BirthDate = txtBirthDate.Text });
                MessageBox.Show("Author updated successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating author: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtId.Text) || !int.TryParse(txtId.Text, out int id))
                {
                    MessageBox.Show("Please select an author to delete", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                if (MessageBox.Show("Are you sure you want to delete this author?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _repository.DeleteAuthor(id);
                    MessageBox.Show("Author deleted successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                    ClearInputs();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting author: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
