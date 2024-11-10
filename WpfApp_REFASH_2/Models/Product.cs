using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp_REFASH
{
    //Inheritance (Goods)
    public class Product : Goods
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } // Add this property if needed
        public string Image { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public string Size { get; set; }
        public int Stock { get; set; }
        public string Description { get; set; }

        public Product(string name, string description, int productID, string image, decimal price, string category, string size, int stock)
            : base(name, description)
        {
            ProductName = name;
            ProductID = productID;
            Description = description;
            Image = image;
            Price = price;
            Category = category;
            Size = size;
            Stock = stock;
        }

        
        //Override Method DisplayInfo for Product (Polymorphism)
        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Console.WriteLine($"Product ID: {ProductID}\nPrice: {Price}\nCategory: {Category}\nSize: {Size}\nStock: {Stock}");
        }
        // Update Stock
        public void UpdateStock(int newStock)
        {
            Stock = newStock;
            Console.WriteLine($"Stock updated for product {ProductID}. New stock: {Stock}.");
        }
    }
}
