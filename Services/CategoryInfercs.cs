using System.Text.Json;

namespace BusuMatchProject.Services
{
    public class CategoryInfercs
    {
        public static string LoadCategories(string filePath, string textToMatch)
        {
            if (!File.Exists(filePath))
                return "error";

            var json = File.ReadAllText(filePath);

            try
            {
                var categories = JsonSerializer.Deserialize<List<CategoryItem>>(json);
                if (categories == null) return "other";

                foreach (var category in categories)
                {
                    foreach (var keyword in category.Keywords)
                    {
                        if (textToMatch.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                        {
                            return category.Category;
                        }
                    }
                }

                return "other";
            }
            catch
            {
                return "error";
            }
        }

        private class CategoryItem
        {
            public string Category { get; set; } = string.Empty;
            public List<string> Keywords { get; set; } = new();
        }
    }
}
