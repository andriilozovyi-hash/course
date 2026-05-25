using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace QuotesApp
{
    public class AddForm : Form
    {
        public Quote? ResultQuote { get; private set; }
        
        private TextBox txtText = new TextBox { Width = 250 };
        private TextBox txtAuthor = new TextBox { Width = 250 };
        private ComboBox cbCategory = new ComboBox { Width = 215, DropDownStyle = ComboBoxStyle.DropDownList };
        private Button btnAddCat = new Button { Text = "+", Width = 30, Height = 23 };
        private TextBox txtYear = new TextBox { Width = 250 };

        public AddForm(List<string> availableCategories, Quote? existing = null)
        {
            this.Text = existing == null ? "Додати вислів" : "Редагувати вислів";
            this.Size = new System.Drawing.Size(300, 380);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;

            // Заповнюємо категорії
            cbCategory.Items.AddRange(availableCategories.ToArray());
            if (cbCategory.Items.Count > 0) cbCategory.SelectedIndex = 0;

            FlowLayoutPanel panel = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            panel.Controls.Add(new Label { Text = "Текст вислову:", AutoSize = true });
            panel.Controls.Add(txtText);
            
            panel.Controls.Add(new Label { Text = "Автор:", AutoSize = true });
            panel.Controls.Add(txtAuthor);
            
            panel.Controls.Add(new Label { Text = "Категорія:", AutoSize = true });
            
            // Контейнер для ComboBox та кнопки "+" в один рядок
            FlowLayoutPanel catContainer = new FlowLayoutPanel { Width = 260, Height = 30, Margin = new Padding(0) };
            catContainer.Controls.Add(cbCategory);
            catContainer.Controls.Add(btnAddCat);
            panel.Controls.Add(catContainer);

            panel.Controls.Add(new Label { Text = "Рік:", AutoSize = true });
            panel.Controls.Add(txtYear);

            // Логіка додавання нової категорії прямо у вікні вислову
            btnAddCat.Click += (s, e) => {
                string newCat = Microsoft.VisualBasic.Interaction.InputBox("Введіть назву нової категорії:", "Нова категорія", "");
                newCat = newCat.Trim();
                if (!string.IsNullOrEmpty(newCat))
                {
                    if (!cbCategory.Items.Contains(newCat))
                    {
                        cbCategory.Items.Add(newCat);
                    }
                    cbCategory.Text = newCat;
                }
            };

            Button btnOk = new Button { Text = "OK", DialogResult = DialogResult.OK, Width = 80, Margin = new Padding(0, 20, 0, 0) };
            btnOk.Click += (s, e) => {
                if (!int.TryParse(txtYear.Text, out int year)) {
                    MessageBox.Show("Рік має бути числом!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.DialogResult = DialogResult.None;
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtText.Text) || string.IsNullOrWhiteSpace(txtAuthor.Text)) {
                    MessageBox.Show("Текст та автор не можуть бути порожніми!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.DialogResult = DialogResult.None;
                    return;
                }
                ResultQuote = new Quote(txtText.Text, txtAuthor.Text, cbCategory.Text, year);
            };
            panel.Controls.Add(btnOk);

            this.Controls.Add(panel);

            if (existing != null) {
                txtText.Text = existing.Text;
                txtAuthor.Text = existing.Author;
                cbCategory.Text = existing.Category;
                txtYear.Text = existing.Year.ToString();
            }
        }
    }
}