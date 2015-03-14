using System.Globalization;

namespace TinyMetroWpfLibrary.Utility
{
    public static class Constants
    {
        public const string GLOBAL_CAN_SAVE_RESULT_FILE_NAME = "CanSave";
        public const string GLOBAL_SETTINGS_PROPERTY_NAME_DistanceUnit = "DistanceUnit";
        public const string GLOBAL_SETTINGS_PROPERTY_NAME_ProjectDirectory = "ProjectDirectory";
        public const string GLOBAL_SETTINGS_PROPERTY_NAME_UserEmail = "UserEmail";
        public const string GLOBAL_SETTINGS_PROPERTY_NAME_TutorialEnabled = "TutorialEnabled";
        public const string GLOBAL_SETTINGS_PROPERTY_NAME_PacketSlicingSize = "PacketSlicingSize";
        public const string GLOBAL_SETTINGS_FILE_PATH = @"Config\GlobalSetting.config";
        public const string GLOBAL_SETTINGS_PATH = @"Config";
        public const int GLOBAL_FILE_NAME_LENGTH = 50;
        public const int GLOBAL_MINIMUM_SIGNAL = -101;
        public const double GLOBAL_FEET_METER = 3.2808398950131;

        public const string UX_PROFILE_EXT = "aup";
        public const string UX_RESULT_EXT = "aur";
        public static readonly string UX_DEFAULT_PROFILE_PATH = string.Format(@"UXProfileTemplate\factory_default.{0}", UX_PROFILE_EXT);
        public static readonly string UX_USER_DIR = @"Root\UX_Profile";
        public const string UX_TEMP_DIR = @"Root\UserExperience";

        public const string UX_Config_Type_Connection = "Connection";
        public const string UX_Config_Type_DNS = "DNS";
        public const string UX_Config_Type_Ping = "Ping";
        public const string UX_Config_Type_Trace = "Trace";
        public const string UX_Config_Type_HTTP_Download = "HTTP_Download";
        public const string UX_Config_Type_FTP_Download = "FTP_Download";
        public const string UX_Config_Type_Online_Video = "Online_Video";
        public const string UX_Config_Type_Online_Audio = "Online_Audio";
        public const string UX_Config_Type_Web_Browsing = "Web_Browsing";
        public const int UX_Initial_ConfigID = int.MaxValue;
        public const string UX_ReturnValueSucceeded = "Succeeded";
        public const string UX_ReturnValueFailed = "Failed";
        public const string Survey_TEMP_DIR = @"Root\Survey";

        public const string Finder_TEMP_DIR = @"Root\Finder";
        public const string Finder_Profile_EXT = "afp";
        public const string Finder_Result_EXT = "afr";
        public const int Finder_Remove_Focus_After_Idle_AP = 5;
        public const int Finder_Remove_Focus_After_Idle_Station = 10;

        public const string Discovery_TEMP_DIR = @"Root\Discovery";
        public const string Discovery_Result_FILE_NAME = "DiscoveryResult.adr";
        public const int Discovery_Refresh_Interval_Native = 5000;
        public const int Discovery_Refresh_Interval_Promiscuous = 1000;
        public const string Discovery_TraceFile_TEMP_DIR = @"Root\TraceFile";
        public const string Discovery_Catpture_FILE_EXT = ".amm";
        public const int Discovery_Network_Pie_Chart_Count = 8;

        public const string PROJECTS_ROOT_PATH = @"\AirMagnet\AirCheck Wi-Fi Tester Projects";
        public const string PROJECTS_TEMP_DIR = "projecttemp";
        public const string PROJECTS_FILE_EXT = ".zip";
        public const string PROJECTS_INFO_FILE_NAME = "ProjectInfo";
        public const string PROJECTS_TASKINFO_FILE_NAME = "TaskInfo";
        public const string PROJECTS_TASKTYPE_KEY_UX = "UX";
        public const string PROJECTS_TASKTYPE_KEY_SURVEY = "Survey";
        public const string PROJECTS_TASKTYPE_KEY_FINDER = "Finder";
        public const string PROJECTS_TASKTYPE_KEY_DISCOVERY = "Discovery";
        public const string PROJECTS_SUBDIR_USEREXPERIENCE = @"\UserExperience";
        public const string PROJECTS_SUBDIR_SURVEY = @"\Survey";
        public const string PROJECTS_SUBDIR_DISCOVERY = @"\Discovery";
        public const string PROJECTS_SUBDIR_FINDER = @"\Finder";
        public const string PROJECTS_SUBDIR_TRACEFILE = @"\TraceFile";
        public const string PROJECTS_SUBDIR_FLOORPLAN = @"\FloorPlan";
        public const string PROJECTS_SUBDIR_ANNOTATION = @"\Annotation";
        public const string PROJECTS_SUBDIR_VIDEO = @"\Video";
        public const string PROJECTS_SUBDIR_AUDIO = @"\Audio";
        public const string PROJECTS_SUBDIR_IMAGE = @"\Image";
        public static string PROJECTS_TITLE_EMPTY_TITLE = "";
        public static int PROJECTS_MAX_COUNT = 100;
        public static string PROJECTS_CREATETIME_FORMAT = "yyyy-MM-dd HH:mm";
        public static int TASKS_MAX_COUNT = 100;

