using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class CallInProgress
    {
        public int Id { get; set; } // מספר מזהה של ישות ההקצאה
        public int CallId { get; set; } // מספר מזהה רץ של ישות הקריאה
        public CallType CallType { get; set; } // סוג הקריאה
        public string Description { get; set; } // תיאור מילולי
        public string Address { get; set; } // כתובת מלאה של הקריאה
        public DateTime OpenTime { get; set; } // זמן פתיחה
        public DateTime? MaxEndTime { get; set; } // זמן מקסימלי לסיום הקריאה
        public DateTime StartTreatmentTime { get; set; } // זמן כניסה לטיפול
        public double DistanceFromVolunteer { get; set; } // מרחק קריאה מהמתנדב
        public CallStatus Status { get; set; } // סטטוס הקריאה
    }
}
