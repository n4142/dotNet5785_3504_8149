namespace BO;
public enum Position
{
    Manager,
    Volunteer
}

public enum DistanceType
{
    Air,
    Walk,
    Drive
}

public enum CallType
{
    SuicidalSituations,
    PanicAttacks,
    SuddenCrises,
    ViolenceOrDanger,
    Loneliness,
    Depression,
    FamilyIssues,
    Soldiers,
    PTSD,
    All
}

public enum EndingTimeType
{
    TakenCareOf,
    CanceledByVolunteer,
    CanceledByManager,
    Expired
}

public enum CallStatus
{
    Open,
    Closed,
    InProgress,
    OpenAtRisk,
    Expired,
    InProgressAtRisk
}
public enum CompletionType
{
    Completed,
    SelfCanceled,
    ExpiredCanceled,
    AdminCanceled
}
public enum TimeUnit
{
    Minute,
    Hour,
    Day,
    Month,
    Year, 
    UNDEFINED
}
public enum VolunteerSortBy
{
    Id,
    FullName,
    TotalHandledCalls,
    TotalCanceledCalls,
    TotalExpiredCalls
}
public enum FilteredBy
{
    Status
}