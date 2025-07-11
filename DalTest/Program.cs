﻿using DO;
using Dal;
using DalApi;
using System.Data.SqlTypes;

namespace DalTest
    
{
    internal class Program
    {
        //private static ICall? s_dal.Call = new CallImplementation();
        //private static IVolunteer? s_dalVolunteer = new VolunteerImplementation();
        //private static IAssignment? s_dalAssignment = new AssignmentImplementation();
        //private static IConfig? s_dalConfig = new ConfigImplementation();
        //static readonly IDal s_dal = new DalList(); //stage 2
        //static readonly IDal s_dal = new DalXml(); //stage 3
        static readonly IDal s_dal = Factory.Get; //stage 4

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

            return new Call( callType, address, latitude, longitude, openingTime, maxTime, description);
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

            return new Assignment( callId, volunteerId, entryTime, endingTime, endingTimeType);
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
                    s_dal.Volunteer?.Create(volunteer);
                    break;
                case "CallSubMenu":
                    Call call = CreateCall(id);
                    s_dal.Call?.Create(call);
                    break;
                case "AssignmentSubMenu":
                    Assignment assignment = CreateAssignment(id);
                    s_dal.Assignment?.Create(assignment);
                    break;
            }
        }

        private static void Read(string entityName, int id)
        {
            switch (entityName)
            {
                case "VolunteerSubMenu":
                    Console.WriteLine(s_dal.Volunteer?.Read(id));
                    break;
                case "CallSubMenu":
                    Console.WriteLine(s_dal.Call?.Read(id));
                    break;
                case "AssignmentSubMenu":
                    Console.WriteLine(s_dal.Assignment?.Read(id));
                    break;
            }
        }

        private static void ReadAll(string entityName)
        {
            switch (entityName)
            {
                case "VolunteerSubMenu":
                    var volunteers = s_dal.Volunteer?.ReadAll();
                    if (volunteers != null && volunteers.Any())
                    {
                        foreach (var volunteer in volunteers)
                        {
                            Console.WriteLine(volunteer); // Ensure `Volunteer` has a meaningful `ToString()` override
                        }
                    }
                    else
                    {
                        Console.WriteLine("No volunteers found.");
                    }
                    break;
                case "CallSubMenu":
                    var calls = s_dal.Call?.ReadAll();
                    if (calls != null && calls.Any())
                    {
                        foreach (var call in calls)
                        {
                            Console.WriteLine(call); // Ensure `Call` has a meaningful `ToString()` override
                        }
                    }
                    else
                    {
                        Console.WriteLine("No calls found.");
                    }
                    break;
                case "AssignmentSubMenu":
                    var assignments = s_dal.Assignment?.ReadAll();
                    if (assignments != null && assignments.Any())
                    {
                        foreach (var assignment in assignments)
                        {
                            Console.WriteLine(assignment); // Ensure `Assignment` has a meaningful `ToString()` override
                        }
                    }
                    else
                    {
                        Console.WriteLine("No assignments found.");
                    }
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
                    s_dal.Volunteer?.Update(updatedVolunteer);
                    Console.WriteLine("Volunteer updated successfully!");
                    break;
                case "CallSubMenu":
                    Call updatedCall = CreateCall(id);
                    s_dal.Call?.Update(updatedCall);
                    Console.WriteLine("Call updated successfully!");
                    break;
                case "AssignmentSubMenu":
                    Assignment updatedAssignment = CreateAssignment(id);
                    s_dal.Assignment?.Update(updatedAssignment);
                    Console.WriteLine("Assignment updated successfully!");
                    break;
            }
        }

        private static void Delete(string entityName, int id)
        {
            switch (entityName)
            {
                case "VolunteerSubMenu":
                    s_dal.Volunteer?.Delete(id);
                    Console.WriteLine("Volunteer deleted successfully!");
                    break;
                case "CallSubMenu":
                    s_dal.Call?.Delete(id);
                    Console.WriteLine("Call deleted successfully!");
                    break;
                case "AssignmentSubMenu":
                    s_dal.Assignment?.Delete(id);
                    Console.WriteLine("Assignment deleted successfully!");
                    break;
            }
        }

        private static void DeleteAll(string entityName)
        {
            switch (entityName)
            {
                case "VolunteerSubMenu":
                    s_dal.Volunteer?.DeleteAll();
                    Console.WriteLine("All volunteers deleted!");
                    break;
                case "CallSubMenu":
                    s_dal.Call?.DeleteAll();
                    Console.WriteLine("All calls deleted!");
                    break;
                case "AssignmentSubMenu":
                    s_dal.Assignment?.DeleteAll();
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
            var volunteers = s_dal.Volunteer?.ReadAll();
            if (volunteers != null && volunteers.Any())
            {
                Console.WriteLine("Volunteers:");
                foreach (var volunteer in volunteers)
                {
                    Console.WriteLine(volunteer); // Ensure `Volunteer` has a meaningful `ToString()` override
                }
                Console.WriteLine("Calls:");
                var calls = s_dal.Call?.ReadAll();
                if (calls != null && calls.Any())
                {
                    foreach (var call in calls)
                    {
                        Console.WriteLine(call); // Ensure `Call` has a meaningful `ToString()` override
                    }
                }
                Console.WriteLine("Assignments:");
                var assignments = s_dal.Assignment?.ReadAll();
                if (assignments != null && assignments.Any())
                {
                    foreach (var assignment in assignments)
                    {
                        Console.WriteLine(assignment); // Ensure `Assignment` has a meaningful `ToString()` override
                    }
                }
            }
        }

        private static void ResetDatabase()
        {
            s_dal.Config.Reset();
            s_dal.Volunteer.DeleteAll();
            s_dal.Call.DeleteAll();
            s_dal.Assignment.DeleteAll();
        }
        private static void DisplayMainMenu()
        {
            MainMenuOptions choice;
            do
            {
                // Display the main menu options
                foreach (MainMenuOptions option in Enum.GetValues(typeof(MainMenuOptions)))
                {
                    Console.WriteLine($"{(int)option}. {option}");
                }

                Console.Write("Enter your choice: ");
                if (!Enum.TryParse(Console.ReadLine(), out choice) || !Enum.IsDefined(typeof(MainMenuOptions), choice))
                {
                    Console.WriteLine("Invalid choice. Please try again.");
                    continue;
                }

                switch (choice)
                {
                    case MainMenuOptions.VolunteerSubMenu:
                    case MainMenuOptions.CallSubMenu:
                    case MainMenuOptions.AssignmentSubMenu:
                        EntityMenu(choice.ToString());
                        break;
                    case MainMenuOptions.InitializingData:
                        //Initialization.DO(s_dalVolunteer, s_dalCall, s_dalAssignment, s_dalConfig);/
                        //Initialization.Do(s_dal); //stage 2
                        Initialization.Do(); //stage 4
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
                    case MainMenuOptions.Exit:
                        Console.WriteLine("Exiting program...");
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            } while (choice != MainMenuOptions.Exit);
        }

        private static void ConfigMenu()
        {
            ConfigSubmenu choice;
            do
            {
                Console.WriteLine("Config Submenu:");
                foreach (ConfigSubmenu option in Enum.GetValues(typeof(ConfigSubmenu)))
                {
                    Console.WriteLine($"{(int)option}. {option}");
                }

                Console.Write("Enter your choice: ");
                if (!Enum.TryParse(Console.ReadLine(), out choice) || !Enum.IsDefined(typeof(ConfigSubmenu), choice))
                {
                    Console.WriteLine("Invalid choice. Please try again.");
                    continue;
                }

                try
                {
                    switch (choice)
                    {
                        case ConfigSubmenu.AdvanceClockByMinute:
                            s_dal.Config.Clock = s_dal.Config.Clock.AddMinutes(1);
                            break;

                        case ConfigSubmenu.AdvanceClockByHour:
                            s_dal.Config.Clock = s_dal.Config.Clock.AddHours(1);
                            break;

                        case ConfigSubmenu.AdvanceClockByDay:
                            s_dal.Config.Clock = s_dal.Config.Clock.AddDays(1);
                            break;

                        case ConfigSubmenu.AdvanceClockByMonth:
                            s_dal.Config.Clock = s_dal.Config.Clock.AddMonths(1);
                            break;

                        case ConfigSubmenu.AdvanceClockByYear:
                            s_dal.Config.Clock = s_dal.Config.Clock.AddYears(1);
                            break;

                        case ConfigSubmenu.DisplayClock:
                            Console.WriteLine($"Current Clock: {s_dal.Config.Clock}");
                            break;

                        case ConfigSubmenu.ChangeClockOrRiskRange:
                            Console.Write("Enter new risk range in hours: ");
                            double riskHours = double.Parse(Console.ReadLine()!);
                            s_dal.Config.RiskRange = TimeSpan.FromHours(riskHours);
                            break;

                        case ConfigSubmenu.DisplayConfigVar:
                            Console.WriteLine($"Clock: {s_dal.Config.Clock}");
                            Console.WriteLine($"Risk Range: {s_dal.Config.RiskRange}");
                            break;

                        case ConfigSubmenu.Reset:
                            s_dal.Config.Reset();
                            break;

                        case ConfigSubmenu.Exit:
                            Console.WriteLine("Exiting Config menu.");
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
            } while (choice != ConfigSubmenu.Exit);
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


