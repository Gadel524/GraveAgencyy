using GraveAgency.Connections;
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
    /// Логика взаимодействия для CoffinDetailWindow.xaml
    /// </summary>
    public partial class CoffinDetailWindow : Window
    {
        private int coffinID;

        public CoffinDetailWindow(int coffinId)
        {
            InitializeComponent();
            coffinID = coffinId;
            LoadCoffinDetails();
            btnClose.Click += (s, e) => this.Close();
        }

        private void LoadCoffinDetails()
        {
            var coffin = Dbb.dbb.Coffins.FirstOrDefault(c => c.CoffinID == coffinID);

            if (coffin != null)
            {
                txtName.Text = coffin.Name;
                txtMaterial.Text = coffin.Material;

                if (coffin.HasEmergencyButton == true)
                {
                    txtButton.Text = "Есть кнопка экстренного выхода";
                }
                else
                {
                    txtButton.Text = "Нет кнопки выхода";
                }

                txtPrice.Text = coffin.Price.ToString("C");

                if (coffin.Description != null)
                {
                    txtDescription.Text = coffin.Description;
                }
                else
                {
                    txtDescription.Text = "Описание отсутствует";
                }

                if (coffin.Rating != null)
                {
                    string stars = "";
                    int ratingValue = (int)coffin.Rating;
                    for (int i = 0; i < ratingValue; i++)
                    {
                        stars = stars + "★";
                    }
                    for (int i = ratingValue; i < 5; i++)
                    {
                        stars = stars + "☆";
                    }
                    txtRating.Text = stars + " (" + coffin.Rating.Value.ToString("0.0") + ")";
                }
                else
                {
                    txtRating.Text = "☆☆☆☆☆ (нет оценок)";
                }

                var orderItems = Dbb.dbb.OrderItems.Where(oi => oi.CoffinID == coffinID).ToList();
                var orderIds = new System.Collections.Generic.List<int>();
                foreach (var oi in orderItems)
                {
                    orderIds.Add(oi.OrderID);
                }

                var orders = Dbb.dbb.Orders.Where(o => orderIds.Contains(o.OrderID)).ToList();

                var orderList = new System.Collections.Generic.List<string>();
                if (coffin.Rating.HasValue)
                {
                    string stars = "";
                    int ratingInt = (int)coffin.Rating.Value;

                    for (int i = 0; i < ratingInt; i++)
                    {
                        stars = stars + "★";
                    }
                    for (int i = ratingInt; i < 5; i++)
                    {
                        stars = stars + "☆";
                    }

                    decimal ratingValue = coffin.Rating.Value;
                    txtRating.Text = stars + " (" + ratingValue.ToString("0.0") + ")";
                }
                else
                {
                    txtRating.Text = "☆☆☆☆☆ (нет оценок)";
                }

                lstOrders.ItemsSource = orderList;
            }
        }
    }
}
