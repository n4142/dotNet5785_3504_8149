
//using DalApi;
//using DO;
//using Dal;

//namespace DalTest
//{
//    internal class Program
//    {

//        private static ICall? s_dalCall = new CallImplementation();
//        private static IVolunteer? s_dalVolunteer = new VolunteerImplementation();
//        private static IAssignment? s_dalAssignment = new AssignmentImplementation();
//        private static IConfig? s_dalConfig = new ConfigImplementation();

//        private enum MainMenuOptions
//        {
//            Exit,
//            VolunteerSubMenu,
//            CallSubMenu,
//            AssignmentSubMenu,
//            InitializingData,
//            ShowAllData,
//            ConfigSubMenu,
//            ResetDataAndConfigData
//        }

//        private enum SubMenuOptions
//        {
//            Exit,
//            Create,
//            Read,
//            ReadAll,
//            Update,
//            Delete,
//            DeleteAll
//        }
//        private enum ConfigMenu
//        {
//            Exit,
//            AdvanceClockByMinute,
//            AdvanceClockByHour,
//            AdvanceClockByDay,
//            AdvanceClockByMonth,
//            AdvanceClockByYear,
//            DisplayClock,
//            ChangeClockOrRiskRange,
//            DisplayConfigVar,
//            Reset
//        }
//        static Call CreateCall(int id)
//        {
//            Console.Write("Enter Call Type (1 for Type1, 2 for Type2, etc.): ");
//            CallType callType = (CallType)int.Parse(Console.ReadLine()!);

//            Console.Write("Enter Full Address: ");
//            string address = Console.ReadLine()!;

//            Console.Write("Enter Latitude: ");
//            double latitude = double.Parse(Console.ReadLine()!);

//            Console.Write("Enter Longitude: ");
//            double longitude = double.Parse(Console.ReadLine()!);

//            Console.Write("Enter Opening Time (YYYY-MM-DD HH:MM): ");
//            DateTime openingTime = DateTime.Parse(Console.ReadLine()!);

//            Console.Write("Enter Max Time Finish Calling (YYYY-MM-DD HH:MM): ");
//            string maxTimeStr = Console.ReadLine()!;
//            DateTime? maxTime = DateTime.Parse(maxTimeStr)!;

//            Console.Write("Enter Verbal Description (optional): ");
//            string? description = Console.ReadLine();

//            return new Call(id, callType, address, latitude, longitude, openingTime, maxTime, description);
//        }
//        private static Assignment CreateAssignment(int id)
//        {
//            Console.Write("Enter Call ID: ");
//            int callId = int.Parse(Console.ReadLine()!);

//            Console.Write("Enter Volunteer ID: ");
//            int volunteerId = int.Parse(Console.ReadLine()!);

//            Console.Write("Enter Entry Time of Treatment (YYYY-MM-DD HH:MM): ");
//            DateTime entryTime = DateTime.Parse(Console.ReadLine()!);

//            Console.Write("Enter Ending Time of Treatment ( YYYY-MM-DD HH:MM): ");
//            string endingTimeStr = Console.ReadLine()!;
//            DateTime? endingTime = string.IsNullOrWhiteSpace(endingTimeStr) ? null : DateTime.Parse(endingTimeStr);

//            Console.Write("Enter Ending Time Type (1 for TakenCareOf, 2 for CanceledByVolunteer, etc.): ");
//            string endingTimeTypeStr = Console.ReadLine()!;
//            EndingTimeType? endingTimeType = (EndingTimeType)int.Parse(endingTimeTypeStr);

//            return new Assignment(id, callId, volunteerId, entryTime, endingTime, endingTimeType);
//        }
//        private static Volunteer CreateVolunteer(int id)
//        {

//            Console.Write("Enter Full Name: ");
//            string fullName = Console.ReadLine()!;

//            Console.Write("Enter Phone: ");
//            string phone = Console.ReadLine()!;

//            Console.Write("Enter Email: ");
//            string email = Console.ReadLine()!;

