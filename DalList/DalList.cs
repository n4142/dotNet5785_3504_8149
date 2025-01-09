namespace Dal;
using DalApi;

sealed internal class DalList : IDal
{
    public static IDal Instance { get; } = new DalList();
    private DalList() { }
    public IAssignment Assignment { get; } = new AssignmentImplementation();

    public IVolunteer Volunteer { get; } = new VolunteerImplementation();

    public ICall Call { get; } = new CallImplementation();

    public IConfig Config { get; } = new ConfigImplementation();

    public void ResetDB()
    {
        Assignment.DeleteAll();
        Volunteer.DeleteAll();
        Call.DeleteAll();
        Config.Reset();
    }
}
