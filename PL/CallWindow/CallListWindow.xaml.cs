using BlApi;
using BO;
using PL.Call; // אם SingleCallWindow נמצא שם
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PL.Call
{
    public partial class CallListWindow : Window
    {
        private readonly IBl bl = Factory.Get();
        private ObservableCollection<CallInList> callList;

        public CallListWindow()
        {
            InitializeComponent();
            bl.Call.AddObserver(UpdateList); // תבנית observer
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                callList = new ObservableCollection<CallInList>(bl.Call.GetCallList());
                CallDataGrid.ItemsSource = callList;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load calls: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateList()
        {
            Dispatcher.Invoke(() =>
            {
                callList.Clear();
                foreach (var item in bl.Call.GetCallList())
                    callList.Add(item);
            });
        }

        private void CallDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (CallDataGrid.SelectedItem is CallInList selected)
            {
               // new SingleCallWindow(selected.CallId).ShowDialog();
            }
        }
    }
}
