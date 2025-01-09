//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using DalApi;
using System.Diagnostics;
namespace Dal;
//stage 3
sealed internal class DalXML : IDal
{
    public static IDal Instance { get; } = new DalXML();
    private DalXML() { }
    public IConfig Config{ get; } = new ConfigImplementation();

    public IAssignment Assignment { get; } = new AssignmentImplementation();

    public ICall Call { get; } = new CallImplementation();

    public IVolunteer Volunteer { get; } =  new VolunteerImplementation();

    public void ResetDB()
    {
        Assignment.DeleteAll();
        Volunteer.DeleteAll();
        Call.DeleteAll();
        Config.Reset();
    }
}
