using System;
using System.Windows.Forms;

namespace QuotesApp
{
    public class AddForm : Form
    {
        public Quote? NewQuote { get; private set; }
        
        private TextBox txtText = new TextBox { Top = 20, Left = 20, Width = 250, PlaceholderText = "Текст вислову" };
        private TextBox txtAuthor = new TextBox { Top = 50, Left = 20, Width = 250, PlaceholderText = "Автор" };
        private Button btnSave = new Button { Text = "Зберегти", Top = 100, Left = 20, Width = 100, DialogResult = DialogResult.OK };

        public AddForm()
        {
            this.Text = "Додати запис";
            this.Size = new System.Drawing.Size(320, 200);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Controls.AddRange(new Control[] { txtText, txtAuthor, btnSave });

            btnSave.Click += (s, e) => {
                NewQuote = new Quote(txtText.Text, txtAuthor.Text, "Загальне", 2026);
            };
        }
    }
}