        public const int AutoRefreshTick = 5000;
        public const int MAX_FLOORPLAN_NUM = 30;
        public const string FLOOR_PLAN_INFO_EXT = "afp";
        public const string FLOOR_PLAN_DEFAULT_NAME = "Floor plan";
        public const string FLOOR_PLAN_PATH = @".\Root\FloorPlan";
        public const string SURVEY_SETTING_FILE_NAME = "SurveySetting.config";
        public const string SURVEY_PATH = @".\Root\Survey";
        public const string SURVEY_PROJECT_EXT = "svp";
        public const string SURVEY_NAVIGATE_KEY_FLOORPLAN = "FloorPlan";
        public const string SURVEY_NAVIGATE_KEY_DATAMGR = "DataMgr";
        public const double SURVEY_PICKER_POSITION_INVALID = -10000;
        public const string SURVEY_FLOORPLAN_TEMPLATES = @"FloorplanTemplates";
        public const int SURVEY_CALIBRATION_MAX_DIMENSION = 50000;
        public const int SURVEY_CALIBRATION_MIN_DIMENSION = 5;
        public const int SURVEY_BPS_TO_KBPS = 1000;
        public const int SURVEY_KBPS_TO_MBPS = 1000;
        public const int SURVEY_BPS_TO_bps = 8;
        public const int SURVEY_MAX_AUTOSAMPLING_COUNT = 200;

        public const double COMMON_FILE_NAME_MAXLENGTH = 50;
        public const string SURVEY_REPORT_SIGNALHEATMAP_EXTENSION = "-SignalHeatMap.png";
        public const string SURVEY_REPORT_SIGNALPATH_EXTENSION = "-SignalPath.png";
        public const string SURVEY_REPORT_THROUGHPUTHEATMAP_EXTENSION = "_ThroughHeatMap.png";
        public const string SURVEY_REPORT_THROUGHPUTPATH_EXTENSION = "_ThroughPath.png";
        public const string SURVEY_REPORT_FOLDER_NAME = "ReportData";

        public const double MAX_COLOR_LEGEND_SIGNAL_VALUE = 100;
        public const double MIN_COLOR_LEGEND_SIGNAL_VALUE = 0;
        public const double MAX_COLOR_LEGEND_THROUTHPUT_VALUE = 400;
        public const double MIN_COLOR_LEGEND_THROUGHPUT_VALUE = 0;
        public const int COLOR_LEGEND_COLOR_BLOCK_COUNT = 20;
        public const int COLOR_LEGEND_MAX_COLOR_INDEX = 100;
        public const int COLOR_LEGEND_OutRangeInt32Color = -2894893;
        public const int COLOR_LEGEND_TransParentInt32Color = 0;

        public const string SURVEY_TOOLTIP_SIGNAL = "Signal";
        public const string SURVEY_TOOLTIP_THROUGHPUT= "Throughput";
        public const double TASK_ANNOTATION_MAXFILESIZE = 10 * 1024 * 1024;

        public const string REPORT_SURVEY_FLOORPLAN_SIGNAL_TEMP = "ReportFloorPlanTemp-Signal.jpg";
        public const string REPORT_SURVEY_FLOORPLAN_THROUGHPUT_TEMP = "ReportFloorPlanTemp-Throughput.jpg";

        public const int MAX_ALL_CHANNEL_SCAN_SECONDS = 60;
        public const string REPORT_FONT_NAME_ENGLISH = "Arial";
        public const string REPORT_FONT_NAME_CHINESE = "SimSun";
        public const string REPORT_FONT_NAME_JAPANESE = "SimSun";
        public const string REPORT_FONT_NAME_KOREAN = "Batang";

        public const int MAX_RESET_SIGNAL_SECONDS = 12;

        public const int CONNECTION_TIME_MS = 30 * 1000;

        public const int PING_COUNT_AFTER_CONNECTION = 15;

        //UX TestConfig related constants.
        //Association
        public static readonly string AP_Connection_Param_ConnectionType = "ConnectionType";
        public static readonly string AP_Connection_Param_SelectedAPorSSID = "SelectedAPorSSID";
        public static readonly string AP_Connection_Param_APName = "APName";
        public static readonly string AP_Connection_Param_APMac = "APMac";
        public static readonly string AP_Connection_Param_APMedia = "APMedia";
        public static readonly string AP_Connection_Param_SSID = "SSID";
        public static readonly string AP_Connection_Param_InHotSpot = "InHotSpot";
        public static readonly string AP_Connection_Param_UseConnectionProfile = "UseConnectionProfile";
        public static readonly string AP_Connection_Param_NumOfTests = "NumOfTests";
        public static readonly string AP_Connection_Param_TimeOut = "TimeOut";

        public static readonly string AP_Connection_Threshold_SuccessRate = "SuccessRate";
        public static readonly string AP_Connection_Threshold_APSignalStrength = "APSignalStrength";
        public static readonly string AP_Connection_Threshold_APNoiseLevel = "APNoiseLevel";

