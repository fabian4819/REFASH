using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp_REFASH
{
    internal abstract class Goods
    {
        //Encapsulation (protected access modifier)
        protected string Name { get; set; }
        protected string Description { get; set; }
        //Constructor
        protected Goods(string name, string description)
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
