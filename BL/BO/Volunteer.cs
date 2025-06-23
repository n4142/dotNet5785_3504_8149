using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class Volunteer
    {
        public int Id { get; init; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public Position MyPosition { get; set; }
        public string? Password { get; set; }
        public string? CurrentAddress { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? MaxDistance { get; set; }
        public DistanceType MyDistance { get; set; } = DistanceType.Air;
        public int HandledCalls { get; set; }
        public int CanceledCalls { get; set; }
        public int ExpiredCalls { get; set; }
        public BO.CallInProgress? CallInProgress { get; set; }
        public override string ToString() => ToStringProperty();

        public string ToStringProperty()
        {
            return string.Join(", ", GetType()
                .GetProperties()
                .Select(prop => $"{prop.Name}: {prop.GetValue(this) ?? "null"}"));
        }

    }
}
