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
        private BindingList<Quote> _allQuotes = new BindingList<Quote>();
        private DataGridView dgv = new DataGridView();
        private string filePath = "data.json";

        // Поля фильтрации
        private TextBox fAuthor = new TextBox();
        private TextBox fCat = new TextBox();
        private NumericUpDown fYearFrom = new NumericUpDown { Minimum = 0, Maximum = 2026, Value = 0 };
        private NumericUpDown fYearTo = new NumericUpDown { Minimum = 0, Maximum = 2026, Value = 2026 };

        public Form1()
        {
            InitializeInterface();
            LoadData();
        }

        private void InitializeInterface()
        {
            this.Text = "Довідник крилатих висловів";
            this.Size = new System.Drawing.Size(950, 600);

            // Главный контейнер
            TableLayoutPanel mainLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2 };
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 75));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));

            // Левая часть: Таблица и кнопки
            Panel leftPanel = new Panel { Dock = DockStyle.Fill };
            dgv.Dock = DockStyle.Fill;
            dgv.DataSource = _allQuotes;
            dgv.ReadOnly = true;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            Panel btnPanel = new Panel { Dock = DockStyle.Top, Height = 50 };
            Button btnAdd = new Button { Text = "Додати", Left = 10, Top = 10 };
            Button btnEdit = new Button { Text = "Редагувати", Left = 100, Top = 10 };
            Button btnDel = new Button { Text = "Видалити", Left = 190, Top = 10 };

            btnAdd.Click += (s, e) => {
                using var f = new AddForm();
                if (f.ShowDialog() == DialogResult.OK && f.ResultQuote != null) {
                    _allQuotes.Add(f.ResultQuote);
                    SaveData();
                }
            };

            btnEdit.Click += (s, e) => {
                if (dgv.CurrentRow == null) return;
                var selected = (Quote)dgv.CurrentRow.DataBoundItem;
                using var f = new AddForm(selected);
                if (f.ShowDialog() == DialogResult.OK && f.ResultQuote != null) {
                    int idx = _allQuotes.IndexOf(selected);
                    _allQuotes[idx] = f.ResultQuote;
                    SaveData();
                }
            };

            btnDel.Click += (s, e) => {
                if (dgv.CurrentRow == null) return;
                if (MessageBox.Show("Видалити цей запис?", "Підтвердження", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                    _allQuotes.RemoveAt(dgv.CurrentRow.Index);
                    SaveData();
                }
            };

            btnPanel.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDel });
            leftPanel.Controls.Add(dgv);
            leftPanel.Controls.Add(btnPanel);

            // Правая часть: Фильтрация
            FlowLayoutPanel filterPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            filterPanel.Controls.Add(new Label { Text = "ФІЛЬТРАЦІЯ", Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold) });
            filterPanel.Controls.Add(new Label { Text = "Автор:", Width = 150 });
            filterPanel.Controls.Add(fAuthor);
            filterPanel.Controls.Add(new Label { Text = "Категорія:", Width = 150 });
            filterPanel.Controls.Add(fCat);
            filterPanel.Controls.Add(new Label { Text = "Рік від:", Width = 150 });
            filterPanel.Controls.Add(fYearFrom);
            filterPanel.Controls.Add(new Label { Text = "Рік до:", Width = 150 });
            filterPanel.Controls.Add(fYearTo);

            Button btnApply = new Button { Text = "Застосувати", Width = 100, Margin = new Padding(0, 10, 0, 0) };
            btnApply.Click += ApplyFilters;
            Button btnReset = new Button { Text = "Скинути", Width = 100 };
            btnReset.Click += (s, e) => { dgv.DataSource = _allQuotes; };

            filterPanel.Controls.Add(btnApply);
            filterPanel.Controls.Add(btnReset);

            mainLayout.Controls.Add(leftPanel, 0, 0);
            mainLayout.Controls.Add(filterPanel, 1, 0);
            this.Controls.Add(mainLayout);
        }

        private void ApplyFilters(object? sender, EventArgs e)
        {
            var filtered = _allQuotes.Where(q => 
                (string.IsNullOrEmpty(fAuthor.Text) || q.Author.Contains(fAuthor.Text, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(fCat.Text) || q.Category.Contains(fCat.Text, StringComparison.OrdinalIgnoreCase)) &&
                (q.Year >= fYearFrom.Value && q.Year <= fYearTo.Value)
            ).ToList();
            dgv.DataSource = new BindingList<Quote>(filtered);
        }

        private void SaveData() => File.WriteAllText(filePath, JsonSerializer.Serialize(_allQuotes));
        private void LoadData() {
            if (File.Exists(filePath)) {
                var data = JsonSerializer.Deserialize<List<Quote>>(File.ReadAllText(filePath));
                if (data != null) foreach (var q in data) _allQuotes.Add(q);
            }
        }
    }
}