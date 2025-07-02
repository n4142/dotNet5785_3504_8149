using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Helpers.Tools;

namespace Helpers
{
    internal static class CallManager
    {
        private static IDal s_dal = Factory.Get; //stage 4
        internal static ObserverManager Observers = new(); //stage 5

        public static BO.CallStatus GetCallStatus(int callId)
        {
            lock (AdminManager.BlMutex)
            {
                var call = s_dal.Call.Read(callId) ?? throw new KeyNotFoundException($"Call with ID {callId} not found.");
                var assignment = s_dal.Assignment.ReadAll().Find(a => a.CallId == callId);
                TimeSpan? timeLeft = call.MaxTimeFinishCalling - AdminManager.Now;

                if (call.MaxTimeFinishCalling.HasValue && timeLeft < TimeSpan.Zero)
                    return BO.CallStatus.Expired;
                if (assignment != null && timeLeft <= s_dal.Config.RiskRange)
                    return BO.CallStatus.InProgressAtRisk;
                if (assignment != null)
                    return BO.CallStatus.InProgress;
                if (timeLeft <= s_dal.Config.RiskRange)
                    return BO.CallStatus.OpenAtRisk;

                return BO.CallStatus.Open;
            }
        }

        internal static BO.CallInList ConvertToBOCallInList(DO.Call call)
        {
            List<DO.Assignment> assignments;
            DO.Assignment? lastAssignment;
            DO.Volunteer? volunteer;
            lock (AdminManager.BlMutex)
            {
                assignments = s_dal.Assignment.ReadAll().Where(a => a.CallId == call.Id).ToList();
                lastAssignment = assignments
                    .OrderByDescending(a => a.EntryTimeOfTreatment)
                    .FirstOrDefault();
                volunteer = s_dal.Volunteer.ReadAll().Find(v => v.Id == lastAssignment?.VolunteerId);
            }

            return new BO.CallInList
            {
                Id = lastAssignment == null ? null : lastAssignment.Id,
                CallId = call.Id,
                CallType = (BO.CallType)call.MyCall,
                OpenTime = call.OpeningTime,
                TimeRemaining = call.MaxTimeFinishCalling - AdminManager.Now,
                LastVolunteerName = volunteer?.FullName ?? string.Empty,
                Status = GetCallStatus(call.Id),
                TotalAssignments = assignments.Count,
                TotalCompletionTime = lastAssignment?.MyEndingTime == EndingTimeType.TakenCareOf
                    ? lastAssignment.EndingTimeOfTreatment - lastAssignment.EntryTimeOfTreatment
                    : null
            };
        }

        internal static async Task ChecksLogicalValidation(BO.Call call)
        {
            if (call.MaxCompletionTime.HasValue && call.MaxCompletionTime < call.OpenTime)
                throw new ArgumentException("Max completion time must be after the open time.");

            if (string.IsNullOrWhiteSpace(call.FullAddress))
                throw new ArgumentException("Address cannot be empty or whitespace.");

            double latitude, longitude;
            try
            {
                (latitude, longitude) = await GetCoordinatesFromAddress(call.FullAddress);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Invalid address or not found.", ex);
            }

            call.Latitude = latitude;
            call.Longitude = longitude;
        }

        internal static void ValidateCallDetails(BO.Call call)
        {
            if (string.IsNullOrWhiteSpace(call.FullAddress) ||
                !(call.Latitude >= -90 && call.Latitude <= 90 &&
                  call.Longitude >= -180 && call.Longitude <= 180))
                throw new ArgumentException("The address must be valid with latitude and longitude.");

            if (!Enum.IsDefined(typeof(BO.CallType), call.CallType))
                throw new ArgumentException("Invalid call type.");

            if (!string.IsNullOrEmpty(call.Description) && call.Description.Length > 500)
                throw new ArgumentException("Description is too long (maximum 500 characters).");

            if (call.CallAssignments != null && call.CallAssignments.Any(a => a.EntryTime < call.OpenTime))
                throw new ArgumentException("Assignments cannot start before the call's open time.");
        }

        internal static void updateExpiredCalls()
        {
            List<DO.Call> expiredCalls;
            List<DO.Assignment> assignments;
            List<int> updatedAssignmentIds = new();
            bool newAssignmentsCreated = false;

            lock (AdminManager.BlMutex)
            {
                expiredCalls = s_dal.Call.ReadAll()
                    .Where(c => GetCallStatus(c.Id) == BO.CallStatus.Expired)
                    .ToList();

                assignments = s_dal.Assignment.ReadAll();

                foreach (var call in expiredCalls)
                {
                    var assignment = assignments.Find(a => a.CallId == call.Id);

                    if (assignment != null)
                    {
                        s_dal.Assignment.Update(new DO.Assignment
                        {
                            Id = assignment.Id,
                            CallId = assignment.CallId,
                            VolunteerId = assignment.VolunteerId,
                            EntryTimeOfTreatment = assignment.EntryTimeOfTreatment,
                            EndingTimeOfTreatment = AdminManager.Now,
                            MyEndingTime = DO.EndingTimeType.Expired
                        });

                        updatedAssignmentIds.Add(assignment.Id);
                    }
                    else
                    {
                        s_dal.Assignment.Create(new DO.Assignment
                        {
                            CallId = call.Id,
                            VolunteerId = 0,
                            EntryTimeOfTreatment = null,
                            EndingTimeOfTreatment = AdminManager.Now,
                            MyEndingTime = DO.EndingTimeType.Expired
                        });

                        newAssignmentsCreated = true;
                    }
                }
            }

            foreach (var id in updatedAssignmentIds)
                AssignmentManager.Observers.NotifyItemUpdated(id);

            if (newAssignmentsCreated)
                AssignmentManager.Observers.NotifyListUpdated();
        }

        internal static void SimulateCallActivity()
        {
            Thread.CurrentThread.Name = $"CallSimulator_{Thread.CurrentThread.ManagedThreadId}";

            List<DO.Call> callsToCheck;
            lock (AdminManager.BlMutex)
            {
                callsToCheck = s_dal.Call.ReadAll().ToList();
            }

            foreach (var call in callsToCheck)
            {
                var status = GetCallStatus(call.Id);

                if (status == BO.CallStatus.Expired)
                    continue;

                if (call.MaxTimeFinishCalling.HasValue && AdminManager.Now > call.MaxTimeFinishCalling)
                {
                    lock (AdminManager.BlMutex)
                    {
                        var assignment = s_dal.Assignment.ReadAll().Find(a => a.CallId == call.Id);
                        if (assignment != null)
                        {
                            s_dal.Assignment.Update(assignment with
                            {
                                EndingTimeOfTreatment = AdminManager.Now,
                                MyEndingTime = DO.EndingTimeType.Expired
                            });
                            AssignmentManager.Observers.NotifyItemUpdated(assignment.Id);
                        }
                        else
                        {
                            s_dal.Assignment.Create(new DO.Assignment
                            {
                                CallId = call.Id,
                                VolunteerId = 0,
                                EntryTimeOfTreatment = null,
                                EndingTimeOfTreatment = AdminManager.Now,
                                MyEndingTime = DO.EndingTimeType.Expired
                            });
                            AssignmentManager.Observers.NotifyListUpdated();
                        }
                    }
                }
            }
        }
        private static async Task CompleteCallWithCoordinatesAsync(BO.Call call)
        {
            // Implementation goes here
            // Example:
            await ChecksLogicalValidation(call);
            // Additional logic for completing the call with coordinates
        }
    }
}
