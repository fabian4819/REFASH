using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp_REFASH.ViewModels
{
    public class UserSession
    {
        private static Customer _currentCustomer;
        public static Customer CurrentCustomer
        {
            get => _currentCustomer;
            set => _currentCustomer = value; // Notifikasi bisa ditambahkan di sini jika diperlukan
        }
    }

}
 