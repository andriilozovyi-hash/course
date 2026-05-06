using System;
using System.Windows.Forms;

namespace QuotesApp
{
    public class AddForm : Form
    {
        public Quote? ResultQuote { get; private set; }
        
        private TextBox txtText = new TextBox { Width = 250 };
        private TextBox txtAuthor = new TextBox { Width = 250 };
        private ComboBox cbCategory = new ComboBox { Width = 250 };
        private TextBox txtYear = new TextBox { Width = 250 };

        public AddForm(Quote? existing = null)
        {
            this.Text = existing == null ? "Додати вислів" : "Редагувати вислів";
            this.Size = new System.Drawing.Size(300, 350);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;

            // Категории (можно дополнить)
            cbCategory.Items.AddRange(new string[] { "Філософія", "Наука", "Гумор", "Мистецтво" });
            cbCategory.SelectedIndex = 0;

            FlowLayoutPanel panel = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            panel.Controls.Add(new Label { Text = "Текст вислову:", AutoSize = true });
            panel.Controls.Add(txtText);
            panel.Controls.Add(new Label { Text = "Автор:", AutoSize = true });
            panel.Controls.Add(txtAuthor);
            panel.Controls.Add(new Label { Text = "Категорія:", AutoSize = true });
            panel.Controls.Add(cbCategory);
            panel.Controls.Add(new Label { Text = "Рік:", AutoSize = true });
            panel.Controls.Add(txtYear);

            Button btnOk = new Button { Text = "OK", DialogResult = DialogResult.OK, Margin = new Padding(0, 20, 0, 0) };
            btnOk.Click += (s, e) => {
                if (!int.TryParse(txtYear.Text, out int year)) {
                    MessageBox.Show("Рік має бути числом!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
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