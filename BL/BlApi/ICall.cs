using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlApi
{
    /// <summary>
    /// Interface for managing calls in the system.
    /// </summary>
    public interface ICall
    {
        int[] GetCallQuantities();/// Retrieves an array of call quantities based on their status using GroupBy LINQ.
        IEnumerable<BO.CallInList> GetCallList(Enum? filterField=null, object? filterValue=null, Enum? sortField=null);/// Retrieves a sorted and filtered list of calls.
        BO.Call GetCallDetails(int callId);/// Retrieves details of a call, including its assignments.
        void UpdateCallDetails(BO.Call call);/// Updates the details of a given call with validation checks.
        void DeleteCall(int callId);/// Deletes a call if it meets the necessary conditions.
        void AddCall(BO.Call call);/// Adds a new call to the system with validation checks.
        IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId, BO.CallType? callTypeFilter=null, BO.ClosedCallInList? sortField=null);/// Retrieves a sorted and filtered list of closed calls assigned to a volunteer.
        IEnumerable<BO.OpenCallInList> GetOpenCallsForVolunteerSelection(int volunteerId, BO.CallType? callTypeFilter=null, BO.OpenCallInList? sortField=null);/// Retrieves open calls available for a volunteer, sorted and filtered.
        void CompleteCallTreatment(int volunteerId, int assignmentId);/// Marks a call as completed by the volunteer.
        void CancelCallTreatment(int requesterId, int assignmentId);/// Cancels a call assignment with validation of authorization.
        void AssignCallToVolunteer(int volunteerId, int callId);/// Assigns a call to a volunteer if it meets the necessary conditions.
    }
}
