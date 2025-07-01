//// VolunteerWindow.xaml.cs
//using System;
//using System.Globalization;
//using System.Windows;
//using System.Windows.Data;
//using BO;

//namespace PL.Volunteer
//{
//    public partial class VolunteerWindow : Window
//    {
//        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

//        public string ButtonText
//        {
//            get => (string)GetValue(ButtonTextProperty);
//            set => SetValue(ButtonTextProperty, value);
//        }
//        public static readonly DependencyProperty ButtonTextProperty =
//            DependencyProperty.Register(nameof(ButtonText), typeof(string), typeof(VolunteerWindow));

//        public BO.Volunteer CurrentVolunteer
//        {
//            get => (BO.Volunteer)GetValue(CurrentVolunteerProperty);
//            set => SetValue(CurrentVolunteerProperty, value);
//        }
//        public static readonly DependencyProperty CurrentVolunteerProperty =
//            DependencyProperty.Register(nameof(CurrentVolunteer), typeof(BO.Volunteer), typeof(VolunteerWindow));

//        private readonly Action _observer;

//        public VolunteerWindow(int id = 0)
//        {
//            ButtonText = id == 0 ? "Add" : "Update";
//            InitializeComponent();

//            if (id == 0)
//                CurrentVolunteer = new BO.Volunteer();
//            else
//                try { CurrentVolunteer = s_bl.Volunteer.GetVolunteer(id); }
//                catch (Exception ex)
//                {
//                    MessageBox.Show($"Error loading volunteer: {ex.Message}");
//                    Close();
//                }

//            _observer = () =>
//            {
//                int vid = CurrentVolunteer.Id;
//                CurrentVolunteer = null;
//                CurrentVolunteer = s_bl.Volunteer.GetVolunteer(vid);
//            };
//        }

//        private void Window_Loaded(object sender, RoutedEventArgs e)
//        {
//            if (CurrentVolunteer.Id != 0)
//                s_bl.Volunteer.AddObserver(CurrentVolunteer.Id, _observer);
//        }

//        private void Window_Closed(object sender, EventArgs e)
//        {
//            if (CurrentVolunteer.Id != 0)
//                s_bl.Volunteer.RemoveObserver(CurrentVolunteer.Id, _observer);
//        }

//        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
//        {
//            try
//            {
//                if (ButtonText == "Add")
//                    s_bl.Volunteer.AddVolunteer(CurrentVolunteer);
//                else
//                    s_bl.Volunteer.UpdateVolunteerDetails(CurrentVolunteer.Id, CurrentVolunteer);

//                MessageBox.Show($"{ButtonText} successful.");
//                Close();
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"{ButtonText} failed: {ex.Message}");
//            }
//        }
//    }

//    public class UpdateModeToReadOnlyConverter : IValueConverter
//    {
//        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
//        {
//            return value?.ToString() == "Update";
//        }

//        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
//        {
//            throw new NotImplementedException();
//        }
//    }

//    public class UpdateModeToVisibilityConverter : IValueConverter
//    {
//        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
//        {
//            return value?.ToString() == "Update" ? Visibility.Visible : Visibility.Collapsed;
//        }

