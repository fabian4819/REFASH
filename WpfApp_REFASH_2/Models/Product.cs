using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp_REFASH
{
    //Inheritance (Goods)
    public class Product : Goods, INotifyPropertyChanged
    {
        private int _productID;
        private string _productName;
        private string _image;
        private decimal _price;
        private string _category;
        private string _size;
        private int _stock;
        private string _description;

        public int ProductID
        {
            get => _productID;
            set => SetField(ref _productID, value);
        }

        public string ProductName
        {
            get => _productName;
            set => SetField(ref _productName, value);
        }

        public string Image
        {
            get => _image;
            set => SetField(ref _image, value);
        }

        public decimal Price
        {
            get => _price;
            set => SetField(ref _price, value);
        }

        public string Category
        {
            get => _category;
            set => SetField(ref _category, value);
        }

        public string Size
        {
            get => _size;
            set => SetField(ref _size, value);
        }

        public int Stock
        {
            get => _stock;
            set => SetField(ref _stock, value);
        }

        public string Description
        {
            get => _description;
            set => SetField(ref _description, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public Product(string name, string description, int productID, string image, decimal price, string category, string size, int stock)
            : base(name, description)  // Assuming Goods also takes these parameters.
        {
            ProductName = name;
            ProductID = productID;
            Description = description;
            Image = image;
            Price = price;
            Category = category;
            Size = size;
            Stock = stock;
        }
    }
}
