//// PL/Volunteer/VolunteerClosedCallsWindow.xaml.cs
//using System;
//using System.Collections.ObjectModel;
//using System.ComponentModel;
//using System.Linq;
//using System.Windows;
//using System.Windows.Data;
//using BO;
//using BlApi;

//namespace PL.Volunteer
//{
//    // Change the class name to match the XAML: VolunteerClosedCallsWindow
//    public partial class VolunteerClosedCallsWindow : Window, INotifyPropertyChanged
//    {
//        private readonly IBl _bl = Factory.Get();

//        public event PropertyChangedEventHandler? PropertyChanged;

//        public ObservableCollection<ClosedCallInList> ClosedCalls { get; set; } = new();
//        public ICollectionView CallsView { get; set; }

//        private string? _filterText;
//        public string? FilterText
//        {
//            get => _filterText;
//            set
//            {
//                _filterText = value;
//                CallsView.Refresh();
//                OnPropertyChanged(nameof(FilterText));
//            }
//        }

//        public VolunteerClosedCallsWindow(int volunteerId)
//        {
//            InitializeComponent();
//            var bl = Factory.Get();
//            // Load closed calls for the volunteer
//            ClosedCalls = new ObservableCollection<ClosedCallInList>(
//                bl.Volunteer.GetClosedCallsByVolunteer(volunteerId)
//            );
//            DataContext = this;

//            var closedCalls = _bl.Volunteer.GetClosedCallsByVolunteer(volunteerId);
//            foreach (var call in closedCalls)
//                ClosedCalls.Add(call);

//            CallsView = CollectionViewSource.GetDefaultView(ClosedCalls);
//            CallsView.Filter = FilterCalls;
//        }

//        private bool FilterCalls(object obj)
//        {
//            if (string.IsNullOrWhiteSpace(FilterText)) return true;
//            if (obj is not ClosedCallInList call) return false;

//            return call.Id.ToString().Contains(FilterText, StringComparison.OrdinalIgnoreCase)
//                || call.CallType.ToString().Contains(FilterText, StringComparison.OrdinalIgnoreCase)
//                || call.Address.Contains(FilterText, StringComparison.OrdinalIgnoreCase)
//                || call.EndStatus.ToString().Contains(FilterText, StringComparison.OrdinalIgnoreCase);
//        }

//        protected void OnPropertyChanged(string propertyName) =>
//            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

//        private void Close_Click(object sender, RoutedEventArgs e)
//        {
//            Close();
//        }
//    }
//}
// PL/Volunteer/VolunteerClosedCallsWindow.xaml.cs
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using BO;
using BlApi;

namespace PL.Volunteer
{
    public partial class VolunteerClosedCallsWindow : Window, INotifyPropertyChanged
    {
        private readonly IBl _bl = Factory.Get();

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<ClosedCallInList> ClosedCalls { get; set; }
        public ICollectionView CallsView { get; set; }

        private string? _filterText;
        public string? FilterText
        {
            get => _filterText;
            set
            {
                _filterText = value;
                CallsView.Refresh();
                OnPropertyChanged(nameof(FilterText));
            }
        }

        public VolunteerClosedCallsWindow(int volunteerId)
        {
            InitializeComponent();
            DataContext = this; // קודם כל לקבוע את ה-DataContext

            ClosedCalls = new ObservableCollection<ClosedCallInList>(
                _bl.Volunteer.GetClosedCallsByVolunteer(volunteerId)
            );

            CallsView = CollectionViewSource.GetDefaultView(ClosedCalls);
            CallsView.Filter = FilterCalls;
        }


        private bool FilterCalls(object obj)
        {
            if (string.IsNullOrWhiteSpace(FilterText)) return true;
            if (obj is not ClosedCallInList call) return false;

            return call.Id.ToString().Contains(FilterText, StringComparison.OrdinalIgnoreCase)
                || call.CallType.ToString().Contains(FilterText, StringComparison.OrdinalIgnoreCase)
                || call.Address.Contains(FilterText, StringComparison.OrdinalIgnoreCase)
                || call.EndStatus.ToString().Contains(FilterText, StringComparison.OrdinalIgnoreCase);
        }

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
