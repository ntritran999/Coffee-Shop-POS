namespace AdvancedSearch.Models
{
    public class SearchCriteria
    {
        public string Keyword { get; set; }
        public string Category { get; set; }
        public double? MinPrice { get; set; }
        public double? MaxPrice { get; set; }
    }
}
