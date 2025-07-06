namespace DalTest;
using DalApi;
using DO;


public class Initialization
    {
    //private static ICall? s_dalCall;
    //private static IVolunteer? s_dalVolunteer;
    //private static IAssignment? s_dalAssignment;
    //private static IConfig? s_dalConfig;
    private static IDal? s_dal; //stage 2

    private static readonly Random s_rand = new();
        private static void CreateVolunteer()
        {
        //const int MIN_ID = 100000000;
        //const int MAX_ID = 999999999;
            int[] israeliIds =
                { 605656537, 852170240, 639825686, 590901518, 963951819, 255336760, 612332221, 407664853, 890530835, 578539355, 680082534, 201438678, 678719766, 421671066, 245068002, 417482171 };
        string[] names =
                {"Danah Cohen", "Yossi Levi", "Michal Rosenberg", "Alon Goldstein", "Shira Peled", "Roi Barak", "Netta Shemesh", "Tomer Saban", "Lian Bloch", "Ido Melamed", "Yael Peretz", "Eliyah Kahanov", "Maya Hershkowitz", "Gilad Oren", "Efrat Kaplan"};
            string[] phons =
                {"0504142102", "0533128532", "0527648207", "0556784521", "0546587412", "0527684698", "0527612233", "0527695898", "0546581256","0527612589","0527648693","0527631459","0527638520","0527641120","0527648965"};
            string[] addresses =
                {"Herzl St 1, Tel Aviv", "Ben Yehuda St 10, Haifa",  "Hanevi'im St 15, Jerusalem",  "Rothschild Blvd 50, Tel Aviv",  "Jabotinsky St 125, Ramat Gan",  "King George St 20, Jerusalem",  "Allenby St 45, Tel Aviv",  "Bialik St 7, Ramat Gan",  "Dizengoff St 100, Tel Aviv",  "Weizmann St 5, Rehovot",  "Yehuda Halevi St 30, Tel Aviv",  "Hertzl St 10, Beer Sheva",  "Sokolov St 15, Herzliya",  "Begin Blvd 8, Ashdod",  "Ben Gurion St 1, Bat Yam"    };
            (double latitude, double longitude)[] coordinates =
                {  (32.0595, 34.7705),  (32.8195, 34.9983),  (31.7857, 35.2208),  (32.0633, 34.7735),  (32.0885, 34.8037),  (31.7762, 35.2137),  (32.0641, 34.7698),  (32.0827, 34.8235),  (32.0758, 34.7742),  (31.8947, 34.8112),  (32.0622, 34.7701),  (31.2518, 34.7913),  (32.1663, 34.8426),  (31.8025, 34.6457),  (32.0153, 34.7504) };
            s_dal!.Volunteer.Create(new(215553504, "Nechami", "0525381648", "Nechami@gmail.com", true, Position.Manager, "5555", "Hatikva 6", 31.958240, 34.879992, 10));
            for (int i = 0; i < 15; i++)
            {
                s_dal!.Volunteer.Create(new(
                    //s_rand.Next(MIN_ID, MAX_ID),
                    israeliIds[i],
                    names[i],
                    phons[i],
                    $"{names[i].Replace(" ", "")}@gmail.com",
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
            string[] addresses = {"Herzl St 1, Tel Aviv",  "Ben Yehuda St 10, Haifa",  "Hanevi'im St 15, Jerusalem",  "Rothschild Blvd 50, Tel Aviv",  "Jabotinsky St 125, Ramat Gan",  "King George St 20, Jerusalem",  "Allenby St 45, Tel Aviv",  "Bialik St 7, Ramat Gan",  "Dizengoff St 100, Tel Aviv",  "Weizmann St 5, Rehovot",  "Yehuda Halevi St 30, Tel Aviv",  "Hertzl St 10, Beer Sheva",  "Sokolov St 15, Herzliya",  "Begin Blvd 8, Ashdod",
  "Ben Gurion St 1, Bat Yam",  "HaMasger St 22, Tel Aviv",  "Pinsker St 12, Haifa",  "Shderot HaNasi 45, Haifa",  "Emek Refaim St 20, Jerusalem",  "Arlozorov St 5, Rishon LeZion",
  "Hahagana St 15, Petah Tikva",
  "Bar Ilan St 9, Ashkelon",
  "Lev HaIr St 3, Eilat",
  "Shalom Aleichem St 8, Tel Aviv",
  "Sheinkin St 15, Tel Aviv",
  "Trumpeldor St 10, Tel Aviv",
  "Yigal Alon St 55, Tel Aviv",
  "Tchernichovsky St 25, Haifa",
  "David HaMelech St 10, Netanya",
  "Kaplan St 4, Jerusalem",
  "Shderot Yerushalayim 35, Jaffa",
  "Frishman St 24, Tel Aviv",
  "Nahalat Binyamin St 60, Tel Aviv",
  "Hahistadrut St 50, Haifa",
  "Rabin Blvd 12, Beersheba",
  "Moshe Dayan Blvd 18, Holon",
  "Ehad HaAm St 2, Tel Aviv",
  "Habonim St 6, Kfar Saba",
  "Nordau Blvd 22, Tel Aviv",
  "Zabotinsky St 50, Ramat Gan",
  "Shmuel Hanatziv St 7, Jerusalem",
  "Einstein Blvd 32, Haifa",
  "Balfour St 40, Bat Yam",
  "Hertzl St 120, Hadera",
  "Shaar HaGai St 4, Modi'in",
  "Hayarkon St 200, Tel Aviv",
  "Sheshet Hayamim St 8, Ashdod",
  "King David St 11, Jerusalem",
  "HaAliyah St 22, Acre",
  "Neve Shaanan St 30, Haifa" };
            (double latitude, double longitude)[] coordinates = { (32.0595, 34.7705),
  (32.8195, 34.9983),
  (31.7857, 35.2208),
  (32.0633, 34.7735),
  (32.0885, 34.8037),
  (31.7762, 35.2137),
  (32.0641, 34.7698),
  (32.0827, 34.8235),
  (32.0758, 34.7742),
  (31.8947, 34.8112),
  (32.0622, 34.7701),
  (31.2518, 34.7913),
  (32.1663, 34.8426),
  (31.8025, 34.6457),
  (32.0153, 34.7504),
  (32.0619, 34.7813),
  (32.8215, 34.9956),
  (32.8077, 35.0004),
  (31.7656, 35.2108),
  (31.9734, 34.7927),
  (32.0873, 34.8695),
  (31.6781, 34.5748),
  (29.5533, 34.9501),
  (32.0715, 34.7687),
  (32.0667, 34.7739),
  (32.0731, 34.7714),
  (32.0713, 34.7932),
  (32.7944, 35.0012),
  (32.3329, 34.8576),
  (31.7765, 35.2224),
  (32.0497, 34.7592),
  (32.0812, 34.7731),
  (32.0603, 34.7748),
  (32.8072, 35.0033),
  (31.2554, 34.7890),
  (32.0167, 34.7683),
  (32.0702, 34.7695),
  (32.1713, 34.9062),
  (32.0899, 34.7816),
  (32.0815, 34.8196),
  (31.7814, 35.2190),
  (32.7974, 34.9898),
  (32.0227, 34.7521),
  (32.4398, 34.9197),
  (31.9092, 35.0075),
  (32.1928, 34.7672),
  (31.8046, 34.6627),
  (31.7743, 35.2245),
  (32.9274, 35.0871),
  (32.7922, 35.0342)};
        string[] descriptions = {
    "An elderly woman who wants to end her life due to loneliness and lack of meaning",
    "A teenage boy struggling to cope with classroom bullying and expressing fears of social rejection",
    "A young adult in his 20s experiencing panic attacks in daily life and finding it difficult to leave his home",
    "A young woman with postpartum depression feeling unable to function as a mother",
    "A man in his 40s struggling to cope with job loss and expressing hopelessness about the future",
    "A high school teacher feeling burned out and unable to balance work with personal life",
    "A student overwhelmed by academic stress and exams, considering dropping out of university",
    "An older adult recently widowed, feeling lost and unable to move forward",
    "A teenage girl with self-esteem issues, believing she is not as worthy as her peers",
    "A single mother experiencing heavy emotional and financial pressure with little support from her surroundings",
    "A soldier recently discharged from the army struggling to adjust to civilian life",
    "A refugee coping with trauma from war and separation from family",
    "A young entrepreneur facing repeated business failures and feeling defeated",
    "A child of divorced parents feeling torn between two households",
    "A man in his 50s facing an identity crisis after early retirement",
    "A caregiver exhausted from taking care of a chronically ill parent",
    "A woman experiencing social isolation after moving to a new city",
    "A teenager addicted to social media and feeling disconnected in real life",
    "A recent graduate feeling aimless and unsure about future career plans",
    "A man struggling with gambling addiction and hiding it from his family",
    "A nurse emotionally overwhelmed by constant exposure to patient suffering",
    "A woman battling chronic illness and feeling like a burden to her family",
    "A high school senior anxious about college admissions and future expectations",
    "A young man dealing with body image issues and negative self-talk",
    "A middle-aged man going through a painful divorce and custody battle",
    "A woman in an abusive relationship feeling trapped and helpless",
    "A child dealing with the sudden death of a sibling",
    "A man feeling depressed after immigrating and facing cultural dissonance",
    "A mother of a child with special needs feeling unsupported and exhausted",
    "A professional athlete who had to retire early due to injury",
    "An introverted teenager who feels invisible at school and home",
    "A man suffering from insomnia due to financial stress",
    "A young woman coping with a recent miscarriage and grief",
    "An artist feeling creatively blocked and doubting their talent",
    "A new teacher struggling with classroom management and self-doubt",
    "A person struggling with obsessive-compulsive behaviors interfering with daily life",
    "A woman unable to find a partner and feeling unlovable",
    "A man diagnosed with a terminal illness and fearing death",
    "A teenager struggling with anger issues and frequent outbursts",
    "A university student experiencing severe homesickness",
    "A widower in his 70s refusing to engage in any social activity",
    "A firefighter suffering from PTSD after a traumatic rescue",
    "A single father overwhelmed by parenting responsibilities",
    "A young adult facing discrimination due to ethnicity or religion",
    "A woman struggling to care for her children while working multiple jobs",
    "A man recently released from prison and unable to find work",
    "A college student dealing with social anxiety and isolation",
    "A woman in a high-pressure job experiencing constant anxiety",
    "A man going through a midlife crisis and questioning life choices",
    "A teenager whose best friend moved away and feels abandoned"
};

        DateTime start = new DateTime(2025, 1, 1);
            for (int i = 0; i < 50; i++)
            {
                DateTime startingTime = start.AddDays(s_rand.Next(0,365));
                s_dal!.Call.Create(new Call(
                (CallType)s_rand.Next(Enum.GetValues(typeof(CallType)).Length),
                addresses[i],
                coordinates[i].latitude,
                coordinates[i].longitude,
                startingTime,
                startingTime.AddDays(s_rand.Next(1, 30)),
                descriptions[i]
                ));
            };
        }
    //private static void CreateAssignment()
    //{
    //    List<Volunteer> volunteerList = s_dal!.Volunteer.ReadAll();
    //    List<Call> callsList = s_dal!.Call.ReadAll();
    //    for (int i = 0; i < 50; i++)
    //    {
    //        DateTime?[] endingTimeOptions = { null, callsList[i].MaxTimeFinishCalling, callsList[i].OpeningTime.AddMinutes(s_rand.Next(5, 59)) };
    //        s_dal!.Assignment.Create(new Assignment(
    //            callsList[i].Id,
    //            volunteerList[s_rand.Next(volunteerList.Count)].Id,
    //            callsList[i].OpeningTime.AddMinutes(s_rand.Next(0, 60)),
    //            endingTimeOptions[s_rand.Next(endingTimeOptions.Length)],
    //            (EndingTimeType)s_rand.Next(Enum.GetValues(typeof(EndingTimeType)).Length - 1)
    //            ));
    //    };
    //}
    private static void CreateAssignment()
    {
        List<Volunteer> volunteers = s_dal!.Volunteer.ReadAll();
        List<Call> calls = s_dal!.Call.ReadAll();
        List<Assignment> assignments = new();

        // רק 80% מהמתנדבים יהיו זמינים להקצאות
        List<Volunteer> activeVolunteers = volunteers
            .OrderBy(_ => s_rand.Next())
            .Take((int)(volunteers.Count * 0.8))
            .ToList();

        foreach (var call in calls)
        {
            // כמה הקצאות יהיו לקריאה זו (0–3)
            int numAssignments = s_rand.Next(0, 4);

            for (int i = 0; i < numAssignments; i++)
            {
                var volunteer = activeVolunteers[s_rand.Next(activeVolunteers.Count)];

                // בדיקה: האם למתנדב כבר יש הקצאה פתוחה?
                bool hasOpenAssignment = assignments.Any(a =>
                    a.VolunteerId == volunteer.Id && a.EndingTimeOfTreatment == null);

                if (hasOpenAssignment)
                    continue; // דלג על מתנדב עם טיפול פעיל

                // זמן כניסה לטיפול בין פתיחה לסיום
                DateTime entry = RandomBetween(call.OpeningTime, call.MaxTimeFinishCalling ?? call.OpeningTime.AddHours(2));

                // 70% סיכוי לסיים את הטיפול
                DateTime? end = s_rand.NextDouble() < 0.7
                    ? entry.AddMinutes(s_rand.Next(10, 60))
                    : null;

                // סוג סיום אם יש
                EndingTimeType? type = end == null
                    ? null
                    : (EndingTimeType?)s_rand.Next(Enum.GetValues(typeof(EndingTimeType)).Length);

                // הוספה לרשימת ההקצאות
                assignments.Add(new Assignment(
                    call.Id,
                    volunteer.Id,
                    entry,
                    end,
                    type
                ));
            }
        }

        // שמירה לדאטה
        foreach (var a in assignments)
        {
            s_dal.Assignment.Create(a);
        }
    }

    // פונקציית עזר להגריל תאריך
    private static DateTime RandomBetween(DateTime start, DateTime end)
    {
        if (start >= end) return start;
        var range = (end - start).TotalMinutes;
        return start.AddMinutes(s_rand.Next((int)range));
    }


    //public static void Do(IDal dal) //stage 2
    public static void Do() //stage 4
    {
        //s_dalAssignment = dalAssignment ?? throw new NullReferenceException("DAL object can not be null!");
        //s_dalCall = dalCall ?? throw new NullReferenceException("DAL object can not be null!");
        //s_dalConfig = dalConfig ?? throw new NullReferenceException("DAL object can not be null!");
        //s_dalVolunteer = dalVolunteer ?? throw new NullReferenceException("DAL object can not be null!");
        //s_dal = dal ?? throw new NullReferenceException("DAL object can not be null!"); //stage 2
        s_dal = DalApi.Factory.Get; //stage 4
        //Console.WriteLine("Reset Configuration values and List values...");
        //s_dalConfig.Reset();
        //s_dalCall.DeleteAll();
        //s_dalAssignment.DeleteAll();
        //s_dalVolunteer.DeleteAll();
        s_dal!.ResetDB();
        Console.WriteLine("Initializing  lists ...");
        CreateVolunteer();
        CreateCall();
        CreateAssignment();
    }
};

