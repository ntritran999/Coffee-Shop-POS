namespace AdvancedSearch.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public double Price { get; set; }
        public string Tags { get; set; }

        public string FormattedPrice => $"${Price:F2}";
    }
}
