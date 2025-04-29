using System.Text.Json;

namespace BusuMatchProject.Services
{
    public class CategoryInfercs
    {

        public static string LoadCategories(string FilePath)
        {
            if (!File.Exists(FilePath))
            {
                return "error";
            }

            var text = File.ReadAllText(FilePath);
            var categories = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(text);

            foreach (var category in categories)
            {
                var categoryName = category["category"]?.ToString();
                var keyWords = JsonSerializer.Deserialize<string>("keyword");
                if (keyWords != null)
                {
                    foreach (var keyword in keyWords)
                    {
                        if (text.Contains(keyword, StringComparison.OrdinalIgnoreCase)){
                            return categoryName;
                        }
                    }
                }



            }
            return "other";
        }
    }

}
