using BlApi;
using BO;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace PL.Call
{
    public partial class ChooseCallWindow : Window, INotifyPropertyChanged
    {
        private readonly IBl bl = Factory.Get();
        private readonly int volunteerId;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<OpenCallInList> Calls { get; set; } = new();

        public Array CallTypeValues => Enum.GetValues(typeof(CallType));

        private CallType? selectedCallType;
        public CallType? SelectedCallType
        {
            get => selectedCallType;
            set
            {
                if (selectedCallType != value)
                {
                    selectedCallType = value;
                    OnPropertyChanged(nameof(SelectedCallType));
                    LoadCalls(); // רענון אוטומטי עם שינוי סינון
                }
            }
        }

        public OpenCallInList? SelectedCall { get; set; }

        public ChooseCallWindow(int volunteerId)
        {
            InitializeComponent();
            this.volunteerId = volunteerId;
            DataContext = this;
            LoadCalls();
        }
                private void LoadCalls()
        {
            try
            {
                Calls.Clear();
                var list = bl.Call.GetOpenCallsForVolunteerSelection(volunteerId, SelectedCallType);
                foreach (var c in list)
                    Calls.Add(c);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading calls: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TakeCall_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCall == null)
            {
                MessageBox.Show("Please select a call first.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                bl.Call.AssignCallToVolunteer(volunteerId,SelectedCall.Id);
                MessageBox.Show("Call assigned successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to assign call: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
