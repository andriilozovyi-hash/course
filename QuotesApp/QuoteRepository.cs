using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace QuotesApp
{
    public class QuoteRepository
    {
        private readonly string _filePath;
        
        // Головна колекція висловів
        public BindingList<Quote> Quotes { get; private set; }
        
        // Список категорій для ComboBox
        public List<string> Categories { get; private set; }

        public QuoteRepository(string filePath = "data.json")
        {
            _filePath = filePath;
            Quotes = new BindingList<Quote>();
            Categories = new List<string> { "Філософія", "Наука", "Гумор", "Мистецтво" }; // Дефолтні
            LoadData();
        }

        public void Add(Quote quote)
        {
            if (quote == null) return;
            Quotes.Add(quote);
            UpdateCategoriesList();
            SaveData();
        }

        public void Update(Quote oldQuote, Quote updatedQuote)
        {
            int idx = Quotes.IndexOf(oldQuote);
            if (idx != -1 && updatedQuote != null)
            {
                Quotes[idx] = updatedQuote;
                UpdateCategoriesList();
                SaveData();
            }
        }

        public void Delete(Quote quote)
        {
            if (quote != null && Quotes.Contains(quote))
            {
                Quotes.Remove(quote);
                UpdateCategoriesList();
                SaveData();
            }
        }

        // Додавання нової категорії вручну
        public bool AddCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category)) return false;
            
            // Перевіряємо, чи немає вже такої категорії (ігноруючи регістр)
            bool exists = Categories.Any(c => c.Equals(category, StringComparison.OrdinalIgnoreCase));
            if (!exists)
            {
                Categories.Add(category);
                return true;
            }
            return false;
        }

        // Комбінована фільтрація: Автор + Категорія + Рік + ПОШУК ЗА ТЕКСТОМ
        public BindingList<Quote> GetFiltered(string textSearch, string author, string category, decimal yearFrom, decimal yearTo)
        {
            var filtered = Quotes.Where(q => 
                (string.IsNullOrEmpty(textSearch) || q.Text.Contains(textSearch, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(author) || q.Author.Contains(author, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(category) || q.Category.Equals(category, StringComparison.OrdinalIgnoreCase)) &&
                (q.Year >= yearFrom && q.Year <= yearTo)
            ).ToList();

            return new BindingList<Quote>(filtered);
        }

        // Експорт у звичайний гарний текстовий файл для друку
        public void ExportToTxt(string targetPath)
        {
            using StreamWriter writer = new StreamWriter(targetPath, false, System.Text.Encoding.UTF8);
            writer.WriteLine("==================================================");
            writer.WriteLine($"        ЗВІТ: ДОВІДНИК КРИЛАТИХ ВИСЛОВІВ        ");
            writer.WriteLine($"         Дата генерації: {DateTime.Now:dd.MM.yyyy}       ");
            writer.WriteLine("==================================================");
            writer.WriteLine();

            int counter = 1;
            foreach (var q in Quotes)
            {
                writer.WriteLine($"{counter}. Вислів: \"{q.Text}\"");
                writer.WriteLine($"   Автор:   {q.Author}");
                writer.WriteLine($"   Рубрика: {q.Category} ({q.Year} р.)");
                writer.WriteLine("--------------------------------------------------");
                counter++;
            }
        }

        private void SaveData()
        {
            try
            {
                string json = JsonSerializer.Serialize(Quotes, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Помилка збереження даних: {ex.Message}", "Помилка", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        private void LoadData()
        {
            if (!File.Exists(_filePath)) return;
            try
            {
                string json = File.ReadAllText(_filePath);
                var data = JsonSerializer.Deserialize<List<Quote>>(json);
                if (data != null)
                {
                    foreach (var q in data) Quotes.Add(q);
                }
                UpdateCategoriesList();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Помилка завантаження даних: {ex.Message}", "Помилка", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        // Збирає унікальні категорії з наявних у файлі висловів
        private void UpdateCategoriesList()
        {
            foreach (var q in Quotes)
            {
                if (!string.IsNullOrWhiteSpace(q.Category) && !Categories.Any(c => c.Equals(q.Category, StringComparison.OrdinalIgnoreCase)))
                {
                    Categories.Add(q.Category);
                }
            }
        }
    }
}