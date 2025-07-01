
namespace BlImplementation;

using BO;
using DO;
using Helpers;
using System.Linq;
using System.Linq.Expressions;

internal class VolunteerImplementation : BlApi.IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public void AddVolunteer(BO.Volunteer volunteer)
    {
        try
        {
            VolunteerManager.ChecksLogicalValidation(volunteer);
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
            _dal.Volunteer.Create(doVolunteer);
            VolunteerManager.Observers.NotifyListUpdated(); // הדיווח על שינוי ברשימה
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
        if (_dal.Assignment.ReadAll().Where(a => a.VolunteerId == id).Count() != 0)
            throw new ArgumentException("This volunteer can not be deleted");
        try
        {
            _dal.Volunteer.Delete(id);
            VolunteerManager.Observers.NotifyListUpdated(); // ⬅️ הדיווח על שינוי ברשימה
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

    public BO.Volunteer GetVolunteer(int id)
    {
        try
        {
            var volunteer = _dal.Volunteer.Read(id) ?? throw new BO.BlDoesNotExistException($"Call with ID {id} not found.");
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
                CanceledCalls = _dal.Assignment.ReadAll().Where(a => a.VolunteerId == id && a.MyEndingTime == DO.EndingTimeType.CanceledByVolunteer).Count(),
                ExpiredCalls = _dal.Assignment.ReadAll().Where(a => a.VolunteerId == id && a.MyEndingTime == DO.EndingTimeType.Expired).Count(),
                HandledCalls = _dal.Assignment.ReadAll().Where(a => a.VolunteerId == id && a.MyEndingTime == DO.EndingTimeType.CanceledByVolunteer).ToList().Count(),
                CallInProgress = VolunteerManager.getCallInProgress(_dal.Volunteer.Read(id))
            };
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralDatabaseException("Error getting volunteers details", ex);
        }
    }


    public IEnumerable<BO.VolunteerInList> GetVolunteerList(bool? filter = null, BO.VolunteerSortBy? sort = null)
    {
        var volunteersList = _dal.Volunteer.ReadAll()
                                            .Select(v => VolunteerManager.convertToVolunteerInList(v))
                                            .ToList();
        if (filter != null)
        {
            volunteersList = filter == true
                ? volunteersList.Where(v => v.IsActive).ToList()
                : volunteersList.Where(v => v.IsActive == false).ToList();
        }
        if (sort != null)
        {
            var property = typeof(BO.CallInList).GetProperty(sort.ToString());
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
        var closedStatuses = new DO.EndingTimeType?[]
        {
        DO.EndingTimeType.TakenCareOf,
        DO.EndingTimeType.CanceledByManager,
        DO.EndingTimeType.CanceledByVolunteer,
        DO.EndingTimeType.Expired
        };

        var assignments = _dal.Assignment.ReadAll()
            .Where(a => a.VolunteerId == volunteerId && closedStatuses.Contains(a.MyEndingTime))
            .ToList();

        var result = assignments.Select(a =>
        {
            var call = _dal.Call.Read(a.CallId);
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
        var volunteer = _dal.Volunteer.ReadAll().Find(v => v.Id == id);

        if (volunteer != null)
            if (volunteer.Password.Equals(password) == true)
                return (BO.Position)volunteer.MyPosition;
            else
                throw new BO.BlUnauthorizedAccessException("Wrong Password");
        else
            throw new BO.BlDoesNotExistException($"User with id {id} not found");


    }


    public void UpdateVolunteerDetails(int id, BO.Volunteer volunteer)
    {
        if (volunteer.MyPosition == BO.Position.Manager || volunteer.Id == id)
        {
            try
            {
                VolunteerManager.ValidateVolunteer(volunteer);
                VolunteerManager.ChecksLogicalValidation(volunteer);
                var oldVolunteer = _dal.Volunteer.Read(volunteer.Id);
                if ((BO.Position)oldVolunteer.MyPosition != volunteer.MyPosition && volunteer.MyPosition != BO.Position.Manager)
                    throw new BlUnauthorizedAccessException("Only manager is allowed to change volunteers position");
            }
            catch (Exception ex)
            {
                throw new BlGeneralDatabaseException("Error updating the volunteer in the data layer.", ex);
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
                _dal.Volunteer.Update(doVolunteer);
                VolunteerManager.Observers.NotifyItemUpdated(volunteer.Id); // ⬅️ דיווח על פריט בודד
                VolunteerManager.Observers.NotifyListUpdated(); // ⬅️ דיווח על הרשימה
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
VolunteerManager.Observers.AddListObserver(listObserver); //stage 5
    public void AddObserver(int id, Action observer) =>
VolunteerManager.Observers.AddObserver(id, observer); //stage 5
    public void RemoveObserver(Action listObserver) =>
VolunteerManager.Observers.RemoveListObserver(listObserver); //stage 5
    public void RemoveObserver(int id, Action observer) =>
VolunteerManager.Observers.RemoveObserver(id, observer); //stage 5

}

