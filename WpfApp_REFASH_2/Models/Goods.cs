using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp_REFASH
{
    public abstract class Goods
    {
        //Encapsulation (protected access modifier)
        public string Name { get; set; }
        public string Description { get; set; }
        //Constructor
        public Goods(string name, string description)
        {
            Name = name;
            Description = description;
        }
        //Virtual method for polymorphism
        public virtual void DisplayInfo()
        {
            Console.WriteLine($"Name: {Name}\nDescription: {Description}");
        }

    }
}
