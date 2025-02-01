
namespace BlImplementation;

using Helpers;

internal class VolunteerImplementation :BlApi.IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public void AddVolunteer(BO.Volunteer volunteer)
    {
        try
        {
            VolunteerManager.ChecksLogicalValidation(volunteer);
            VolunteerManager.ValidateVolunteer(volunteer);
        }
        catch (Exception ex)
        {
            throw new Exception("Error adding the volunteer in the data layer.", ex);
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
        }
        catch (KeyNotFoundException ex)
        {
            throw new ArgumentException($"No volunteer found with ID {volunteer.Id}.", ex);
        }
        catch (Exception ex)
        {
            throw new Exception("Error adding the volunteer in the data layer.", ex);
        }
    }

    public void DeleteVolunteer(int id)
    {
        if (_dal.Assignment.ReadAll().Where(a => a.VolunteerId == id).Count() != 0)
            throw new ArgumentException("This volunteer can not be delited");
        try
        {
            _dal.Volunteer.Delete(id);
        }
        catch (KeyNotFoundException ex)
        {
            throw new ArgumentException($"Volunteer with ID {id} does not exist.", ex);
        }
        catch (Exception ex)
        {
            throw new Exception("Error deleting the volunteer from the data layer.", ex);
        }
    }

    public BO.Volunteer GetVolunteer(int id)
    {
        try
        {
            var volunteer = _dal.Volunteer.Read(id) ?? throw new KeyNotFoundException($"Call with ID {id} not found.");
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
                canceledCalls = _dal.Assignment.ReadAll().Where(a => a.VolunteerId == id && a.MyEndingTime == DO.EndingTimeType.CanceledByVolunteer).Count(),
                expiredCalls = _dal.Assignment.ReadAll().Where(a => a.VolunteerId == id && a.MyEndingTime == DO.EndingTimeType.Expired).Count(),
                handledCalls = _dal.Assignment.ReadAll().Where(a => a.VolunteerId == id && a.MyEndingTime == DO.EndingTimeType.CanceledByVolunteer).ToList().Count(),
                callInProgress = VolunteerManager.getCallInProgress(_dal.Volunteer.Read(id))
            };
        }
        catch (Exception ex)
        {
            throw new Exception("Error getting volunteers details", ex);
        }
    }

    public IEnumerable<BO.VolunteerInList> GetVolunteerList(bool? filter = null, BO.VolunteerInList? sort = null)
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

    public BO.Position Login(string userName, int password)
    {
        var volunteer = _dal.Volunteer.ReadAll().Find(v => v.FullName == userName);
        if (volunteer != null)
            if (volunteer.Password.Equals(password) == true)
                return (BO.Position)volunteer.MyPosition;
            else
                throw new NotImplementedException("Wrong Password");
        else
            throw new KeyNotFoundException($"User with name {userName} not found");
    }

    public void UpdateVolunteerDetails(int id,BO.Volunteer volunteer)
    {
        if(volunteer.MyPosition==BO.Position.Manager||volunteer.Id==id)
        {
            try
            {
                VolunteerManager.ValidateVolunteer(volunteer);
                VolunteerManager.ChecksLogicalValidation(volunteer);
                var oldVolunteer=_dal.Volunteer.Read(volunteer.Id);
                if ((BO.Position)oldVolunteer.MyPosition != volunteer.MyPosition && volunteer.MyPosition != BO.Position.Manager)
                    throw new ArgumentException("Only manager is allowed to change volunteers position");
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating the volunteer in the data layer.", ex);
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
            }
            catch (KeyNotFoundException ex)
            {
                throw new ArgumentException($"No volunteer found with ID {volunteer.Id}.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating the volunteer in the data layer.", ex);
            }
        }
    }
}

