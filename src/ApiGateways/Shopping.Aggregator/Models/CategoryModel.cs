namespace Shopping.Aggregator.Models
{
    public class CategoryModel
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public bool IsMiniTruck { get; set; }
        public int CategoryOrder { get; set; }
    }
}