//            Console.Write("Is Active? (true/false): ");
//            bool isActive = bool.Parse(Console.ReadLine()!);

//            Console.Write("Enter Position (1 for Manager, 2 for Volunteer, etc.): ");
//            Position position = (Position)int.Parse(Console.ReadLine()!);

//            Console.Write("Enter Password: ");
//            string? password = Console.ReadLine();

//            Console.Write("Enter Current Address: ");
//            string? currentAddress = Console.ReadLine();

//            Console.Write("Enter Latitude: ");
//            double latitude = double.Parse(Console.ReadLine()!);

//            Console.Write("Enter Longitude (optional): ");
//            double longitude = double.Parse(Console.ReadLine()!);

//            Console.Write("Enter Max Distance (optional): ");
//            double maxDistance = double.Parse(Console.ReadLine()!);

//            Console.Write("Enter Distance Type (1 for Air, 2 for Road, etc.): ");
//            DistanceType distanceType = (DistanceType)int.Parse(Console.ReadLine()!);

//            return new Volunteer(id, fullName, phone, email, isActive, position, password, currentAddress, latitude, longitude, maxDistance, distanceType);
//        }
//        private static void Create(string entityName)
//        {
//            Console.WriteLine("Enter your ID:");
//            int id = int.Parse(Console.ReadLine()!);

//            switch (entityName)
//            {
//                case "VolunteerSubMenu":
//                    Volunteer volunteer = CreateVolunteer(id);
//                    s_dalVolunteer?.Create(volunteer);

//                    break;
//                case "CallSubMenu":
//                    Call call = CreateCall(id);
//                    s_dalCall?.Create(call);

//                    break;
//                case "AssignmentSubMenu":
//                    Assignment assignment = CreateAssignment(id);
//                    s_dalAssignment?.Create(assignment);

//                    break;
//            }
//        }

//        private static void Read(string entityName, int id)
//        {
//            switch (entityName)
//            {
//                case "VolunteerSubMenu":
//                    Console.WriteLine(s_dalVolunteer?.Read(id));
//                    break;
//                case "CallSubMenu":
//                    Console.WriteLine(s_dalCall?.Read(id));
//                    break;
//                case "AssignmentSubMenu":
//                    Console.WriteLine(s_dalAssignment?.Read(id));
//                    break;
//            }
//        }

//        private static void ReadAll(string entityName)
//        {
//            switch (entityName)
//            {
//                case "VolunteerSubMenu":
//                    Console.WriteLine(s_dalVolunteer?.ReadAll());
//                    break;
//                case "CallSubMenu":
//                    Console.WriteLine(s_dalCall?.ReadAll());
//                    break;
//                case "AssignmentSubMenu":
//                    Console.WriteLine(s_dalAssignment?.ReadAll());
//                    break;
//            }
//        }

//        private static void Update(string entityName)
//        {
//            Console.WriteLine("Enter your ID:");
//            int id = int.Parse(Console.ReadLine()!);

//            switch (entityName)
//            {
//                case "VolunteerSubMenu":
//                    Volunteer updatedVolunteer = CreateVolunteer(id);
//                    s_dalVolunteer?.Update(updatedVolunteer);
//                    Console.WriteLine("Volunteer updated successfully!");
//                    break;
//                case "CallSubMenu":
//                    Call updatedCall = CreateCall(id);
//                    s_dalCall?.Update(updatedCall);
//                    Console.WriteLine("Call updated successfully!");
//                    break;
//                case "AssignmentSubMenu":
//                    Assignment updatedAssignment = CreateAssignment(id);
//                    s_dalAssignment?.Update(updatedAssignment);
//                    Console.WriteLine("Assignment updated successfully!");
//                    break;
//            }
//        }

