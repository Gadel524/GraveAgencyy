using GraveAgency.Connections;
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
using System.Windows.Shapes;

namespace GraveAgency.Windows
{
    /// <summary>
    /// Логика взаимодействия для CatalogWindow.xaml
    /// </summary>
    public partial class CatalogWindow : Page
    {
        private List<Coffins> allCoffins;

        public CatalogWindow()
        {
            InitializeComponent();
            LoadMaterials();
            LoadCoffins();
            btnSearch.Click += BtnSearch_Click;
        }

        private void LoadMaterials()
        {
            cmbMaterial.Items.Add("Все");
            List<string> materials = new List<string>();
            var materialQuery = Dbb.dbb.Coffins.Select(c => c.Material).Distinct();
            foreach (var m in materialQuery)
            {
                materials.Add(m);
            }
            foreach (string material in materials)
            {
                cmbMaterial.Items.Add(material);
            }
            cmbMaterial.SelectedIndex = 0;
        }

        private void LoadCoffins()
        {
            var query = Dbb.dbb.Coffins.AsQueryable();

            string searchText = txtSearch.Text.Trim();
            if (searchText != "")
            {
                query = query.Where(c => c.Name.Contains(searchText) || c.Material.Contains(searchText));
            }

            string selectedMaterial = cmbMaterial.SelectedItem as string;
            if (selectedMaterial != null && selectedMaterial != "Все")
            {
                query = query.Where(c => c.Material == selectedMaterial);
            }

            if (chkOnlyWithButton.IsChecked == true)
            {
                query = query.Where(c => c.HasEmergencyButton == true);
            }

            allCoffins = query.ToList();
            itemsCatalog.ItemsSource = allCoffins;
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadCoffins();
        }

        private void BtnDetails_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Coffins coffin = btn.Tag as Coffins;

            CoffinDetailWindow detailWindow = new CoffinDetailWindow(coffin.CoffinID);
            detailWindow.ShowDialog();
        }

        private void BtnAddToCart_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Coffins coffin = btn.Tag as Coffins;

            CartPage.AddToCart(coffin);
            MessageBox.Show(coffin.Name + " добавлен в корзину!", "Корзина", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Coffins coffin = btn.Tag as Coffins;

            Window editWindow = new Window();
            editWindow.Title = "Редактирование гроба";
            editWindow.Width = 400;
            editWindow.Height = 250;
            editWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            editWindow.Background = System.Windows.Media.Brushes.Black;
            editWindow.FontFamily = new System.Windows.Media.FontFamily("Times New Roman");

            StackPanel panel = new StackPanel();
            panel.Margin = new Thickness(20);

            TextBlock lblName = new TextBlock();
            lblName.Text = "Название:";
            lblName.Foreground = System.Windows.Media.Brushes.White;
            lblName.Margin = new Thickness(0, 0, 0, 5);

            TextBox txtName = new TextBox();
            txtName.Text = coffin.Name;
            txtName.Background = System.Windows.Media.Brushes.DarkGray;
            txtName.Foreground = System.Windows.Media.Brushes.Black;
            txtName.Margin = new Thickness(0, 0, 0, 15);
            txtName.Padding = new Thickness(5);

            TextBlock lblPrice = new TextBlock();
            lblPrice.Text = "Цена:";
            lblPrice.Foreground = System.Windows.Media.Brushes.White;
            lblPrice.Margin = new Thickness(0, 0, 0, 5);

            TextBox txtPrice = new TextBox();
            txtPrice.Text = coffin.Price.ToString();
            txtPrice.Background = System.Windows.Media.Brushes.DarkGray;
            txtPrice.Foreground = System.Windows.Media.Brushes.Black;
            txtPrice.Margin = new Thickness(0, 0, 0, 15);
            txtPrice.Padding = new Thickness(5);

            Button btnSave = new Button();
            btnSave.Content = "СОХРАНИТЬ";
            btnSave.Background = System.Windows.Media.Brushes.Purple;
            btnSave.Foreground = System.Windows.Media.Brushes.White;
            btnSave.Padding = new Thickness(10);
            btnSave.FontWeight = FontWeights.Bold;
            btnSave.Cursor = Cursors.Hand;

            btnSave.Click += (s, ev) => {
                try
                {
                    coffin.Name = txtName.Text;
                    coffin.Price = decimal.Parse(txtPrice.Text);
                    Dbb.dbb.SaveChanges();
                    LoadCoffins();
                    MessageBox.Show("Гроб обновлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    editWindow.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

            panel.Children.Add(lblName);
            panel.Children.Add(txtName);
            panel.Children.Add(lblPrice);
            panel.Children.Add(txtPrice);
            panel.Children.Add(btnSave);

            editWindow.Content = panel;
            editWindow.ShowDialog();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Coffins coffin = btn.Tag as Coffins;

            MessageBoxResult result = MessageBox.Show("Удалить гроб " + coffin.Name + "?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    Dbb.dbb.Coffins.Remove(coffin);
                    Dbb.dbb.SaveChanges();
                    LoadCoffins();
                    MessageBox.Show("Гроб удален!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка удаления: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}


