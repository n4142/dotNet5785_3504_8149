using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BO;

namespace PL.Volunteer
{
    public partial class VolunteerListWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public CallType SelectedCallType { get; set; } = CallType.All;

        public IEnumerable<VolunteerInList> VolunteerList
        {
            get { return (IEnumerable<VolunteerInList>)GetValue(VolunteerListProperty); }
            set { SetValue(VolunteerListProperty, value); }
        }

        public static readonly DependencyProperty VolunteerListProperty =
            DependencyProperty.Register(nameof(VolunteerList), typeof(IEnumerable<VolunteerInList>), typeof(VolunteerListWindow), new PropertyMetadata(null));

        public VolunteerInList? SelectedVolunteer { get; set; }

        public VolunteerListWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void QueryVolunteerList()
        {
            var allVolunteers = s_bl.Volunteer.GetVolunteerList();

            if (SelectedCallType != CallType.All)
            {
                allVolunteers = allVolunteers
                    .Where(v => v.CallInTreatmenType == SelectedCallType);
            }

            VolunteerList = allVolunteers.ToList();

            if (!VolunteerList.Any())
                MessageBox.Show("No volunteers found.");
        }

        private void VolunteerListObserver() => QueryVolunteerList();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Volunteer.AddObserver(VolunteerListObserver);
            QueryVolunteerList();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Volunteer.RemoveObserver(VolunteerListObserver);
        }

        private void CallTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CallTypeComboBox.SelectedItem is CallType selected)
            {
                SelectedCallType = selected;
                QueryVolunteerList();
            }
        }

        private void AddVolunteer_Click(object sender, RoutedEventArgs e)
        {
            new VolunteerWindow().Show();
        }

        private void VolunteerList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedVolunteer != null)
            {
                new VolunteerWindow(SelectedVolunteer.Id).Show();
            }
        }
    }
}