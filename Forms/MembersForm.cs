using System;
using System.Drawing;
using System.Windows.Forms;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using System.Collections.Generic;

namespace LibraryManagementSystem
{
    public class MembersForm : Form
    {
        private DataGridView dgvMembers;
        private TextBox txtMemberId, txtFirstName, txtLastName, txtEmail, txtPhone, txtAddress;
        private Button btnAdd, btnUpdate, btnDelete, btnClear;
        private MemberRepository _repository;

        public MembersForm()
        {
            _repository = new MemberRepository();
            InitializeUI();
            LoadMembers();
        }

        private void InitializeUI()
        {
            this.Text = "Manage Members";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;

            // Main Layout
            TableLayoutPanel mainLayout = new TableLayoutPanel();
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.RowCount = 2;
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 320F)); // Increased height
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Grid area
            mainLayout.Padding = new Padding(10);

            // Input Group
            GroupBox inputGroup = new GroupBox();
            inputGroup.Text = "Member Details";
            inputGroup.Dock = DockStyle.Fill;
            inputGroup.Font = new Font("Segoe UI", 10);

            // Inner Layout
            TableLayoutPanel inputLayout = new TableLayoutPanel();
            inputLayout.Dock = DockStyle.Fill;
            inputLayout.ColumnCount = 4;
            inputLayout.RowCount = 5;
            inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            
            // Set rows to AutoSize to fit content
            inputLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            inputLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            inputLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            inputLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            inputLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            
            inputLayout.Padding = new Padding(10);

            // Row 0 - ID
            inputLayout.Controls.Add(CreateLabel("Member ID:"), 0, 0);
            txtMemberId = CreateTextBox();
            txtMemberId.ReadOnly = true;
            txtMemberId.PlaceholderText = "Auto-Generated";
            inputLayout.Controls.Add(txtMemberId, 1, 0);

            // Row 1
            inputLayout.Controls.Add(CreateLabel("First Name:"), 0, 1);
            txtFirstName = CreateTextBox();
            inputLayout.Controls.Add(txtFirstName, 1, 1);

            inputLayout.Controls.Add(CreateLabel("Last Name:"), 2, 1);
            txtLastName = CreateTextBox();
            inputLayout.Controls.Add(txtLastName, 3, 1);

            // Row 2
            inputLayout.Controls.Add(CreateLabel("Email:"), 0, 2);
            txtEmail = CreateTextBox();
            inputLayout.Controls.Add(txtEmail, 1, 2);

            inputLayout.Controls.Add(CreateLabel("Phone:"), 2, 2);
            txtPhone = CreateTextBox();
            inputLayout.Controls.Add(txtPhone, 3, 2);

            // Row 3
            inputLayout.Controls.Add(CreateLabel("Address:"), 0, 3);
            txtAddress = CreateTextBox();
            inputLayout.Controls.Add(txtAddress, 1, 3);
            inputLayout.SetColumnSpan(txtAddress, 3);

            // Row 4 - Buttons
            FlowLayoutPanel buttonPanel = new FlowLayoutPanel();
            buttonPanel.Dock = DockStyle.Fill;
            buttonPanel.FlowDirection = FlowDirection.LeftToRight;
            buttonPanel.AutoSize = true;
            buttonPanel.Padding = new Padding(0, 10, 0, 0); // Add top padding to separate from inputs

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
            inputLayout.SetColumnSpan(buttonPanel, 4);

            inputGroup.Controls.Add(inputLayout);

            // Grid
            dgvMembers = new DataGridView();
            dgvMembers.Dock = DockStyle.Fill;
            dgvMembers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvMembers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMembers.MultiSelect = false;
            dgvMembers.ReadOnly = true;
            dgvMembers.AllowUserToAddRows = false;
            dgvMembers.BackgroundColor = Color.White;
            dgvMembers.BorderStyle = BorderStyle.None;
            dgvMembers.RowHeadersVisible = false;
            dgvMembers.SelectionChanged += DgvMembers_SelectionChanged;

            mainLayout.Controls.Add(inputGroup, 0, 0);
            mainLayout.Controls.Add(dgvMembers, 0, 1);
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

        private void LoadMembers()
        {
            try
            {
                var members = _repository.GetAllMembers();
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Loaded {members.Count} members");
                dgvMembers.DataSource = null;
                dgvMembers.DataSource = members;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR] LoadMembers failed: {ex.Message}");
                MessageBox.Show("Error loading members: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvMembers_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvMembers.SelectedRows.Count > 0)
            {
                var row = dgvMembers.SelectedRows[0];
                var member = (Member)row.DataBoundItem;

                txtMemberId.Text = member.MemberId.ToString();
                txtFirstName.Text = member.FirstName;
                txtLastName.Text = member.LastName;
                txtEmail.Text = member.Email;
                txtPhone.Text = member.Phone;
                txtAddress.Text = member.Address;
            }
        }

        private void ClearInputs()
        {
            txtMemberId.Text = "";
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtEmail.Text = "";
            txtPhone.Text = "";
            txtAddress.Text = "";
            dgvMembers.ClearSelection();
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtFirstName.Text) || string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                MessageBox.Show("First Name and Last Name are required.");
                return false;
            }
            
            // Validate email if provided
            if (!string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                if (!txtEmail.Text.Contains("@") || !txtEmail.Text.Contains("."))
                {
                    MessageBox.Show("Please enter a valid email address.");
                    return false;
                }
            }
            
            return true;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs()) return;

            try
            {
                Member member = new Member
                {
                    FirstName = txtFirstName.Text,
                    LastName = txtLastName.Text,
                    Email = txtEmail.Text,
                    Phone = txtPhone.Text,
                    Address = txtAddress.Text,
                    JoinDate = DateTime.Now
                };
                _repository.AddMember(member);
                MessageBox.Show("Member added successfully!");
                LoadMembers();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding member: " + ex.Message);
            }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMemberId.Text))
            {
                MessageBox.Show("Select a member to update.");
                return;
            }
            if (!ValidateInputs()) return;

            try
            {
                Member member = new Member
                {
                    MemberId = int.Parse(txtMemberId.Text),
                    FirstName = txtFirstName.Text,
                    LastName = txtLastName.Text,
                    Email = txtEmail.Text,
                    Phone = txtPhone.Text,
                    Address = txtAddress.Text
                };
                _repository.UpdateMember(member);
                MessageBox.Show("Member updated successfully!");
                LoadMembers();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating member: " + ex.Message);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMemberId.Text))
            {
                MessageBox.Show("Select a member to delete.");
                return;
            }

            if (MessageBox.Show("Are you sure you want to delete this member?", "Confirm Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    _repository.DeleteMember(int.Parse(txtMemberId.Text));
                    MessageBox.Show("Member deleted successfully!");
                    LoadMembers();
                    ClearInputs();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting member: " + ex.Message);
                }
            }
        }
    }
}
