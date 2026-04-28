using System;

namespace QuotesApp
{
    public class Quote
    {
        public string Text { get; set; } = "";
        public string Author { get; set; } = "";
        public string Category { get; set; } = "";
        public int Year { get; set; }

        public Quote() { }

        public Quote(string text, string author, string category, int year)
        {
            Text = text;
            Author = author;
            Category = category;
            Year = year;
        }
    }
}