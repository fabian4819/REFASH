using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using WpfApp_REFASH.DataAccess;

namespace WpfApp_REFASH
{
    // Inheritance (User)
    internal class Admin : User
    {
        //Property for Admin
        public string AdminID { get; private set; }
        private int TotalContent { get; set; }
        //Constructor for Admin
        public Admin(string name, string email, string phoneNumber, string password, string role, string adminID)
           : base(name, email, phoneNumber, password, role)
        {
            AdminID = adminID;
            TotalContent = 0;
        }
        // Override Method UpdateProfile for Admin (Polymorphism)
        
    }
}