//        private static void Delete(string entityName, int id)
//        {
//            switch (entityName)
//            {
//                case "VolunteerSubMenu":
//                    s_dalVolunteer?.Delete(id);
//                    Console.WriteLine("Volunteer deleted successfully!");
//                    break;
//                case "CallSubMenu":
//                    s_dalCall?.Delete(id);
//                    Console.WriteLine("Call deleted successfully!");
//                    break;
//                case "AssignmentSubMenu":
//                    s_dalAssignment?.Delete(id);
//                    Console.WriteLine("Assignment deleted successfully!");
//                    break;
//            }
//        }

//        private static void DeleteAll(string entityName)
//        {
//            switch (entityName)
//            {
//                case "VolunteerSubMenu":
//                    s_dalVolunteer?.DeleteAll();
//                    Console.WriteLine("All volunteers deleted!");
//                    break;
//                case "CallSubMenu":
//                    s_dalCall?.DeleteAll();
//                    Console.WriteLine("All calls deleted!");
//                    break;
//                case "AssignmentSubMenu":
//                    s_dalAssignment?.DeleteAll();
//                    Console.WriteLine("All assignments deleted!");
//                    break;
//            }
//        }
//        private static void EntityMenu(string entityName)
//        {
//            SubMenuOptions choice;
//            do
//            {
//                Console.WriteLine($"Submenu for {entityName}:");
//                foreach (var option in Enum.GetValues(typeof(SubMenuOptions)))
//                {
//                    Console.WriteLine($"{(int)option}. {option}");
//                }

//                Console.Write("Enter your choice: ");
//                Enum.TryParse(Console.ReadLine(), out choice);

//                try
//                {
//                    switch (choice)
//                    {
//                        case SubMenuOptions.Create:
//                            Create(entityName);
//                            break;
//                        case SubMenuOptions.Read:
//                            Console.WriteLine("Enter ID:");
//                            int id = int.Parse(Console.ReadLine()!);
//                            Read(entityName, id);
//                            break;
//                        case SubMenuOptions.ReadAll:
//                            ReadAll(entityName);
//                            break;
//                        case SubMenuOptions.Update:
//                            Update(entityName);
//                            break;
//                        case SubMenuOptions.Delete:
//                            Console.WriteLine("Enter ID:");
//                            int deleteId = int.Parse(Console.ReadLine()!);
//                            Delete(entityName, deleteId);
//                            break;
//                        case SubMenuOptions.DeleteAll:
//                            DeleteAll(entityName);
//                            break;
//                        case SubMenuOptions.Exit:
//                            Console.WriteLine($"Exiting {entityName} menu.");
//                            break;
//                        default:
//                            Console.WriteLine("Invalid choice.");
//                            break;
//                    }
//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine($"An error occurred: {ex.Message}");
//                }
//            } while (choice != SubMenuOptions.Exit);
//        }
//        private static void ShowAllData()
//        {
//            Console.WriteLine("Volunteers:");
//            Console.WriteLine(s_dalVolunteer?.ReadAll());

//            Console.WriteLine("Calls:");
//            Console.WriteLine(s_dalCall?.ReadAll());

//            Console.WriteLine("Assignments:");
//            Console.WriteLine(s_dalAssignment?.ReadAll());
//        }
//        private static void ResetDatabase()
//        {
//            s_dalConfig.Reset();
//            s_dalVolunteer.DeleteAll();
//            s_dalCall.DeleteAll();
//            s_dalAssignment.DeleteAll();
//        }
//        private static void DisplayMainMenu()
//        {
//            foreach (MainMenuOption option in Enum.GetValues(typeof(MainMenuOption)))
//            {
//                Console.WriteLine($"{(int)option}.{option}");
//            }
//            MainMenuOptions choice = Console.ReadLine();
//            while (choice != MainMenuOptions.Exit)
//            switch (choice)
//            {
//                case MainMenuOptions.VolunteerSubMenu
//                case MainMenuOptions.CallSubMenu
//                case MainMenuOptions.AssignmentSubMenu
//                        DisplayEntityMenu(choice);
//                        break;
//                case MainMenuOptions.InitializingData
//                        Initialization.Do();
//                        break;
//                case MainMenuOptions.ShowAllData
//                        ShowAllData();
//                        break;
//                case MainMenuOptions.ConfigSubMenu
//                        DisplayConfigSubMenu();
//                        break;
//                case MainMenuOptions.ResetDataAndConfigData
//                        ResetDataAndConfigData();
//                        break;
//                }
//        }
//        private static void ConfigMenu()
//        {

