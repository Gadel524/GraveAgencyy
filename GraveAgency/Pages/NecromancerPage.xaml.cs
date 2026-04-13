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
    /// Логика взаимодействия для NecromancerWindow.xaml
    /// </summary>
    public partial class NecromancerPage : Page
    {
        public NecromancerPage()
        {
            InitializeComponent();
            LoadData();
            btnPlanSeance.Click += BtnPlanSeance_Click;
            btnCompleteSuccess.Click += BtnCompleteSuccess_Click;
            btnCompleteFail.Click += BtnCompleteFail_Click;
        }

        private void LoadData()
        {
            var orders = Dbb.dbb.Orders.Where(o => o.Status == "Новый" || o.Status == "В обработке").ToList();
            cmbOrders.ItemsSource = orders;
            cmbOrders.DisplayMemberPath = "OrderID";
            cmbOrders.SelectedValuePath = "OrderID";

            var necromancer = Dbb.dbb.Necromancers.FirstOrDefault(n => n.UserID == Session.CurrentUserID);

            if (necromancer != null)
            {
                int successCount = 0;
                int failCount = 0;

                if (necromancer.NecromancerID != null)
                {
                    successCount = Dbb.dbb.ResurrectionHistory.Count(r => r.NecromancerID == necromancer.NecromancerID && r.Success == true);
                    failCount = Dbb.dbb.ResurrectionHistory.Count(r => r.NecromancerID == necromancer.NecromancerID && r.Success == false);
                }

                int total = successCount + failCount;
                int successRate = 0;
                if (total > 0)
                {
                    successRate = successCount * 100 / total;
                }

                txtSuccessCount.Text = "Успешных воскрешений: " + successCount;
                txtFailCount.Text = "Неудачных воскрешений: " + failCount;
                txtSuccessRate.Text = "Процент успеха: " + successRate + "%";
            }

            var allResurrections = Dbb.dbb.ResurrectionHistory.ToList();
            dgResurrections.ItemsSource = allResurrections;
        }

        private void BtnPlanSeance_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Сеанс воскрешения запланирован!", "Планирование", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnCompleteSuccess_Click(object sender, RoutedEventArgs e)
        {
            if (cmbOrders.SelectedItem == null)
            {
                MessageBox.Show("Выберите заказ!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Orders selectedOrder = cmbOrders.SelectedItem as Orders;

            try
            {
                var necromancer = Dbb.dbb.Necromancers.FirstOrDefault(n => n.UserID == Session.CurrentUserID);

                if (necromancer == null)
                {
                    MessageBox.Show("Некромант не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                ResurrectionHistory history = new ResurrectionHistory();
                //history.ClientID = selectedOrder.ClientID;
                history.NecromancerID = necromancer.NecromancerID;
                history.OrderID = selectedOrder.OrderID;
                history.ResurrectionDate = DateTime.Now;
                history.Success = true;
                history.Notes = "Успешное воскрешение";

                Dbb.dbb.ResurrectionHistory.Add(history);

                selectedOrder.Status = "Завершен";

                if (selectedOrder.ClientID != null)
                {
                    var client = Dbb.dbb.Clients.FirstOrDefault(c => c.ClientID == selectedOrder.ClientID);
                    if (client != null)
                    {
                        client.IsAlive = true;
                    }
                }

                Dbb.dbb.SaveChanges();

                MessageBox.Show("Воскрешение прошло успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCompleteFail_Click(object sender, RoutedEventArgs e)
        {
            if (cmbOrders.SelectedItem == null)
            {
                MessageBox.Show("Выберите заказ!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Orders selectedOrder = cmbOrders.SelectedItem as Orders;

            try
            {
                var necromancer = Dbb.dbb.Necromancers.FirstOrDefault(n => n.UserID == Session.CurrentUserID);

                if (necromancer == null)
                {
                    MessageBox.Show("Некромант не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                ResurrectionHistory history = new ResurrectionHistory();
               //history.ClientID = selectedOrder.ClientID;
                history.NecromancerID = necromancer.NecromancerID;
                history.OrderID = selectedOrder.OrderID;
                history.ResurrectionDate = DateTime.Now;
                history.Success = false;
                history.Notes = "Воскрешение не удалось";

                Dbb.dbb.ResurrectionHistory.Add(history);
                selectedOrder.Status = "Отменен";
                Dbb.dbb.SaveChanges();

                MessageBox.Show("Воскрешение не удалось!", "Неудача", MessageBoxButton.OK, MessageBoxImage.Warning);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
