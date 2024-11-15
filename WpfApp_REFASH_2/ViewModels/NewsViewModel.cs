using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp_REFASH.ViewModels
{
    public class NewsViewModel
    {
        public Customer Customer { get; private set; }
        public ObservableCollection<Content> ContentItems { get; private set; }

        public NewsViewModel(Customer customer)
        {
            Customer = customer;
            ContentItems = Customer.GetAllContent();
            foreach (var content in ContentItems)
            {
                content.Customer = Customer;
            }
        }

        private void LoadContentItems()
        {
            ContentItems = Customer?.GetAllContent();
            foreach (var content in ContentItems)
            {
                content.Customer = Customer;
            }
        }
    }
}
