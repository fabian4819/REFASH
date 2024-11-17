using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp_REFASH.ViewModels
{
    public class UserSession
    {
        // Untuk Customer
        private static Customer _currentCustomer;
        public static Customer CurrentCustomer
        {
            get => _currentCustomer;
            set => _currentCustomer = value;
        }

        // Untuk Admin
        private static Admin _currentAdmin;
        public static Admin CurrentAdmin
        {
            get => _currentAdmin;
            set => _currentAdmin = value;
        }

        // Optional: Method untuk membersihkan session
        public static void ClearSession()
        {
            _currentCustomer = null;
            _currentAdmin = null;
        }
    }
}