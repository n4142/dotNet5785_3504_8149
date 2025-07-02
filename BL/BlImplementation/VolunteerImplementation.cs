
//namespace BlImplementation;

//using BO;
//using DO;
//using Helpers;
//using System.Linq;
//using System.Linq.Expressions;

//internal class VolunteerImplementation : BlApi.IVolunteer
//{
//    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

//    public void AddVolunteer(BO.Volunteer volunteer)
//    {

//        AdminManager.ThrowOnSimulatorIsRunning();
//        lock (AdminManager.BlMutex)
//        {
//        try
//        {
//            VolunteerManager.ChecksLogicalValidation(volunteer);
//            VolunteerManager.ValidateVolunteer(volunteer);
//        }
//        catch (DO.DalAlreadyExistException ex)
//        {
//            throw new BO.BlAlreadyExistsException("Error adding the volunteer in the data layer.", ex);
//        }

//            DO.Volunteer doVolunteer = new DO.Volunteer()
//            {
//                Id = volunteer.Id,
//                FullName = volunteer.FullName,
//                Phone = volunteer.Phone,
//                Email = volunteer.Email,
//                IsActive = volunteer.IsActive,
//                MyPosition = (DO.Position)volunteer.MyPosition,
//                Password = volunteer.Password,
//                CurrentAddress = volunteer.CurrentAddress,
//                Latitude = volunteer.Latitude,
//                Longitude = volunteer.Longitude,
//                MaxDistance = volunteer.MaxDistance,
//                MyDistance = (DO.DistanceType)volunteer.MyDistance
//            };
//}
//        try
//        {
//            lock (AdminManager.BlMutex)
//                _dal.Volunteer.Create(doVolunteer);
//            VolunteerManager.Observers.NotifyListUpdated(); // הדיווח על שינוי ברשימה
//        }
//        catch (DO.DalDoesNotExistException ex)
//        {
//            throw new BO.BlDoesNotExistException($"No volunteer found with ID {volunteer.Id}.", ex);
//        }
//        catch (Exception ex)
//        {
//            throw new BlGeneralDatabaseException("Error adding the volunteer in the data layer.", ex);
//        }
//    }


//    public void DeleteVolunteer(int id)
//    {
//        AdminManager.ThrowOnSimulatorIsRunning();
//        if (_dal.Assignment.ReadAll().Where(a => a.VolunteerId == id).Count() != 0)
//            throw new ArgumentException("This volunteer can not be deleted");
//        try
//        {
//            lock (AdminManager.BlMutex)
//                _dal.Volunteer.Delete(id);
//            VolunteerManager.Observers.NotifyListUpdated(); // ⬅️ הדיווח על שינוי ברשימה
//        }
//        catch (DO.DalDoesNotExistException ex)
//        {
//            throw new BO.BlDoesNotExistException($"Volunteer with ID {id} does not exist.", ex);
//        }
//        catch (Exception ex)
//        {
//            throw new BlGeneralDatabaseException("Error deleting the volunteer from the data layer.", ex);
//        }
//    }

//    public BO.Volunteer GetVolunteer(int id)
//    {
//  {
//            lock (AdminManager.BlMutex) 
//                try
//            {

