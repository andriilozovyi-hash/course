using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;

namespace QuotesApp
{
    public partial class Form1 : Form
    {
        private BindingList<Quote> _quotes = new BindingList<Quote>();
        private DataGridView dgv = new DataGridView();
        private TextBox txtSearch = new TextBox();
        private string filePath = "quotes_data.json";

        public Form1()
        {
            InitializeProjectUI();
            LoadDataFromDisk();
        }

        private void InitializeProjectUI()
        {
            this.Text = "Курсова робота: Збірка крилатих висловів";
            this.Size = new System.Drawing.Size(800, 550);

            // Поле поиска
            txtSearch.Dock = DockStyle.Top;
            txtSearch.PlaceholderText = "Пошук за автором (введіть текст)...";
            txtSearch.TextChanged += (s, e) => {
                var searchText = txtSearch.Text.ToLower();
                var filtered = _quotes.Where(q => q.Author.ToLower().Contains(searchText)).ToList();
                dgv.DataSource = new BindingList<Quote>(filtered);
            };
            this.Controls.Add(txtSearch);

            // Настройка таблицы
            dgv.Dock = DockStyle.Fill;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.DataSource = _quotes;
            dgv.AllowUserToAddRows = false;
            this.Controls.Add(dgv);

            // Панель управления
            Panel pnl = new Panel { Dock = DockStyle.Bottom, Height = 70 };
            Button btnAdd = new Button { Text = "Додати вислів", Left = 20, Top = 20, Width = 120 };
            Button btnDel = new Button { Text = "Видалити", Left = 150, Top = 20, Width = 100 };

            btnAdd.Click += (s, e) => {
                using var f = new AddForm();
                if (f.ShowDialog() == DialogResult.OK && f.NewQuote != null) {
                    _quotes.Add(f.NewQuote);
                    SaveDataToDisk();
                }
            };

            btnDel.Click += (s, e) => {
                if (dgv.CurrentRow != null) {
                    _quotes.RemoveAt(dgv.CurrentRow.Index);
                    SaveDataToDisk();
                }
            };

            pnl.Controls.AddRange(new Control[] { btnAdd, btnDel });
            this.Controls.Add(pnl);
        }

        private void SaveDataToDisk()
        {
            string json = JsonSerializer.Serialize(_quotes);
            File.WriteAllText(filePath, json);
        }

        private void LoadDataFromDisk()
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                var list = JsonSerializer.Deserialize<List<Quote>>(json);
                if (list != null) {
                    _quotes.Clear();
                    foreach (var q in list) _quotes.Add(q);
                }
            }
        }
    }
}