using BusuMatchProject.Models;
using System.Text.RegularExpressions;

using BusuMatchProject.Models;
using System.Text.RegularExpressions;

namespace BusuMatchProject.Services
{
    public static class InvoiceParser
    {
        public static InvoiceData Parse(string text)
        {
            var invoice = new InvoiceData
            {
                RawText = text,
                Language = text.Any(c => c >= 'א' && c <= 'ת') ? "he" : "en"
            };

            var lines = text.Split('\n').Select(l => l.Trim()).Where(l => l.Length > 1).ToArray();

            if (invoice.Language == "he")
                ParseHebrew(lines, invoice);
            else
                ParseEnglish(lines, invoice);

            invoice.Category = InferCategory(text);
            return invoice;
        }

        private static void ParseHebrew(string[] lines, InvoiceData invoice)
        {
            var text = string.Join("\n", lines);

            foreach (var line in lines)
            {
                if (invoice.InvoiceDate == null && Regex.IsMatch(line, @"תאריך( תשלום| הפקה| פרעון)?"))
                {
                    var match = Regex.Match(line, @"(\d{1,2}[./\-]\d{1,2}[./\-]\d{2,4})");
                    if (match.Success)
                        invoice.InvoiceDate = match.Groups[1].Value;
                }

                if (invoice.InvoiceNumber == null && Regex.IsMatch(line, @"חשבונית.*?(\d{2,10})"))
                {
                    var match = Regex.Match(line, @"(\d{2,10})");
                    if (match.Success)
                        invoice.InvoiceNumber = match.Groups[1].Value;
                }

                if (invoice.TotalWithVat == null && line.Contains("סה\"כ"))
                {
                    var match = Regex.Match(line, @"([\d.,]{2,10})");
                    if (match.Success)
                        invoice.TotalWithVat = CleanAmount(match.Groups[1].Value);
                }

                if (invoice.TotalBeforeVat == null && (line.Contains("לפני מע\"מ") || line.Contains("0%")))
                {
                    var match = Regex.Match(line, @"([\d.,]{2,10})");
                    if (match.Success)
                        invoice.TotalBeforeVat = CleanAmount(match.Groups[1].Value);
                }

                if (invoice.BusinessId == null && Regex.IsMatch(line, @"(עוסק מורשה|ח\\.פ|מספר עוסק)"))
                {
                    var match = Regex.Match(line, @"(\d{7,9})");
                    if (match.Success)
                        invoice.BusinessId = match.Groups[1].Value;
                }

                if (invoice.BusinessName == null && Regex.IsMatch(line, @"(לכבוד|מאת|החברה שלך)"))
                {
                    var match = Regex.Match(line, @"[:\-]?\s*(.*)");
                    if (match.Success)
                        invoice.BusinessName = match.Groups[1].Value.Trim(':', '|', ' ');
                }

                if (invoice.BusinessName == null && lines.IndexOf(line) == 0 && line.Length < 40 && !line.Any(char.IsDigit))
                {
                    invoice.BusinessName = line;
                }
            }

            invoice.DocumentType = text.Contains("חשבונית") ? "Invoice" :
                                   text.Contains("קבלה") ? "Receipt" : "Unknown";
        }

        private static void ParseEnglish(string[] lines, InvoiceData invoice)
        {
            var text = string.Join("\n", lines);

            foreach (var line in lines)
            {
                if (invoice.InvoiceDate == null && line.ToLower().Contains("date"))
                {
                    var match = Regex.Match(line, @"(\d{1,2}[./\-]\d{1,2}[./\-]\d{2,4})");
                    if (match.Success)
                        invoice.InvoiceDate = match.Groups[1].Value;
                }

                if (invoice.InvoiceNumber == null && line.ToLower().Contains("invoice"))
                {
                    var match = Regex.Match(line, @"(\d{3,})");
                    if (match.Success)
                        invoice.InvoiceNumber = match.Groups[1].Value;
                }

                if (invoice.TotalWithVat == null && line.ToLower().Contains("total"))
                {
                    var match = Regex.Match(line, @"([\d.,]{2,10})");
                    if (match.Success)
                        invoice.TotalWithVat = CleanAmount(match.Groups[1].Value);
                }

                if (invoice.TotalBeforeVat == null && line.ToLower().Contains("before tax"))
                {
                    var match = Regex.Match(line, @"([\d.,]{2,10})");
                    if (match.Success)
                        invoice.TotalBeforeVat = CleanAmount(match.Groups[1].Value);
                }

                if (invoice.BusinessName == null && (line.ToLower().Contains("from") || line.ToLower().Contains("bill to")))
                {
                    invoice.BusinessName = line;
                }

                if (invoice.BusinessId == null && line.ToLower().Contains("vat"))
                {
                    var match = Regex.Match(line, @"(\d{7,15})");
                    if (match.Success)
                        invoice.BusinessId = match.Groups[1].Value;
                }
            }

            invoice.DocumentType = text.Contains("Invoice") ? "Invoice" :
                                   text.Contains("Receipt") ? "Receipt" : "Unknown";
        }

        private static string CleanAmount(string raw)
        {
            return raw.Replace(",", "").Replace("₪", "").Trim();
        }

        private static int IndexOf(this string[] lines, string line)
        {
            for (int i = 0; i < lines.Length; i++)
                if (lines[i] == line) return i;
            return -1;
        }

        private static string InferCategory(string text)
        {
            var lower = text.ToLower();
            if (lower.Contains("שופרסל") || lower.Contains("סופר") || lower.Contains("מכולת")) return "מזון";
            if (lower.Contains("פז") || lower.Contains("דלק") || lower.Contains("סונול")) return "רכב";
            if (lower.Contains("מחשבים") || lower.Contains("טכנולוגיה") || lower.Contains("תמיכה")) return "IT";
            if (lower.Contains("שירותי ניקיון") || lower.Contains("תחזוקה")) return "תפעול";
            if (lower.Contains("קורס") || lower.Contains("הדרכה") || lower.Contains("סדנה")) return "הדרכה";
            return "לא מסווג";
        }
    }
}
