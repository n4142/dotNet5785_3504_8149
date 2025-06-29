using BlApi;
using BO;
using System;
using System.Windows;

namespace PL
{
    public partial class LoginWindow : Window
    {
        private readonly IBl bl = Factory.Get();

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(IdTextBox.Text, out int id))
                    throw new FormatException("ID must be a number");

                string password = PasswordBox.Password;

                Position position = bl.Volunteer.Login(IdTextBox.Text, password);

                if (position == Position.Manager)
                {
                    MessageBoxResult result = MessageBox.Show(
                        "Login successful as Manager.\nDo you want to enter the Admin Panel?",
                        "Login Success",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question
                    );

                    if (result == MessageBoxResult.Yes)
                    {
                        new MainWindow().Show(); // מסך ניהול ראשי
                    }
                    else
                    {
                        new VolunteerWindow(id).Show(); // מסך מתנדב רגיל
                    }
                }
                else
                {
                    new VolunteerWindow(id).Show(); // מסך מתנדב רגיל
                }

                this.Close();
            }
            catch (BO.BlDoesNotExistException)
            {
                ShowError("User does not exist.");
            }
            catch (BO.BlArgumentException)
            {
                ShowError("Incorrect password.");
            }
            catch (Exception ex)
            {
                ShowError("Login failed: " + ex.Message);
            }
        }

        private void ShowError(string message)
        {
            ErrorTextBlock.Text = message;
            ErrorTextBlock.Visibility = Visibility.Visible;
        }
    }
}
