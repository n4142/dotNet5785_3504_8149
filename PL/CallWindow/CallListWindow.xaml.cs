
using BlApi;
using BO;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PL.Call
{
    public partial class CallListWindow : Window, INotifyPropertyChanged
    {
        private readonly IBl bl = Factory.Get();

        private ObservableCollection<CallInList> callList = new();
        public ObservableCollection<CallInList> CallList
        {
            get => callList;
            set
            {
                callList = value;
                OnPropertyChanged(nameof(CallList));
            }
        }

        public CallListWindow()
        {
            InitializeComponent();
            DataContext = this;

            Loaded += CallListWindow_Loaded;
            Closed += CallListWindow_Closed;

            bl.Call.AddObserver(UpdateList);
            LoadData();
        }

        private void CallListWindow_Loaded(object sender, RoutedEventArgs e)
        {
            bl.Admin.AddClockObserver(ClockObserver);
            bl.Admin.AddConfigObserver(ConfigObserver);
        }

        private void CallListWindow_Closed(object sender, EventArgs e)
        {
            bl.Admin.RemoveClockObserver(ClockObserver);
            bl.Admin.RemoveConfigObserver(ConfigObserver);
        }

        private void ClockObserver()
        {
            Dispatcher.Invoke(UpdateList);
        }

        private void ConfigObserver()
        {
            Dispatcher.Invoke(UpdateList);
        }

        private void LoadData()
        {
            try
            {
                CallList = new ObservableCollection<CallInList>(bl.Call.GetCallList());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load calls: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateList()
        {
            var updated = bl.Call.GetCallList().ToList();
            CallList.Clear();
            foreach (var item in updated)
                CallList.Add(item);
        }

        private void ApplySort<TKey>(Func<CallInList, TKey> keySelector)
        {
            var sorted = CallList.OrderBy(keySelector).ToList();
            CallList.Clear();
            foreach (var item in sorted)
                CallList.Add(item);
        }

        private void FilterAndSortList(CallStatus statusFilter)
        {
            var list = bl.Call.GetCallList();
            if (statusFilter != CallStatus.All)
                list = list.Where(c => c.Status == statusFilter);

            CallList.Clear();
            foreach (var item in list)
                CallList.Add(item);
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

        private void SortByOpenTime_Click(object sender, RoutedEventArgs e) =>
            ApplySort(c => c.OpenTime);

        private void SortByRemainingTime_Click(object sender, RoutedEventArgs e) =>
            ApplySort(c => c.TimeRemaining);

        private void SortByCompletionTime_Click(object sender, RoutedEventArgs e) =>
            ApplySort(c => c.TotalCompletionTime);

        private void AddCall_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddCallWindow();
            window.ShowDialog();
        }

        private void CallDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (CallDataGrid.SelectedItem is CallInList selected)
            {
                new SingleCallWindow(selected.CallId).Show();
            }
        }

        private void DeleteCall_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is CallInList call)
            {
                try
                {
                    bl.Call.DeleteCall(call.CallId);
                    CallList.Remove(call);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Cannot delete call: " + ex.Message, "Delete Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

