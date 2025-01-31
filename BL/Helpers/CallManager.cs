using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Helpers.Tools;

namespace Helpers
{
    internal static class CallManager
    {
        private static IDal s_dal = Factory.Get; //stage 4
        public static BO.CallStatus GetCallStatus(int callId)
        {
            var call = s_dal.Call.Read(callId) ?? throw new KeyNotFoundException($"Call with ID {callId} not found.");
            var assignment = s_dal.Assignment.ReadAll().Find(a => a.CallId == callId);
            TimeSpan? timeLeft = call.MaxTimeFinishCalling - ClockManager.Now;

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
        public static object GetFieldValue(BO.Call call, string fieldName)
        {
            return fieldName switch
            {
                nameof(BO.Call.CallType) => call.CallType!,
                nameof(BO.Call.FullAddress) => call.FullAddress!,
                nameof(BO.Call.Latitude) => call.Latitude!,
                nameof(BO.Call.Longitude) => call.Longitude!,
                nameof(BO.Call.OpenTime) => call.OpenTime!,
                nameof(BO.Call.MaxCompletionTime) => call.MaxCompletionTime!,
                nameof(BO.Call.Description) => call.Description!,
                nameof(BO.Call.Status) => call.Status!,
                _ => throw new ArgumentException("Invalid field name", nameof(fieldName))
            };
        }
        internal static BO.CallInList ConvertToBOCallInList(DO.Call call)
        {
            var assignments = s_dal.Assignment.ReadAll().Where(a => a.CallId == call.Id).ToList();
            var lastAssignment = assignments
                .OrderByDescending(a => a.EntryTimeOfTreatment)
                .FirstOrDefault();
            var volunteer = s_dal.Volunteer.ReadAll().Find(v => v.Id == lastAssignment.VolunteerId);

            return new BO.CallInList
            {
                Id = lastAssignment == null ? null : lastAssignment.Id,
                CallId = call.Id,
                CallType = (BO.CallType)call.MyCall,
                OpenTime = call.OpeningTime,
                TimeRemaining = call.MaxTimeFinishCalling - ClockManager.Now,
                LastVolunteerName = volunteer.FullName,
                Status = GetCallStatus(call.Id),
                TotalAssignments= assignments.Count,
                TotalCompletionTime=lastAssignment.MyEndingTime==EndingTimeType.TakenCareOf? lastAssignment.EndingTimeOfTreatment-lastAssignment.EntryTimeOfTreatment:null
            };
        }
        internal static async Task ChecksLogicalValidation(BO.Call call)
        {
            //Checks if the maximum end time is greater than the start time
            if (call.MaxCompletionTime.HasValue && call.MaxCompletionTime < call.OpenTime)
            {
                throw new ArgumentException("Max completion time must be after the open time.");
            }
            //Check that the address is written correctly
            if (string.IsNullOrWhiteSpace(call.FullAddress))
            {
                throw new ArgumentException("Address cannot be empty or whitespace.");
            }

            double latitude, longitude;
            var geocodingService = new GeocodingService();
            try
            {
                (latitude, longitude) = await geocodingService.GetCoordinatesFromAddress(call.FullAddress);
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
            // Validate that the address is a valid address with latitude and longitude
            if (string.IsNullOrWhiteSpace(call.FullAddress) ||
                !(call.Latitude >= -90 && call.Latitude <= 90 &&
                  call.Longitude >= -180 && call.Longitude <= 180))
            {
                throw new ArgumentException("The address must be valid with latitude and longitude.");
            }
            // Validate the call type is valid
            if (!Enum.IsDefined(typeof(BO.CallType), call.CallType))
            {
                throw new ArgumentException("Invalid call type.");
            }
            // Validate the description length
            if (!string.IsNullOrEmpty(call.Description) && call.Description.Length > 500)
            {
                throw new ArgumentException("Description is too long (maximum 500 characters).");
            }
            // Validate that there are no assignments in the past
            if (call.CallAssignments != null && call.CallAssignments.Any(a => a.EntryTime < call.OpenTime))
            {
                throw new ArgumentException("Assignments cannot start before the call's open time.");
            }
        }

    }
}
