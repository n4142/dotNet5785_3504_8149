
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

    public async void AddCall(BO.Call call)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        try
        {
           await CallManager.ChecksLogicalValidation(call);
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
            Latitude = call.Latitude ?? throw new ArgumentNullException(nameof(call.Latitude), "Latitude cannot be null"),
            Longitude = call.Longitude ?? throw new ArgumentNullException(nameof(call.Longitude), "Longitude cannot be null"),
            OpeningTime = call.OpenTime,
            MaxTimeFinishCalling = call.MaxCompletionTime ?? throw new ArgumentNullException(nameof(call.MaxCompletionTime), "MaxCompletionTime cannot be null"),
        };
        try
        {
            lock (AdminManager.BlMutex)
                _dal.Call.Create(doCall);
            CallManager.Observers.NotifyListUpdated(); // stage 5
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

    public void UpdateCallsToExpired()
    {
        List<DO.Call> allCalls;
        List<int> updatedCallIds = new();

        // שלב 1: קבלת עותק קונקרטי של כל הקריאות
        lock (AdminManager.BlMutex)
        {
            allCalls = _dal.Call.ReadAll().ToList();
        }

        // שלב 2: עדכון הקריאות שפגו תוקפן והוספת ה-Id שלהן לרשימה
        foreach (var call in allCalls)
        {
            if (CallManager.GetCallStatus(call.Id) == BO.CallStatus.Open &&
                AdminManager.Now > call.MaxTimeFinishCalling)
            {
                List<DO.Assignment> assignments;
                lock (AdminManager.BlMutex)
                {
                    assignments = _dal.Assignment.ReadAll()
                        .Where(a => a.CallId == call.Id)
                        .ToList();

                    foreach (var assignment in assignments)
                    {
                        if (assignment.MyEndingTime == null)
                        {
                            var updatedAssignment = assignment with
                            {
                                EndingTimeOfTreatment = AdminManager.Now,
                                MyEndingTime = DO.EndingTimeType.Expired
                            };
                            _dal.Assignment.Update(updatedAssignment);
                        }
                    }
                }

                updatedCallIds.Add(call.Id);
            }
        }

        // שלב 3: הוצאת הודעות למשקיפים מחוץ ל-lock
        foreach (var callId in updatedCallIds)
        {
            CallManager.Observers.NotifyItemUpdated(callId);
            CallManager.Observers.NotifyListUpdated();
        }
    }

    public void AssignCallToVolunteer(int volunteerId, int callId)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        var callStatus = CallManager.GetCallStatus(callId);

        bool hasOpenAssignment;
        lock (AdminManager.BlMutex)
            hasOpenAssignment = _dal.Assignment.ReadAll()
                .Any(a => a.CallId == callId && a.MyEndingTime == DO.EndingTimeType.TakenCareOf);

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
            EntryTimeOfTreatment = AdminManager.Now,
            EndingTimeOfTreatment = null,
            MyEndingTime = null
        };
        try
        {
            lock (AdminManager.BlMutex)
                _dal.Assignment.Create(doAssign);

            CallManager.Observers.NotifyListUpdated(); // Stage 5
            CallManager.Observers.NotifyItemUpdated(callId); // Stage 5
            VolunteerManager.Observers.NotifyItemUpdated(volunteerId);// Stage 5
        }
        catch (Exception ex)
        {
            throw new BlGeneralDatabaseException("Error adding the assignment in the data layer.", ex);
        }
    }

    public void CancelCallTreatment(bool isManager, int volunteerId, int assignmentId)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        DO.Volunteer volunteer;
        DO.Assignment assignment;
        lock (AdminManager.BlMutex)
        {
            volunteer = _dal.Volunteer.Read(volunteerId);
            assignment = _dal.Assignment.Read(assignmentId);
        }

        if (assignment.VolunteerId != volunteerId && !isManager)
            throw new BlUnauthorizedAccessException("This volunteer is not allowed canceling treatment");

        if (assignment.EndingTimeOfTreatment != null || assignment.MyEndingTime == DO.EndingTimeType.Expired || assignment.MyEndingTime == DO.EndingTimeType.CanceledByManager)
            throw new BlInvalidTimeUnitException("It is not possible to report the end of treatment because the offer is not open");

        DO.Assignment updatedAssignment = new DO.Assignment
        {
            Id = assignment.Id,
            CallId = assignment.CallId,
            VolunteerId = volunteerId,
            EntryTimeOfTreatment = assignment.EntryTimeOfTreatment,
            EndingTimeOfTreatment = DateTime.Now,
            MyEndingTime = volunteer.MyPosition == DO.Position.Manager
         ? DO.EndingTimeType.CanceledByManager
         : DO.EndingTimeType.CanceledByVolunteer
        };

        try
        {
            lock (AdminManager.BlMutex)
                _dal.Assignment.Update(updatedAssignment);
            CallManager.Observers.NotifyItemUpdated(assignment.CallId); // Stage 5
            CallManager.Observers.NotifyListUpdated(); // Stage 5
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
        DO.Assignment assignment;
        lock (AdminManager.BlMutex)
            assignment = _dal.Assignment.Read(assignmentId);

        if (assignment.VolunteerId != volunteerId)
            throw new BlUnauthorizedAccessException("This volunteer is not allowed because the assignment is not in his name");

        if (assignment.MyEndingTime == DO.EndingTimeType.Expired || assignment.MyEndingTime == DO.EndingTimeType.CanceledByManager)
            throw new BlInvalidTimeUnitException("It is not possible to report the end of treatment because the offer is not open");
        DO.Assignment updatedAssignment = new DO.Assignment
        {
            Id = assignment.Id,
            CallId = assignment.CallId,
            VolunteerId = volunteerId,
            EntryTimeOfTreatment = assignment.EntryTimeOfTreatment,
            EndingTimeOfTreatment = AdminManager.Now,
            MyEndingTime = DO.EndingTimeType.TakenCareOf
        };

        try
        {
            lock (AdminManager.BlMutex)
                _dal.Assignment.Update(updatedAssignment);
            CallManager.Observers.NotifyListUpdated(); // Stage 5
            CallManager.Observers.NotifyItemUpdated(updatedAssignment.CallId); // Stage 5
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
        AdminManager.ThrowOnSimulatorIsRunning();
        DO.Call call;
        lock (AdminManager.BlMutex)
            call = _dal.Call.Read(callId) ?? throw new BO.BlDoesNotExistException($"No call found with ID {callId}.");

        bool hasAssignments;
        lock (AdminManager.BlMutex)
            hasAssignments = _dal.Assignment.ReadAll().Any(a => a.CallId == callId);
        BO.CallStatus type = CallManager.GetCallStatus(call.Id);
        if (CallManager.GetCallStatus(call.Id) == BO.CallStatus.Closed || CallManager.GetCallStatus(call.Id) == BO.CallStatus.Expired || hasAssignments)
            throw new BlInvalidTimeUnitException("Cannot delete call. It is either not open or has been assigned to a volunteer.");

        try
        {
            lock (AdminManager.BlMutex)
                _dal.Call.Delete(callId);
            CallManager.Observers.NotifyListUpdated(); //stage 5 
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

    public void UpdateCallDetails(BO.Call call)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
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
            Latitude = call.Latitude ?? throw new ArgumentNullException(nameof(call.Latitude), "Latitude cannot be null"),
            Longitude = call.Longitude ?? throw new ArgumentNullException(nameof(call.Longitude), "Longitude cannot be null"),
            OpeningTime = call.OpenTime,
            MaxTimeFinishCalling = call.MaxCompletionTime,
        };

        try
        {
            lock (AdminManager.BlMutex)
                _dal.Call.Update(doCall);
            CallManager.Observers.NotifyItemUpdated(call.Id);//stage 5 
            CallManager.Observers.NotifyListUpdated();//stage 5 
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

    public BO.Call GetCallDetails(int callId)
    {
        DO.Call call;
        lock (AdminManager.BlMutex)
            call = _dal.Call.Read(callId) ?? throw new BO.BlDoesNotExistException($"Call with ID {callId} not found.");

        List<BO.CallAssignInList> assignmentsList;
        lock (AdminManager.BlMutex)
        {
            assignmentsList = _dal.Assignment.ReadAll()
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
        }

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
        List<DO.Call> allCalls;
        lock (AdminManager.BlMutex)
            allCalls = _dal.Call.ReadAll().ToList();

        IEnumerable<CallInList> callList = allCalls
            .Select(c => CallManager.ConvertToBOCallInList(c))
            .ToList();
        foreach (var call in callList)
        {
            call.Status = CallManager.GetCallStatus(call.CallId);
        }
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
        List<DO.Call> allCalls;
        lock (AdminManager.BlMutex)
            allCalls = _dal.Call.ReadAll().ToList();

        var groupedCalls = allCalls.GroupBy(c => CallManager.GetCallStatus(c.Id))
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
        List<DO.Call> allCalls;
        List<DO.Assignment> allAssignments;
        lock (AdminManager.BlMutex)
        {
            allCalls = _dal.Call.ReadAll().ToList();
            allAssignments = _dal.Assignment.ReadAll().ToList();
        }

        var volunteersClosedCalls = allCalls
            .Where(c => CallManager.GetCallStatus(c.Id).ToString() == "Closed" &&
                        allAssignments.Any(a => a.VolunteerId == volunteerId && a.CallId == c.Id))
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

        IEnumerable<BO.ClosedCallInList> calls = volunteersClosedCalls.Select(c => new ClosedCallInList
        {
            Id = c.Id,
            CallType = (BO.CallType)c.MyCall,
            Address = c.FullAddressCall,
            OpenTime = c.OpeningTime,
            StartTime = (DateTime)allAssignments.Find(a => c.Id == a.CallId).EntryTimeOfTreatment,
            EndTime = (DateTime)allAssignments.Find(a => c.Id == a.CallId).EndingTimeOfTreatment,
            EndStatus = (BO.EndingTimeType)allAssignments.Find(a => c.Id == a.CallId).MyEndingTime,
        });
        return calls;
    }

    //public IEnumerable<OpenCallInList> GetOpenCallsForVolunteerSelection(int volunteerId, BO.CallType? callTypeFilter = null, OpenCallInList? sortField = null)
    //{
    //    DO.Volunteer volunteer;
    //    List<DO.Call> calls;
    //    lock (AdminManager.BlMutex)
    //    {
    //        volunteer = _dal.Volunteer.Read(volunteerId);
    //        calls = _dal.Call.ReadAll()
    //            .Where(c => CallManager.GetCallStatus(c.Id).ToString() == "Open" || CallManager.GetCallStatus(c.Id).ToString() == "OpenAtRisk")
    //            .ToList();
    //    }

    //    if (callTypeFilter != null)
    //        calls = calls.Where(c => (BO.CallType)c.MyCall == callTypeFilter).ToList();

    //    if (sortField != null)
    //    {
    //        var property = typeof(BO.CallInList).GetProperty(sortField.ToString());
    //        if (property != null)
    //        {
    //            calls = calls.OrderBy(c => property.GetValue(c)).ToList();
    //        }
    //    }
    //    else
    //    {
    //        calls = calls.OrderBy(c => c.Id).ToList();
    //    }

    //    IEnumerable<OpenCallInList> openCallsList = calls.Select(c => new OpenCallInList
    //    {
    //        Id = c.Id,
    //        CallType = (BO.CallType)c.MyCall,
    //        Description = c.VerbalDescription,
    //        Address = c.FullAddressCall,
    //        OpenTime = c.OpeningTime,
    //        MaxEndTime = c.MaxTimeFinishCalling,
    //        DistanceFromVolunteer = CalculateDistance(volunteer.Latitude.Value, volunteer.Longitude.Value, c.Latitude, c.Longitude)
    //    });
    //    return openCallsList;
    //}

    public IEnumerable<OpenCallInList> GetOpenCallsForVolunteerSelection( int volunteerId,BO.CallType? callTypeFilter = null, OpenCallInList? sortField = null)
    {
        DO.Volunteer volunteer;
        List<DO.Call> calls;

        // שליפה תחת נעילה
        lock (AdminManager.BlMutex)
        {
            volunteer = _dal.Volunteer.Read(volunteerId);
            calls = _dal.Call.ReadAll()
                .Where(c =>
                {
                    var status = CallManager.GetCallStatus(c.Id);
                    return status == BO.CallStatus.Open || status == BO.CallStatus.OpenAtRisk;
                })
                .ToList();
        }

        // סינון לפי סוג קריאה
        if (callTypeFilter != null)
            calls = calls.Where(c => (BO.CallType)c.MyCall == callTypeFilter).ToList();

        // סינון לפי מרחק
        if (volunteer.Latitude != null && volunteer.Longitude != null && volunteer.MaxDistance != null)
        {
            calls = calls.Where(c =>
                Tools.CalculateDistance(
                    c.Latitude, c.Longitude,
                    volunteer.Latitude.Value, volunteer.Longitude.Value)
                <= volunteer.MaxDistance.Value
            ).ToList();
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


    private async Task CompleteCallWithCoordinatesAsync(BO.Call call)
    {
        try
        {
            var coordinates = await Tools.GetCoordinatesFromAddress(call.FullAddress);
            var latitude = coordinates.Latitude;
            var longitude = coordinates.Longitude;

            call.Latitude = latitude;
            call.Longitude = longitude;

            DO.Call doCall = new DO.Call
            {
                Id = call.Id,
                MyCall = (DO.CallType)call.CallType,
                VerbalDescription = call.Description,
                FullAddressCall = call.FullAddress,
                Latitude = call.Latitude ?? throw new ArgumentNullException(nameof(call.Latitude), "Latitude cannot be null"),
                Longitude = call.Longitude ?? throw new ArgumentNullException(nameof(call.Longitude), "Longitude cannot be null"),
                OpeningTime = call.OpenTime,
                MaxTimeFinishCalling = call.MaxCompletionTime,
            };

            lock (AdminManager.BlMutex)
                _dal.Call.Update(doCall);

            CallManager.Observers.NotifyItemUpdated(call.Id);
        }
        catch (Exception ex)
        {
            // Optional: Log or handle the exception
        }
    }


    public void AddObserver(Action listObserver) =>
        CallManager.Observers.AddListObserver(listObserver); //stage 5
    public void AddObserver(int id, Action observer) =>
        CallManager.Observers.AddObserver(id, observer); //stage 5
    public void RemoveObserver(Action listObserver) =>
        CallManager.Observers.RemoveListObserver(listObserver); //stage 5
    public void RemoveObserver(int id, Action observer) =>
        CallManager.Observers.RemoveObserver(id, observer); //stage 5
};
