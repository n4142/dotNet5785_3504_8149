using DalApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Helpers;
internal static class VolunteerManager
{
    private static readonly Random s_rand = new(); // stage 7
    private static IDal s_dal = Factory.Get; //stage 4
    internal static ObserverManager Observers = new(); //stage 5 

    public static BO.CallInProgress? getCallInProgress(DO.Volunteer volunteer)
    {
        lock (AdminManager.BlMutex)
        {
            var assignment = s_dal.Assignment.ReadAll()
               .Find(a => a.VolunteerId == volunteer.Id &&
                   (CallManager.GetCallStatus(a.CallId) == BO.CallStatus.InProgress
                    || CallManager.GetCallStatus(a.CallId) == BO.CallStatus.InProgressAtRisk));

            if (assignment == null)
                return null;

            var call = s_dal.Call.ReadAll().Find(c => c.Id == assignment.CallId);
            if (call == null)
                throw new BO.BlDoesNotExistException($"Call with ID {assignment.CallId} not found.");

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
                DistanceFromVolunteer = Tools.CalculateDistance(call.Latitude, call.Longitude, (double)volunteer.Latitude!, (double)volunteer.Longitude!),
                Status = CallManager.GetCallStatus(call.Id)
            };
        }
    }

    public static BO.VolunteerInList convertToVolunteerInList(DO.Volunteer v)
    {
        lock (AdminManager.BlMutex)
        {
            var callInProgress = getCallInProgress(v);

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
                CallInTreatmentId = callInProgress?.Id ?? 0,
                CallInTreatmenType = callInProgress != null ? (BO.CallType)callInProgress.CallType : default,
            };
        }
    }

    public static void ValidateVolunteer(BO.Volunteer volunteer)
    {
        if (string.IsNullOrWhiteSpace(volunteer.FullName) || volunteer.FullName.Length < 2)
            throw new ArgumentException("Full name must contain at least two characters.");

        if (!Regex.IsMatch(volunteer.Phone, @"^0\d{8,9}$"))
            throw new ArgumentException("Invalid phone number format.");

        if (!Regex.IsMatch(volunteer.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            throw new ArgumentException("Invalid email address format.");

        if (volunteer.MaxDistance.HasValue && volunteer.MaxDistance <= 0)
            throw new ArgumentException("Max distance must be a positive number.");

        if (volunteer.Latitude.HasValue && (volunteer.Latitude < -90 || volunteer.Latitude > 90))
            throw new ArgumentException("Latitude must be between -90 and 90.");
        if (volunteer.Longitude.HasValue && (volunteer.Longitude < -180 || volunteer.Longitude > 180))
            throw new ArgumentException("Longitude must be between -180 and 180.");
    }

    public static async Task ChecksLogicalValidation(BO.Volunteer volunteer)
    {
        if (!IsValidIsraeliID(volunteer.Id.ToString()))
            throw new ArgumentException("Invalid Israeli ID number.");

        if (string.IsNullOrWhiteSpace(volunteer.CurrentAddress))
            throw new ArgumentException("Address cannot be empty or whitespace.");

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

    public static void UpdateVolunteersToInactive()
    {
        List<DO.Volunteer> allVolunteers;
        List<int> updatedIds = new();

        lock (AdminManager.BlMutex)
        {
            allVolunteers = s_dal.Volunteer.ReadAll().ToList();

            foreach (var volunteer in allVolunteers)
            {
                s_dal.Volunteer.Update(volunteer with { IsActive = false });
                updatedIds.Add(volunteer.Id);
            }
        }

        foreach (int id in updatedIds)
            Observers.NotifyItemUpdated(id);
    }

    internal static void SimulateVolunteerActivity()
    {
        Thread.CurrentThread.Name = $"VolunteerSimulator_{Thread.CurrentThread.ManagedThreadId}";

        List<DO.Volunteer> activeVolunteers;
        lock (AdminManager.BlMutex)
        {
            activeVolunteers = s_dal.Volunteer.ReadAll().Where(v => v.IsActive).ToList();
        }

        LinkedList<int> updatedVolunteers = new();

        foreach (var volunteer in activeVolunteers)
        {
            BO.CallInProgress? callInProgress;
            lock (AdminManager.BlMutex)
            {
                callInProgress = getCallInProgress(volunteer);
            }

            if (callInProgress == null)
                continue;

            TimeSpan treatmentDuration = AdminManager.Now - callInProgress.StartTreatmentTime;
            double distanceKm = callInProgress.DistanceFromVolunteer;
            double expectedMinutes = distanceKm * 2 + s_rand.Next(3, 8);

            if (treatmentDuration.TotalMinutes > expectedMinutes)
            {
                try
                {
                    lock (AdminManager.BlMutex)
                    {
                        s_dal.Assignment.Update(new DO.Assignment
                        {
                            Id = callInProgress.Id,
                            CallId = callInProgress.CallId,
                            VolunteerId = volunteer.Id,
                            EntryTimeOfTreatment = callInProgress.StartTreatmentTime,
                            EndingTimeOfTreatment = AdminManager.Now,
                            MyEndingTime = DO.EndingTimeType.TakenCareOf
                        });
                    }
                    updatedVolunteers.AddLast(volunteer.Id);
                }
                catch { }
            }
            else if (s_rand.NextDouble() < 0.1)
            {
                try
                {
                    lock (AdminManager.BlMutex)
                    {
                        s_dal.Assignment.Update(new DO.Assignment
                        {
                            Id = callInProgress.Id,
                            CallId = callInProgress.CallId,
                            VolunteerId = volunteer.Id,
                            EntryTimeOfTreatment = callInProgress.StartTreatmentTime,
                            EndingTimeOfTreatment = AdminManager.Now,
                            MyEndingTime = DO.EndingTimeType.CanceledByVolunteer
                        });
                    }
                    updatedVolunteers.AddLast(volunteer.Id);
                }
                catch { }
            }
        }

        foreach (int id in updatedVolunteers)
        {
            Observers.NotifyItemUpdated(id);
            Observers.NotifyListUpdated();
        }
    }
}
