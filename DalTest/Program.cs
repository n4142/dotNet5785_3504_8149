
using DalApi;
using DO;
using System;
using System.Runtime.CompilerServices;

namespace DalTest
{
    internal class Program
    {
        // Stage 1: DAL Implementations
        private static ICall? s_dalCall = new CallImplementation();
        private static IVolunteer? s_dalVolunteer = new VolunteerImplementation();
        private static IAssignment? s_dalAssignment = new AssignmentImplementation();
        private static IConfig? s_dalConfig = new ConfigImplementation();

        // Enum for the main menu options
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

        // Enum for the sub-menu options
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

        private static void DisplayMainMenu()
        {
            foreach (MainMenuOption option in Enum.GetValues(typeof(MainMenuOption)))
            {
                Console.WriteLine($"{(int)option}.{option}");
            }
            MainMenuOptions choice = Console.ReadLine();
            while(choice!=MainMenuOptions.Exit)
            switch (choice)
            {
                case MainMenuOptions.VolunteerSubMenu
                case MainMenuOptions.CallSubMenu
                case MainMenuOptions.AssignmentSubMenu
                        DisplayEntityMenu(choice);
                        break;
                case MainMenuOptions.InitializingData
                        Initialization.Do();
                        break;
                case MainMenuOptions.ShowAllData
                        ShowAllData();
                        break;
                case MainMenuOptions.ConfigSubMenu
                        DisplayConfigSubMenu();
                        break;
                case MainMenuOptions.ResetDataAndConfigData
                        ResetDataAndConfigData();
                        break;
                }
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
