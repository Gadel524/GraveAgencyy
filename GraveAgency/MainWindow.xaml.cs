using GraveAgency.Helpers;
using GraveAgency.Pages;
using GraveAgency.Windows;
using System;
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

namespace GraveAgency
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            txtUserName.Text = Session.CurrentUsername;

            btnCatalog.Click += BtnCatalog_Click;
            btnCart.Click += BtnCart_Click;
            btnProfile.Click += BtnProfile_Click;
            btnLogout.Click += BtnLogout_Click;

            if (Session.IsNecromancer)
            {
                btnNecromancer.Visibility = Visibility.Visible;
                btnNecromancer.Click += BtnNecromancer_Click;
            }

            if (Session.IsAdmin)
            {
                btnAdmin.Visibility = Visibility.Visible;
                btnAdmin.Click += BtnAdmin_Click;
            }

            MainContent.Content = new CatalogWindow();
        }

        private void BtnCatalog_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new CatalogWindow();
        }

        private void BtnCart_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new CartPage();
        }

        private void BtnProfile_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new ProfilePage();
        }

        private void BtnNecromancer_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new NecromancerPage();
        }

        private void BtnAdmin_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new AdminPage();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            Session.Logout();
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}
