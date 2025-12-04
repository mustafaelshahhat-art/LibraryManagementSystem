using System;
using System.Drawing;
using System.Windows.Forms;

namespace LibraryManagementSystem.Forms
{
    public class MainMenuForm : Form
    {
        public MainMenuForm()
        {
            this.Text = "Library Management System";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 4
            };
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            for (int i = 0; i < 4; i++)
                mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));

            var btnBooks = CreateButton("إدارة الكتب", () => OpenForm(new BooksForm()));
            var btnMembers = CreateButton("إدارة الأعضاء", () => OpenForm(new MembersForm()));
            var btnLoans = CreateButton("إدارة الإعارات", () => OpenForm(new LoansForm()));
            var btnAuthors = CreateButton("إدارة المؤلفين", () => OpenForm(new AuthorsForm()));
            var btnPublishers = CreateButton("إدارة الناشرين", () => OpenForm(new PublishersForm()));
            var btnCategories = CreateButton("إدارة الفئات", () => OpenForm(new CategoriesForm()));
            var btnReports = CreateButton("التقارير", () => OpenForm(new ReportsForm()));

            mainLayout.Controls.Add(btnBooks, 0, 0);
            mainLayout.Controls.Add(btnMembers, 1, 0);
            mainLayout.Controls.Add(btnLoans, 0, 1);
            mainLayout.Controls.Add(btnReports, 1, 1);
            mainLayout.Controls.Add(btnAuthors, 0, 2);
            mainLayout.Controls.Add(btnPublishers, 1, 2);
            mainLayout.Controls.Add(btnCategories, 0, 3);

            this.Controls.Add(mainLayout);
        }

        private Button CreateButton(string text, Action onClick)
        {
            var btn = new Button
            {
                Text = text,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Margin = new Padding(20)
            };
            btn.Click += (s, e) => onClick();
            return btn;
        }

        private void OpenForm(Form form)
        {
            this.Hide();
            form.FormClosed += (s, e) => this.Show();
            form.Show();
        }
    }
}
