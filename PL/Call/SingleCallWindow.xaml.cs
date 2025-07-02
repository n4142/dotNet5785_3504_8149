using BlApi;
using BO;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace PL.Call
{
    public partial class SingleCallWindow : Window
    {
        private readonly IBl bl = Factory.Get();
        public BO.Call CurrentCall { get; set; }
        public ObservableCollection<CallAssignInList> Assignments { get; set; }
        public Array StatusOptions => Enum.GetValues(typeof(CallStatus));
        public bool IsEditing { get; set; } = true;
        public bool HasAssignments => Assignments != null && Assignments.Count > 0;

        public SingleCallWindow(int callId)
        {
            InitializeComponent();

            try
            {
                CurrentCall = bl.Call.GetCallDetails(callId);
                Assignments = new ObservableCollection<CallAssignInList>(
                    CurrentCall.CallAssignments
                );

                DataContext = this;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load call: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }
        private void Update_Click(object sender, RoutedEventArgs e)
        {
            if (IsEditing)
            {
                IsEditing = false;
                DataContext = null;
                DataContext = this;
                return; // מפעיל מצב עריכה בלבד
            }

            try
            {
                bl.Call.UpdateCallDetails(CurrentCall);
                MessageBox.Show("Call updated successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Update failed: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
