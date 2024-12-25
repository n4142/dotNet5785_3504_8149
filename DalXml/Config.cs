using System;

namespace Dal;

internal static class Config
{
    internal const string s_data_config_xml = "data-config.xml";
    internal const string s_assignments_xml = "assignments.xml";
    internal const string s_calls_xml = "calls.xml";
    internal const string s_volunteers_xml = "volunteers.xml";
    internal const int StartCallId = 1;
    internal const int StartAssignmentId = 1;


    //internal static int StartCallId
    //{
    //    get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "StartCallId");
    //    private set => XMLTools.SetConfigIntVal(s_data_config_xml, "StartCallId", value);
    //}
    internal static int NextCallId
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextCallId");
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextCallId", value);
    }
    //internal static int StartAssignmentId
    //{
    //    get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "StartAssignmentId");
    //    private set => XMLTools.SetConfigIntVal(s_data_config_xml, "StartAssignmentId", value);
    //}
    internal static int NextAssignmentId
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextAssignmentId");
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextAssignmentId", value);
    }
    internal static DateTime Clock
    {
        get => XMLTools.GetConfigDateVal(s_data_config_xml, "Clock");
        set => XMLTools.SetConfigDateVal(s_data_config_xml, "Clock", value);
    }
    internal static TimeSpan RiskRange
    {
        get => XMLTools.GetConfigTimeSpanVal(s_data_config_xml, "RiskRange");
        set => XMLTools.SetConfigTimeSpanVal(s_data_config_xml, "RiskRange", value);
    }

    internal static void Reset()
    {
        NextCallId = StartCallId;
        NextAssignmentId = StartAssignmentId;
        Clock = DateTime.Now;
        RiskRange = TimeSpan.FromHours(1);
    }
}