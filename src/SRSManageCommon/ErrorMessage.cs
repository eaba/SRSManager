#nullable enable
using System;
using System.Collections.Generic;

namespace SrsManageCommon
{
    /// <summary>
    /// error code
    /// </summary>
    [Serializable]
    public enum ErrorNumber : int
    {
        None = 0, //success
        InitSystem = -5000, //Initialization system error
        ConfigFile = -5001, //configuration file not found
        LicenseFileNotFind = -5002, //lic file not found
        LicenseFileReadFail = -5003, //lic file format error
        HardSerialError = -5004, //Hardware code verification failed
        TimeExpiration = -5005, //lic authorization time expires
        NotSet = -5006, //The system is not set up, it may be a first-time installation
        DatabaseError = -5007, //database error
        StorageDiskSettingFail = -5008, //Disk setup error
        StorageSpaceNotEnough = -5009, //not enough storage space
        GetVersionError = -5010, //Error getting version information
        StopSrsError = -5011, //End SRS error
        StartRuningSrsError = -5012, //Start an already started SRS exception
        StartSrsError = -5013, //SRS starts abnormally
        SrsTerminated = -5014, //SRS is not running
        SrsReloadError = -5015, //SRS realod fail
        SrsCreateError = -5016, //Failed to create SRS instance
        SrsNotFound = -5017, //SRS executable not found
        SrsObjectNotInit = -5018, //SRS object not created
        FunctionInputParamsError = -5019, //function parameter error
        SrsSubInstanceAlreadyExists = -5020, //The SRS configuration sub-instance already exists
        SrsSubInstanceNotFound = -5021, //SRS configuration sub-instance not found
        SrsConfigFunctionUnsupported = -5022, //feature not yet supported
        SystemCheckPasswordFail = -5023, //Failed to detect password
        SystemCheckAllowKeyFail = -5024, //Access control check failed
        SystemSessionExcept = -5025, //session exception
        SystemCheckAllowKeyOrSessionFail = -5026, //Access control check failed
        OnvifMonitorNotInit = -5027, //non-onvif device
        OnvifPtzKeepMoveOnlyUpdownleftright = -5028,
        OnvifPtzMoveExcept = -5029,
        OnvifMonitorListIsNull = -5030, //onvif device list is empty
        OnvifConfigLoadExcept = -5031, //Failed to read onvif configuration file
        OnvifConfigWriteExcept = -5032, //Failed to write Onvif configuration file 
        SrsInstanceExists = -5033, //SRS instance already exists
        SrsInstanceConfigPathExists = -5034, //SRS configuration file path is duplicated
        SrsInstanceListenExists = -5035, //The listening port of SRS has conflicted
        SrsInstanceHttpApiListenExists = -5035, //The HttpApi listening port of SRS has conflicted
        SrsInstanceHttpServerListenExists = -5036, //The HttpServer listening port of SRS has conflicted
        SrsInstanceRtcServerListenExists = -5037, //The RtcServer listening port of SRS has conflicted
        SrsInstanceStreamCasterListenExists = -5038, //The StreamCaster listening port of SRS has conflicted
        SrsInstanceStreamCasterSipListenExists = -5039, //The StreamCaster[Sip] listening port of SRS has conflicted
        SrsInstanceSrtServerListenExists = -5040, //The SrtServer listening port of SRS has conflicted
        SystemWebApiException = -5041, //WebApi system level exception
        SrsClientNotGB28181 = -5041, //Not a gb28181 device
        SrsStreamNotExists = -5042, //media stream does not exist
        SrsDvrPlanNotExists = -5043, //Recording schedule does not exist
        SrsDvrPlanTimeLimitExcept = -5044, //The time in the recording plan is abnormal
        OnvifMonitorInitExcept = -5045, //onvif device initialization exception
        SrsDvrPlanAlreadyExists = -5046, //Recording plan already exists
        SystemDataBaseExcept = -5047, //Abnormal database operation
        SystemDataBaseLimited = -5048, //Database operations are limited
        SystemSessionItWorks = -5049, //session has not expired
        SystemDataBaseRecordNotExists = -5050, //record does not exist
        DvrVideoFileNotExists = -5051, //Recording file does not exist
        DvrCutMergeTimeLimit =-5052, //The time span exceeds 10 minutes, synchronous return is not allowed, please use asynchronous callback method
        DvrCutMergeFileNotFound =-5053,//No related video files were found within the time period
        DvrCutProcessQueueLimit =-5054,//The processing queue is full, please try again later
        DvrCutMergeTaskNotExists =-5055,//The merge and crop task has been completed or does not exist
        SrsGb28181IsDisabled6000 = 6000, //GB28181 service is not enabled
        SrsGb28181SessionOrMediaChannelExists6001 = 6001, //SIP session or media channel already exists
        SrsGb28181SessionOrMediaChannelNotExists6002 = 6002, //SIP session or media channel does not exist
        SrsGb28181RtpOutOfPort6003 = 6003, //The number of RTP ports has been exhausted
        SrsGb28181RtpPortModeInvalid6004 = 6004, //invalid port assignment
        SrsGb28181InputParamsError6005 = 6005, //The input parameter is wrong, the parameter cannot be empty
        SrsGb28181FuncationUnsupported6006 = 6006, //This API is not supported
        SrsGb28181SipNotStarted6007 = 6007, //SIP service is not enabled
        SrsGb28181SipInviteFailed6008 = 6008, //SIP INVITE fail
        SrsGb28181SipByeFailed6009 = 6009, //SIP BYE fail
        SrsGb28181SipInviteSucceeded6010 = 6010, //INVITE Called successfully
        SrsGb28181CreateRtmpRemuxFailed6011 = 6011, //Failed to create media channel RTMP compositor
        SrsGb28181SipChannelOffline6012 = 6012, //SIP device is offline
        SrsGb28181SipChannelNotExists6013 = 6013, //SIP device does not exist
        SrsGb28181SipSendSipRawDataFailed6014 = 6014, //Failed to send SIP_RAW_DATA
        SrsGb28181SipParseMessageFailed6015 = 6015, //SIP message parsing failed
        SrsGb28181SipPtzControlFailed6016 = 6016, //PTZ control failed
        SrsGb28181SipPtzControlButNotInvite6017 = 6017, //The channel is not in the INVITE state, and the PTZ cannot be controlled
        SrsGb28181SipPtzCmdError6018 = 6018, //invalid gimbal command
        Other = -6000 //other abnormalities
    }

