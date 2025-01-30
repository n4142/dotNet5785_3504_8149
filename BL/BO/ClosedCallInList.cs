using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class ClosedCallInList
    {
        public int Id { get; set; } // מספר מזהה רץ של ישות הקריאה
        public CallType CallType { get; set; } // סוג הקריאה
        public string Address { get; set; } // כתובת מלאה של הקריאה
        public DateTime OpenTime { get; set; } // זמן פתיחה
        public DateTime StartTime { get; set; } // זמן כניסה לטיפול
        public DateTime? EndTime { get; set; } // זמן סיום הטיפול בפועל
        public CallStatus EndStatus { get; set; } // סוג סיום הטיפול
    }
}
