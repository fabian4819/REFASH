using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp_REFASH
{
    //Inheritance (Goods)
    internal class Collection : Goods
    {
        //Property for Collection
        public string CollectionID { get; set; }
        public string Status { get; set; }
        public string Category { get; set; }
        //Constructor
        public Collection(string name, string description, string collectionID, string status, string category)
            : base(name, description)
        {
            CollectionID = collectionID;
            Status = status;
            Category = category;
        }

        //Override Method DisplayInfo for Collection (Polymorphism)
        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Console.WriteLine($"Collection ID: {CollectionID}\nStatus: {Status}\nCategory: {Category}");
        }

    }
}
