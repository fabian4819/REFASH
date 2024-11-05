using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp_REFASH
{
    //Inheritance (Goods)
    internal class Product : Goods
    {
        //Property for Product
        public string ProductID { get; set; }
        public byte[] Image { get; set; }
        public string Price { get; set; }
        public string Category { get; set; }
        public string Size { get; set; }
        public int Stock { get; set; }

        //Constructor
        public Product(string name, string description, string productID, byte[] image, string price, string category, string size, int stock)
            : base(name, description)
        {
            ProductID = productID;
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