    /// <summary>
    /// Error code description
    /// </summary>
    [Serializable]
    public static class ErrorMessage
    {
        public static Dictionary<ErrorNumber, string>? ErrorDic;

        public static void Init()
        {
            ErrorDic = new Dictionary<ErrorNumber, string>();
            ErrorDic[ErrorNumber.None] = "no error";
            ErrorDic[ErrorNumber.ConfigFile] = "configuration file not found";
            ErrorDic[ErrorNumber.InitSystem] = "Initialization system error";
            ErrorDic[ErrorNumber.DatabaseError] = "database error";
            ErrorDic[ErrorNumber.HardSerialError] = "license verification failed";
            ErrorDic[ErrorNumber.LicenseFileNotFind] = "license file not found";
            ErrorDic[ErrorNumber.LicenseFileReadFail] = "Failed to read license file";
            ErrorDic[ErrorNumber.Other] = "unknown mistake";
            ErrorDic[ErrorNumber.TimeExpiration] = "License authorization time expires";
            ErrorDic[ErrorNumber.NotSet] = "Server information is not set, please log in to the settings page";
            ErrorDic[ErrorNumber.StorageDiskSettingFail] = "Disk setup error";
            ErrorDic[ErrorNumber.StorageSpaceNotEnough] = "not enough storage space";
            ErrorDic[ErrorNumber.GetVersionError] = "Error getting version information";
            ErrorDic[ErrorNumber.StopSrsError] = "Exception when ending the SRS process";
            ErrorDic[ErrorNumber.StartRuningSrsError] = "This SRS process is already running";
            ErrorDic[ErrorNumber.StartSrsError] = "The SRS process is abnormally started";
            ErrorDic[ErrorNumber.SrsTerminated] = "SRS is not running";
            ErrorDic[ErrorNumber.SrsReloadError] = "SRS configuration refresh failed";
            ErrorDic[ErrorNumber.SrsCreateError] = "Failed to create an SRS instance";
            ErrorDic[ErrorNumber.SrsNotFound] = "SRS executable does not exist";
            ErrorDic[ErrorNumber.SrsObjectNotInit] = "SRS control object not created";
            ErrorDic[ErrorNumber.FunctionInputParamsError] = "wrong function parameter";
            ErrorDic[ErrorNumber.SrsSubInstanceAlreadyExists] = "The configuration sub-instance already exists";
            ErrorDic[ErrorNumber.SrsSubInstanceNotFound] = "The configuration sub-instance does not exist";
            ErrorDic[ErrorNumber.SrsConfigFunctionUnsupported] = "The required function is not yet supported";
            ErrorDic[ErrorNumber.OnvifMonitorNotInit] = "Non-onvif device, initialization failed";
            ErrorDic[ErrorNumber.OnvifPtzKeepMoveOnlyUpdownleftright] = "Only up, down, left and right are supported in continuous PTZ movement mode";
            ErrorDic[ErrorNumber.OnvifPtzMoveExcept] = "PTZ movement control is abnormal";
            ErrorDic[ErrorNumber.SystemCheckPasswordFail] = "Authentication failed";
            ErrorDic[ErrorNumber.SystemCheckAllowKeyFail] = "Access control check failed";
            ErrorDic[ErrorNumber.SystemSessionExcept] = "Session exception";
            ErrorDic[ErrorNumber.SystemCheckAllowKeyOrSessionFail] = "Access control or session expiration exception";
            ErrorDic[ErrorNumber.SystemSessionItWorks] = "The current Session has not expired and does not need to be refreshed";
            ErrorDic[ErrorNumber.OnvifMonitorListIsNull] = "Onvif device list is empty";
            ErrorDic[ErrorNumber.OnvifConfigLoadExcept] = "Failed to read Onvif configuration file";
            ErrorDic[ErrorNumber.OnvifConfigWriteExcept] = "Failed to write Onvif configuration file";
            ErrorDic[ErrorNumber.SrsInstanceExists] = "SRS instance already exists";
            ErrorDic[ErrorNumber.SrsInstanceConfigPathExists] = "SRS configuration file path is duplicated";
            ErrorDic[ErrorNumber.SrsInstanceListenExists] = "The listening port of SRS has conflicted";
            ErrorDic[ErrorNumber.SrsInstanceHttpApiListenExists] = "The HttpApi listening port of SRS has conflicted";
            ErrorDic[ErrorNumber.SrsInstanceHttpServerListenExists] = "The HttpServer listening port of SRS has conflicted";
            ErrorDic[ErrorNumber.SrsInstanceRtcServerListenExists] = "The RtcServer listening port of SRS has conflicted";
            ErrorDic[ErrorNumber.SrsInstanceStreamCasterListenExists] = "The StreamCaster listening port of SRS has conflicted";
            ErrorDic[ErrorNumber.SrsInstanceStreamCasterSipListenExists] = "The StreamCaster[Sip] listening port of SRS has conflicted";
            ErrorDic[ErrorNumber.SrsInstanceSrtServerListenExists] = "The SrtServer listening port of SRS has conflicted";
            ErrorDic[ErrorNumber.SrsClientNotGB28181] = "This device is not a GB28181 device";
            ErrorDic[ErrorNumber.SrsGb28181IsDisabled6000] = "GB28181 service is not enabled";
            ErrorDic[ErrorNumber.SrsGb28181SessionOrMediaChannelExists6001] = "SIP session or media channel already exists";
            ErrorDic[ErrorNumber.SrsGb28181SessionOrMediaChannelNotExists6002] = "SIP session or media channel does not exist";
            ErrorDic[ErrorNumber.SrsGb28181RtpOutOfPort6003] = "The number of RTP ports has been exhausted";
            ErrorDic[ErrorNumber.SrsGb28181RtpPortModeInvalid6004] = "invalid port assignment";
            ErrorDic[ErrorNumber.SrsGb28181InputParamsError6005] = "The input parameter is wrong, the parameter cannot be empty";
            ErrorDic[ErrorNumber.SrsGb28181FuncationUnsupported6006] = "This API is not supported";
            ErrorDic[ErrorNumber.SrsGb28181SipNotStarted6007] = "SIP service is not enabled";
            ErrorDic[ErrorNumber.SrsGb28181SipInviteFailed6008] = "SIP INVITE fail";
            ErrorDic[ErrorNumber.SrsGb28181SipByeFailed6009] = "SIP BYE fail";
            ErrorDic[ErrorNumber.SrsGb28181SipInviteSucceeded6010] = "INVITE Called successfully";
            ErrorDic[ErrorNumber.SrsGb28181CreateRtmpRemuxFailed6011] = "Failed to create media channel RTMP compositor";
            ErrorDic[ErrorNumber.SrsGb28181SipChannelOffline6012] = "SIP device is offline";
            ErrorDic[ErrorNumber.SrsGb28181SipChannelNotExists6013] = "SIP device does not exist";
            ErrorDic[ErrorNumber.SrsGb28181SipSendSipRawDataFailed6014] = "Failed to send SIP_RAW_DATA";
            ErrorDic[ErrorNumber.SrsGb28181SipParseMessageFailed6015] = "SIP message parsing failed";
            ErrorDic[ErrorNumber.SrsGb28181SipPtzControlFailed6016] = "PTZ control failed";
            ErrorDic[ErrorNumber.SrsGb28181SipPtzControlButNotInvite6017] = "The channel is not in the INVITE state, and the PTZ cannot be controlled";
            ErrorDic[ErrorNumber.SrsGb28181SipPtzCmdError6018] = "invalid gimbal command";
            ErrorDic[ErrorNumber.SystemWebApiException] = "WebApi system level exception";
            ErrorDic[ErrorNumber.SrsStreamNotExists] = "Media stream information does not exist";
            ErrorDic[ErrorNumber.SrsDvrPlanNotExists] = "The recording plan does not exist";
            ErrorDic[ErrorNumber.SrsDvrPlanTimeLimitExcept] = "In this recording plan, the weekly recording start time and end time need to be greater than 120 seconds";
            ErrorDic[ErrorNumber.OnvifMonitorInitExcept] = "Onvif device initialization exception";
            ErrorDic[ErrorNumber.SrsDvrPlanAlreadyExists] = "Recording schedule already exists";
            ErrorDic[ErrorNumber.SystemDataBaseExcept] = "Abnormal database operation";
            ErrorDic[ErrorNumber.SystemDataBaseLimited] = "The database operation is limited, and it is not allowed to query more than 10,000 data at a time, and the pageIndex starts from 1";
            ErrorDic[ErrorNumber.SystemDataBaseRecordNotExists] = "There is no such record in the database";
            ErrorDic[ErrorNumber.DvrVideoFileNotExists] = "Recording file does not exist";
            ErrorDic[ErrorNumber.DvrCutMergeTimeLimit] = "The task time span limit is exceeded, the synchronous task time span exceeds 10 minutes, and the asynchronous task time span exceeds 120 minutes";
            ErrorDic[ErrorNumber.DvrCutMergeFileNotFound] = "No related video files were found within the time period";
            ErrorDic[ErrorNumber.DvrCutProcessQueueLimit] = "The merge request processing queue is full, please try again later";
            ErrorDic[ErrorNumber.DvrCutMergeTaskNotExists] = "Merge request task completed or does not exist";
            
        }
    }
}