//            private static void ConfigMenu()
//            {
//                Console.WriteLine("Config Menu:");
//                foreach (ConfigSubmenu option in Enum.GetValues(typeof(ConfigSubmenu))) // שינוי ConfigSubmenu ל-ConfigSubmenu
//                {
//                    Console.WriteLine($"{(int)option}. {option}");
//                }
//                Console.Write("Select an option: ");
//                if (!Enum.TryParse(Console.ReadLine(), out ConfigSubmenu userInput))
//                    throw new FormatException("Invalid choice");

//                // השתמש ב-userInput במקום Input
//                while (userInput != ConfigSubmenu.Exit)  // תיקון ממ Input ל-userInput
//                {
//                    switch (userInput)
//                    {
//                        case ConfigMenu.AdvanceClockByMinute:
//                            s_dalConfig.Clock = s_dalConfig.Clock.AddMinutes(1);
//                            break;

//                        case ConfigMenu.AdvanceClockByHour:
//                            s_dalConfig.Clock = s_dalConfig.Clock.AddHours(1);
//                            break;

//                        case ConfigMenu.AdvanceClockByDay:  // שינוי מ-ConfigMenu.AdvanceClockByHour ל-AdvanceClockByDay
//                            s_dalConfig.Clock = s_dalConfig.Clock.AddDays(1);
//                            break;

//                        case ConfigMenu.AdvanceClockByMonth:
//                            s_dalConfig.Clock = s_dalConfig.Clock.AddMonths(1);
//                            break;

//                        case ConfigMenu.AdvanceClockByYear:  // תיקון ממ-ConfigMenu.AdvanceClockByHour ל-AdvanceClockByYear
//                            s_dalConfig.Clock = s_dalConfig.Clock.AddYears(1);
//                            break;

//                        case ConfigMenu.DisplayClock:
//                            Console.WriteLine($"Clock: {s_dalConfig.Clock}");  // הסר את קריאת פונקציית Clock()
//                            break;

//                        case ConfigMenu.ChangeClockOrRiskRange:  // תיקון מ-ConfigMenu.ChangeSpan ל-ChangeClockOrRiskRange
//                            Console.WriteLine("Enter a time span in the format [hh:mm:ss]:");
//                            if (!TimeSpan.TryParse(Console.ReadLine(), out TimeSpan newRiskRange))
//                                throw new FormatException("Invalid time format.");
//                            s_dalConfig.RiskRange = newRiskRange;  // תיקון מ-Config ל-s_dalConfig
//                            break;

//                        case ConfigMenu.DisplayConfigVar:
//                            // כאן אפשר להוסיף תצוגה של משתנים נוספים, אם יש.
//                            break;

//                        case ConfigMenu.Reset:
//                            s_dalConfig.Reset();  // תיקון קריאת reset
//                            break;

//                        default:
//                            Console.WriteLine("Invalid option.");
//                            break;
//                    }

//                    // הצגת התפריט שוב לאחר כל בחירה
//                    Console.WriteLine("Config Menu:");
//                    foreach (ConfigSubmenu option in Enum.GetValues(typeof(ConfigSubmenu)))
//                    {
//                        Console.WriteLine($"{(int)option}. {option}");
//                    }
//                    Console.Write("Select an option: ");
//                    if (!Enum.TryParse(Console.ReadLine(), out userInput))
//                        throw new FormatException("Invalid choice");
//                }
//            }

