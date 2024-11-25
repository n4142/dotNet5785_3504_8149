
namespace DO;
/// <summary>
/// Call Entity
/// </summary>
/// <param name="Id">unique ID , indicates the allocation entity</param>
/// <param name="CallType">The type of calling</param>
/// <param name="VerbalDescription">Description of the calling. Additional details about the calling</param>
/// <param name="FullAddressCall">Full address.</param>
/// <param name="Latitude">Indicate how far a point on the earth is north or south of the equator </param>
/// <param name="Longitude">Indicate how far a point on the earth is east or west of the equator </param>
/// <param name="OpeningTime">Time (date and hour) when the reading was opened by the administrator</param>
/// <param name="MaxTimeFinishCalling">"Time (date and hour) by which the reading must be closed </param>

public record Call
(

  CallType MyCall,
  string FullAddressCall,
  double Latitude,
  double Longitude,
  DateTime OpeningTime,
  DateTime? MaxTimeFinishCalling = null,
  string? VerbalDescription = null
  )
{
    public int Id { get; init; }
    /// <summary>
    /// Default constructor for stage 3
    /// </summary>
    public Call() : this(default(CallType), "NoAddress", 0, 0, DateTime.MinValue) { }


}