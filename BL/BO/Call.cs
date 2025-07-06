using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class Call
    {
        public int Id { get; set; }
        public CallType CallType { get; set; } // ENUM
        public string Description { get; set; }
        public string FullAddress { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public DateTime OpenTime { get; set; }
        public DateTime? MaxCompletionTime { get; set; }
        public CallStatus Status { get; set; } // ENUM
        public List<CallAssignInList> CallAssignments { get; set; }
    }
}

