using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

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
        public byte[] ImageData { get; set; }
        public BitmapImage BitmapImage { get; set; }
        //Constructor
        public Collection(string name, string description, int collectionID, string status, string category, string image_path)
            : base(name, description)
        {
            CollectionID = collectionID;
            Status = status;
            Category = category;
            ImagePath = image_path;
            Description = description;
        }
        public Collection(string name, string description, string category = "Undefined", string imagePath = "default.png")
            : base(name, description) 
        {
            Name = name;
            Description = description;
            Category = category;
            ImagePath = imagePath;
        }
        public Collection(string name, string description, string category = "Undefined", byte[] imageData = null)
            : base(name, description)
        {
            Name = name;
            Description = description;
            Category = category;
            ImageData = imageData;
        }
        public Collection(string name, string description, string category = "Undefined", BitmapImage bitmapImage = null)
            : base(name, description)
        {
            Name = name;
            Description = description;
            Category = category;
            BitmapImage = bitmapImage;
        }
        public Collection(string name, string description, int collectionID, string status, string category, string image_path, byte[] image_data )
             : base(name, description)
        {
            CollectionID = collectionID;
            Status = status;
            Category = category;
            ImagePath = image_path;
            Description = description;
            ImageData = image_data;
        }
        public Collection(string name, string description, int collectionID, string status, string category, string image_path, BitmapImage bitmapImage)
             : base(name, description)
        {
            CollectionID = collectionID;
            Status = status;
            Category = category;
            ImagePath = image_path;
            Description = description;
            BitmapImage = bitmapImage;
        }


        //Override Method DisplayInfo for Collection (Polymorphism)
        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Console.WriteLine($"Collection ID: {CollectionID}\nStatus: {Status}\nCategory: {Category}");
        }

    }
}