//        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
// VolunteerWindow.xaml.cs
using BO;
using BlApi;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace PL.Volunteer
{
    public partial class VolunteerWindow : Window
    {
        static readonly IBl s_bl = Factory.Get();

        /// <summary>
        /// get and set functions
        /// </summary>
        public BO.Volunteer CurrentVolunteer
        {
            get { return (BO.Volunteer)GetValue(CurrentVolunteerProperty); }
            set { SetValue(CurrentVolunteerProperty, value); }
        }

        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register(nameof(CurrentVolunteer), typeof(BO.Volunteer), typeof(VolunteerWindow), new PropertyMetadata(null));

        public ObservableCollection<Position> Roles { get; } = new ObservableCollection<Position>((Position[])Enum.GetValues(typeof(Position)));
        public ObservableCollection<DistanceType> DistanceTypes { get; } = new ObservableCollection<DistanceType>((DistanceType[])Enum.GetValues(typeof(DistanceType)));

        public BO.CallInProgress? CurrentCall
        {
            get => (BO.CallInProgress?)GetValue(CurrentCallProperty);
            set => SetValue(CurrentCallProperty, value);
        }

        private bool isEditMode = false;

        private bool managerMode => CurrentVolunteer.MyPosition == Position.Manager;



        public static readonly DependencyProperty CurrentCallProperty =
            DependencyProperty.Register(nameof(CurrentCall), typeof(BO.CallInProgress), typeof(VolunteerWindow));

        public string ButtonText
        {
            get { return (string)GetValue(ButtonTextProperty); }
            set { SetValue(ButtonTextProperty, value); }
        }

        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register(nameof(ButtonText), typeof(string), typeof(VolunteerWindow), new PropertyMetadata("Add"));

        public string ButtonCallText
        {
            get { return (string)GetValue(ButtonCallTextProperty); }
            set { SetValue(ButtonCallTextProperty, value); }
        }

        public static readonly DependencyProperty ButtonCallTextProperty =
            DependencyProperty.Register(nameof(ButtonCallText), typeof(string), typeof(VolunteerWindow), new PropertyMetadata("See Calls In Progress"));

        // Add password property for simple binding
        public string VolunteerPassword
        {
            get { return (string)GetValue(VolunteerPasswordProperty); }
            set { SetValue(VolunteerPasswordProperty, value); }
        }

        public static readonly DependencyProperty VolunteerPasswordProperty =
            DependencyProperty.Register(nameof(VolunteerPassword), typeof(string), typeof(VolunteerWindow),
                new PropertyMetadata(string.Empty, OnPasswordChanged));

        private static void OnPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VolunteerWindow window && window.CurrentVolunteer != null)
            {
                window.CurrentVolunteer.Password = e.NewValue?.ToString() ?? string.Empty;
            }
        }

        /// <summary>
        /// constructor for adding a new volunteer
        /// </summary>
        public VolunteerWindow()
        {
            InitializeComponent();
            CurrentVolunteer = new BO.Volunteer();
            CurrentCall = CurrentVolunteer.CallInProgress;
            ButtonText = "Add";
            DataContext = this;
            ButtonCallText = "Assign A Call";
        }

        /// <summary>
        /// function that receives a selected volunteer from the list for editing
        /// </summary>
        /// <param name="selectedVolunteer"></param>
        public VolunteerWindow(VolunteerInList selectedVolunteer)
        {
            InitializeComponent();
            DataContext = this;

            try
            {
                CurrentVolunteer = s_bl.Volunteer.GetVolunteer(selectedVolunteer.Id);
                if (CurrentVolunteer == null)
                    throw new Exception("Volunteer not found");

                // Don't show the actual password for security - leave it empty
                VolunteerPassword = string.Empty;
                ButtonText = "Update";
                ButtonCallText = "See Calls In Progress";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load volunteer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                CurrentVolunteer = new BO.Volunteer();
                ButtonText = "Add";
                ButtonCallText = "Assign A Call";
            }
        }

        /// <summary>
        /// Constructor for loading an existing volunteer by ID number for editing
        /// </summary>
        /// <param name="idNumber"></param>
        public VolunteerWindow(int idNumber)
        {
            InitializeComponent();
            DataContext = this;

            try
            {
                CurrentVolunteer = s_bl.Volunteer.GetVolunteer(idNumber);
                if (CurrentVolunteer != null)
                {
                    // Don't show the actual password for security - leave it empty
                    VolunteerPassword = string.Empty;
                }
                ButtonText = "Update";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load volunteer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                CurrentVolunteer = new BO.Volunteer();
                ButtonText = "Add";
                ButtonCallText = "Assign A Call";
            }
        }

        /// <summary>
        /// Button click event handler for opening the ChooseCallWindow,
        /// allowing the current volunteer to select an available call.
        /// </summary>
        /// <param name="sender">The source of the event (typically the button).</param>
        /// <param name="e">Event data associated with the click.</param>
        private void ChooseCall_Click(object sender, RoutedEventArgs e)
        {
            var chooseWindow = new Call.ChooseCallWindow(CurrentVolunteer.Id);
            chooseWindow.ShowDialog();
        }


        /// <summary>
        /// Button click event handler for adding or updating a volunteer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CurrentVolunteer == null)
                {
                    MessageBox.Show("No volunteer data available.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (ButtonText == "Add")
                {
                    // תנאים להוספת מתנדב חדש
                    if (string.IsNullOrWhiteSpace(VolunteerPassword))
                    {
                        MessageBox.Show("Password is required for new volunteers.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (VolunteerPassword.Length < 8)
                    {
                        MessageBox.Show("Password must be at least 8 characters long.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    CurrentVolunteer.Password = VolunteerPassword;
                    s_bl.Volunteer.AddVolunteer(CurrentVolunteer);
                    MessageBox.Show("Volunteer added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                }
                else if (ButtonText == "Update")
                {
                    if (!isEditMode)
                    {
                        // הפעלה של מצב עריכה
                        isEditMode = true;
                        SetEditMode(true);
                        return;
                    }

                    // שלב ביצוע העדכון בפועל
                    if (!string.IsNullOrWhiteSpace(VolunteerPassword) && VolunteerPassword.Length < 8)
                    {
                        MessageBox.Show("Password must be at least 8 characters long.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (!string.IsNullOrWhiteSpace(VolunteerPassword))
                        CurrentVolunteer.Password = VolunteerPassword;

                    s_bl.Volunteer.UpdateVolunteerDetails(CurrentVolunteer.Id, CurrentVolunteer);
                    MessageBox.Show("Volunteer updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show($"Operation failed: {ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Exception inner = ex;
                while (inner.InnerException != null)
                    inner = inner.InnerException;

                MessageBox.Show(inner.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);

            }
        }
        private void SetEditMode(bool enableEdit)
        {
            txtId.IsReadOnly = !enableEdit;
            txtFullName.IsReadOnly = !enableEdit;
            txtPhoneNumber.IsReadOnly = !enableEdit;
            txtEmail.IsReadOnly = !enableEdit;
            txtAddress.IsReadOnly = !enableEdit;
            cmbRole.IsEnabled = enableEdit;
            ckbxActive.IsEnabled = enableEdit;
            txtMaxDistance.IsReadOnly = !enableEdit;
            cmbDistanceType.IsEnabled = enableEdit;
        }


        /// <summary>
        /// function that closes the window when the close button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void VolunteerPage_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Optional: Add logic if needed before window closes
        }

        private void ButtonSeeCallinProgress_Click(object sender, RoutedEventArgs e)
        {
            // Implementation for seeing calls in progress
        }

        /// <summary>
        /// Cancels the current call assigned to the volunteer.
        /// </summary>
        private void CancelCall_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CurrentCall == null)
                {
                    MessageBox.Show("No current call to cancel.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                s_bl.Call.CancelCallTreatment(CurrentCall.CallId, CurrentVolunteer.Id);
                MessageBox.Show("Call canceled.", "Canceled", MessageBoxButton.OK, MessageBoxImage.Information);

                CurrentCall = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to cancel call: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Report the end of reading treatment
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EndOfTreatment_Click(object sender, RoutedEventArgs e)
        { 
            //todo
        }

    }
}