using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class CallInList
    {
        public int Id { get; set; }
        public int CallId { get; set; }
        public CallType CallType { get; set; } // ENUM
        public DateTime OpenTime { get; set; }
        public TimeSpan? TimeRemaining { get; set; } // Calculated based on max time and current time
        public string LastVolunteerName { get; set; }
        public TimeSpan? TotalCompletionTime { get; set; } // Calculated if the call has been completed
        public CallStatus Status { get; set; } // ENUM
        public int TotalAssignments { get; set; } // Number of assignments for this call
    }

}
