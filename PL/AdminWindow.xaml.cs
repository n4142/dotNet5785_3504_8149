using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using BlApi;
using BO;
using PL.Call;
using PL.Volunteer;

namespace PL
{
    public partial class AdminWindow : Window, INotifyPropertyChanged
    {
        static readonly IBl s_bl = Factory.Get();

        public AdminWindow()
        {
            InitializeComponent();

            Interval = 1;
            IsSimulatorRunning = false;

            Loaded += AdminWindow_Loaded;
            Closed += AdminWindow_Closed;
            DataContext = this;
        }

        // תכונות סטטיסטיקה עם OnPropertyChanged
        private int totalOpenCalls;
        public int TotalOpenCalls
        {
            get => totalOpenCalls;
            set { totalOpenCalls = value; OnPropertyChanged(nameof(TotalOpenCalls)); }
        }

        private int totalClosedCalls;
        public int TotalClosedCalls
        {
            get => totalClosedCalls;
            set { totalClosedCalls = value; OnPropertyChanged(nameof(TotalClosedCalls)); }
        }

        private int totalExpiredCalls;
        public int TotalExpiredCalls
        {
            get => totalExpiredCalls;
            set { totalExpiredCalls = value; OnPropertyChanged(nameof(TotalExpiredCalls)); }
        }

        private int totalOpenAtRiskCalls;
        public int TotalOpenAtRiskCalls
        {
            get => totalOpenAtRiskCalls;
            set { totalOpenAtRiskCalls = value; OnPropertyChanged(nameof(TotalOpenAtRiskCalls)); }
        }

        private int totalInProgressCalls;
        public int TotalInProgressCalls
        {
            get => totalInProgressCalls;
            set { totalInProgressCalls = value; OnPropertyChanged(nameof(TotalInProgressCalls)); }
        }

        private int totalInProgressAtRiskCalls;
        public int TotalInProgressAtRiskCalls
        {
            get => totalInProgressAtRiskCalls;
            set { totalInProgressAtRiskCalls = value; OnPropertyChanged(nameof(TotalInProgressAtRiskCalls)); }
        }

        public DateTime CurrentTime
        {
            get => (DateTime)GetValue(CurrentTimeProperty);
            set => SetValue(CurrentTimeProperty, value);
        }
        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register(nameof(CurrentTime), typeof(DateTime), typeof(AdminWindow));

        public TimeSpan MaxYearRange
        {
            get => (TimeSpan)GetValue(MaxYearRangeProperty);
            set => SetValue(MaxYearRangeProperty, value);
        }
        public static readonly DependencyProperty MaxYearRangeProperty =
            DependencyProperty.Register(nameof(MaxYearRange), typeof(TimeSpan), typeof(AdminWindow));

        public int Interval
        {
            get => (int)GetValue(IntervalProperty);
            set => SetValue(IntervalProperty, value);
        }
        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register(nameof(Interval), typeof(int), typeof(AdminWindow), new PropertyMetadata(1));

        public bool IsSimulatorRunning
        {
            get => (bool)GetValue(IsSimulatorRunningProperty);
            set => SetValue(IsSimulatorRunningProperty, value);
        }
        public static readonly DependencyProperty IsSimulatorRunningProperty =
            DependencyProperty.Register(nameof(IsSimulatorRunning), typeof(bool), typeof(AdminWindow), new PropertyMetadata(false));

        private void AdminWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CurrentTime = s_bl.Admin.GetClock();
            MaxYearRange = s_bl.Admin.GetMaxRange();

            s_bl.Admin.AddClockObserver(ClockObserver);
            s_bl.Admin.AddConfigObserver(ConfigObserver);

            UpdateCallStatistics();
        }

        private void AdminWindow_Closed(object sender, EventArgs e)
        {
            s_bl.Admin.RemoveClockObserver(ClockObserver);
            s_bl.Admin.RemoveConfigObserver(ConfigObserver);

            if (IsSimulatorRunning)
            {
                s_bl.Admin.StopSimulator();
                IsSimulatorRunning = false;
            }
        }

        private void ClockObserver()
        {
            Dispatcher.Invoke(() =>
            {
                CurrentTime = s_bl.Admin.GetClock();
                UpdateCallStatistics(); // מעדכן את הספירות
            });
        }

        private void ConfigObserver()
        {
            Dispatcher.Invoke(() => MaxYearRange = s_bl.Admin.GetMaxRange());
            UpdateCallStatistics();
        }

        private void UpdateCallStatistics()
        {
            TotalOpenCalls = s_bl.Call.GetCallList(FilteredBy.Status, CallStatus.Open).Count();
            TotalClosedCalls = s_bl.Call.GetCallList(FilteredBy.Status, CallStatus.Closed).Count();
            TotalExpiredCalls = s_bl.Call.GetCallList(FilteredBy.Status, CallStatus.Expired).Count();
            TotalOpenAtRiskCalls = s_bl.Call.GetCallList(FilteredBy.Status, CallStatus.OpenAtRisk).Count();
            TotalInProgressCalls = s_bl.Call.GetCallList(FilteredBy.Status, CallStatus.InProgress).Count();
            TotalInProgressAtRiskCalls = s_bl.Call.GetCallList(FilteredBy.Status, CallStatus.InProgressAtRisk).Count();
        }

        private void btnAddOneMinute_Click(object sender, RoutedEventArgs e) => s_bl.Admin.ForwardClock(TimeUnit.Minute);
        private void btnAddOneHour_Click(object sender, RoutedEventArgs e) => s_bl.Admin.ForwardClock(TimeUnit.Hour);
        private void btnAddOneDay_Click(object sender, RoutedEventArgs e) => s_bl.Admin.ForwardClock(TimeUnit.Day);
        private void btnAddOneMonth_Click(object sender, RoutedEventArgs e) => s_bl.Admin.ForwardClock(TimeUnit.Month);
        private void btnAddOneYear_Click(object sender, RoutedEventArgs e) => s_bl.Admin.ForwardClock(TimeUnit.Year);

        private void btnUpdateMaxYearRange_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Admin.SetMaxRange(MaxYearRange);
                MessageBox.Show("Max Year Range updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating Max Year Range: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnOpenListScreen_Click(object sender, RoutedEventArgs e)
        {
            new VolunteerListWindow().Show();
        }

        private void btnManageCalls_Click(object sender, RoutedEventArgs e)
        {
            new CallListWindow().Show();
        }

        private async void btnInitializeDB_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to initialize the database?", "Confirm Initialize", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                foreach (Window w in Application.Current.Windows)
                    if (w != this) w.Close();

                Mouse.OverrideCursor = Cursors.Wait;
                try
                {
                    await Task.Delay(100);
                    await Task.Run(() => s_bl.Admin.InitializeDB());
                    MessageBox.Show("Database initialized successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error initializing database: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    Mouse.OverrideCursor = null;
                }
            }
        }

        private void btnResetDB_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to reset the database? This will delete all data!", "Confirm Reset", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                try
                {
                    s_bl.Admin.ResetDB();
                    MessageBox.Show("Database reset successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error resetting database: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    Mouse.OverrideCursor = null;
                }
            }
        }

        private async void btnToggleSimulator_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!IsSimulatorRunning)
                {
                    s_bl.Admin.StartSimulator(Interval);
                    IsSimulatorRunning = true;
                }
                else
                {
                    await Task.Run(() => s_bl.Admin.StopSimulator());
                    IsSimulatorRunning = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Simulator error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

