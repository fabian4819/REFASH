using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp_REFASH
{
    //Inheritance (Goods)

    public class Collection : Goods
    {
        //Property for Collection
        public int CollectionID { get; set; }
        public string Status { get; set; }
        public string Category { get; set; }
        public string ImagePath { get; set; }
        //Constructor
        public Collection(string name, string description, int collectionID, string status, string category, string image_path)
            : base(name, description)
        {
            CollectionID = collectionID;
            Status = status;
            Category = category;
            ImagePath = image_path;
        }

        //Override Method DisplayInfo for Collection (Polymorphism)
        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Console.WriteLine($"Collection ID: {CollectionID}\nStatus: {Status}\nCategory: {Category}");
        }

    }
}
