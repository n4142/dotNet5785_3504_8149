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
    public interface IVolunteer :IObservable
    {
        BO.Position Login(int id, string password);///Logs in a volunteer using  id and password.
        IEnumerable<BO.VolunteerInList> GetVolunteerList(bool? filter=null,BO.VolunteerSortBy? sort=null);///Retrieves a list of volunteers with optional filtering and sorting.
        BO.Volunteer GetVolunteer(int id);///Retrieves a volunteer by their ID.
        void UpdateVolunteerDetails(int id,BO.Volunteer volunteer);///Updates the details of an existing volunteer.
        void DeleteVolunteer(int id);/// Deletes a volunteer by their ID.
        void AddVolunteer(BO.Volunteer volunteer);/// Adds a new volunteer to the system.
    }
}
