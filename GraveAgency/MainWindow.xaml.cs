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
            btnCatalog.Click += (s, e) => {
                MainContent.Content = new CatalogWindow();
            };

            btnCart.Click += (s, e) => {
                MainContent.Content = new CartPage();
            };

            btnProfile.Click += (s, e) => {
                MainContent.Content = new ProfilePage();
            };

            btnNecromancer.Click += (s, e) => {
                try
                {
                    NecromancerPage necWindow = new NecromancerPage();
                    MainContent.Content = necWindow;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка открытия NecromancerWindow: " + ex.Message);
                }
            };
            btnAdmin.Click += (s, e) => {
                try
                {
                    AdminPage adminWindow = new AdminPage();
                    MainContent.Content = adminWindow;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка открытия AdminWindow: " + ex.Message);
                }
            };

            btnLogout.Click += BtnLogout_Click;
            MainContent.Content = new CatalogWindow();
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