//            static void Main(string[] args)
//        {
//            try
//            {
//              DisplayMainMenu();
//            }
//            catch (Exception e)
//            {
//                Console.WriteLine($"An error occurred: {e.Message}");
//            }
//        }
//    }
//}
using DalApi;
using DO;
using Dal;

namespace DalTest
{
    internal class Program
    {
        private static ICall? s_dalCall = new CallImplementation();
        private static IVolunteer? s_dalVolunteer = new VolunteerImplementation();
        private static IAssignment? s_dalAssignment = new AssignmentImplementation();
        private static IConfig? s_dalConfig = new ConfigImplementation();

        private enum MainMenuOptions
        {
            Exit,
            VolunteerSubMenu,
            CallSubMenu,
            AssignmentSubMenu,
            InitializingData,
            ShowAllData,
            ConfigSubMenu,
            ResetDataAndConfigData
        }

        private enum SubMenuOptions
        {
            Exit,
            Create,
            Read,
            ReadAll,
            Update,
            Delete,
            DeleteAll
        }

        private enum ConfigSubmenu // שינוי ל-ConfigSubmenu
        {
            Exit,
            AdvanceClockByMinute,
            AdvanceClockByHour,
            AdvanceClockByDay,
            AdvanceClockByMonth,
            AdvanceClockByYear,
            DisplayClock,
            ChangeClockOrRiskRange,
            DisplayConfigVar,
            Reset
        }

        static Call CreateCall(int id)
        {
            Console.Write("Enter Call Type (1 for Type1, 2 for Type2, etc.): ");
            CallType callType = (CallType)int.Parse(Console.ReadLine()!);

            Console.Write("Enter Full Address: ");
            string address = Console.ReadLine()!;

            Console.Write("Enter Latitude: ");
            double latitude = double.Parse(Console.ReadLine()!);

            Console.Write("Enter Longitude: ");
            double longitude = double.Parse(Console.ReadLine()!);

            Console.Write("Enter Opening Time (YYYY-MM-DD HH:MM): ");
            DateTime openingTime = DateTime.Parse(Console.ReadLine()!);

            Console.Write("Enter Max Time Finish Calling (YYYY-MM-DD HH:MM): ");
            string maxTimeStr = Console.ReadLine()!;
            DateTime? maxTime = DateTime.Parse(maxTimeStr)!;

            Console.Write("Enter Verbal Description (optional): ");
            string? description = Console.ReadLine();

            return new Call(id, callType, address, latitude, longitude, openingTime, maxTime, description);
        }

        private static Assignment CreateAssignment(int id)
        {
            Console.Write("Enter Call ID: ");
            int callId = int.Parse(Console.ReadLine()!);

            Console.Write("Enter Volunteer ID: ");
            int volunteerId = int.Parse(Console.ReadLine()!);

            Console.Write("Enter Entry Time of Treatment (YYYY-MM-DD HH:MM): ");
            DateTime entryTime = DateTime.Parse(Console.ReadLine()!);

            Console.Write("Enter Ending Time of Treatment ( YYYY-MM-DD HH:MM): ");
            string endingTimeStr = Console.ReadLine()!;
            DateTime? endingTime = string.IsNullOrWhiteSpace(endingTimeStr) ? null : DateTime.Parse(endingTimeStr);

            Console.Write("Enter Ending Time Type (1 for TakenCareOf, 2 for CanceledByVolunteer, etc.): ");
            string endingTimeTypeStr = Console.ReadLine()!;
            EndingTimeType? endingTimeType = (EndingTimeType)int.Parse(endingTimeTypeStr);

            return new Assignment(id, callId, volunteerId, entryTime, endingTime, endingTimeType);
        }