//                var volunteer = _dal.Volunteer.Read(id) ?? throw new BO.BlDoesNotExistException($"Call with ID {id} not found.");
//                return new BO.Volunteer
//                {
//                    Id = volunteer.Id,
//                    FullName = volunteer.FullName,
//                    Phone = volunteer.Phone,
//                    Email = volunteer.Email,
//                    Password = volunteer.Password,
//                    CurrentAddress = volunteer.CurrentAddress,
//                    Longitude = volunteer.Longitude,
//                    Latitude = volunteer.Latitude,
//                    MyPosition = (BO.Position)volunteer.MyPosition,
//                    IsActive = volunteer.IsActive,
//                    MaxDistance = volunteer.MaxDistance,
//                    MyDistance = (BO.DistanceType)volunteer.MyDistance,
//                    CanceledCalls = _dal.Assignment.ReadAll().Where(a => a.VolunteerId == id && a.MyEndingTime == DO.EndingTimeType.CanceledByVolunteer).Count(),
//                    ExpiredCalls = _dal.Assignment.ReadAll().Where(a => a.VolunteerId == id && a.MyEndingTime == DO.EndingTimeType.Expired).Count(),
//                    HandledCalls = _dal.Assignment.ReadAll().Where(a => a.VolunteerId == id && a.MyEndingTime == DO.EndingTimeType.CanceledByVolunteer).ToList().Count(),
//                    CallInProgress = VolunteerManager.getCallInProgress(_dal.Volunteer.Read(id))
//                };
//            }
//            catch (Exception ex)
//            {
//                throw new BO.BlGeneralDatabaseException("Error getting volunteers details", ex);
//            }
//        }
//    }


//    public IEnumerable<BO.VolunteerInList> GetVolunteerList(bool? filter = null, BO.VolunteerSortBy? sort = null)
//    {
//        lock (AdminManager.BlMutex)
//        { var volunteersList = _dal.Volunteer.ReadAll()
//                                            .Select(v => VolunteerManager.convertToVolunteerInList(v))
//                                            .ToList();
//        if (filter != null)
//        {
//            volunteersList = filter == true
//                ? volunteersList.Where(v => v.IsActive).ToList()
//                : volunteersList.Where(v => v.IsActive == false).ToList();
//        }
//        if (sort != null)
//        {
//            var property = typeof(BO.CallInList).GetProperty(sort.ToString());
//            if (property != null)
//                volunteersList = volunteersList.OrderBy(c => property.GetValue(c)).ToList();
//        }
//        else
//        {
//            volunteersList = volunteersList.OrderBy(c => c.Id).ToList();
//        }
//            return volunteersList;
//        }
//    }
//    public IEnumerable<ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId)
//    {
//        {var closedStatuses = new DO.EndingTimeType?[]
//        {
//        DO.EndingTimeType.TakenCareOf,
//        DO.EndingTimeType.CanceledByManager,
//        DO.EndingTimeType.CanceledByVolunteer,
//        DO.EndingTimeType.Expired
//        };

//        var assignments = _dal.Assignment.ReadAll()
//            .Where(a => a.VolunteerId == volunteerId && closedStatuses.Contains(a.MyEndingTime))
//            .ToList();

//        var result = assignments.Select(a =>
//        {
//            var call = _dal.Call.Read(a.CallId);
//            if (call == null) return null;

//            return new ClosedCallInList
//            {
//                Id = a.Id,
//                CallType = (BO.CallType)call.MyCall,
//                Address = call.FullAddressCall,
//                OpenTime = call.OpeningTime,
//                StartTime = a.EntryTimeOfTreatment ?? throw new InvalidOperationException("EntryTimeOfTreatment cannot be null."),
//                EndTime = a.EndingTimeOfTreatment,
//                EndStatus = (BO.EndingTimeType)a.MyEndingTime
//            };
//        }).Where(c => c != null).ToList();

//        return result!;}
//    }

//    public BO.Position Login(int id, string password)
//    {
//        var volunteer = _dal.Volunteer.ReadAll().Find(v => v.Id == id);

//        if (volunteer != null)
//            if (volunteer.Password.Equals(password) == true)
//                return (BO.Position)volunteer.MyPosition;
//            else
//                throw new BO.BlUnauthorizedAccessException("Wrong Password");
//        else
//            throw new BO.BlDoesNotExistException($"User with id {id} not found");


//    }


