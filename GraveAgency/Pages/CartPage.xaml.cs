using GraveAgency.Connections;
using GraveAgency.Helpers;
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

namespace GraveAgency.Pages
{
    /// <summary>
    /// Логика взаимодействия для CartWindow.xaml
    /// </summary>
    public partial class CartPage : Page
    {
        private static List<CartItem> cartItems = new List<CartItem>();

        public class CartItem
        {
            public int CoffinID { get; set; }
            public string Name { get; set; }
            public string Material { get; set; }
            public string ButtonText { get; set; }
            public decimal Price { get; set; }
        }

        public CartPage()
        {
            InitializeComponent();
            btnClearCart.Click += BtnClearCart_Click;
            btnCheckout.Click += BtnCheckout_Click;
            LoadCart();
        }

        public static void AddToCart(Coffins coffin)
        {
            bool exists = false;
            foreach (CartItem item in cartItems)
            {
                if (item.CoffinID == coffin.CoffinID)
                {
                    exists = true;
                    break;
                }
            }

            if (exists == false)
            {
                CartItem newItem = new CartItem();
                newItem.CoffinID = coffin.CoffinID;
                newItem.Name = coffin.Name;
                newItem.Material = coffin.Material;
                if (coffin.HasEmergencyButton == true)
                {
                    newItem.ButtonText = "С кнопкой";
                }
                else
                {
                    newItem.ButtonText = "Без кнопки";
                }
                newItem.Price = coffin.Price;
                cartItems.Add(newItem);
            }
        }

        public static void ClearCart()
        {
            cartItems.Clear();
        }

        public static List<CartItem> GetCartItems()
        {
            return cartItems;
        }

        public static int GetCartCount()
        {
            return cartItems.Count;
        }

        private void LoadCart()
        {
            dgCart.ItemsSource = null;
            dgCart.ItemsSource = cartItems;

            decimal total = 0;
            foreach (CartItem item in cartItems)
            {
                total = total + item.Price;
            }
            txtTotal.Text = total.ToString("C");

            if (cartItems.Count == 0)
            {
                txtEmptyCart.Visibility = Visibility.Visible;
                dgCart.Visibility = Visibility.Collapsed;
            }
            else
            {
                txtEmptyCart.Visibility = Visibility.Collapsed;
                dgCart.Visibility = Visibility.Visible;
            }
        }

        private void BtnRemoveFromCart_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            CartItem item = btn.Tag as CartItem;
            cartItems.Remove(item);
            LoadCart();
        }

        private void BtnClearCart_Click(object sender, RoutedEventArgs e)
        {
            cartItems.Clear();
            LoadCart();
        }

        private void BtnCheckout_Click(object sender, RoutedEventArgs e)
        {
            if (cartItems.Count == 0)
            {
                MessageBox.Show("Корзина пуста!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                decimal totalAmount = 0;
                foreach (CartItem item in cartItems)
                {
                    totalAmount = totalAmount + item.Price;
                }

                Orders newOrder = new Orders();
                newOrder.UserID = Session.CurrentUserID;
                newOrder.ClientID = null;
                newOrder.OrderDate = DateTime.Now;
                newOrder.Status = "Новый";
                newOrder.TotalAmount = totalAmount;

                Dbb.dbb.Orders.Add(newOrder);
                Dbb.dbb.SaveChanges();

                foreach (CartItem item in cartItems)
                {
                    OrderItems orderItem = new OrderItems();
                    orderItem.OrderID = newOrder.OrderID;
                    orderItem.CoffinID = item.CoffinID;
                    orderItem.Quantity = 1;
                    orderItem.Price = item.Price;
                    Dbb.dbb.OrderItems.Add(orderItem);
                }

                Dbb.dbb.SaveChanges();

                MessageBox.Show("Заказ оформлен! Номер заказа: " + newOrder.OrderID, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                cartItems.Clear();
                LoadCart();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка оформления заказа: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
