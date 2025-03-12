using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class VolunteerInList
    {
        public int Id { get; set; } // ת.ז מתנדב
        public string FullName { get; set; } // שם מלא
        public bool IsActive { get; set; } // פעיל
        public int TotalHandledCalls { get; set; } // סך קריאות שטופלו
        public int TotalCanceledCalls { get; set; } // סך קריאות שבוטלו
        public int CallInTreatmentId {  get; set; } //מספר מזהה של קריאה שבטיפול המתנדב כרגע
        public CallType CallInTreatmenType { get; set; } //סוג קריאה שבטיפול המתנדב כרגע
    }
}