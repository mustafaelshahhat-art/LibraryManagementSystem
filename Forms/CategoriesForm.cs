using System;
using System.Drawing;
using System.Windows.Forms;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem
{
    public class CategoriesForm : Form
    {
        private DataGridView dgvCategories;
        private TextBox txtName, txtId;
        private Button btnAdd, btnUpdate, btnDelete, btnClear;
        private CategoryRepository _repository;

        public CategoriesForm()
        {
            _repository = new CategoryRepository();
            InitializeUI();
            LoadData();
        }

        private void InitializeUI()
        {
            this.Text = "Manage Categories";
            this.Size = new Size(800, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;

            TableLayoutPanel mainLayout = new TableLayoutPanel();
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.RowCount = 2;
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainLayout.Padding = new Padding(10);

            GroupBox inputGroup = new GroupBox();
            inputGroup.Text = "Category Details";
            inputGroup.Dock = DockStyle.Fill;
            inputGroup.Font = new Font("Segoe UI", 10);
            inputGroup.AutoSize = true;
            inputGroup.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            TableLayoutPanel inputLayout = new TableLayoutPanel();
            inputLayout.Dock = DockStyle.Fill;
            inputLayout.ColumnCount = 2;
            inputLayout.RowCount = 3;
            inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));
            inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
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

            inputLayout.Controls.Add(buttonPanel, 0, 2);
            inputLayout.SetColumnSpan(buttonPanel, 2);

            inputGroup.Controls.Add(inputLayout);

            dgvCategories = new DataGridView();
            dgvCategories.Dock = DockStyle.Fill;
            dgvCategories.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCategories.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCategories.MultiSelect = false;
            dgvCategories.ReadOnly = true;
            dgvCategories.AllowUserToAddRows = false;
            dgvCategories.BackgroundColor = Color.White;
            dgvCategories.RowHeadersVisible = false;
            dgvCategories.SelectionChanged += DgvCategories_SelectionChanged;
            dgvCategories.Margin = new Padding(0, 10, 0, 0);

            mainLayout.Controls.Add(inputGroup, 0, 0);
            mainLayout.Controls.Add(dgvCategories, 0, 1);
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
                var categories = _repository.GetAllCategories();
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Loaded {categories.Count} categories");
                dgvCategories.DataSource = null;
                dgvCategories.DataSource = categories;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR] LoadData failed: {ex.Message}");
                MessageBox.Show("Error loading categories: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvCategories_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCategories.SelectedRows.Count > 0)
            {
                var category = (Category)dgvCategories.SelectedRows[0].DataBoundItem;
                txtId.Text = category.CategoryId.ToString();
                txtName.Text = category.Name;
            }
        }

        private void ClearInputs()
        {
            txtId.Text = "";
            txtName.Text = "";
            dgvCategories.ClearSelection();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text)) return;
            _repository.AddCategory(new Category { Name = txtName.Text });
            LoadData();
            ClearInputs();
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtId.Text)) return;
            _repository.UpdateCategory(new Category { CategoryId = int.Parse(txtId.Text), Name = txtName.Text });
            LoadData();
            ClearInputs();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtId.Text)) return;
            if (MessageBox.Show("Delete this category?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                _repository.DeleteCategory(int.Parse(txtId.Text));
                LoadData();
                ClearInputs();
            }
        }
    }
}
