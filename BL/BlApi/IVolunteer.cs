using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlApi
{
    /// <summary>
    /// Interface for managing volunteers in the system.
    /// </summary>
    public interface IVolunteer
    {
        BO.Position Login(string userName, int password);///Logs in a volunteer using a username and password.
        IEnumerable<BO.VolunteerInList> GetVolunteerList(bool? filter=null,BO.VolunteerInList? sort=null);///Retrieves a list of volunteers with optional filtering and sorting.
        BO.Volunteer GetVolunteer(int id);///Retrieves a volunteer by their ID.
        void UpdateVolunteerDetails(int id,BO.Volunteer volunteer);///Updates the details of an existing volunteer.
        void DeleteVolunteer(int id);/// Deletes a volunteer by their ID.
        void AddVolunteer(BO.Volunteer volunteer);/// Adds a new volunteer to the system.
    }
}
