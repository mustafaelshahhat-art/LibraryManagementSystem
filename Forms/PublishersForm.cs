using System;
using System.Drawing;
using System.Windows.Forms;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem
{
    public class PublishersForm : Form
    {
        private DataGridView dgvPublishers;
        private TextBox txtName, txtAddress, txtContact, txtId;
        private Button btnAdd, btnUpdate, btnDelete, btnClear;
        private PublisherRepository _repository;

        public PublishersForm()
        {
            _repository = new PublisherRepository();
            InitializeUI();
            LoadData();
        }

        private void InitializeUI()
        {
            this.Text = "Manage Publishers";
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
            inputGroup.Text = "Publisher Details";
            inputGroup.Dock = DockStyle.Fill;
            inputGroup.Font = new Font("Segoe UI", 10);
            inputGroup.AutoSize = true;
            inputGroup.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            TableLayoutPanel inputLayout = new TableLayoutPanel();
            inputLayout.Dock = DockStyle.Fill;
            inputLayout.ColumnCount = 2;
            inputLayout.RowCount = 5;
            inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));
            inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            inputLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
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

            // Address
            inputLayout.Controls.Add(new Label { Text = "Address:", AutoSize = true }, 0, 2);
            txtAddress = new TextBox { Dock = DockStyle.Fill, Multiline = true, Height = 40 };
            inputLayout.Controls.Add(txtAddress, 1, 2);

            // Contact
            inputLayout.Controls.Add(new Label { Text = "Contact:", AutoSize = true }, 0, 3);
            txtContact = new TextBox { Dock = DockStyle.Fill };
            inputLayout.Controls.Add(txtContact, 1, 3);

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

            dgvPublishers = new DataGridView();
            dgvPublishers.Dock = DockStyle.Fill;
            dgvPublishers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvPublishers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPublishers.MultiSelect = false;
            dgvPublishers.ReadOnly = true;
            dgvPublishers.AllowUserToAddRows = false;
            dgvPublishers.BackgroundColor = Color.White;
            dgvPublishers.RowHeadersVisible = false;
            dgvPublishers.SelectionChanged += DgvPublishers_SelectionChanged;
            dgvPublishers.Margin = new Padding(0, 10, 0, 0);

            mainLayout.Controls.Add(inputGroup, 0, 0);
            mainLayout.Controls.Add(dgvPublishers, 0, 1);
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
                var publishers = _repository.GetAllPublishers();
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Loaded {publishers.Count} publishers");
                dgvPublishers.DataSource = null;
                dgvPublishers.DataSource = publishers;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR] LoadData failed: {ex.Message}");
                MessageBox.Show("Error loading publishers: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvPublishers_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvPublishers.SelectedRows.Count > 0)
            {
                var pub = (Publisher)dgvPublishers.SelectedRows[0].DataBoundItem;
                txtId.Text = pub.PublisherId.ToString();
                txtName.Text = pub.Name;
                txtAddress.Text = pub.Address;
                txtContact.Text = pub.ContactInfo;
            }
        }

        private void ClearInputs()
        {
            txtId.Text = "";
            txtName.Text = "";
            txtAddress.Text = "";
            txtContact.Text = "";
            dgvPublishers.ClearSelection();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text)) return;
            _repository.AddPublisher(new Publisher { Name = txtName.Text, Address = txtAddress.Text, ContactInfo = txtContact.Text });
            LoadData();
            ClearInputs();
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtId.Text)) return;
            _repository.UpdatePublisher(new Publisher { PublisherId = int.Parse(txtId.Text), Name = txtName.Text, Address = txtAddress.Text, ContactInfo = txtContact.Text });
            LoadData();
            ClearInputs();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtId.Text)) return;
            if (MessageBox.Show("Delete this publisher?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                _repository.DeletePublisher(int.Parse(txtId.Text));
                LoadData();
                ClearInputs();
            }
        }
    }
}
