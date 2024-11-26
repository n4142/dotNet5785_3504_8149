

namespace DO;
/// <summary>
/// Volunteer Entity
/// </summary>
/// <param name="Id">unique ID (created automatically - provide 0 as an argument)</param>
/// <param name="FullName">FullName of volunteer</param>
/// <param name="Phone">PhoneNumber of volunteer</param>
/// <param name="Email">Email of volunteer</param>
/// <param name="Password">Password of volunteer</param>
/// <param name="CurrentAddress">CurrentAddress of volunteer</param>
/// <param name="Position">Position of volunteer - can be a manager or a volunteer</param>
/// <param name="Latitude">Indicate how far a point on the earth is north or south of the equator </param>
/// <param name="Longitude">Indicate how far a point on the earth is east or west of the equator </param>
/// <param name="IsActive">Indicate if  the volunteer is active or not </param>
/// <param name="MaxDistance">The  MaxDistance  to receive a call</param>
/// <param name="DistanceType">DistanceType - type of Distance ,default - AIR </param>
///
public record Volunteer
(
int Id,
string FullName,
string Phone,
string Email,
bool IsActive,
Position MyPosition,
string? Password = null,
string? CurrentAddress = null,
double? Latitude = null,
double? Longitude = null,
double? MaxDistance = null,
DistanceType MyDistance = DistanceType.Air
)

{
    /// <summary>
    /// Default constructor for stage 3
    /// </summary>

    public Volunteer() : this(0, "noName", "noPhone", "noEmail", false, Position.Volunteer) { }
}

