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
    PTSD
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
    InProgress,
    AtRisk
}
public enum CompletionType
{
    Completed,
    Canceled,
    Expired
}
