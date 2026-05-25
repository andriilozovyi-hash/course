using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace QuotesApp
{
    public partial class Form1 : Form
    {
        // Використовуємо наш новий клас-колекцію
        private QuoteRepository _repository = new QuoteRepository();
        private DataGridView dgv = new DataGridView();

        // Поля фільтрації та пошуку
        private TextBox fTextSearch = new TextBox { Width = 200 }; // НОВЕ ПОЛЕ
        private TextBox fAuthor = new TextBox { Width = 200 };
        private ComboBox fCat = new ComboBox { Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
        private NumericUpDown fYearFrom = new NumericUpDown { Minimum = -3000, Maximum = 2026, Value = -3000, Width = 200 };
        private NumericUpDown fYearTo = new NumericUpDown { Minimum = -3000, Maximum = 2026, Value = 2026, Width = 200 };

        public Form1()
        {
            InitializeInterface();
            RefreshCategoryFilter();
        }

        private void InitializeInterface()
        {
            this.Text = "Довідник крилатих висловів";
            this.Size = new System.Drawing.Size(1000, 620);
            this.StartPosition = FormStartPosition.CenterScreen;

            TableLayoutPanel mainLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2 };
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 72));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28));

            // Левая часть: Таблица и кнопки CRUD + ТХТ
            Panel leftPanel = new Panel { Dock = DockStyle.Fill };
            dgv.Dock = DockStyle.Fill;
            dgv.DataSource = _repository.Quotes; // Пряма прив'язка до репозиторію
            dgv.ReadOnly = true;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            Panel btnPanel = new Panel { Dock = DockStyle.Top, Height = 50 };
            Button btnAdd = new Button { Text = "Додати", Left = 10, Top = 10, Width = 90 };
            Button btnEdit = new Button { Text = "Редагувати", Left = 110, Top = 10, Width = 90 };
            Button btnDel = new Button { Text = "Видалити", Left = 210, Top = 10, Width = 90 };
            Button btnExport = new Button { Text = "Зберегти у TXT", Left = 320, Top = 10, Width = 120 }; // КНОПКА ЕКСПОРТУ

            btnAdd.Click += (s, e) => {
                using var f = new AddForm(_repository.Categories);
                if (f.ShowDialog() == DialogResult.OK && f.ResultQuote != null) {
                    _repository.Add(f.ResultQuote);
                    RefreshCategoryFilter();
                }
            };

            btnEdit.Click += (s, e) => {
                if (dgv.CurrentRow == null) return;
                var selected = (Quote)dgv.CurrentRow.DataBoundItem;
                using var f = new AddForm(_repository.Categories, selected);
                if (f.ShowDialog() == DialogResult.OK && f.ResultQuote != null) {
                    _repository.Update(selected, f.ResultQuote);
                    RefreshCategoryFilter();
                }
            };

            btnDel.Click += (s, e) => {
                if (dgv.CurrentRow == null) return;
                var selected = (Quote)dgv.CurrentRow.DataBoundItem;
                if (MessageBox.Show("Видалити цей запис?", "Підтвердження", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
                    _repository.Delete(selected);
                    RefreshCategoryFilter();
                }
            };

            btnExport.Click += (s, e) => {
                using SaveFileDialog sfd = new SaveFileDialog { Filter = "Текстові файли (*.txt)|*.txt", FileName = "Quotes_Report.txt" };
                if (sfd.ShowDialog() == DialogResult.OK) {
                    _repository.ExportToTxt(sfd.FileName);
                    MessageBox.Show("Дані успішно експортовано у текстовий файл!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };

            btnPanel.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDel, btnExport });
            leftPanel.Controls.Add(dgv);
            leftPanel.Controls.Add(btnPanel);

            // Правая часть: Фильтрация и поиск
            FlowLayoutPanel filterPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(15), FlowDirection = FlowDirection.TopDown };
            filterPanel.Controls.Add(new Label { Text = "ПОШУК ТА ФІЛЬТРАЦІЯ", Font = new System.Drawing.Font("Arial", 11, System.Drawing.FontStyle.Bold), Margin = new Padding(0,0,0,15) });
            
            filterPanel.Controls.Add(new Label { Text = "Пошук за текстом вислову:", AutoSize = true });
            filterPanel.Controls.Add(fTextSearch);
            
            filterPanel.Controls.Add(new Label { Text = "Автор:", AutoSize = true, Margin = new Padding(0, 10, 0, 0) });
            filterPanel.Controls.Add(fAuthor);
            
            filterPanel.Controls.Add(new Label { Text = "Категорія:", AutoSize = true, Margin = new Padding(0, 10, 0, 0) });
            filterPanel.Controls.Add(fCat);
            
            filterPanel.Controls.Add(new Label { Text = "Рік від:", AutoSize = true, Margin = new Padding(0, 10, 0, 0) });
            filterPanel.Controls.Add(fYearFrom);
            
            filterPanel.Controls.Add(new Label { Text = "Рік до:", AutoSize = true, Margin = new Padding(0, 10, 0, 0) });
            filterPanel.Controls.Add(fYearTo);

            // Панель для ідеально вирівняних кнопок фільтра
            FlowLayoutPanel filterButtonsPanel = new FlowLayoutPanel { Width = 210, Height = 40, Margin = new Padding(0, 20, 0, 0) };
            Button btnApply = new Button { Text = "Застосувати", Width = 100, Height = 30, Margin = new Padding(0, 0, 5, 0) };
            Button btnReset = new Button { Text = "Скинути", Width = 100, Height = 30, Margin = new Padding(5, 0, 0, 0) };

            btnApply.Click += ApplyFilters;
            btnReset.Click += (s, e) => {
                fTextSearch.Text = "";
                fAuthor.Text = "";
                if (fCat.Items.Count > 0) fCat.SelectedIndex = 0;
                fYearFrom.Value = -3000;
                fYearTo.Value = 2026;
                dgv.DataSource = _repository.Quotes; 
            };

            filterButtonsPanel.Controls.AddRange(new Control[] { btnApply, btnReset });
            filterPanel.Controls.Add(filterButtonsPanel);

            mainLayout.Controls.Add(leftPanel, 0, 0);
            mainLayout.Controls.Add(filterPanel, 1, 0);
            this.Controls.Add(mainLayout);
        }

        private void ApplyFilters(object? sender, EventArgs e)
        {
            string selectedCat = fCat.Text == "Всі категорії" ? "" : fCat.Text;
            dgv.DataSource = _repository.GetFiltered(fTextSearch.Text, fAuthor.Text, selectedCat, fYearFrom.Value, fYearTo.Value);
        }

        // Оновлює фільтр категорій на головній формі
        private void RefreshCategoryFilter()
        {
            fCat.Items.Clear();
            fCat.Items.Add("Всі категорії");
            fCat.Items.AddRange(_repository.Categories.ToArray());
            fCat.SelectedIndex = 0;
        }
    }
}