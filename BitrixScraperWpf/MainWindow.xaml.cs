using ScraperLogic;

namespace BitrixScraperWpf
{
    using System;
    using System.Windows;
    using ScraperLogic.Repository;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(ScraperController controller)
        {
            InitializeComponent();
            DataContext = controller;
        }
    }
}