using DalApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Helpers;
internal static class VolunteerManager
{
    private static IDal s_dal = Factory.Get; //stage 4

    public static BO.CallInProgress getCallInProgress(DO.Volunteer volunteer)
    {
        var assignment = s_dal.Assignment.ReadAll()
           .Find(a => a.VolunteerId.Equals(volunteer.Id) == true
           && CallManager.GetCallStatus(a.CallId) == BO.CallStatus.InProgress || CallManager.GetCallStatus(a.CallId) == BO.CallStatus.InProgressAtRisk);
        var call = s_dal.Call.ReadAll().Find(c => c.Id == assignment.CallId);
        return new BO.CallInProgress
        {
            Id = assignment.Id,
            CallId = assignment.CallId,
            CallType = (BO.CallType)call.MyCall,
            Description = call.VerbalDescription,
            Address = call.FullAddressCall,
            OpenTime = call.OpeningTime,
            MaxEndTime = call.MaxTimeFinishCalling,
            StartTreatmentTime = (DateTime)assignment.EntryTimeOfTreatment,
            DistanceFromVolunteer = Tools.CalculateDistance(call.Latitude, call.Longitude, (double)volunteer.Latitude, (double)volunteer.Longitude),
            Status = CallManager.GetCallStatus(call.Id)
        };
    }
    public static BO.VolunteerInList convertToVolunteerInList(DO.Volunteer v)
    {
        return new BO.VolunteerInList
        {
            Id = v.Id,
            FullName = v.FullName,
            IsActive = v.IsActive,
            TotalHandledCalls = s_dal.Assignment.ReadAll()
                                   .Where(a => a.VolunteerId == v.Id && a.MyEndingTime == DO.EndingTimeType.TakenCareOf)
                                    .Count(),
            TotalCanceledCalls = s_dal.Assignment.ReadAll()
                                   .Where(a => a.VolunteerId == v.Id && a.MyEndingTime == DO.EndingTimeType.CanceledByVolunteer)
                                    .Count(),
            TotalExpiredCalls = s_dal.Assignment.ReadAll()
                                   .Where(a => a.VolunteerId == v.Id && a.MyEndingTime == DO.EndingTimeType.Expired)
                                    .Count(),
            CallInTreatmentId = getCallInProgress(v).Id,
            CallInTreatmenType = (BO.CallType)getCallInProgress(v).CallType
        };
    }

    /// Validates the given volunteer object to ensure all fields have a correct format.
    public static void ValidateVolunteer(BO.Volunteer volunteer)
    {
        // Validate full name - Cannot be empty and must be at least 2 characters
        if (string.IsNullOrWhiteSpace(volunteer.FullName) || volunteer.FullName.Length < 2)
            throw new ArgumentException("Full name must contain at least two characters.");

        // Validate phone number - Must be 9-10 digits, starting with 0
        if (!Regex.IsMatch(volunteer.Phone, @"^0\d{8,9}$"))
            throw new ArgumentException("Invalid phone number format.");

        // Validate email format
        if (!Regex.IsMatch(volunteer.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            throw new ArgumentException("Invalid email address format.");

        // Validate max distance – Must be a positive number if provided
        if (volunteer.MaxDistance.HasValue && volunteer.MaxDistance <= 0)
            throw new ArgumentException("Max distance must be a positive number.");

        // Validate latitude and longitude ranges
        if (volunteer.Latitude.HasValue && (volunteer.Latitude < -90 || volunteer.Latitude > 90))
            throw new ArgumentException("Latitude must be between -90 and 90.");
        if (volunteer.Longitude.HasValue && (volunteer.Longitude < -180 || volunteer.Longitude > 180))
            throw new ArgumentException("Longitude must be between -180 and 180.");
    }

    public static async Task ChecksLogicalValidation(BO.Volunteer volunteer)
    {
        // Validate ID - Must be a 9-digit number with a valid checksum
        if (!IsValidIsraeliID(volunteer.Id.ToString()))
            throw new ArgumentException("Invalid Israeli ID number.");

        //Check that the address is written correctly
        if (string.IsNullOrWhiteSpace(volunteer.CurrentAddress))
        {
            throw new ArgumentException("Address cannot be empty or whitespace.");
        }

        double latitude, longitude;
        try
        {
            (latitude, longitude) = await Tools.GetCoordinatesFromAddress(volunteer.CurrentAddress);
        }
        catch (Exception ex)
        {
            throw new ArgumentException("Invalid address or not found.", ex);
        }

        volunteer.Latitude = latitude;
        volunteer.Longitude = longitude;
    }

    /// Checks if the given Israeli ID number is valid based on its checksum.
    private static bool IsValidIsraeliID(string id)
    {
        if (!Regex.IsMatch(id, @"^\d{9}$"))
            return false;

        int sum = 0;
        for (int i = 0; i < 9; i++)
        {
            int digit = (id[i] - '0') * ((i % 2) + 1);
            sum += digit > 9 ? digit - 9 : digit;
        }
        return sum % 10 == 0;
    }

}
