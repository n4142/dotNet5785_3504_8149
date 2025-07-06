using BlApi;
using BO;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace PL.Call
{
    public partial class AddCallWindow : Window, INotifyPropertyChanged
    {
        private readonly IBl bl = Factory.Get();

        public event PropertyChangedEventHandler PropertyChanged;
        public BO.Call Call { get; set; } = new();
        public Array CallTypeValues => Enum.GetValues(typeof(CallType));

        public DateTime? MaxDate { get; set; } = DateTime.Now;
        public string MaxTime { get; set; } = "12:00";

        public AddCallWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // בדיקת תקינות שדות
                if (Call.CallType == null ||
                    string.IsNullOrWhiteSpace(Call.Description) ||
                    string.IsNullOrWhiteSpace(Call.FullAddress) ||
                    MaxDate == null ||
                    !TimeSpan.TryParse(MaxTime, out var time))
                {
                    ShowError("All fields are required, including valid time format.");
                    return;
                }

                Call.MaxCompletionTime = MaxDate.Value.Date + time;

                bl.Call.AddCall(Call);
                MessageBox.Show("Call added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                ShowError("Failed to add call: " + ex.Message);
            }
        }

        private void ShowError(string message)
        {
            ErrorTextBlock.Text = message;
            ErrorTextBlock.Visibility = Visibility.Visible;
        }

        private void Notify(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