        private static Volunteer CreateVolunteer(int id)
        {
            Console.Write("Enter Full Name: ");
            string fullName = Console.ReadLine()!;

            Console.Write("Enter Phone: ");
            string phone = Console.ReadLine()!;

            Console.Write("Enter Email: ");
            string email = Console.ReadLine()!;

            Console.Write("Is Active? (true/false): ");
            bool isActive = bool.Parse(Console.ReadLine()!);

            Console.Write("Enter Position (1 for Manager, 2 for Volunteer, etc.): ");
            Position position = (Position)int.Parse(Console.ReadLine()!);

            Console.Write("Enter Password: ");
            string? password = Console.ReadLine();

            Console.Write("Enter Current Address: ");
            string? currentAddress = Console.ReadLine();

            Console.Write("Enter Latitude: ");
            double latitude = double.Parse(Console.ReadLine()!);

            Console.Write("Enter Longitude (optional): ");
            double longitude = double.Parse(Console.ReadLine()!);

            Console.Write("Enter Max Distance (optional): ");
            double maxDistance = double.Parse(Console.ReadLine()!);

            Console.Write("Enter Distance Type (1 for Air, 2 for Road, etc.): ");
            DistanceType distanceType = (DistanceType)int.Parse(Console.ReadLine()!);

            return new Volunteer(id, fullName, phone, email, isActive, position, password, currentAddress, latitude, longitude, maxDistance, distanceType);
        }

        private static void Create(string entityName)
        {
            Console.WriteLine("Enter your ID:");
            int id = int.Parse(Console.ReadLine()!);

            switch (entityName)
            {
                case "VolunteerSubMenu":
                    Volunteer volunteer = CreateVolunteer(id);
                    s_dalVolunteer?.Create(volunteer);
                    break;
                case "CallSubMenu":
                    Call call = CreateCall(id);
                    s_dalCall?.Create(call);
                    break;
                case "AssignmentSubMenu":
                    Assignment assignment = CreateAssignment(id);
                    s_dalAssignment?.Create(assignment);
                    break;
            }
        }

        private static void Read(string entityName, int id)
        {
            switch (entityName)
            {
                case "VolunteerSubMenu":
                    Console.WriteLine(s_dalVolunteer?.Read(id));
                    break;
                case "CallSubMenu":
                    Console.WriteLine(s_dalCall?.Read(id));
                    break;
                case "AssignmentSubMenu":
                    Console.WriteLine(s_dalAssignment?.Read(id));
                    break;
            }
        }

        private static void ReadAll(string entityName)
        {
            switch (entityName)
            {
                case "VolunteerSubMenu":
                    Console.WriteLine(s_dalVolunteer?.ReadAll());
                    break;
                case "CallSubMenu":
                    Console.WriteLine(s_dalCall?.ReadAll());
                    break;
                case "AssignmentSubMenu":
                    Console.WriteLine(s_dalAssignment?.ReadAll());
                    break;
            }
        }

        private static void Update(string entityName)
        {
            Console.WriteLine("Enter your ID:");
            int id = int.Parse(Console.ReadLine()!);

            switch (entityName)
            {
                case "VolunteerSubMenu":
                    Volunteer updatedVolunteer = CreateVolunteer(id);
                    s_dalVolunteer?.Update(updatedVolunteer);
                    Console.WriteLine("Volunteer updated successfully!");
                    break;
                case "CallSubMenu":
                    Call updatedCall = CreateCall(id);
                    s_dalCall?.Update(updatedCall);
                    Console.WriteLine("Call updated successfully!");
                    break;
                case "AssignmentSubMenu":
                    Assignment updatedAssignment = CreateAssignment(id);
                    s_dalAssignment?.Update(updatedAssignment);
                    Console.WriteLine("Assignment updated successfully!");
                    break;
            }
        }

        private static void Delete(string entityName, int id)
        {
            switch (entityName)
            {
                case "VolunteerSubMenu":
                    s_dalVolunteer?.Delete(id);
                    Console.WriteLine("Volunteer deleted successfully!");
                    break;
                case "CallSubMenu":
                    s_dalCall?.Delete(id);
                    Console.WriteLine("Call deleted successfully!");
                    break;
                case "AssignmentSubMenu":
                    s_dalAssignment?.Delete(id);
                    Console.WriteLine("Assignment deleted successfully!");
                    break;
            }
        }