//    public void UpdateVolunteerDetails(int id, BO.Volunteer volunteer)
//    {
//        AdminManager.ThrowOnSimulatorIsRunning();
//        if (volunteer.MyPosition == BO.Position.Manager || volunteer.Id == id)
//        {
//            try
//            {
//                VolunteerManager.ValidateVolunteer(volunteer);
//                VolunteerManager.ChecksLogicalValidation(volunteer);
//                var oldVolunteer = _dal.Volunteer.Read(volunteer.Id);
//                if ((BO.Position)oldVolunteer.MyPosition != volunteer.MyPosition && volunteer.MyPosition != BO.Position.Manager)
//                    throw new BlUnauthorizedAccessException("Only manager is allowed to change volunteers position");
//            }
//            catch (Exception ex)
//            {
//                throw new BlGeneralDatabaseException("Error updating the volunteer in the data layer.", ex);
//            }

//            DO.Volunteer doVolunteer = new DO.Volunteer()
//            {
//                Id = volunteer.Id,
//                FullName = volunteer.FullName,
//                Phone = volunteer.Phone,
//                Email = volunteer.Email,
//                IsActive = volunteer.IsActive,
//                MyPosition = (DO.Position)volunteer.MyPosition,
//                Password = volunteer.Password,
//                CurrentAddress = volunteer.CurrentAddress,
//                Latitude = volunteer.Latitude,
//                Longitude = volunteer.Longitude,
//                MaxDistance = volunteer.MaxDistance,
//                MyDistance = (DO.DistanceType)volunteer.MyDistance
//            };
//            try
//            {
//                lock (AdminManager.BlMutex)
//                    _dal.Volunteer.Update(doVolunteer);
//                VolunteerManager.Observers.NotifyItemUpdated(volunteer.Id); // ⬅️ דיווח על פריט בודד
//                VolunteerManager.Observers.NotifyListUpdated(); // ⬅️ דיווח על הרשימה
//            }
//            catch (DO.DalDoesNotExistException ex)
//            {
//                throw new BO.BlDoesNotExistException($"No volunteer found with ID {volunteer.Id}.", ex);
//            }
//            catch (Exception ex)
//            {
//                throw new BlGeneralDatabaseException("Error updating the volunteer in the data layer.", ex);
//            }
//        }
//    }

//    public void AddObserver(Action listObserver) =>
//VolunteerManager.Observers.AddListObserver(listObserver); //stage 5
//    public void AddObserver(int id, Action observer) =>
//VolunteerManager.Observers.AddObserver(id, observer); //stage 5
//    public void RemoveObserver(Action listObserver) =>
//VolunteerManager.Observers.RemoveListObserver(listObserver); //stage 5
//    public void RemoveObserver(int id, Action observer) =>
//VolunteerManager.Observers.RemoveObserver(id, observer); //stage 5

//}
namespace BlImplementation;
using BlApi;
using BO;
using DalApi;
using DO;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using static Helpers.Tools;

