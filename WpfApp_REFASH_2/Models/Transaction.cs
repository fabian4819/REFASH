using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp_REFASH
{
    public class Transaction : Product
    {
        public int OrderID { get; set; } // Order ID
        public DateTime OrderDate { get; set; } // Order date
        public int Quantity { get; set; } // Quantity of the product in the order
        public decimal TotalPricePerItem { get; set; } // Total price per item
        public decimal TotalOrderPrice { get; set; } // Total price for the order
        public string Status { get; set; }

        // Constructor that initializes both Product and Transaction properties
        public Transaction(
            int orderID,
            DateTime orderDate,
            string productName,
            string description,
            string image,
            decimal price,
            string category,
            string size,
            int stock,
            int quantity,
            decimal totalPricePerItem,
            decimal totalOrderPrice,
            string status
        ) : base(productName, description, 0, image, price, category, size, stock)
        {
            OrderID = orderID;
            OrderDate = orderDate;
            Quantity = quantity;
            TotalPricePerItem = totalPricePerItem;
            TotalOrderPrice = totalOrderPrice;
            Status = status;
        }
    }
}
