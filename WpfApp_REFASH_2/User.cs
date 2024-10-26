using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp_REFASH
{
    public abstract class User
    {
        //Encapsulation (protected access modifier)
        protected string Name { get; set; }
        protected string Email { get; set; }
        protected string PhoneNumber { get; set; }

        //Constructor
        protected User(string name, string email, string phoneNumber)
        {
            Name = name;
            Email = email;
            PhoneNumber = phoneNumber;
        }
        //Login
        public virtual void Login()
        {
            Console.WriteLine($"{Name} logged in.");
        }
        //Register
        public virtual void Register()
        {
            Console.WriteLine($"{Name} registered.");
        }
        //Logout
        public virtual void Logout()
        {
            Console.WriteLine($"{Name} logged out.");
        }
        //Change Password
        public void ChangePassword()
        {
            Console.WriteLine("Password changed.");
        }
        //Polymorphism (updateProfile method)
        public virtual void UpdateProfile()
        {
            Console.WriteLine("Profile updated.");
        }
    }
}
