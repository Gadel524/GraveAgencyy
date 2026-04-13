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
    /// Логика взаимодействия для ProfileWindow.xaml
    /// </summary>
    public partial class ProfilePage : Page
    {
        public ProfilePage()
        {
            InitializeComponent();
            LoadProfile();
            LoadGriefLevel();
            btnSaveProfile.Click += BtnSaveProfile_Click;
            btnChangePassword.Click += BtnChangePassword_Click;
            btnSaveGriefLevel.Click += BtnSaveGriefLevel_Click;
            sliderGriefLevel.ValueChanged += SliderGriefLevel_ValueChanged;
        }

        private void LoadProfile()
        {
            try
            {
                var user = Dbb.dbb.Users.FirstOrDefault(u => u.UserID == Session.CurrentUserID);

                if (user != null)
                {
                    txtUsername.Text = user.Username;
                    txtEmail.Text = user.Email;
                    txtCreatedAt.Text = user.CreatedAt?.ToString("dd.MM.yyyy HH:mm") ?? "Дата неизвестна";
                    var role = Dbb.dbb.Roles.FirstOrDefault(r => r.RoleID == user.RoleID);
                    if (role != null)
                    {
                        txtRole.Text = role.RoleName;
                    }
                    else
                    {
                        txtRole.Text = "Неизвестно";
                    }
                }
                else
                {
                    txtUsername.Text = "Ошибка загрузки";
                    txtEmail.Text = "Ошибка загрузки";
                    txtRole.Text = "Ошибка загрузки";
                    txtCreatedAt.Text = "Ошибка загрузки";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки профиля: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadGriefLevel()
        {
            try
            {
            
                int savedLevel = Session.GriefLevel;
                sliderGriefLevel.Value = savedLevel;
                UpdateGriefDescription(savedLevel);
                txtGriefLevel.Text = savedLevel.ToString();
            }
            catch (Exception)
            {
                sliderGriefLevel.Value = 5;
                txtGriefLevel.Text = "5";
            }
        }

        private void SliderGriefLevel_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int level = (int)e.NewValue;
            txtGriefLevel.Text = level.ToString();
            UpdateGriefDescription(level);
        }

        private void UpdateGriefDescription(int level)
        {
            if (level <= 2)
            {
                txtGriefDescription.Text = "Легкая печаль";
                txtGriefDescription.Foreground = System.Windows.Media.Brushes.LightGray;
            }
            else if (level <= 4)
            {
                txtGriefDescription.Text = "Умеренный траур";
                txtGriefDescription.Foreground = System.Windows.Media.Brushes.Gray;
            }
            else if (level <= 6)
            {
                txtGriefDescription.Text = "Глубокая скорбь";
                txtGriefDescription.Foreground = System.Windows.Media.Brushes.Purple;
            }
            else if (level <= 8)
            {
                txtGriefDescription.Text = "Неутешное горе";
                txtGriefDescription.Foreground = System.Windows.Media.Brushes.DarkRed;
            }
            else
            {
                txtGriefDescription.Text = "Предельный траур";
                txtGriefDescription.Foreground = System.Windows.Media.Brushes.Red;
            }
        }

        private void BtnSaveGriefLevel_Click(object sender, RoutedEventArgs e)
        {
            int level = (int)sliderGriefLevel.Value;
            Session.GriefLevel = level;

            txtMessage.Text = "Уровень траура сохранен: " + level + " / 10";
            txtMessage.Foreground = System.Windows.Media.Brushes.Green;

            System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Tick += (s, ev) => {
                txtMessage.Text = "";
                timer.Stop();
            };
            timer.Start();
        }

        private void BtnSaveProfile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var user = Dbb.dbb.Users.FirstOrDefault(u => u.UserID == Session.CurrentUserID);

                if (user != null)
                {
                    user.Email = txtEmail.Text;
                    Dbb.dbb.SaveChanges();

                    txtMessage.Text = "Профиль сохранен!";
                    txtMessage.Foreground = System.Windows.Media.Brushes.Green;

                    System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
                    timer.Interval = TimeSpan.FromSeconds(3);
                    timer.Tick += (s, ev) => {
                        txtMessage.Text = "";
                        timer.Stop();
                    };
                    timer.Start();
                }
            }
            catch (Exception ex)
            {
                txtMessage.Text = "Ошибка: " + ex.Message;
                txtMessage.Foreground = System.Windows.Media.Brushes.Red;
            }
        }

        private void BtnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            string oldPassword = txtOldPassword.Password;
            string newPassword = txtNewPassword.Password;
            string confirmPassword = txtConfirmPassword.Password;

            if (oldPassword == "" || newPassword == "" || confirmPassword == "")
            {
                MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Новый пароль и подтверждение не совпадают!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (newPassword.Length < 3)
            {
                MessageBox.Show("Пароль должен содержать минимум 3 символа!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var user = Dbb.dbb.Users.FirstOrDefault(u => u.UserID == Session.CurrentUserID);

                if (user != null)
                {
                    if (user.PasswordHash == oldPassword)
                    {
                        user.PasswordHash = newPassword;
                        Dbb.dbb.SaveChanges();

                        MessageBox.Show("Пароль успешно изменен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                        txtOldPassword.Password = "";
                        txtNewPassword.Password = "";
                        txtConfirmPassword.Password = "";
                    }
                    else
                    {
                        MessageBox.Show("Неверный текущий пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Пользователь не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка изменения пароля: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

