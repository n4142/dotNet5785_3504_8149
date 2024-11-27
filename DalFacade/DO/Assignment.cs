
namespace DO;
/// <summary>
/// Assignment Entity
/// </summary>
/// <param name="Id">unique ID , indicates the allocation entity</param>
/// <param name="CallId">Represents a number that identifies the call that the volunteer chose to handle</param>
/// <param name="VolunteerId">represents the ID of the volunteer who chose to take care of the reading</param>
/// <param name="EntryTimeOfTreatment">Time (date and time) when the current call was processed. The time when for the first time the current volunteer chose to take care of her.</param>
/// <param name="EndingTimeOfTreatment"Time (date and time) when the current volunteer finished handling the current call</param>
/// <param name="endingTimeType">The manner in which the treatment of the current reading was completed by the current volunteer</param>


public record Assignment
(

  int CallId,
  int VolunteerId,
  DateTime? EntryTimeOfTreatment,
  DateTime? EndingTimeOfTreatment = null,
  EndingTimeType? MyEndingTime = null
)
{
    public int Id { get; set; }
    /// <summary>
    /// Default constructor for stage 3
    /// </summary>
    public Assignment() : this(0, 0,null, DateTime.MinValue) { }
}