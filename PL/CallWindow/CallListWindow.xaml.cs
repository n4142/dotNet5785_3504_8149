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
        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string? selected = (e.AddedItems[0] as ComboBoxItem)?.Content.ToString();
            switch (selected)
            {
                case "Open Time":
                    ApplySort(c => c.OpenTime);
                    break;
                case "Time Remaining":
                    ApplySort(c => c.TimeRemaining);
                    break;
                case "Completion Time":
                    ApplySort(c => c.TotalCompletionTime);
                    break;
            }
        }

        private void StatusFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StatusFilterComboBox.SelectedItem is CallStatus selectedStatus)
            {
                FilterAndSortList(selectedStatus);
            }
        }

        private void FilterAndSortList(CallStatus statusFilter)
        {
            var list = bl.Call.GetCallList();

            if (statusFilter != CallStatus.All)
                list = list.Where(c => c.Status == statusFilter);

            callList.Clear();
            foreach (var item in list)
                callList.Add(item);
        }
        private void SortByOpenTime_Click(object sender, RoutedEventArgs e) =>
            ApplySort(c => c.OpenTime);

        private void SortByRemainingTime_Click(object sender, RoutedEventArgs e) =>
            ApplySort(c => c.TimeRemaining);

        private void SortByCompletionTime_Click(object sender, RoutedEventArgs e) =>
            ApplySort(c => c.TotalCompletionTime);

        private void ApplySort<TKey>(Func<CallInList, TKey> keySelector)
        {
            var sorted = callList.OrderBy(keySelector).ToList();
            callList.Clear();
            foreach (var item in sorted)
                callList.Add(item);
        }
        private void AddCall_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddCallWindow(); // חלון הוספה
            window.ShowDialog();
        }


        private void CallDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (CallDataGrid.SelectedItem is CallInList selected)
            {
               // new SingleCallWindow(selected.CallId).ShowDialog();
            }
        }
        private void DeleteCall_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is CallInList call)
            {
                try
                {
                    bl.Call.DeleteCall(call.CallId);
                    callList.Remove(call);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Cannot delete call: " + ex.Message, "Delete Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

    }
}
