
namespace DalTest;
using DalApi;
using DO;
//לבקש מגיפיטי:
//15 כתובות רנדומליות+קורדינציות
//50 כתובות רנדומליות+קורדינציות
//10 תאורים קצרים מקרים של פניות לעזרה נפשית
public class Initialization
{
    private static ICall? s_dalCall;
    private static IVolunteer? s_dalVolunteer;
    private static IAssignment? s_dalAssignment;
    private static IConfig? s_dalConfig;

    private static readonly Random s_rand = new();
    private static void CreateVolunteer()
    {
        const int MIN_ID = 100000000;
        const int MAX_ID = 999999999;
        string[] names =
            {"Danah Cohen", "Yossi Levi", "Michal Rosenberg", "Alon Goldstein", "Shira Peled", "Roi Barak", "Netta Shemesh", "Tomer Saban", "Lian Bloch", "Ido Melamed", "Yael Peretz", "Eliyah Kahanov", "Maya Hershkowitz", "Gilad Oren", "Efrat Kaplan"};
        string[] phons =
            {"0504142102", "0533128532", "0527648207", "0556784521", "0546587412", "0527684698", "0527612233", "0527695898", "0546581256","0527612589","0527648693","0527631459","0527638520","0527641120","0527648965"};
        string[] addresses = { "" };
        (float latitude, float longitude)[] coordinates = { (1, 2) };
        s_dalVolunteer!.Create(new(s_rand.Next(MIN_ID, MAX_ID), "Nechami", "0525381648", "Nechami@gmail.com", true, Position.Manager, "5555", "Hatikva 6", 31.958240, 34.879992, 10));
        for (int i = 0; i < 15; i++)
        {
            s_dalVolunteer!.Create(new(
                s_rand.Next(MIN_ID, MAX_ID),
                names[i],
                phons[i],
                $"{names[i]}@gmail.com",
                true, Position.Volunteer,
                phons[i].Substring(5, 4), //a password created by manager
                addresses[i],
                coordinates[i].latitude,
                coordinates[i].longitude,
                s_rand.Next(0, 20)
            ));
        }
    }

    private static void CreateCall()
    {
        string[] addresses = { "" };
        (float latitude, float longitude)[] coordinates = { (1, 2) };
        string[] descriptions = { "" };
        DateTime start = new DateTime(2022, 1, 1);
        for (int i = 0; i < 50; i++)
        {
            DateTime startingTime = start.AddDays(s_rand.Next((int)(s_dalConfig!.Clock - start).TotalDays));
            s_dalCall!.Create(new Call(
            (CallType)s_rand.Next(Enum.GetValues(typeof(CallType)).Length),
            addresses[i],
            coordinates[i].latitude,
            coordinates[i].longitude,
            startingTime,
            startingTime.AddMinutes(s_rand.Next(30, 60)),
            descriptions[s_rand.Next(descriptions.Length)]
            ));
        };
    }
    //פה עדיין יש באגים לא ברורים
    private static void CreateAssignment()
    {
        List<Volunteer> volunteerList = s_dalVolunteer.ReadAll();
        List<Call> callsList = s_dalCall.ReadAll();
        for (int i = 0; i < 50; i++)
        {
            DateTime[] endingTimeOptions = { null, callsList[i].MaxTimeFinishCalling, callsList[i].OpeningTime.AddMinutes(s_rand.Next(5, 59)) };
            s_dalAssignment.Create(new Assignment(
                callsList[i].Id,
                volunteerList[s_rand.Next(volunteerList.Count)].Id,
                callsList[i].OpeningTime.AddMinutes(s_rand.Next(0, 60)),
                endingTimeOptions[s_rand.Next(endingTimeOptions.Length)],
 (EndingTimeType)s_rand.Next(Enum.GetValues(typeof(EndingTimeType)).Length - 1)
                ));
        };
    }
}
