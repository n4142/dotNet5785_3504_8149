using BO;
using BlApi;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;

namespace PL.Volunteer
{
    public partial class VolunteerWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

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
    DependencyProperty.Register(nameof(CurrentVolunteer), typeof(BO.Volunteer), typeof(VolunteerWindow),
        new PropertyMetadata(null, OnCurrentVolunteerChanged));

        private static void OnCurrentVolunteerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VolunteerWindow window && window.CurrentVolunteer != null)
            {
                window.ManagerMode = window.CurrentVolunteer.MyPosition == Position.Manager;
            }
        }


        public ObservableCollection<Position> Roles { get; } = new ObservableCollection<Position>((Position[])Enum.GetValues(typeof(Position)));
        public ObservableCollection<DistanceType> DistanceTypes { get; } = new ObservableCollection<DistanceType>((DistanceType[])Enum.GetValues(typeof(DistanceType)));

        public BO.CallInProgress? CurrentCall
        {
            get => (BO.CallInProgress?)GetValue(CurrentCallProperty);
            set => SetValue(CurrentCallProperty, value);
        }
        public VolunteerInList LoggedInVolunteer { get; set; }

        private bool isEditMode = false;
        public bool IsAddMode => ButtonText == "Add";
        public bool ManagerMode
        {
            get { return (bool)GetValue(ManagerModeProperty); }
            set { SetValue(ManagerModeProperty, value); }
        }

        public static readonly DependencyProperty ManagerModeProperty =
            DependencyProperty.Register(nameof(ManagerMode), typeof(bool), typeof(VolunteerWindow));

        public static readonly DependencyProperty CurrentCallProperty =
            DependencyProperty.Register(nameof(CurrentCall), typeof(BO.CallInProgress), typeof(VolunteerWindow));
        public string ButtonText
        {
            get { return (string)GetValue(ButtonTextProperty); }
            set { SetValue(ButtonTextProperty, value); }
        }

        public static readonly DependencyProperty ButtonTextProperty =
    DependencyProperty.Register(
        nameof(ButtonText),
        typeof(string),
        typeof(VolunteerWindow),
        new PropertyMetadata("Add", OnButtonTextChanged));

        private static void OnButtonTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VolunteerWindow window)
                window.OnPropertyChanged(nameof(IsAddMode)); // טריגר עדכון לבינדינג
        }

        public string ButtonCallText
        {
            get { return (string)GetValue(ButtonCallTextProperty); }
            set { SetValue(ButtonCallTextProperty, value); }
        }

        public static readonly DependencyProperty ButtonCallTextProperty =
            DependencyProperty.Register(nameof(ButtonCallText), typeof(string), typeof(VolunteerWindow), new PropertyMetadata("Choose Call"));

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
            SetEditMode(true);
        }

        /// <summary>
        /// function that receives a selected volunteer from the list for editing
        /// </summary>
        /// <param name="selectedVolunteer"></param>
        public VolunteerWindow(VolunteerInList selectedVolunteer , Position role)
        {
            InitializeComponent();
            DataContext = this;

            try
            {
                CurrentVolunteer = s_bl.Volunteer.GetVolunteer(selectedVolunteer.Id);
                CurrentCall = CurrentVolunteer.CallInProgress;
                if (CurrentVolunteer == null)
                    throw new Exception("Volunteer not found");
                if (CurrentVolunteer == null)
                    throw new Exception("Volunteer not found");
                ManagerMode = role == Position.Manager;
                ButtonText = "Update";
                ButtonCallText = "Assign A Call";
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

                string password = PasswordBox.Password;

                if (ButtonText == "Add")
                {
                    if (string.IsNullOrWhiteSpace(password))
                    {
                        MessageBox.Show("Password is required for new volunteers.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (password.Length < 8)
                    {
                        MessageBox.Show("Password must be at least 8 characters long.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    CurrentVolunteer.Password = password;
                    s_bl.Volunteer.AddVolunteer(CurrentVolunteer);
                    MessageBox.Show("Volunteer added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                }
                else if (ButtonText == "Update")
                {
                    if (!isEditMode)
                    {
                        isEditMode = true;
                        SetEditMode(true);
                        return;
                    }

                    if (!string.IsNullOrWhiteSpace(password) && password.Length < 8)
                    {
                        MessageBox.Show("Password must be at least 8 characters long.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (!string.IsNullOrWhiteSpace(password))
                        CurrentVolunteer.Password = password;

                    s_bl.Volunteer.UpdateVolunteerDetails(CurrentVolunteer.Id, CurrentVolunteer);
                    MessageBox.Show("Volunteer updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }

            
            }
            catch (Exception ex)
            {
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
        private void DeleteVolunteer_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this volunteer?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    s_bl.Volunteer.DeleteVolunteer(CurrentVolunteer.Id);
                    MessageBox.Show("Volunteer deleted successfully.");
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting volunteer: {ex.Message}");
                }
            }
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

                s_bl.Call.CancelCallTreatment(ManagerMode ,CurrentVolunteer.Id,CurrentCall.Id);
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
            try
            {
                s_bl.Call.CompleteCallTreatment( CurrentVolunteer.Id, CurrentCall.Id);
                MessageBox.Show("Call Completed.", "Completed", MessageBoxButton.OK, MessageBoxImage.Information);

                CurrentCall = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error reporting completion of call processing: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtMaxDistance_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private void ShowClosedCalls_Click(object sender, RoutedEventArgs e)
        {
            // Use CurrentVolunteer.Id to pass the volunteer ID to the VolunteerClosedCallsWindow
            if (CurrentVolunteer != null)
            {
                var closedCallsWindow = new VolunteerClosedCallsWindow(CurrentVolunteer.Id); // Adjusted constructor usage
                closedCallsWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("No volunteer selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


    }
}