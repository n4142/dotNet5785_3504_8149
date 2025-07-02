using System.Runtime.CompilerServices;

namespace Dal;
/// <summary>
/// Configuration Entity
/// </summary>
/// <param name="NextCallId">an ID number for the next new call</param>
/// <param name="NextAssignmentId">an ID number for a new instance of the assignment entity between volunteer and call</param>
/// <param name="Clock">A system clock that is maintained separately from the actual computer clock</param>
/// <param name="RiskRange">Time range from which onwards reading is considered at risk</param>
internal static class Config
{
    internal const int StartCallId = 1;
    private static int nextCallId = StartCallId;
    internal static int NextCallId { get => nextCallId++; }

    internal const int StartAssignmentId = 1;
    private static int nextAssignmentId = StartAssignmentId;
    internal static int NextAssignmentId { get => nextAssignmentId++; }
    internal static DateTime Clock { get; set; } =  DateTime.Now;

    internal static TimeSpan RiskRange { get; set; } = TimeSpan.FromHours(1);
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    internal static void Reset()
    {
        nextCallId = StartCallId;
        nextAssignmentId = StartAssignmentId;
        Clock = DateTime.Now;
        RiskRange = TimeSpan.FromHours(1);
    }


}
