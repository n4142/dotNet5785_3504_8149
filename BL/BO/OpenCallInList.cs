using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class OpenCallInList
    {
        public int Id { get; set; } // מספר מזהה רץ של ישות הקריאה
        public CallType CallType { get; set; } // סוג הקריאה
        public string Description { get; set; } // תיאור מילולי
        public string Address { get; set; } // כתובת מלאה של הקריאה
        public DateTime OpenTime { get; set; } // זמן פתיחה
        public DateTime? MaxEndTime { get; set; } // זמן מקסימלי לסיום הקריאה
        public double DistanceFromVolunteer { get; set; } // מרחק הקריאה מהמתנדב
    }
  
}
