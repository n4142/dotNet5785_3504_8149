

namespace BlImplementation;
using BlApi;
using BO;
using DalApi;
using DO;
using Helpers;
using System;
using System.Collections.Generic;
using static Helpers.Tools;

internal class CallImplementation : BlApi.ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public void AddCall(BO.Call call)
    {
        try
        {
            CallManager.ChecksLogicalValidation(call);
            CallManager.ValidateCallDetails(call);
        }
        catch (Exception ex)
        {
            throw new BlGeneralDatabaseException("Error adding the call in the data layer.", ex);
        }
        DO.Call doCall = new DO.Call
        {
            Id = call.Id,
            MyCall = (DO.CallType)call.CallType,
            VerbalDescription = call.Description,
            FullAddressCall = call.FullAddress,
            Latitude = call.Latitude,
            Longitude = call.Longitude,
            OpeningTime = call.OpenTime,
            MaxTimeFinishCalling = call.MaxCompletionTime,
        };

        try
        {
            _dal.Call.Create(doCall);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"No call found with ID {call.Id}.", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlDoesNotExistException("Error adding the call in the data layer.", ex);
        }
    }

    public void AssignCallToVolunteer(int volunteerId, int callId)
    {
        var callStatus = CallManager.GetCallStatus(callId);

        // Check if the call already has an active assignment
        bool hasOpenAssignment = _dal.Assignment.ReadAll()
            .Any(a => a.CallId == callId && a.MyEndingTime == DO.EndingTimeType.TakenCareOf);

        // Validate the request
        if (hasOpenAssignment)
            throw new BlInvalidTimeUnitException("The call is already being handled by another volunteer.");

        if (callStatus == BO.CallStatus.Expired)
            throw new BlInvalidTimeUnitException("Cannot assign a call that has expired.");

        if (callStatus != BO.CallStatus.Open && callStatus != BO.CallStatus.OpenAtRisk)
            throw new BlInvalidTimeUnitException("Cannot assign a call that is not open.");
        DO.Assignment doAssign = new DO.Assignment
        {
            CallId = callId,
            VolunteerId = volunteerId,
            EntryTimeOfTreatment = ClockManager.Now,
            EndingTimeOfTreatment = null,
            MyEndingTime = null
        };
        try
        {
            _dal.Assignment.Create(doAssign);
        }
        catch (Exception ex)
        {
            throw new BlGeneralDatabaseException("Error adding the assignment in the data layer.", ex);
        }
    }

    public void CancelCallTreatment(int requesterId, int assignmentId)
    {
        var volunteer=_dal.Volunteer.Read(requesterId);
        var assignment = _dal.Assignment.Read(assignmentId);
        //checks if the volunteer is allowed canceling the treatment
        if (assignment.VolunteerId != requesterId||volunteer.MyPosition!=DO.Position.Manager)
            throw new BlUnauthorizedAccessException("This volunteer is not allowed canceling treatment");

        //checks if the assignment is open
        if (assignment.EndingTimeOfTreatment == null || assignment.MyEndingTime == DO.EndingTimeType.Expired || assignment.MyEndingTime == DO.EndingTimeType.CanceledByManager)
            throw new BlInvalidTimeUnitException("It is not possible to report the end of treatment because the offer is not open");

        DO.Assignment updatedAssignment = new DO.Assignment
        {
            Id = assignment.Id,
            CallId = assignment.CallId,
            VolunteerId = requesterId,
            EntryTimeOfTreatment = assignment.EntryTimeOfTreatment,
            EndingTimeOfTreatment = ClockManager.Now,
            MyEndingTime = volunteer.MyPosition==DO.Position.Manager
                ?DO.EndingTimeType.CanceledByManager
                :DO.EndingTimeType.CanceledByVolunteer
        };
        try
        {
            _dal.Assignment.Update(updatedAssignment);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Assignment with ID {assignmentId} does not exist.", ex);
        }
        catch (Exception ex)
        {
            throw new BlGeneralDatabaseException("Error updating the assignment from the data layer.", ex);
        }
    }

    public void CompleteCallTreatment(int volunteerId, int assignmentId)
    {
        var assignment=_dal.Assignment.Read(assignmentId);
        //checks if the volunteer is allowed reporting the end of treatment
        if (assignment.VolunteerId != volunteerId)
            throw new BlUnauthorizedAccessException("This volunteer is not allowed because the assignment is not in his name");
       
        //checks if the assignment is open
        if (assignment.EndingTimeOfTreatment == null || assignment.MyEndingTime == DO.EndingTimeType.Expired || assignment.MyEndingTime == DO.EndingTimeType.CanceledByManager)
            throw new BlInvalidTimeUnitException("It is not possible to report the end of treatment because the offer is not open");
        DO.Assignment updatedAssignment=new DO.Assignment {
            Id = assignment.Id,
            CallId = assignment.CallId,
            VolunteerId = volunteerId,
            EntryTimeOfTreatment = assignment.EntryTimeOfTreatment,
            EndingTimeOfTreatment=ClockManager.Now,
            MyEndingTime=DO.EndingTimeType.TakenCareOf
        };
        try
        {
            _dal.Assignment.Update(updatedAssignment);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Assignment with ID {assignmentId} does not exist.", ex); 
        }
        catch (Exception ex)
        {
            throw new BlGeneralDatabaseException("Error updating the assignment from the data layer.", ex);
        }
    }

    public void DeleteCall(int callId)
    {
        var call = _dal.Call.Read(callId) ?? throw new BO.BlDoesNotExistException($"No call found with ID {callId}.");
        //checks if call can be deleted
        if (CallManager.GetCallStatus(call.Id) != BO.CallStatus.Open || _dal.Assignment.ReadAll().Any(a => a.CallId == callId))
        {
            throw new BlInvalidTimeUnitException("Cannot delete call. It is either not open or has been assigned to a volunteer.");
        }
        try
        {
            _dal.Call.Delete(callId);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Call with ID {callId} does not exist.", ex); 
        }
        catch (Exception ex)
        {
            throw new BlGeneralDatabaseException("Error deleting the call from the data layer.", ex); 
        }
    }

    public BO.Call GetCallDetails(int callId)
    {
        var call=_dal.Call.Read(callId)?? throw new BO.BlDoesNotExistException($"Call with ID {callId} not found.");
        List<BO.CallAssignInList> assignmentsList = _dal.Assignment.ReadAll()
            .Where(a => a.CallId == callId)
            .Select(a => new BO.CallAssignInList
            {
                VolunteerId = a.VolunteerId,
                VolunteerName = _dal.Volunteer.Read(a.VolunteerId).FullName,
                EntryTime = (DateTime)a.EntryTimeOfTreatment,
                ActualCompletionTime = a.EndingTimeOfTreatment,
                CompletionType = (BO.CompletionType)a.MyEndingTime
            })
            .ToList();

        return new BO.Call
        {
            Id = callId,
            CallType = (BO.CallType)call.MyCall,
            Description = call.VerbalDescription,
            FullAddress = call.FullAddressCall,
            Latitude = call.Latitude,
            Longitude = call.Longitude,
            OpenTime = call.OpeningTime,
            MaxCompletionTime = call.MaxTimeFinishCalling,
            Status = CallManager.GetCallStatus(callId),
            CallAssignments = assignmentsList
        };
   }

    public IEnumerable<CallInList> GetCallList(Enum? filterField = null, object? filterValue = null, Enum? sortField = null)
    {
        IEnumerable<CallInList> callList = _dal.Call.ReadAll()
                 .Select(c => CallManager.ConvertToBOCallInList(c))
                 .ToList();
        if (filterField != null && filterValue != null)
        {
            var property = typeof(BO.CallInList).GetProperty(filterField.ToString());
            if (property != null)
            {
                callList = callList.Where(c => property.GetValue(c)?.Equals(filterValue) == true);
            }
        }
        if (sortField != null)
        {
            var property = typeof(BO.CallInList).GetProperty(sortField.ToString());
            if (property != null)
            {
                callList = callList.OrderBy(c => property.GetValue(c));
            }
        }
        else
        {
            callList = callList.OrderBy(c => c.CallId);
        }
        return callList.ToList();
    }
          
    public int[] GetCallQuantities()
    { 
         var groupedCalls = _dal.Call.ReadAll().GroupBy(c =>CallManager.GetCallStatus(c.Id))
                                    .Select(group => new { Status = group.Key, Count = group.Count() })
                                    .ToArray();
        int[] quantities = Enum.GetValues(typeof(CallStatus))
        .Cast<CallStatus>()
        .Select(status => groupedCalls
             .FirstOrDefault(g => g.Status == status)?.Count ?? 0)
        .ToArray();
        return quantities;
    }

    public IEnumerable<ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId, BO.CallType? callTypeFilter = null, ClosedCallInList? sortField = null)
    {
        var volunteersClosedCalls = _dal.Call.ReadAll()
            .Where(c => CallManager.GetCallStatus(c.Id).ToString() == "Closed" &&
                        _dal.Assignment.ReadAll().Any(a => a.VolunteerId == volunteerId && a.CallId == c.Id))
            .ToList();

        if (callTypeFilter != null)
            volunteersClosedCalls = volunteersClosedCalls
                .Where(c => (BO.CallType)c.MyCall == callTypeFilter)
                .ToList();
        if (sortField != null)
        {
            var property = typeof(BO.CallInList).GetProperty(sortField.ToString());
            if (property != null)
            {
                volunteersClosedCalls = volunteersClosedCalls.OrderBy(c => property.GetValue(c)).ToList();
            }
        }
        else
        {
            volunteersClosedCalls = volunteersClosedCalls.OrderBy(c => c.Id).ToList();
        }

        IEnumerable<BO.ClosedCallInList> calls=volunteersClosedCalls.Select(c => new ClosedCallInList
        {
            Id = c.Id,
            CallType = (BO.CallType)c.MyCall,
            Address = c.FullAddressCall,
            OpenTime = c.OpeningTime,
            StartTime = (DateTime)_dal.Assignment.ReadAll().Find(a => c.Id == a.CallId).EntryTimeOfTreatment,
            EndTime = (DateTime)_dal.Assignment.ReadAll().Find(a => c.Id == a.CallId).EndingTimeOfTreatment,
            EndStatus = (BO.EndingTimeType)_dal.Assignment.ReadAll().Find(a => c.Id == a.CallId).MyEndingTime,
        });
        return calls;
    }

    public IEnumerable<OpenCallInList> GetOpenCallsForVolunteerSelection(int volunteerId, BO.CallType? callTypeFilter = null, OpenCallInList? sortField = null)
    {
        var volunteer = _dal.Volunteer.Read(volunteerId);
        var calls = _dal.Call.ReadAll()
            .Where(c => CallManager.GetCallStatus(c.Id).ToString() == "Open" || CallManager.GetCallStatus(c.Id).ToString() == "OpenAtRisk")
            .ToList();
        if (callTypeFilter != null)
            calls = calls.Where(c => (BO.CallType)c.MyCall == callTypeFilter).ToList();
        if (sortField != null)
        {
            var property = typeof(BO.CallInList).GetProperty(sortField.ToString());
            if (property != null)
            {
                calls = calls.OrderBy(c => property.GetValue(c)).ToList();
            }
        }
        else
        {
            calls = calls.OrderBy(c => c.Id).ToList();
        }
        IEnumerable<OpenCallInList> openCallsList = calls.Select(c => new OpenCallInList
        {
            Id = c.Id,
            CallType = (BO.CallType)c.MyCall,
            Description = c.VerbalDescription,
            Address = c.FullAddressCall,
            OpenTime = c.OpeningTime,
            MaxEndTime = c.MaxTimeFinishCalling,
            DistanceFromVolunteer = CalculateDistance(volunteer.Latitude.Value, volunteer.Longitude.Value, c.Latitude, c.Longitude)

        });
        return openCallsList;
    }

    public void UpdateCallDetails(BO.Call call)
    {
        try
        {
            CallManager.ChecksLogicalValidation(call);
            CallManager.ValidateCallDetails(call);
        }
        catch (Exception ex)
        {
            throw new BlGeneralDatabaseException("Error updating the call in the data layer.", ex);
        }

        DO.Call doCall = new DO.Call
        {
            Id = call.Id,
            MyCall = (DO.CallType)call.CallType,
            VerbalDescription = call.Description,
            FullAddressCall = call.FullAddress,
            Latitude = call.Latitude,
            Longitude = call.Longitude,
            OpeningTime = call.OpenTime,
            MaxTimeFinishCalling = call.MaxCompletionTime,
        };

        try
        {
            _dal.Call.Update(doCall);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"No call found with ID {call.Id}.", ex);
        }
        catch (Exception ex)
        {
            throw new BlGeneralDatabaseException("Error updating the call in the data layer.", ex);
        }
    }

};

