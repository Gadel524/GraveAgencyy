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

namespace GraveAgency.Pages
{
    /// <summary>
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class AdminPage : Page
    {
        private Users selectedUser;
        private Coffins selectedCoffin;
        private Orders selectedOrder;
        private Necromancers selectedNecromancer;

        public AdminPage()
        {
            InitializeComponent();
            LoadUsers();
            LoadCoffins();
            LoadOrders();
            LoadNecromancers();
            LoadStatistics();

            btnSearchUser.Click += BtnSearchUser_Click;
            btnAddUser.Click += BtnAddUser_Click;
            btnEditUser.Click += BtnEditUser_Click;
            btnDeleteUser.Click += BtnDeleteUser_Click;

            btnAddCoffin.Click += BtnAddCoffin_Click;
            btnEditCoffin.Click += BtnEditCoffin_Click;
            btnDeleteCoffin.Click += BtnDeleteCoffin_Click;

            btnFilterOrders.Click += BtnFilterOrders_Click;
            btnRefreshOrders.Click += BtnRefreshOrders_Click;
            btnUpdateOrderStatus.Click += BtnUpdateOrderStatus_Click;
            btnViewOrderDetails.Click += BtnViewOrderDetails_Click;

            btnAddNecromancer.Click += BtnAddNecromancer_Click;
            btnEditNecromancer.Click += BtnEditNecromancer_Click;
            btnDeleteNecromancer.Click += BtnDeleteNecromancer_Click;

            btnRefreshStats.Click += BtnRefreshStats_Click;

            dgUsers.SelectionChanged += DgUsers_SelectionChanged;
            dgCoffins.SelectionChanged += DgCoffins_SelectionChanged;
            dgOrders.SelectionChanged += DgOrders_SelectionChanged;
            dgNecromancers.SelectionChanged += DgNecromancers_SelectionChanged;
        }

        private void LoadUsers()
        {
            var users = from u in Dbb.dbb.Users
                        join r in Dbb.dbb.Roles on u.RoleID equals r.RoleID
                        select new
                        {
                            u.UserID,
                            u.Username,
                            u.Email,
                            r.RoleName,
                            u.CreatedAt
                        };

            dgUsers.ItemsSource = users.ToList();
        }

        private void BtnSearchUser_Click(object sender, RoutedEventArgs e)
        {
            string searchText = txtUserSearch.Text.Trim();

            if (searchText == "")
            {
                LoadUsers();
            }
            else
            {
                var users = from u in Dbb.dbb.Users
                            join r in Dbb.dbb.Roles on u.RoleID equals r.RoleID
                            where u.Username.Contains(searchText)
                            select new
                            {
                                u.UserID,
                                u.Username,
                                u.Email,
                                r.RoleName,
                                u.CreatedAt
                            };

                dgUsers.ItemsSource = users.ToList();
            }
        }

        private void BtnAddUser_Click(object sender, RoutedEventArgs e)
        {
            Window addWindow = new Window();
            addWindow.Title = "Добавление пользователя";
            addWindow.Width = 400;
            addWindow.Height = 350;
            addWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            addWindow.Background = System.Windows.Media.Brushes.Black;
            addWindow.FontFamily = new System.Windows.Media.FontFamily("Times New Roman");

            StackPanel panel = new StackPanel();
            panel.Margin = new Thickness(20);

            TextBlock lblUsername = new TextBlock();
            lblUsername.Text = "Логин:";
            lblUsername.Foreground = System.Windows.Media.Brushes.White;
            lblUsername.Margin = new Thickness(0, 0, 0, 5);

            TextBox txtUsername = new TextBox();
            txtUsername.Background = System.Windows.Media.Brushes.DarkGray;
            txtUsername.Margin = new Thickness(0, 0, 0, 15);
            txtUsername.Padding = new Thickness(5);

            TextBlock lblPassword = new TextBlock();
            lblPassword.Text = "Пароль:";
            lblPassword.Foreground = System.Windows.Media.Brushes.White;
            lblPassword.Margin = new Thickness(0, 0, 0, 5);

            PasswordBox txtPassword = new PasswordBox();
            txtPassword.Background = System.Windows.Media.Brushes.DarkGray;
            txtPassword.Margin = new Thickness(0, 0, 0, 15);
            txtPassword.Padding = new Thickness(5);

            TextBlock lblEmail = new TextBlock();
            lblEmail.Text = "Email:";
            lblEmail.Foreground = System.Windows.Media.Brushes.White;
            lblEmail.Margin = new Thickness(0, 0, 0, 5);

            TextBox txtEmail = new TextBox();
            txtEmail.Background = System.Windows.Media.Brushes.DarkGray;
            txtEmail.Margin = new Thickness(0, 0, 0, 15);
            txtEmail.Padding = new Thickness(5);

            TextBlock lblRole = new TextBlock();
            lblRole.Text = "Роль:";
            lblRole.Foreground = System.Windows.Media.Brushes.White;
            lblRole.Margin = new Thickness(0, 0, 0, 5);

            ComboBox cmbRole = new ComboBox();
            cmbRole.Background = System.Windows.Media.Brushes.DarkGray;
            cmbRole.Foreground = System.Windows.Media.Brushes.Black;
            cmbRole.Margin = new Thickness(0, 0, 0, 15);
            cmbRole.Padding = new Thickness(5);

            var roles = Dbb.dbb.Roles.ToList();
            foreach (var role in roles)
            {
                cmbRole.Items.Add(role.RoleName);
            }
            cmbRole.SelectedIndex = 1;

            Button btnSave = new Button();
            btnSave.Content = "ДОБАВИТЬ";
            btnSave.Background = System.Windows.Media.Brushes.Purple;
            btnSave.Foreground = System.Windows.Media.Brushes.White;
            btnSave.Padding = new Thickness(10);
            btnSave.FontWeight = FontWeights.Bold;
            btnSave.Cursor = Cursors.Hand;

            btnSave.Click += (s, ev) => {
                try
                {
                    Users newUser = new Users();
                    newUser.Username = txtUsername.Text;
                    newUser.PasswordHash = txtPassword.Password;
                    newUser.Email = txtEmail.Text;
                    newUser.RoleID = cmbRole.SelectedIndex + 1;
                    newUser.CreatedAt = DateTime.Now;

                    Dbb.dbb.Users.Add(newUser);
                    Dbb.dbb.SaveChanges();

                    MessageBox.Show("Пользователь добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadUsers();
                    addWindow.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

            panel.Children.Add(lblUsername);
            panel.Children.Add(txtUsername);
            panel.Children.Add(lblPassword);
            panel.Children.Add(txtPassword);
            panel.Children.Add(lblEmail);
            panel.Children.Add(txtEmail);
            panel.Children.Add(lblRole);
            panel.Children.Add(cmbRole);
            panel.Children.Add(btnSave);

            addWindow.Content = panel;
            addWindow.ShowDialog();
        }

        private void BtnEditUser_Click(object sender, RoutedEventArgs e)
        {
            if (selectedUser == null)
            {
                MessageBox.Show("Выберите пользователя!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Window editWindow = new Window();
            editWindow.Title = "Редактирование пользователя";
            editWindow.Width = 400;
            editWindow.Height = 350;
            editWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            editWindow.Background = System.Windows.Media.Brushes.Black;
            editWindow.FontFamily = new System.Windows.Media.FontFamily("Times New Roman");

            StackPanel panel = new StackPanel();
            panel.Margin = new Thickness(20);

            TextBlock lblUsername = new TextBlock();
            lblUsername.Text = "Логин:";
            lblUsername.Foreground = System.Windows.Media.Brushes.White;
            lblUsername.Margin = new Thickness(0, 0, 0, 5);

            TextBox txtUsername = new TextBox();
            txtUsername.Text = selectedUser.Username;
            txtUsername.Background = System.Windows.Media.Brushes.DarkGray;
            txtUsername.Margin = new Thickness(0, 0, 0, 15);
            txtUsername.Padding = new Thickness(5);

            TextBlock lblEmail = new TextBlock();
            lblEmail.Text = "Email:";
            lblEmail.Foreground = System.Windows.Media.Brushes.White;
            lblEmail.Margin = new Thickness(0, 0, 0, 5);

            TextBox txtEmail = new TextBox();
            txtEmail.Text = selectedUser.Email;
            txtEmail.Background = System.Windows.Media.Brushes.DarkGray;
            txtEmail.Margin = new Thickness(0, 0, 0, 15);
            txtEmail.Padding = new Thickness(5);

            TextBlock lblRole = new TextBlock();
            lblRole.Text = "Роль:";
            lblRole.Foreground = System.Windows.Media.Brushes.White;
            lblRole.Margin = new Thickness(0, 0, 0, 5);

            ComboBox cmbRole = new ComboBox();
            cmbRole.Background = System.Windows.Media.Brushes.DarkGray;
            cmbRole.Foreground = System.Windows.Media.Brushes.Black;
            cmbRole.Margin = new Thickness(0, 0, 0, 15);
            cmbRole.Padding = new Thickness(5);

            var roles = Dbb.dbb.Roles.ToList();
            int roleIndex = 0;
            foreach (var role in roles)
            {
                cmbRole.Items.Add(role.RoleName);
                if (role.RoleID == selectedUser.RoleID)
                {
                    roleIndex = role.RoleID - 1;
                }
            }
            cmbRole.SelectedIndex = roleIndex;

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
                    selectedUser.Username = txtUsername.Text;
                    selectedUser.Email = txtEmail.Text;
                    selectedUser.RoleID = cmbRole.SelectedIndex + 1;

                    Dbb.dbb.SaveChanges();

                    MessageBox.Show("Пользователь обновлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadUsers();
                    editWindow.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

            panel.Children.Add(lblUsername);
            panel.Children.Add(txtUsername);
            panel.Children.Add(lblEmail);
            panel.Children.Add(txtEmail);
            panel.Children.Add(lblRole);
            panel.Children.Add(cmbRole);
            panel.Children.Add(btnSave);

            editWindow.Content = panel;
            editWindow.ShowDialog();
        }

        private void BtnDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (selectedUser == null)
            {
                MessageBox.Show("Выберите пользователя!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBoxResult result = MessageBox.Show("Удалить пользователя " + selectedUser.Username + "?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    Dbb.dbb.Users.Remove(selectedUser);
                    Dbb.dbb.SaveChanges();
                    LoadUsers();
                    MessageBox.Show("Пользователь удален!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DgUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgUsers.SelectedItem != null)
            {
                var selected = dgUsers.SelectedItem;
                var userIdProperty = selected.GetType().GetProperty("UserID");
                if (userIdProperty != null)
                {
                    int userId = (int)userIdProperty.GetValue(selected);
                    selectedUser = Dbb.dbb.Users.FirstOrDefault(u => u.UserID == userId);
                }
            }
        }

        private void LoadCoffins()
        {
            dgCoffins.ItemsSource = Dbb.dbb.Coffins.ToList();
        }

        private void BtnAddCoffin_Click(object sender, RoutedEventArgs e)
        {
            Window addWindow = new Window();
            addWindow.Title = "Добавление гроба";
            addWindow.Width = 450;
            addWindow.Height = 450;
            addWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            addWindow.Background = System.Windows.Media.Brushes.Black;
            addWindow.FontFamily = new System.Windows.Media.FontFamily("Times New Roman");

            StackPanel panel = new StackPanel();
            panel.Margin = new Thickness(20);

            TextBlock lblName = new TextBlock();
            lblName.Text = "Название:";
            lblName.Foreground = System.Windows.Media.Brushes.White;
            lblName.Margin = new Thickness(0, 0, 0, 5);

            TextBox txtName = new TextBox();
            txtName.Background = System.Windows.Media.Brushes.DarkGray;
            txtName.Margin = new Thickness(0, 0, 0, 15);
            txtName.Padding = new Thickness(5);

            TextBlock lblMaterial = new TextBlock();
            lblMaterial.Text = "Материал:";
            lblMaterial.Foreground = System.Windows.Media.Brushes.White;
            lblMaterial.Margin = new Thickness(0, 0, 0, 5);

            TextBox txtMaterial = new TextBox();
            txtMaterial.Background = System.Windows.Media.Brushes.DarkGray;
            txtMaterial.Margin = new Thickness(0, 0, 0, 15);
            txtMaterial.Padding = new Thickness(5);

            TextBlock lblPrice = new TextBlock();
            lblPrice.Text = "Цена:";
            lblPrice.Foreground = System.Windows.Media.Brushes.White;
            lblPrice.Margin = new Thickness(0, 0, 0, 5);

            TextBox txtPrice = new TextBox();
            txtPrice.Background = System.Windows.Media.Brushes.DarkGray;
            txtPrice.Margin = new Thickness(0, 0, 0, 15);
            txtPrice.Padding = new Thickness(5);

            TextBlock lblButton = new TextBlock();
            lblButton.Text = "Кнопка Выход:";
            lblButton.Foreground = System.Windows.Media.Brushes.White;
            lblButton.Margin = new Thickness(0, 0, 0, 5);

            CheckBox chkButton = new CheckBox();
            chkButton.Content = "Есть кнопка";
            chkButton.Foreground = System.Windows.Media.Brushes.White;
            chkButton.Margin = new Thickness(0, 0, 0, 15);

            TextBlock lblDescription = new TextBlock();
            lblDescription.Text = "Описание:";
            lblDescription.Foreground = System.Windows.Media.Brushes.White;
            lblDescription.Margin = new Thickness(0, 0, 0, 5);

            TextBox txtDescription = new TextBox();
            txtDescription.Background = System.Windows.Media.Brushes.DarkGray;
            txtDescription.Margin = new Thickness(0, 0, 0, 15);
            txtDescription.Padding = new Thickness(5);
            txtDescription.Height = 80;
            txtDescription.TextWrapping = TextWrapping.Wrap;

            Button btnSave = new Button();
            btnSave.Content = "ДОБАВИТЬ";
            btnSave.Background = System.Windows.Media.Brushes.Purple;
            btnSave.Foreground = System.Windows.Media.Brushes.White;
            btnSave.Padding = new Thickness(10);
            btnSave.FontWeight = FontWeights.Bold;
            btnSave.Cursor = Cursors.Hand;

            btnSave.Click += (s, ev) => {
                try
                {
                    Coffins newCoffin = new Coffins();
                    newCoffin.Name = txtName.Text;
                    newCoffin.Material = txtMaterial.Text;
                    newCoffin.Price = decimal.Parse(txtPrice.Text);
                    newCoffin.HasEmergencyButton = chkButton.IsChecked == true;
                    newCoffin.Description = txtDescription.Text;
                    newCoffin.Rating = null;

                    Dbb.dbb.Coffins.Add(newCoffin);
                    Dbb.dbb.SaveChanges();

                    MessageBox.Show("Гроб добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadCoffins();
                    addWindow.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

            panel.Children.Add(lblName);
            panel.Children.Add(txtName);
            panel.Children.Add(lblMaterial);
            panel.Children.Add(txtMaterial);
            panel.Children.Add(lblPrice);
            panel.Children.Add(txtPrice);
            panel.Children.Add(lblButton);
            panel.Children.Add(chkButton);
            panel.Children.Add(lblDescription);
            panel.Children.Add(txtDescription);
            panel.Children.Add(btnSave);

            addWindow.Content = panel;
            addWindow.ShowDialog();
        }

        private void BtnEditCoffin_Click(object sender, RoutedEventArgs e)
        {
            if (selectedCoffin == null)
            {
                MessageBox.Show("Выберите гроб!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Window editWindow = new Window();
            editWindow.Title = "Редактирование гроба";
            editWindow.Width = 450;
            editWindow.Height = 450;
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
            txtName.Text = selectedCoffin.Name;
            txtName.Background = System.Windows.Media.Brushes.DarkGray;
            txtName.Margin = new Thickness(0, 0, 0, 15);
            txtName.Padding = new Thickness(5);

            TextBlock lblMaterial = new TextBlock();
            lblMaterial.Text = "Материал:";
            lblMaterial.Foreground = System.Windows.Media.Brushes.White;
            lblMaterial.Margin = new Thickness(0, 0, 0, 5);

            TextBox txtMaterial = new TextBox();
            txtMaterial.Text = selectedCoffin.Material;
            txtMaterial.Background = System.Windows.Media.Brushes.DarkGray;
            txtMaterial.Margin = new Thickness(0, 0, 0, 15);
            txtMaterial.Padding = new Thickness(5);

            TextBlock lblPrice = new TextBlock();
            lblPrice.Text = "Цена:";
            lblPrice.Foreground = System.Windows.Media.Brushes.White;
            lblPrice.Margin = new Thickness(0, 0, 0, 5);

            TextBox txtPrice = new TextBox();
            txtPrice.Text = selectedCoffin.Price.ToString();
            txtPrice.Background = System.Windows.Media.Brushes.DarkGray;
            txtPrice.Margin = new Thickness(0, 0, 0, 15);
            txtPrice.Padding = new Thickness(5);

            TextBlock lblButton = new TextBlock();
            lblButton.Text = "Кнопка Выход:";
            lblButton.Foreground = System.Windows.Media.Brushes.White;
            lblButton.Margin = new Thickness(0, 0, 0, 5);

            CheckBox chkButton = new CheckBox();
            chkButton.Content = "Есть кнопка";
            chkButton.Foreground = System.Windows.Media.Brushes.White;
            chkButton.IsChecked = selectedCoffin.HasEmergencyButton;
            chkButton.Margin = new Thickness(0, 0, 0, 15);

            TextBlock lblDescription = new TextBlock();
            lblDescription.Text = "Описание:";
            lblDescription.Foreground = System.Windows.Media.Brushes.White;
            lblDescription.Margin = new Thickness(0, 0, 0, 5);

            TextBox txtDescription = new TextBox();
            txtDescription.Text = selectedCoffin.Description;
            txtDescription.Background = System.Windows.Media.Brushes.DarkGray;
            txtDescription.Margin = new Thickness(0, 0, 0, 15);
            txtDescription.Padding = new Thickness(5);
            txtDescription.Height = 80;
            txtDescription.TextWrapping = TextWrapping.Wrap;

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
                    selectedCoffin.Name = txtName.Text;
                    selectedCoffin.Material = txtMaterial.Text;
                    selectedCoffin.Price = decimal.Parse(txtPrice.Text);
                    selectedCoffin.HasEmergencyButton = chkButton.IsChecked == true;
                    selectedCoffin.Description = txtDescription.Text;

                    Dbb.dbb.SaveChanges();

                    MessageBox.Show("Гроб обновлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadCoffins();
                    editWindow.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

            panel.Children.Add(lblName);
            panel.Children.Add(txtName);
            panel.Children.Add(lblMaterial);
            panel.Children.Add(txtMaterial);
            panel.Children.Add(lblPrice);
            panel.Children.Add(txtPrice);
            panel.Children.Add(lblButton);
            panel.Children.Add(chkButton);
            panel.Children.Add(lblDescription);
            panel.Children.Add(txtDescription);
            panel.Children.Add(btnSave);

            editWindow.Content = panel;
            editWindow.ShowDialog();
        }

        private void BtnDeleteCoffin_Click(object sender, RoutedEventArgs e)
        {
            if (selectedCoffin == null)
            {
                MessageBox.Show("Выберите гроб!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBoxResult result = MessageBox.Show("Удалить гроб " + selectedCoffin.Name + "?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    Dbb.dbb.Coffins.Remove(selectedCoffin);
                    Dbb.dbb.SaveChanges();
                    LoadCoffins();
                    MessageBox.Show("Гроб удален!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DgCoffins_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedCoffin = dgCoffins.SelectedItem as Coffins;
        }

        private void LoadOrders()
        {
            var orders = Dbb.dbb.Orders.ToList();
            dgOrders.ItemsSource = orders;
        }

        private void BtnFilterOrders_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxItem selectedItem = cmbOrderStatus.SelectedItem as ComboBoxItem;
            string status = selectedItem.Content.ToString();

            if (status == "Все статусы")
            {
                LoadOrders();
            }
            else
            {
                var orders = Dbb.dbb.Orders.Where(o => o.Status == status).ToList();
                dgOrders.ItemsSource = orders;
            }
        }

        private void BtnRefreshOrders_Click(object sender, RoutedEventArgs e)
        {
            LoadOrders();
            cmbOrderStatus.SelectedIndex = 0;
        }

        private void BtnUpdateOrderStatus_Click(object sender, RoutedEventArgs e)
        {
            if (selectedOrder == null)
            {
                MessageBox.Show("Выберите заказ!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Window statusWindow = new Window();
            statusWindow.Title = "Изменение статуса заказа";
            statusWindow.Width = 300;
            statusWindow.Height = 200;
            statusWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            statusWindow.Background = System.Windows.Media.Brushes.Black;
            statusWindow.FontFamily = new System.Windows.Media.FontFamily("Times New Roman");

            StackPanel panel = new StackPanel();
            panel.Margin = new Thickness(20);

            TextBlock lblStatus = new TextBlock();
            lblStatus.Text = "Выберите статус:";
            lblStatus.Foreground = System.Windows.Media.Brushes.White;
            lblStatus.Margin = new Thickness(0, 0, 0, 10);

            ComboBox cmbStatus = new ComboBox();
            cmbStatus.Background = System.Windows.Media.Brushes.DarkGray;
            cmbStatus.Foreground = System.Windows.Media.Brushes.Black;
            cmbStatus.Margin = new Thickness(0, 0, 0, 20);
            cmbStatus.Padding = new Thickness(5);
            cmbStatus.Items.Add("Новый");
            cmbStatus.Items.Add("В обработке");
            cmbStatus.Items.Add("Завершен");
            cmbStatus.Items.Add("Отменен");
            cmbStatus.SelectedItem = selectedOrder.Status;

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
                    selectedOrder.Status = cmbStatus.SelectedItem.ToString();
                    Dbb.dbb.SaveChanges();

                    MessageBox.Show("Статус заказа изменен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadOrders();
                    statusWindow.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

            panel.Children.Add(lblStatus);
            panel.Children.Add(cmbStatus);
            panel.Children.Add(btnSave);

            statusWindow.Content = panel;
            statusWindow.ShowDialog();
        }

        private void BtnViewOrderDetails_Click(object sender, RoutedEventArgs e)
        {
            if (selectedOrder == null)
            {
                MessageBox.Show("Выберите заказ!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var orderItems = from oi in Dbb.dbb.OrderItems
                             join c in Dbb.dbb.Coffins on oi.CoffinID equals c.CoffinID
                             where oi.OrderID == selectedOrder.OrderID
                             select new
                             {
                                 oi.OrderItemID,
                                 c.Name,
                                 oi.Quantity,
                                 oi.Price,
                                 Total = oi.Quantity * oi.Price
                             };

            string itemsText = "Товары в заказе:\n\n";
            foreach (var item in orderItems)
            {
                itemsText = itemsText + item.Name + " x" + item.Quantity + " = " + item.Total.ToString("C") + "\n";
            }
            itemsText = itemsText + "\nОбщая сумма: " + selectedOrder.TotalAmount.ToString("C");

            MessageBox.Show(itemsText, "Детали заказа #" + selectedOrder.OrderID, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DgOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedOrder = dgOrders.SelectedItem as Orders;
        }

        private void LoadNecromancers()
        {
            dgNecromancers.ItemsSource = Dbb.dbb.Necromancers.ToList();
        }

        private void BtnAddNecromancer_Click(object sender, RoutedEventArgs e)
        {
            Window addWindow = new Window();
            addWindow.Title = "Добавление некроманта";
            addWindow.Width = 400;
            addWindow.Height = 400;
            addWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            addWindow.Background = System.Windows.Media.Brushes.Black;
            addWindow.FontFamily = new System.Windows.Media.FontFamily("Times New Roman");

            StackPanel panel = new StackPanel();
            panel.Margin = new Thickness(20);

            TextBlock lblUser = new TextBlock();
            lblUser.Text = "Пользователь:";
            lblUser.Foreground = System.Windows.Media.Brushes.White;
            lblUser.Margin = new Thickness(0, 0, 0, 5);

            ComboBox cmbUser = new ComboBox();
            cmbUser.Background = System.Windows.Media.Brushes.DarkGray;
            cmbUser.Foreground = System.Windows.Media.Brushes.Black;
            cmbUser.Margin = new Thickness(0, 0, 0, 15);
            cmbUser.Padding = new Thickness(5);
            cmbUser.DisplayMemberPath = "Username";
            cmbUser.SelectedValuePath = "UserID";

            var users = Dbb.dbb.Users.Where(u => u.RoleID == 3).ToList();
            foreach (var user in users)
            {
                cmbUser.Items.Add(user);
            }

            TextBlock lblFullName = new TextBlock();
            lblFullName.Text = "ФИО:";
            lblFullName.Foreground = System.Windows.Media.Brushes.White;
            lblFullName.Margin = new Thickness(0, 0, 0, 5);

            TextBox txtFullName = new TextBox();
            txtFullName.Background = System.Windows.Media.Brushes.DarkGray;
            txtFullName.Margin = new Thickness(0, 0, 0, 15);
            txtFullName.Padding = new Thickness(5);

            TextBlock lblExperience = new TextBlock();
            lblExperience.Text = "Опыт (лет):";
            lblExperience.Foreground = System.Windows.Media.Brushes.White;
            lblExperience.Margin = new Thickness(0, 0, 0, 5);

            TextBox txtExperience = new TextBox();
            txtExperience.Background = System.Windows.Media.Brushes.DarkGray;
            txtExperience.Margin = new Thickness(0, 0, 0, 15);
            txtExperience.Padding = new Thickness(5);

            TextBlock lblPhone = new TextBlock();
            lblPhone.Text = "Телефон:";
            lblPhone.Foreground = System.Windows.Media.Brushes.White;
            lblPhone.Margin = new Thickness(0, 0, 0, 5);

            TextBox txtPhone = new TextBox();
            txtPhone.Background = System.Windows.Media.Brushes.DarkGray;
            txtPhone.Margin = new Thickness(0, 0, 0, 15);
            txtPhone.Padding = new Thickness(5);

            Button btnSave = new Button();
            btnSave.Content = "ДОБАВИТЬ";
            btnSave.Background = System.Windows.Media.Brushes.Purple;
            btnSave.Foreground = System.Windows.Media.Brushes.White;
            btnSave.Padding = new Thickness(10);
            btnSave.FontWeight = FontWeights.Bold;
            btnSave.Cursor = Cursors.Hand;

            btnSave.Click += (s, ev) => {
                try
                {
                    Users selectedUserObj = cmbUser.SelectedItem as Users;

                    Necromancers newNecromancer = new Necromancers();
                    newNecromancer.UserID = selectedUserObj.UserID;
                    newNecromancer.FullName = txtFullName.Text;
                    newNecromancer.ExperienceYears = int.Parse(txtExperience.Text);
                    newNecromancer.Phone = txtPhone.Text;
                    newNecromancer.HireDate = DateTime.Now;

                    Dbb.dbb.Necromancers.Add(newNecromancer);
                    Dbb.dbb.SaveChanges();

                    MessageBox.Show("Некромант добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadNecromancers();
                    addWindow.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

            panel.Children.Add(lblUser);
            panel.Children.Add(cmbUser);
            panel.Children.Add(lblFullName);
            panel.Children.Add(txtFullName);
            panel.Children.Add(lblExperience);
            panel.Children.Add(txtExperience);
            panel.Children.Add(lblPhone);
            panel.Children.Add(txtPhone);
            panel.Children.Add(btnSave);

            addWindow.Content = panel;
            addWindow.ShowDialog();
        }

        private void BtnEditNecromancer_Click(object sender, RoutedEventArgs e)
        {
            if (selectedNecromancer == null)
            {
                MessageBox.Show("Выберите некроманта!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Window editWindow = new Window();
            editWindow.Title = "Редактирование некроманта";
            editWindow.Width = 400;
            editWindow.Height = 350;
            editWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            editWindow.Background = System.Windows.Media.Brushes.Black;
            editWindow.FontFamily = new System.Windows.Media.FontFamily("Times New Roman");

            StackPanel panel = new StackPanel();
            panel.Margin = new Thickness(20);

            TextBlock lblFullName = new TextBlock();
            lblFullName.Text = "ФИО:";
            lblFullName.Foreground = System.Windows.Media.Brushes.White;
            lblFullName.Margin = new Thickness(0, 0, 0, 5);

            TextBox txtFullName = new TextBox();
            txtFullName.Text = selectedNecromancer.FullName;
            txtFullName.Background = System.Windows.Media.Brushes.DarkGray;
            txtFullName.Margin = new Thickness(0, 0, 0, 15);
            txtFullName.Padding = new Thickness(5);

            TextBlock lblExperience = new TextBlock();
            lblExperience.Text = "Опыт (лет):";
            lblExperience.Foreground = System.Windows.Media.Brushes.White;
            lblExperience.Margin = new Thickness(0, 0, 0, 5);

            TextBox txtExperience = new TextBox();
            txtExperience.Text = selectedNecromancer.ExperienceYears.ToString();
            txtExperience.Background = System.Windows.Media.Brushes.DarkGray;
            txtExperience.Margin = new Thickness(0, 0, 0, 15);
            txtExperience.Padding = new Thickness(5);

            TextBlock lblPhone = new TextBlock();
            lblPhone.Text = "Телефон:";
            lblPhone.Foreground = System.Windows.Media.Brushes.White;
            lblPhone.Margin = new Thickness(0, 0, 0, 5);

            TextBox txtPhone = new TextBox();
            txtPhone.Text = selectedNecromancer.Phone;
            txtPhone.Background = System.Windows.Media.Brushes.DarkGray;
            txtPhone.Margin = new Thickness(0, 0, 0, 15);
            txtPhone.Padding = new Thickness(5);

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
                    selectedNecromancer.FullName = txtFullName.Text;
                    selectedNecromancer.ExperienceYears = int.Parse(txtExperience.Text);
                    selectedNecromancer.Phone = txtPhone.Text;

                    Dbb.dbb.SaveChanges();

                    MessageBox.Show("Некромант обновлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadNecromancers();
                    editWindow.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

            panel.Children.Add(lblFullName);
            panel.Children.Add(txtFullName);
            panel.Children.Add(lblExperience);
            panel.Children.Add(txtExperience);
            panel.Children.Add(lblPhone);
            panel.Children.Add(txtPhone);
            panel.Children.Add(btnSave);

            editWindow.Content = panel;
            editWindow.ShowDialog();
        }

        private void BtnDeleteNecromancer_Click(object sender, RoutedEventArgs e)
        {
            if (selectedNecromancer == null)
            {
                MessageBox.Show("Выберите некроманта!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBoxResult result = MessageBox.Show("Удалить некроманта " + selectedNecromancer.FullName + "?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    Dbb.dbb.Necromancers.Remove(selectedNecromancer);
                    Dbb.dbb.SaveChanges();
                    LoadNecromancers();
                    MessageBox.Show("Некромант удален!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DgNecromancers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedNecromancer = dgNecromancers.SelectedItem as Necromancers;
        }

        private void LoadStatistics()
        {
            try
            {
                int totalUsers = Dbb.dbb.Users.Count();
                int totalCoffins = Dbb.dbb.Coffins.Count();
                int totalOrders = Dbb.dbb.Orders.Count();
                int totalResurrections = Dbb.dbb.ResurrectionHistory.Count();
                int successResurrections = Dbb.dbb.ResurrectionHistory.Count(r => r.Success == true);
                decimal totalRevenue = Dbb.dbb.Orders.Sum(o => o.TotalAmount);

                txtTotalUsers.Text = "Всего пользователей: " + totalUsers;
                txtTotalCoffins.Text = "Всего гробов: " + totalCoffins;
                txtTotalOrders.Text = "Всего заказов: " + totalOrders;
                txtTotalResurrections.Text = "Всего воскрешений: " + totalResurrections;
                txtSuccessResurrections.Text = "Успешных воскрешений: " + successResurrections;
                txtTotalRevenue.Text = "Общая выручка: " + totalRevenue.ToString("C");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки статистики: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnRefreshStats_Click(object sender, RoutedEventArgs e)
        {
            LoadStatistics();
            MessageBox.Show("Статистика обновлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