        private static void DeleteAll(string entityName)
        {
            switch (entityName)
            {
                case "VolunteerSubMenu":
                    s_dalVolunteer?.DeleteAll();
                    Console.WriteLine("All volunteers deleted!");
                    break;
                case "CallSubMenu":
                    s_dalCall?.DeleteAll();
                    Console.WriteLine("All calls deleted!");
                    break;
                case "AssignmentSubMenu":
                    s_dalAssignment?.DeleteAll();
                    Console.WriteLine("All assignments deleted!");
                    break;
            }
        }

        private static void EntityMenu(string entityName)
        {
            SubMenuOptions choice;
            do
            {
                Console.WriteLine($"Submenu for {entityName}:");
                foreach (var option in Enum.GetValues(typeof(SubMenuOptions)))
                {
                    Console.WriteLine($"{(int)option}. {option}");
                }

                Console.Write("Enter your choice: ");
                Enum.TryParse(Console.ReadLine(), out choice);

                try
                {
                    switch (choice)
                    {
                        case SubMenuOptions.Create:
                            Create(entityName);
                            break;
                        case SubMenuOptions.Read:
                            Console.WriteLine("Enter ID:");
                            int id = int.Parse(Console.ReadLine()!);
                            Read(entityName, id);
                            break;
                        case SubMenuOptions.ReadAll:
                            ReadAll(entityName);
                            break;
                        case SubMenuOptions.Update:
                            Update(entityName);
                            break;
                        case SubMenuOptions.Delete:
                            Console.WriteLine("Enter ID:");
                            int deleteId = int.Parse(Console.ReadLine()!);
                            Delete(entityName, deleteId);
                            break;
                        case SubMenuOptions.DeleteAll:
                            DeleteAll(entityName);
                            break;
                        case SubMenuOptions.Exit:
                            Console.WriteLine($"Exiting {entityName} menu.");
                            break;
                        default:
                            Console.WriteLine("Invalid choice.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            } while (choice != SubMenuOptions.Exit);
        }

        private static void ShowAllData()
        {
            Console.WriteLine("Volunteers:");
            Console.WriteLine(s_dalVolunteer?.ReadAll());

            Console.WriteLine("Calls:");
            Console.WriteLine(s_dalCall?.ReadAll());

            Console.WriteLine("Assignments:");
            Console.WriteLine(s_dalAssignment?.ReadAll());
        }

        private static void ResetDatabase()
        {
            s_dalConfig.Reset();
            s_dalVolunteer.DeleteAll();
            s_dalCall.DeleteAll();
            s_dalAssignment.DeleteAll();
        }

        private static void DisplayMainMenu()
        {
            foreach (MainMenuOptions option in Enum.GetValues(typeof(MainMenuOptions)))
            {
                Console.WriteLine($"{(int)option}. {option}");
            }

            MainMenuOptions choice = (MainMenuOptions)int.Parse(Console.ReadLine()!);

            while (choice != MainMenuOptions.Exit)
            {
                switch (choice)
                {
                    case MainMenuOptions.VolunteerSubMenu:
                    case MainMenuOptions.CallSubMenu:
                    case MainMenuOptions.AssignmentSubMenu:
                        EntityMenu(choice.ToString());
                        break;
                    case MainMenuOptions.InitializingData:
                        Initialization.Do(s_dalVolunteer, s_dalCall, s_dalAssignment, s_dalConfig);
                        break;
                    case MainMenuOptions.ShowAllData:
                        ShowAllData();
                        break;
                    case MainMenuOptions.ConfigSubMenu:
                        ConfigMenu();
                        break;
                    case MainMenuOptions.ResetDataAndConfigData:
                        ResetDatabase();
                        break;
                }
            }
        }

        static void Main(string[] args)
        {
            try
            {
                DisplayMainMenu();
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred: {e.Message}");
            }
        }
    }
}
