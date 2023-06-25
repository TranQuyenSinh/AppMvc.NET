using App.Models;

namespace App.Services
{
    public class ProductService : List<ProductModel>
    {
        public ProductService()
        {
            this.AddRange(new ProductModel[] {
                new ProductModel()
                {
                    Id = 1,
                    Name = "Samsung",
                    Price = 1000
                },
                new ProductModel()
                {
                    Id = 2,
                    Name = "Apple",
                    Price = 3000
                },
                new ProductModel()
                {
                    Id = 3,
                    Name = "Nokia",
                    Price = 500
                },
                new ProductModel()
                {
                    Id = 4,
                    Name = "Realme",
                    Price = 1500
                }
            });
        }
    }
}