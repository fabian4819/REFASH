using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp_REFASH.ViewModels
{
    public class AdminSession
    {
        private static Admin _currentAdmin;
        public static Admin CurrentAdmin
        {
            get => _currentAdmin;
            set => _currentAdmin = value; // Notifikasi bisa ditambahkan di sini jika diperlukan
        }
    }
}
