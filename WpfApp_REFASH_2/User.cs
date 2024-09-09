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
        protected string name;
        protected string email;
        protected string phone_number;

        public void login()
        {
            Console.WriteLine("User logged in.");
        }
        public void register()
        {
            Console.WriteLine("User registered.");
        }
        public void logout()
        {
            Console.WriteLine("User logged out.");
        }
        public void changePassword()
        {
            Console.WriteLine("Password changed.");
        }
        public void updateProfile()
        {
            Console.WriteLine("Profile updated.");
        }
    }
}