        //DNS
        public static readonly string DNS_Param_EditByUserDNS = "EditByUserDNS";
        public static readonly string DNS_Param_URL = "URL";
        public static readonly string DNS_Param_dns1 = "dns1";
        public static readonly string DNS_Param_dns2 = "dns2";
        public static readonly string DNS_Param_NumOfTests = "NumOfTests";
        public static readonly string DNS_Param_TimeOut = "TimeOut";

        public static readonly string DNS_Threshold_SuccessRate = "SuccessRate";

        //DHCP
        public static readonly string DHCP_Param_NumOfTests = "NumOfTests";
        public static readonly string DHCP_Param_TimeOut = "TimeOut";

        public static readonly string DHCP_Threshold_SuccessRate = "SuccessRate";

        //Ping
        public static readonly string PING_Param_SelectIPorDomain = "SelectIPorDomain";
        public static readonly string PING_Param_Domain = "Domain";
        public static readonly string PING_Param_IP = "IP";
        public static readonly string PING_Param_NumOfTests = "NumOfTests";
        public static readonly string PING_Param_TimeOut = "TimeOut";

        public static readonly string PING_Threshold_SuccessRate = "SuccessRate";
        public static readonly string PING_Threshold_AvgRTT = "AvgRTT";
        public static readonly string PING_Threshold_MaxRTT = "MaxRTT";

        //Trace
        public static readonly string Trace_Param_HostName = "HostName";
        public static readonly string Trace_Param_NumOfTests = "NumOfTests";
        public static readonly string Trace_Param_Timeout = "Timeout";

        public static readonly string Trace_Threshold_SuccessRate = "SuccessRate";

        //HTTP Download
        public static readonly string HTTP_Param_URL = "URL";
        public static readonly string HTTP_Param_UseSSL = "UseSSL";
        public static readonly string HTTP_Param_NumOfTests = "NumOfTests";
        public static readonly string HTTP_Param_MaxSize = "MaxSize";
        public static readonly string HTTP_Param_MaxTime = "MaxTime";
        public static readonly string HTTP_Param_Timeout = "Timeout";

        public static readonly string HTTP_Threshold_DownloadAvgThroughput = "DownloadAvgThroughput";
        public static readonly string HTTP_Threshold_SuccessRate = "SuccessRate";

        //FTP Download
        public static readonly string FTP_Param_URL = "URL";
        public static readonly string FTP_Param_Username = "Username";
        public static readonly string FTP_Param_Password = "Password";
        public static readonly string FTP_Param_UseAnonymous = "UseAnonymous";
        public static readonly string FTP_Param_NumOfTests = "NumOfTests";
        public static readonly string FTP_Param_MaxSize = "MaxSize";
        public static readonly string FTP_Param_MaxTime = "MaxTime";
        public static readonly string FTP_Param_Timeout = "Timeout";

        public static readonly string FTP_Threshold_DownloadAvgThroughput = "DownloadAvgThroughput";
        public static readonly string FTP_Threshold_SuccessRate = "SuccessRate";

        //Online Video
        public static readonly string Video_Param_URLToEmbeddedVideo = "URLToEmbeddedVideo";
        public static readonly string Video_Param_SuggestedQuality = "SuggestedQuality";
        public static readonly string Video_Param_TimeOut = "TimeOut";

        public static readonly string Video_Threshold_BufferRatio = "BufferRatio";
        public static readonly string Video_Threshold_ActualPlayTimeRatio = "ActualPlayTimeRatio";
        public static readonly string Video_Threshold_Interruptions = "Interruptions";

        public static readonly int Video_Default_Threshold_Time_Out = 30;

        //Online Audio
        public static readonly string Audio_Param_URLToEmbeddedAudio = "URLToEmbeddedAudio";
        public static readonly string Audio_Param_SuggestedQuality = "SuggestedQuality";
        public static readonly string Audio_Param_TimeOut = "TimeOut";

        public static readonly string Audio_Threshold_BufferRatio = "BufferRatio";
        public static readonly string Audio_Threshold_ActualPlayTimeRatio = "ActualPlayTimeRatio";
        public static readonly string Audio_Threshold_Interruptions = "Interruptions";

        public static readonly int Audio_Default_Threshold_Time_Out = 30;

        //Web Browsing
        public static readonly string WebBrowsing_Param_URLToWebsite = "URLToWebsite";
        public static readonly string WebBrowsing_Param_MaxTime = "MaxTime";
        public static readonly string WebBrowsing_Param_NumOfTests = "NumOfTests";

        public static readonly string WebBrowsing_Threshold_SuccessRate = "SuccessRate";

        public static readonly int UX_Default_Success_Rate = 60;
        public static readonly int UX_Default_Number_Of_Tests = 5;
        public static readonly int UX_Default_Max_Time = 10;
        public static readonly int UX_Default_Time_Out = 10;

        //en-us cluture number format
        public static readonly NumberFormatInfo EN_US_Number_Format = new CultureInfo("en-US", false).NumberFormat;
    }
}
