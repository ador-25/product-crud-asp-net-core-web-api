using ProductCrud.Models;

namespace ProductCrud.ViewModels
{
    public class ProductViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsAvailable { get; set; }
        public int? Id { get; set; }

        public Product GetProduct()
        {
            return new Product()
            {
                Name = this.Name,
                Description = this.Description,
                IsAvailable = this.IsAvailable,
            };
        }
    }
}
