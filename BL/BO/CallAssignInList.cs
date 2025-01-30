using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class CallAssignInList
    {
        public int VolunteerId { get; set; }
        public string VolunteerName { get; set; }
        public DateTime EntryTime { get; set; }
        public DateTime? ActualCompletionTime { get; set; }
        public CompletionType? CompletionType { get; set; }
    }

}
