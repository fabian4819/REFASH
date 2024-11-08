﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp_REFASH
{
    /// <summary>
    /// Interaction logic for NewsCard.xaml
    /// </summary>
    public partial class NewsCard : UserControl
    {
        public NewsCard()
        {
            InitializeComponent();
        }

        private void btn_detail_click(object sender, RoutedEventArgs e)
        {
            NewsDetailWindow newsDetailWindow = new NewsDetailWindow();
            newsDetailWindow.Show();
            Window parentWindow = Window.GetWindow(this);
            parentWindow?.Close();
        }
    }
}