internal class VolunteerImplementation : BlApi.IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public void AddVolunteer(BO.Volunteer volunteer)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        try
        {
            VolunteerManager.ChecksLogicalValidation(volunteer).Wait();
            VolunteerManager.ValidateVolunteer(volunteer);
        }
        catch (DO.DalAlreadyExistException ex)
        {
            throw new BO.BlAlreadyExistsException("Error adding the volunteer in the data layer.", ex);
        }

        DO.Volunteer doVolunteer = new DO.Volunteer()
        {
            Id = volunteer.Id,
            FullName = volunteer.FullName,
            Phone = volunteer.Phone,
            Email = volunteer.Email,
            IsActive = volunteer.IsActive,
            MyPosition = (DO.Position)volunteer.MyPosition,
            Password = volunteer.Password,
            CurrentAddress = volunteer.CurrentAddress,
            Latitude = volunteer.Latitude,
            Longitude = volunteer.Longitude,
            MaxDistance = volunteer.MaxDistance,
            MyDistance = (DO.DistanceType)volunteer.MyDistance
        };

        try
        {
            lock (AdminManager.BlMutex)
                _dal.Volunteer.Create(doVolunteer);
            VolunteerManager.Observers.NotifyListUpdated();
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"No volunteer found with ID {volunteer.Id}.", ex);
        }
        catch (Exception ex)
        {
            throw new BlGeneralDatabaseException("Error adding the volunteer in the data layer.", ex);
        }
    }

    public void DeleteVolunteer(int id)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        lock (AdminManager.BlMutex)
        {
            if (_dal.Assignment.ReadAll().Any(a => a.VolunteerId == id))
                throw new ArgumentException("This volunteer can not be deleted");

            try
            {
                _dal.Volunteer.Delete(id);
            }
            catch (DO.DalDoesNotExistException ex)
            {
                throw new BO.BlDoesNotExistException($"Volunteer with ID {id} does not exist.", ex);
            }
            catch (Exception ex)
            {
                throw new BlGeneralDatabaseException("Error deleting the volunteer from the data layer.", ex);
            }
        }
        VolunteerManager.Observers.NotifyListUpdated();
    }

    public BO.Volunteer GetVolunteer(int id)
    {
        try
        {
            DO.Volunteer volunteer;
            int canceled, expired, handled;
            DO.Volunteer? forCall;

            lock (AdminManager.BlMutex)
            {
                volunteer = _dal.Volunteer.Read(id)
                    ?? throw new BO.BlDoesNotExistException($"Volunteer with ID {id} not found.");

                canceled = _dal.Assignment.ReadAll().Count(a => a.VolunteerId == id && a.MyEndingTime == DO.EndingTimeType.CanceledByVolunteer);
                expired = _dal.Assignment.ReadAll().Count(a => a.VolunteerId == id && a.MyEndingTime == DO.EndingTimeType.Expired);
                handled = _dal.Assignment.ReadAll().Count(a => a.VolunteerId == id && a.MyEndingTime == DO.EndingTimeType.TakenCareOf);

                forCall = _dal.Volunteer.Read(id);
            }

            return new BO.Volunteer
            {
                Id = volunteer.Id,
                FullName = volunteer.FullName,
                Phone = volunteer.Phone,
                Email = volunteer.Email,
                Password = volunteer.Password,
                CurrentAddress = volunteer.CurrentAddress,
                Longitude = volunteer.Longitude,
                Latitude = volunteer.Latitude,
                MyPosition = (BO.Position)volunteer.MyPosition,
                IsActive = volunteer.IsActive,
                MaxDistance = volunteer.MaxDistance,
                MyDistance = (BO.DistanceType)volunteer.MyDistance,
                CanceledCalls = canceled,
                ExpiredCalls = expired,
                HandledCalls = handled,
                CallInProgress = VolunteerManager.getCallInProgress(forCall)
            };
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralDatabaseException("Error getting volunteers details", ex);
        }
    }

    public IEnumerable<BO.VolunteerInList> GetVolunteerList(bool? filter = null, BO.VolunteerSortBy? sort = null)
    {
        List<DO.Volunteer> rawList;
        lock (AdminManager.BlMutex)
            rawList = _dal.Volunteer.ReadAll().ToList();

        var volunteersList = rawList.Select(v => VolunteerManager.convertToVolunteerInList(v)).ToList();

        if (filter != null)
        {
            volunteersList = filter == true
                ? volunteersList.Where(v => v.IsActive).ToList()
                : volunteersList.Where(v => !v.IsActive).ToList();
        }

        if (sort != null)
        {
            var property = typeof(BO.VolunteerInList).GetProperty(sort.ToString());
            if (property != null)
                volunteersList = volunteersList.OrderBy(c => property.GetValue(c)).ToList();
        }
        else
        {
            volunteersList = volunteersList.OrderBy(c => c.Id).ToList();
        }

        return volunteersList;
    }

    public IEnumerable<ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId)
    {
        List<DO.Assignment> assignments;
        lock (AdminManager.BlMutex)
        {
            var closedStatuses = new DO.EndingTimeType?[]
            {
                DO.EndingTimeType.TakenCareOf,
                DO.EndingTimeType.CanceledByManager,
                DO.EndingTimeType.CanceledByVolunteer,
                DO.EndingTimeType.Expired
            };

            assignments = _dal.Assignment.ReadAll()
                .Where(a => a.VolunteerId == volunteerId && closedStatuses.Contains(a.MyEndingTime))
                .ToList();
        }

        var result = assignments.Select(a =>
        {
            DO.Call call;
            lock (AdminManager.BlMutex)
                call = _dal.Call.Read(a.CallId);

            if (call == null) return null;

            return new ClosedCallInList
            {
                Id = a.Id,
                CallType = (BO.CallType)call.MyCall,
                Address = call.FullAddressCall,
                OpenTime = call.OpeningTime,
                StartTime = a.EntryTimeOfTreatment ?? throw new InvalidOperationException("EntryTimeOfTreatment cannot be null."),
                EndTime = a.EndingTimeOfTreatment,
                EndStatus = (BO.EndingTimeType)a.MyEndingTime
            };
        }).Where(c => c != null).ToList();

        return result!;
    }

    public BO.Position Login(int id, string password)
    {
        DO.Volunteer? volunteer;
        lock (AdminManager.BlMutex)
            volunteer = _dal.Volunteer.ReadAll().Find(v => v.Id == id);

        if (volunteer != null)
        {
            if (volunteer.Password == password)
                return (BO.Position)volunteer.MyPosition;
            else
                throw new BO.BlUnauthorizedAccessException("Wrong Password");
        }
        else
        {
            throw new BO.BlDoesNotExistException($"User with id {id} not found");
        }
    }

    public void UpdateVolunteerDetails(int id, BO.Volunteer volunteer)
    {
        AdminManager.ThrowOnSimulatorIsRunning();

        if (volunteer.MyPosition == BO.Position.Manager || volunteer.Id == id)
        {
            try
            {
                VolunteerManager.ValidateVolunteer(volunteer);
                VolunteerManager.ChecksLogicalValidation(volunteer).Wait();

                DO.Volunteer oldVolunteer;
                lock (AdminManager.BlMutex)
                    oldVolunteer = _dal.Volunteer.Read(volunteer.Id);

                if ((BO.Position)oldVolunteer.MyPosition != volunteer.MyPosition && volunteer.MyPosition != BO.Position.Manager)
                    throw new BlUnauthorizedAccessException("Only manager is allowed to change volunteers position");

                DO.Volunteer doVolunteer = new DO.Volunteer()
                {
                    Id = volunteer.Id,
                    FullName = volunteer.FullName,
                    Phone = volunteer.Phone,
                    Email = volunteer.Email,
                    IsActive = volunteer.IsActive,
                    MyPosition = (DO.Position)volunteer.MyPosition,
                    Password = volunteer.Password,
                    CurrentAddress = volunteer.CurrentAddress,
                    Latitude = volunteer.Latitude,
                    Longitude = volunteer.Longitude,
                    MaxDistance = volunteer.MaxDistance,
                    MyDistance = (DO.DistanceType)volunteer.MyDistance
                };

                lock (AdminManager.BlMutex)
                    _dal.Volunteer.Update(doVolunteer);

                VolunteerManager.Observers.NotifyItemUpdated(volunteer.Id);
                VolunteerManager.Observers.NotifyListUpdated();
            }
            catch (DO.DalDoesNotExistException ex)
            {
                throw new BO.BlDoesNotExistException($"No volunteer found with ID {volunteer.Id}.", ex);
            }
            catch (Exception ex)
            {
                throw new BlGeneralDatabaseException("Error updating the volunteer in the data layer.", ex);
            }
        }
    }

    public void AddObserver(Action listObserver) =>
        VolunteerManager.Observers.AddListObserver(listObserver);
    public void AddObserver(int id, Action observer) =>
        VolunteerManager.Observers.AddObserver(id, observer);
    public void RemoveObserver(Action listObserver) =>
        VolunteerManager.Observers.RemoveListObserver(listObserver);
    public void RemoveObserver(int id, Action observer) =>
        VolunteerManager.Observers.RemoveObserver(id, observer);
}

