fork https://github.com/chatop2020/SRSManager

# SRSManager

## 1. Introduction

- SRSManager is used to manage and control the configuration files of the SRS streaming media server, and structure the configuration files to make the configuration files easier to control.
- Manage the SRS process so that it can start, stop, restart, reload configuration and other operations through a series of APIs.
- Provide WEB management interface to realize SRS management in WebApi mode.
- Integrate onvif device management outside of SRS, including onvif device detection, onvif ptz control, onvif meidaurl acquisition, etc.
- The main reason for opening this project is to use SRS in your own projects. This project is opened for the convenience of using SRS to meet the needs of the project, and it is also for the development of
  Make contributions to the source community.
- The project is compiled with .NET 7, SRSWebApi is developed with Asp.NET Core 7 WebApi project, and the Swagger interface debugging document is integrated.
  Onvif-related functions use the Mictlanix.DotNet.Onvif control class library.
- The project does not include the SRS process content, you need to compile the SRS project yourself, the SRS open source address is: https://github.com/ossrs/srs
  This project is coded based on SRS 5.0+ release version.
- This project supports linux and macos, and requires .NET 7 runtime support.
  
## Two, important

- This project is still under development and cannot be used in a production environment.
- For the needs of the project, the source code of srs has been simply modified, so that the device_id is brought with the http_hook, and the device_id comes from the device_id in the heartbeat
- The modification of the srs source code has been proposed in the official git with the official, and I hope the official can consider it.

## Three, the components
- OnvifClient Onvif's control module for discovery, ptz detection, etc.
- SRSApis encapsulates the related function API of the SRS process
- SRSConfFile encapsulates the structured processing of SRS configuration files, and can read and rewrite SRS configuration files
- Relatively common classes and methods used in the SRSManageCommon project
- SRSWebApi opens various interfaces in the SRSApis project in the form of WebApi
- SRSCallBackManager is used to process various callback data of SRS (abandoned, moved to SRSManageCommon project)
- Items starting with Test_ are functional test items for the above part

## 4. Design Considerations
- Since SRS belongs to the custom configuration file format, it is difficult to operate SRS configuration files in other languages ​​or other projects. For the sake of SRS management, it is necessary to configure
  For structured configuration of files, it is necessary to implement structured reading of .conf files, and serialize structured instances into SRS .conf files. This will make it easier to manage the SRS
  Right easy.
- Considering that general cameras do not have the ability to push rtp streams, and only have the characteristics of rtsp stream exposure, consider integrating onvif related functions to automatically detect and discover the rtsp stream address of the camera,
  The control and other functions of the ptz pan/tilt can cooperate with the ingest of the srs for linkage, so that the general camera can realize the video streaming and rtmp output through the ingest of the SRS.
- OnvifClient, SRSApis, SRSConfFile, SRSManageCommon, SRSWebApi interdependent engineering group, this set needs to implement a complete Onvif+SRS
  The control unit, in which the SRS process instance and the OnvifClient control instance exist in the form of List<Object>, so multiple SRS processes and multiple
  Onvif devices exist at the same time, the SRS process uses uuid to distinguish each other, and the onvif device uses the ip address and uuid in the profile to distinguish different devices and different devices under different devices.
  media stream.
- I call the integration of OnvifClient, SRSApis, SRSConfFile, SRSManageCommon, and SRSWebApi projects a StreamNode, in StreamNode
  Middle~~ I try not to use any relational database components~~ to realize all functions, so as to ensure the maximum degree of freedom of the program and simplify the difficulty of its installation and deployment.
- Slapped face, as the development deepened, it was found that not using database components made many problems complicated, so the FreeSql open source database component was introduced to support the storage and query of related data.
- Encapsulate and forward the original HTTP API of SRS to realize a webapi interface with unified style and unified authentication.

## Five, how to run
+ The project is coded in the environment of Microsoft .net core 3.1. First, please make sure you have the execution environment of .net core 3.1 (support linux, macos)
### configuration file
+ The system configuration file is srswebapi.wconf
+ This configuration file needs to be configured before running the system, and each configuration item will be loaded and checked when the system starts
```
#This is a comment, the beginning of # is a comment, and each line of configuration must end with a semicolon (;)
httpport::5800;
#Webapi listening port
password::password123!@#;
#Password to control access rights, this password needs to be used in the Allow interface, see the content of the allow interface for details
allowkey::0D906284-6801-4B84-AEC9-DCE07FAE81DA	*	192.168.2.*	;
#Allow access key and return ip, if * means all ip can be accessed, ip address or ip address plus mask * means ip or ip segment can be accessed
db::Data Source=192.168.2.35;Port=3306;User ID=root;Password=thisispassword; Initial Catalog=srswebapi;Charset=utf8; SslMode=none;Min pool size=1;
#Database connection string, the database needs to be created manually, and the table system in the database will be created automatically
dbtype::mysql;
#Database type, supports common database services such as mysql, sqlite, oracle, and which databases are supported, please refer to the open source project FreeSql
auto_cleintmanagerinterval::5000;
# Automatic client management monitoring run interval (milliseconds)
auto_logmonitorinterval::300000;
#The running interval of automatic log dump (milliseconds)
auto_dvrplaninterval::60000;
# Interval run time of automatic recording plan (milliseconds)
#auto_keepingeinterval::30000; #don't use this plan anymore
#Automatic ingest keep-alive running interval (milliseconds), this may be a bit problematic, temporarily deprecated
#Increase the parameter enableingestkeeper, whether to enable ingest pull flow monitoring, after enabling, it will monitor each srs process
enableingestkeeper::true;
#Increase the parameter ffmpegpath, which is used to specify the path of the ffmpeg executable file, if not specified, it defaults to the StreamNode directory
ffmpegpath::./ffmpeg;
#Increase the parameter ffmpegthreadcount, which is used to specify the number of ffmpeg threads used when ffmpeg performs video merging, the default is 2 threads, the number of threads should not be too many, 2-4 is more appropriate
ffmpegthreadcount::2;

```
### start command
+ It is recommended to directly dotnet SRSWebApi.dll when debugging and starting
+ official run suggestion nohup dotnet SRSWebapi.dll > ./logs/run.log &
+ Official run will run in background mode

### stop running
+ When running in debug mode, exit the terminal or press Ctrl+C on the terminal, it will end the process and stop running
+ Run in official mode, use the command to find out the pid and then kill the pid
+ check pid under linux
```
ps -aux |grep SRSWebApi.dll
```
+ macos下查pid
```
ps -A|grep SRSWebApi.dll
```
```
kill -9 pid  
```

### Precautions
+ Due to the needs of my own project, I made some minor modifications to the source code of the srs service. The specific modification content can be found at https://github.com/ossrs/srs/issues/1789
  If you use this project for testing, you need to make the same modification to the source code of srs, and then compile srs. Of course, if srs officially accepts my suggestion (it seems to have been accepted),
  In the future, I will directly adopt the official function without modifying the srs source code
+ There must be ffmpeg executable file in the project directory, otherwise the system will report an error
+ The srs executable file is required for the recording of the project, otherwise the system will report an error
+ I'm sorry for those comrades who are completely using ism, the project does not provide WEB management module for the time being, only WebApi module

### other
#### Automation Service Introduction
+ DvrPlanExec automatically executes the recording plan, so that the plan in the recording plan can be implemented
+ IngestMonitor Ingest pulls the stream to keep alive, and when it is found that a large number of ffmpeg logs are written crazily, execute the ingest streamer to restart to ensure that the stream is normal
+ KeepIngestStream old pull stream keep-alive scheme, has been abandoned
+ SrsAndFFmpegLogMonitor is used for dumping Srs logs and FFmpeg logs to ensure that the logs do not explode. When the log file is larger than 10M, it is automatically transferred to another directory and clears the current log file
+ SrsClientManager is used to maintain the connection information (client) of Srs, such as supplementing the IP address of the camera, the rtsp address, maintaining the online list, etc.



## Six, Api interface description
+ The interface is provided in the form of HttpWebApi, and the providing method is http://serverip:apiport/interface type/API method
+ Interface calling method: HttpGet, HttpPost
+ When the input parameter is a simple parameter, use the HttpGet method to call, and use the HttpPost method to call the complex object parameter
+ The input parameters and output results of the interface are json encapsulation (some interface input and output are simple results, and the basic type is used as the input and output int, string, bool, etc.)

```
For example, when calling to detect whether the Srs instance is running, you can send the following http request through CURL to get the status
curl -X GET "http://192.168.2.42:5800/GlobalSrs/IsRunning?deviceId=22364bc4-5134-494d-8249-51d06777fb7f" -H "accept: */*"
```
## Seven, abnormal and normal
+ When an exception occurs in the interface call, the API returns HttpStatusCode as 400, and informs the cause of the exception at the same time. The return structure is as follows:
```json
 {
 	"Code": 0, //error code
    "Message": "No Error" //Description of error reason
 }
```
+ When a system-level exception occurs, it will be automatically captured by asp.net core (for example, if the incoming parameter has a format problem, etc.),
asp.net core will return HttpStatusCode as 400, and give the cause of the exception, the return structure is as follows:
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "traceId": "|1e26aa01-4d02465285d0af0c.",
  "errors": {
    "": [
      "A non-empty request body is required."
    ],
    "obj": [
      "The obj field is required."
    ]
  }
}
```
+ When the interface call is normal, the HttpStatusCode is 200, and the returned data can be received according to the output parameter requirements, and the serialized json will be returned to the corresponding entity class type

## Eight, interface calling convention
+ Time zone: +8 zone
+ Time format: yyyy-MM-dd HH:mm:ss
+ Call method: HttpGet|HttpPost
+ Time-consuming operation: adopt the method of http callback, when an operation is time-consuming (such as /DvrPlan/CutOrMergeVideoFile), the interface requires the callback address to be passed in when requesting, and the interface call is notified through the callback address after the operation is completed Apply relevant results
+ All interfaces for writing Srs configuration (Set, Delete, Update, Insert|Create) will not rewrite the configuration file after the operation is completed, and the application needs to call the /System/RefreshSrsObject interface to write the latest configuration information into the corresponding Srs process configuration file, and automatically reload the configuration file to refresh the Srs running parameters
## Nine, interface description

### Guess the interface you need - FastUseful
+ The interfaces that you may use frequently are all in this interface class, because our classmates who develop web management by ourselves need it, so I integrated it under this module
  The interfaces are all relatively simple, so I won’t describe the input and output in detail anymore, and will focus on the special ones.
 
#### /FastUseful/GetOnvifMonitorInfoByIngest
+ Obtain the Onvif streaming device information under an ingest
+ Because it is a streaming device under the ingest, the input parameters must be specified to an ingest in detail
+ Therefore, it is necessary to use deviceId&vhostDomian&ingestName three input conditions to locate an accurate ingest, so as to prepare to obtain the relevant information of this Onvif device
+ see an example
```
curl -X GET "http://192.168.2.42:5800/FastUseful/GetOnvifMonitorInfoByIngest?deviceId=22364bc4-5134-494d-8249-51d06777fb7f&vhostDomain=__defaultvhost__&ingestName=192.168.2.164_Media1" -H "accept: */*"
```
```json
{
  "host": "192.168.2.164",
  "username": "",
  "password": "",
  "mediaSourceInfoList": [
    {
      "sourceToken": "VideoSource_1",
      "framerate": 25,
      "width": 1920,
      "height": 1080
    }
  ],
  "onvifProfileLimitList": [
    {
      "profileToken": "Profile_1",
      "mediaUrl": "rtsp://192.168.2.164:554/LiveMedia/ch1/Media1",
      "ptzMoveSupport": true,
      "absoluteMove": true,
      "relativeMove": true,
      "continuousMove": true
    },
    {
      "profileToken": "Profile_2",
      "mediaUrl": "rtsp://192.168.2.164:554/LiveMedia/ch1/Media2",
      "ptzMoveSupport": true,
      "absoluteMove": true,
      "relativeMove": true,
      "continuousMove": true
    }
  ],
  "isInited": true
}
```           
#### /FastUseful/GetStreamInfoByVhostIngestName
+ Obtain flow information through IngestName
+ see examples
```
curl -X GET "http://192.168.2.42:5800/FastUseful/GetStreamInfoByVhostIngestName?deviceId=22364bc4-5134-494d-8249-51d06777fb7f&vhostDomain=__defaultvhost__&ingestName=192.168.2.164_Media1" -H "accept: */*"
```
```json
{
  "deviceId": "22364bc4-5134-494d-8249-51d06777fb7f",
  "vhostDomain": "__defaultvhost__",
  "ingestName": "192.168.2.164_Media1",
  "liveStream": "/live/192.168.2.164_Media1",
  "app": "live",
  "stream": "192.168.2.164_Media1",
  "monitorType": "Onvif",
  "ipAddress": "192.168.2.164",
  "username": "",
  "password": ""
}
```
#### /FastUseful/GetAllIngestByDeviceId
+ Get a list of all Ingest instances
+ return will be List<VhostIngestConfClass?>

#### /FastUseful/OnOrOffVhostMinDelay
+ Set a Vhost to low-latency mode, or turn off the low-latency mode of a Vhost

#### /FastUseful/PtzZoomForGb28181
+ Control the focal length of GB28181 equipment (that is, zoom in and zoom out control)
+ This is a bit important, let me explain
+ Enter a class by HttpPost call method, as follows
```json
{
  "deviceId": "string",
  "stream": "string",
  "ptzZoomDir": "MORE",
  "speed": 0,
  "stop": true
}
```
+ deviceId specifies which SRS instance
+ Stream specifies which GB28181 device to control, here directly use the stream id as the device id
+ ptzZoomDir indicates whether to zoom in or zoom out, More zooms in, Less zooms out
+ speed indicates the speed during operation
+ stop, stop information, if it is true, no action will be taken, if it is false, the action of the above parameters will be executed
+ There is a ptzMove operation in the back, which is similar to the Zoom operation

#### /FastUseful/PtzMoveForGb28181
+ Control GB28181 device PTZ movement
+ Same as /FastUseful/PtzZoomForGb28181, also provide an input class
```json
{
  "deviceId": "string",
  "stream": "string",
  "ptzMoveDir": "UP",
  "speed": 0,
  "stop": true
}
```
+ Other parameters are the same as the zoom control, the difference is that ptzmovedir needs to be passed in here, and up, down, left, right are supported

#### /FastUseful/GetClientInfoByStreamValue
+ Obtain client information. StreamNode will maintain a list of currently online clients. Clients refer to camera streams, user playback, streaming devices, etc. These are all srs clients
+ Obtain information about the stream through the stream tag
+ see an example
```
curl -X GET "http://192.168.2.42:5800/FastUseful/GetClientInfoByStreamValue?stream=192.168.2.164_Media1" -H "accept: */*"
```
```json
{
  "id": 1693,
  "device_Id": "22364bc4-5134-494d-8249-51d06777fb7f",
  "monitorIp": "192.168.2.164",
  "client_Id": 20237,
  "clientIp": "127.0.0.1",
  "clientType": "Monitor",
  "monitorType": "Onvif",
  "rtmpUrl": "rtmp://127.0.0.1:1935/live",
  "httpUrl": "",
  "rtspUrl": "rtsp://192.168.2.164:554/LiveMedia/ch1/Media1",
  "vhost": "__defaultVhost__",
  "app": "live",
  "stream": "192.168.2.164_Media1",
  "param": "",
  "isOnline": true,
  "updateTime": "2020-06-18 09:20:46",
  "isPlay": false,
  "pageUrl": null
}
```
+ client_id is the client_id provided by srs, which can be used to kick a certain client
+ clientType marks whether it is a camera, a playback user, or a streaming user
+ monitorType If it is a camera, this field will be used to mark whether it is an onvif device or a gb28181 device
+ isOnline marks whether the device is online
+ isPlay marks whether the user is watching the playback
+ Other fields are not explained

#### /FastUseful/GetRunningSrsInfoList
+ Get the list of running Srs instances and return List<SrsManager?>

#### /FastUseful/StopAllSrs
+ Stop all running instances of Srs
+ Normal stop returns true, otherwise returns the reason for the exception

#### /FastUseful/InitAndStartAllSrs
+ Initialize and start running all Srs instances

#### /FastUseful/KickoffClient
+ Kick a client, client_id is needed here to specify which client to kick

#### /FastUseful/GetStreamStatusById
+ get stream status
+ See examples to understand
```
curl -X GET "http://192.168.2.42:5800/FastUseful/GetStreamStatusById?deviceId=22364bc4-5134-494d-8249-51d06777fb7f&streamId=36408" -H "accept: */*"
```
```json
{
  "code": 0,
  "server": 36405,
  "stream": {
    "id": 36408,
    "name": "chid43590668",
    "vhost": 36406,
    "app": "live",
    "live_ms": 1592460310443,
    "clients": 1,
    "frames": 2188496,
    "send_bytes": 33635407400891344,
    "recv_bytes": 33495078562309496,
    "kbps": {
      "recv_30s": 4064,
      "send_30s": 0
    },
    "publish": {
      "active": "true",
      "cid": 429
    },
    "video": {
      "codec": "H264",
      "profile": "High",
      "level": "4",
      "width": 1920,
      "height": 1088
    },
    "audio": null
  }
}
```
+ streamid is an id from srs, which can be obtained by 
​/FastUseful​/GetStreamListStatusByDeviceId interface to obtain

#### /FastUseful​/GetStreamListStatusByDeviceId
+ The function of the above interface is the same, the above interface is to obtain the state information of a stream, and this one is to obtain all the stream states in a certain srs through deviceId

#### /FastUseful/GetVhostStatusById
+ Get status information of vhost
+ Same as the status information interface of stream, this is the information from inside srs, so the vhostid inside srs needs to be used
+ see examples
```
curl -X GET "http://192.168.2.42:5800/FastUseful/GetVhostStatusById?deviceId=22364bc4-5134-494d-8249-51d06777fb7f&vhostId=36406" -H "accept: */*"
```
```json
{
  "code": 0,
  "server": 36405,
  "vhost": {
    "id": 36406,
    "name": "__defaultVhost__",
    "enabled": "true",
    "clients": 8,
    "streams": 8,
    "send_bytes": 272742742452577120,
    "recv_bytes": 271760350059237470,
    "kbps": {
      "recv_30s": 25324,
      "send_30s": 0
    },
    "hls": {
      "enabled": "false"
    }
  }
}
```
#### /FastUseful/GetVhostListStatusByDeviceId
+ Same as GetStreamListStatusByDeviceId and /FastUseful/GetStreamStatusById, this interface gets the status information of all Vhosts

#### /FastUseful/GetOnOnlinePlayerByDeviceId
+ Get a list of users who are streaming (watching)
+ see examples
```
curl -X GET "http://192.168.2.42:5800/FastUseful/GetOnOnlinePlayerByDeviceId?deviceId=22364bc4-5134-494d-8249-51d06777fb7f" -H "accept: */*"
```
```json
[
  {
    "id": 1706,
    "device_Id": "22364bc4-5134-494d-8249-51d06777fb7f",
    "monitorIp": "192.168.2.164",
    "client_Id": 25211,
    "clientIp": "192.168.2.129",
    "clientType": "User",
    "monitorType": null,
    "rtmpUrl": null,
    "httpUrl": "",
    "rtspUrl": null,
    "vhost": "__defaultVhost__",
    "app": "live",
    "stream": "192.168.2.164_Media1",
    "param": null,
    "isOnline": true,
    "updateTime": "2020-06-18 14:10:52",
    "isPlay": true,
    "pageUrl": "http://localhost:9528/stream-node/controller/node/3/srs/22364bc4-5134-494d-8249-51d06777fb7f/stream"
  },
  {
    "id": 1707,
    "device_Id": "22364bc4-5134-494d-8249-51d06777fb7f",
    "monitorIp": "192.168.2.164",
    "client_Id": 25212,
    "clientIp": "192.168.2.129",
    "clientType": "User",
    "monitorType": null,
    "rtmpUrl": null,
    "httpUrl": "",
    "rtspUrl": null,
    "vhost": "__defaultVhost__",
    "app": "live",
    "stream": "34020000002220000001@34020000001360000002",
    "param": null,
    "isOnline": true,
    "updateTime": "2020-06-18 14:10:52",
    "isPlay": true,
    "pageUrl": "http://localhost:9528/stream-node/controller/node/3/srs/22364bc4-5134-494d-8249-51d06777fb7f/stream"
  }
]
```

#### /FastUseful/GetOnOnlinePlayer
+ Get all stream users

#### /FastUseful/GetOnPublishMonitorListById
+ Get a list of all devices that are streaming, need DeviceId

#### /FastUseful/GetOnPublishMonitorList
+ Obtain a list of all devices that are pushing streams, no need for DeviceId, get the contents of all Srs instances managed by StreamNode

#### /FastUseful/GetOnPublishMonitorById
+ Get a device that is streaming, through its ID, support multiple IDs, separated by spaces

#### /FastUseful/GetOnvifMonitorIngestTemplate
+ Obtain an Ingest template for adding onvif devices
+ see an example
```
curl -X GET "http://192.168.2.42:5800/FastUseful/GetOnvifMonitorIngestTemplate?username=user&password=%20password&rtspUrl=rtsp%3A%2F%2F192.168.2.164%3A554%2FLiveMedia%2Fch1%2FMedia1" -H "accept: */*"
```
```json
{
  "ingestName": "192.168.2.164_media1",
  "enabled": true,
  "input": {
    "type": "stream",
    "url": "rtsp://user: password@192.168.2.164:554/LiveMedia/ch1/Media1"
  },
  "ffmpeg": "./ffmpeg",
  "engines": [
    {
      "enabled": true,
      "perfile": {
        "re": "re;",
        "rtsp_transport": "tcp"
      },
      "iformat": null,
      "vfilter": null,
      "vcodec": "copy",
      "vbitrate": null,
      "vfps": null,
      "vwidth": null,
      "vheight": null,
      "vthreads": null,
      "vprofile": null,
      "vpreset": null,
      "vparams": null,
      "acodec": "copy",
      "abitrate": null,
      "asample_rate": null,
      "achannels": null,
      "aparams": null,
      "oformat": null,
      "output": "rtmp://127.0.0.1/live/192.168.2.164_media1",
      "engineName": null,
      "instanceName": null
    }
  ],
  "instanceName": "192.168.2.164_media1"
}

```
+ The system automatically generates an ingest template, insert this ingest template with the relevant interface in VhostIngest, and you can get a streaming engine

### Recording plan related-DvrPlan
+ Provide relevant interfaces related to recording

#### /DvrPlan/CutOrMergeVideoFile
+ Trim or merge video files
+ It is possible to merge and crop the existing video files of a certain camera (stream) according to time
+ Support cropping and merging at the second level, regardless of the video duration interval recorded by srs
+ Request is HttpPost
+ Request structure:
```json
{
  "startTime": "2020-06-18T07:11:55",
  "endTime": "2020-06-18T07:11:55",
  "deviceId": "string",
  "app": "string",
  "vhostDomain": "string",
  "stream": "string",
  "callbackUrl": "string"
}
```
+ deviceId&app&vhostDomain&stream is uniquely assigned to a stream
+ startTime&endTime, the start and end time of merging or cropping
+ callbackUrl is the callback address, after the clipping or merging service is completed, the result will be called back to the application through callbackUrl
+ Precautions:
1. This interface can return the result synchronously or asynchronously. If you do not write callbackurl and the time interval between starttime and endtime is less than 10 minutes, the result will be returned synchronously.
   Otherwise, the interface will return the result asynchronously
2. Before the result is returned asynchronously, the synchronous request will generate a taskId, and return the video list information involved in merging or cropping to the caller, and when the task is completed, the corresponding information will be returned to the caller through callbackUrl
3. The caller uses taskId to distinguish between different tasks
4. Since the cropping and merging operations are very time-consuming tasks, synchronous processing will be returned only when the maximum required time is less than 10 minutes. It is recommended that all operations be performed in an asynchronous callback mode
#### /DvrPlan/UndoSoftDelete
+ Recover soft deleted video files
+ There are two ways to delete video files, hard delete and soft delete
+ Hard delete will directly delete the video file and mark the file as deleted in the database
+ Soft delete only marks the file as deleted in the database and actually deletes the video file after 24 hours
+ Therefore, soft deleted video files have a chance to recover by calling the interface within 24 hours
+ Undelete is to set the database delete mark back to normal, so that the delete thread will not process this file

#### /DvrPlan/HardDeleteDvrVideoById
+ Hard delete a video file (immediate deletion)

#### /DvrPlan/SoftDeleteDvrVideoById
+ Soft delete a social video file (delete after 24 hours)

#### /DvrPlan/GetDvrVideoList
+ Get video file list
+ HttpPost request
+ The request result is as follows
```json
{
  "pageIndex": 0,
  "pageSize": 0,
  "includeDeleted": true,
  "startTime": "2020-06-18T07:31:20.114Z",
  "endTime": "2020-06-18T07:31:20.114Z",
  "orderBy": [
    {
      "fieldName": "string",
      "orderByDir": "ASC"
    }
  ],
  "deviceId": "string",
  "vhostDomain": "string",
  "app": "string",
  "stream": "string"
}
```
+ pageIndex, pageSize is the paging parameter, set null for no paging, pageIndex should start from 1
+ The interface can return up to 10,000 pieces of data at a time
+ includeDeleted indicates whether to include deleted file records in the returned data
+ startTime&endTime, indicating the time range to get the video file
+ orderBy which field to sort, and the sorting method, orderBy is a List<Orderby?>, which can have multiple fields
+ deviceId, vhostDomain, app, Stream are the only conditions to specify a video file of a stream
+ see an example
```
curl -X POST "http://192.168.2.42:5800/DvrPlan/GetDvrVideoList" -H "accept: */*" -H "Content-Type: application/json" -d "{\"pageIndex\":1,\"pageSize\":2,\"includeDeleted\":true,\"orderBy\":[{\"fieldName\":\"starttime\",\"orderByDir\":\"ASC\"}],\"deviceId\":\"\",\"vhostDomain\":\"\",\"app\":\"\",\"stream\":\"\"}"
```
```json
{
  "dvrVideoList": [
    {
      "id": 1,
      "device_Id": "22364bc4-5134-494d-8249-51d06777fb7f",
      "client_Id": 801,
      "clientIp": "192.168.2.164",
      "clientType": "Monitor",
      "monitorType": "Onvif",
      "videoPath": "/root/StreamNode/22364bc4-5134-494d-8249-51d06777fb7f/wwwroot/dvr/20200613/__defaultVhost__/live/192.168.2.164_Media1/19/20200613192745.mp4",
      "fileSize": 22619964,
      "vhost": "__defaultVhost__",
      "dir": "/root/StreamNode/22364bc4-5134-494d-8249-51d06777fb7f/wwwroot/dvr/20200613/__defaultVhost__/live/192.168.2.164_Media1/19",
      "stream": "192.168.2.164_Media1",
      "app": "live",
      "duration": 121200,
      "startTime": "2020-06-13 19:27:55",
      "endTime": "2020-06-13 19:29:56",
      "param": "",
      "deleted": false,
      "updateTime": "2020-06-13 19:29:56",
      "recordDate": "2020-06-13"
    },
    {
      "id": 2,
      "device_Id": "22364bc4-5134-494d-8249-51d06777fb7f",
      "client_Id": 801,
      "clientIp": "192.168.2.164",
      "clientType": "Monitor",
      "monitorType": "Onvif",
      "videoPath": "/root/StreamNode/22364bc4-5134-494d-8249-51d06777fb7f/wwwroot/dvr/20200613/__defaultVhost__/live/192.168.2.164_Media1/19/20200613192956.mp4",
      "fileSize": 21536369,
      "vhost": "__defaultVhost__",
      "dir": "/root/StreamNode/22364bc4-5134-494d-8249-51d06777fb7f/wwwroot/dvr/20200613/__defaultVhost__/live/192.168.2.164_Media1/19",
      "stream": "192.168.2.164_Media1",
      "app": "live",
      "duration": 120330,
      "startTime": "2020-06-13 19:29:56",
      "endTime": "2020-06-13 19:31:56",
      "param": "",
      "deleted": false,
      "updateTime": "2020-06-13 19:31:56",
      "recordDate": "2020-06-13"
    }
  ],
  "request": {
    "pageIndex": 1,
    "pageSize": 2,
    "includeDeleted": true,
    "startTime": null,
    "endTime": null,
    "orderBy": [
      {
        "fieldName": "starttime",
        "orderByDir": "ASC"
      }
    ],
    "deviceId": "",
    "vhostDomain": "",
    "app": "",
    "stream": ""
  },
  "total": 3117
}
```
#### /DvrPlan/DeleteDvrPlanById
+ delete a recording plan

#### /DvrPlan/OnOrOffDvrPlanById
+ Enable or disable a recording schedule

#### /DvrPlan/SetDvrPlanById
+ Set up a recording schedule
+ Note that each time the setting is to delete the old recording plan (according to the ID), and then write the new recording plan, so please note: the auto-increment ID of the database will change

#### /DvrPlan/CreateDvrPlan
+ Create a recording schedule
+ HttpPost, submit a structure
+ Let's see an example, the following is
```json
{
  "enable": true,
  "deviceId": "string",
  "vhostDomain": "string",
  "app": "string",
  "stream": "string",
  "limitSpace": 0,
  "limitDays": 0,
  "overStepPlan": "StopDvr",
  "timeRangeList": [
    {
      "streamDvrPlanId": 0,
      "weekDay": "Sunday",
      "startTime": "2020-06-18T07:41:31.578Z",
      "endTime": "2020-06-18T07:41:31.578Z"
    }
  ]
}
```
+ enable Whether to execute for this scheme
+ deviceId&vhostDomain&app&stream uniquely specifies a stream
+ limitSpace the space limit (size of all files) after this stream is recorded
+ limitDays time limit (number of days) after this stream has been recorded
+ How to deal with overStepPlan exceeding the time limit or exceeding the space limit, (StopDvr: stop recording DeleteFile: delete file)
+ When limitSapce exceeds the limit, files will be deleted one by one, and when limitDays exceeds the limit, video files will be deleted day by day (note: this is hard delete)
+ timeRangeList recording enabled time range
+ The structure of timeRange:
```json
{
      "streamDvrPlanId": 0,
      "weekDay": "Sunday",
      "startTime": "2020-06-18T07:41:31",
      "endTime": "2020-06-18T07:41:31"
    }
```
+ Just leave streamDvrPlanId empty when modifying, adding, deleting, etc.
+ weekDay indicates the day of the week
+ startTime&endTime means from what time to what time, the date can be specified at will, and the interface will only take the time in the end, which can be accurate to the second

#### /DvrPlan/GetDvrPlan
+ Get a recording schedule

### Authentication Interface-Allow
+ Authenticate webapi access
#### /Allow/RefreshSession
+ Refresh Session
+ All interface calls, except individual interfaces, require Session
+ The following is the request structure, which requires allowkey, refreshCode, and current sessionCode
+ expires can be ignored
```json
{
  "allowKey": "string",
  "refreshCode": "string",
  "sessionCode": "string",
  "expires": 0
}
```

#### /Allow/GetSession
+ Get a Session
```json
{
  "allowKey": "string"
}
``` 
+ It is necessary to obtain a session through AllowKey, which can be used to access various apis while the session is valid. Before the session expires, it is necessary to receive a new session through the RefreshSession interface, and use the new session for communication

#### /Allow/SetAllowByKey
+ Set an allowKey parameter
```json
{
  "password": "string",
  "allowkey": {
    "key": "string",
    "ipArray": [
      "string"
    ]
  }
}
```
+ password is the password in the configuration file

#### /Allow/DelAllowByKey
+ Delete an AllowKey
+ password is the password in the configuration file

#### /Allow/AddAllow
+ add an allowKey
+ password is the password in the configuration file

#### /Allow/GetAllows
+ Get allowKey list
+ password is the password in the configuration file

### Onvif related interface - Onvif
+ Provide functions such as detection and discovery control of onvif devices

#### /Onvif/InitAll
+ Initialize all uninitialized onvif devices

#### /Onvif/InitByIpAddress
+ Initialize the onvif device through the ip address

#### /Onvif/SetPtzZoom
+ Adjust the focal length of onvif equipment (zoom in/zoom out is zoomin|zoomout operation)
+ Request structure:
```json
{
  "ipAddr": "string",
  "profileToken": "string",
  "zoomDir": "MORE"
}
```
+ zoomDir is More to zoom in, and Less to zoom out
+ profileToken is the token of the onvif device, the following interfaces can be obtained

#### /Onvif/GetPtzPosition
+ Get the current x, y, z coordinate position of the onvif device

#### /Onvif/PtzKeepMoveStop
+ Stop continuous movement of onvif devices

#### /Onvif/PtzMove
+ Control the PTZ movement of onvif devices
+ Request structure:
```json
{
  "ipAddr": "string",
  "profileToken": "string",
  "moveDir": "UP",
  "moveType": "RELATIVE"
}
```
+ moveDir is the moving direction of the gimbal, including UP, DOWN, LEFT, RIGHT, UPLEFT, UPRIGHT, DOWNLEFT, DOWNRIGHT, 4 more directions than gb28181
+ moveType is the way the gimbal moves, there are RELATIVE, KEEP relative position movement and continuous movement
#### /Onvif/InitMonitor
+ detect and initialize onvif devices
+ Request structure:
```
{
  "ipAddrs": "string",
  "username": "string",
  "password": "string"
}
```
+ ip, user name, password to detect onvif camera, ipaddrs can have multiple separated by spaces
+ return structure (probe result)
```json
[
  {
    "host": "192.168.2.164",
    "username": "",
    "password": "",
    "mediaSourceInfoList": [
      {
        "sourceToken": "VideoSource_1",
        "framerate": 25,
        "width": 1920,
        "height": 1080
      }
    ],
    "onvifProfileLimitList": [
      {
        "profileToken": "Profile_1",
        "mediaUrl": "rtsp://192.168.2.164:554/LiveMedia/ch1/Media1",
        "ptzMoveSupport": true,
        "absoluteMove": true,
        "relativeMove": true,
        "continuousMove": true
      },
      {
        "profileToken": "Profile_2",
        "mediaUrl": "rtsp://192.168.2.164:554/LiveMedia/ch1/Media2",
        "ptzMoveSupport": true,
        "absoluteMove": true,
        "relativeMove": true,
        "continuousMove": true
      }
    ],
    "isInited": true
  },
  {
    "host": "192.168.2.163",
    "username": "",
    "password": "",
    "mediaSourceInfoList": null,
    "onvifProfileLimitList": null,
    "isInited": false
  }
]
```
#### /Onvif​/GetMonitorList
+ Get a list of onvif devices (with information)
#### /Onvif/GetMonitor
+ Get onvif device instance according to ip
+ see an example
```
curl -X GET "http://192.168.2.42:5800/Onvif/GetMonitor?ipAddress=192.168.2.164" -H "accept: */*"
```
```json
{
  "host": "192.168.2.164",
  "username": "",
  "password": "",
  "mediaSourceInfoList": [
    {
      "sourceToken": "VideoSource_1",
      "framerate": 25,
      "width": 1920,
      "height": 1080
    }
  ],
  "onvifProfileLimitList": [
    {
      "profileToken": "Profile_1",
      "mediaUrl": "rtsp://192.168.2.164:554/LiveMedia/ch1/Media1",
      "ptzMoveSupport": true,
      "absoluteMove": true,
      "relativeMove": true,
      "continuousMove": true
    },
    {
      "profileToken": "Profile_2",
      "mediaUrl": "rtsp://192.168.2.164:554/LiveMedia/ch1/Media2",
      "ptzMoveSupport": true,
      "absoluteMove": true,
      "relativeMove": true,
      "continuousMove": true
    }
  ],
  "isInited": true
}
```

### SRS Global Interface - GlobalSrs
+ Provide an interface for srs control and global parameter modification
#### GlobalSrs/IsRunning
+ Call method: HttpGet
+ Interface function: detect whether the Srs instance is running.
+ Input parameter: deviceId: string
+ Output parameters: true|false: bool|ExceptStruct
#### GlobalSrs/IsInit
+ Call method: HttpGet
+ Interface function: detect whether the Srs instance configuration file is loaded and initialized.
+ Input parameter: deviceId: string
+ Output parameters: true|false: bool|ExceptStruct
#### GlobalSrs/StartSrs
+ Call method: HttpGet
+ Interface function: used to start a Srs instance process (start the srs program ./srs -c config.conf)
+ Input parameter: deviceId: string
+ Output parameters: true|false: bool|ExceptStruct
#### GlobalSrs/StopSrs
+ Call method: HttpGet
+ Interface function: stop the srs process, end the service of srs
+ Input parameter: deviceId: string
+ Output parameters: true|false: bool|ExceptStruct
#### GlobalSrs/RestartSrs
+ Call method: HttpGet
+ Interface function: restart the Srs instance process, the internal logic first SrsStop, then SrsStart
+ Input parameter: deviceId: string
+ Output parameters: true|false: bool|ExceptStruct
#### GlobalSrs/ReloadSrs
+ Call method: HttpGet
+ Interface function: reload the Srs configuration file (hot loading, without stopping the Srs process service) send a SIGHUP signal to the process kill -s SIGHUP pid
+ Input parameter: deviceId: string
+ Output parameters: true|false: bool|ExceptStruct
#### /GlobalSrs/ChangeGlobalParams
+ Call method: Post
+ Interface function: modify the global parameters of Srs
+ Input parameters:
```json
{
  "deviceId": "string", //Srs instance id
  "gm": {
    "heartbeatEnable": true, //whether to enable srs heartbeat
    "heartbeatSummariesEnable": true, //Whether to bring system statistics information when srs heartbeat
    "heartbeatUrl": "string", //srs heartbeat sending url address (the application can take over this address, and it is taken over by StreamNode by default)
    "httpApiEnable": true, //Whether to enable the httpapi of srs, this must be enabled, it needs to be used in StreamNode
    "httpApiListen": 0, //srs httpapi listening interface
    "httpServerEnable": true, // Whether to enable the httpServer of srs, it is recommended to enable
    "httpServerListen": 0, //srs httpserver listening port
    "httpServerPath": "string", //srs's httpserver release directory is equivalent to nginx's wwwroot
    "listen": 0, //srs rtmp listening port, default 1935
    "maxConnections": 0 //The maximum number of connections for srs, default 1000 for linux system, 128 for macos system
  }
}
```
+ Output parameters: true|false: bool|ExceptStruct
+ Note: Do not change this parameter casually

### System interface - System
+ Provide various interfaces at the system and StremNode levels
#### /System/RefreshSrsObject
+ Call method: HttpGet
+ Interface function: write the Srs configuration information in the memory into the corresponding Srs instance configuration file, and send a configuration refresh command to Srs, so that Srs runs in the environment of the refreshed configuration information
+ Input parameter: deviceId: string
+ Output parameters: true|false: bool|ExceptStruct
#### /System/GetAllSrsManagerDeviceId
+ Call method: HttpGet
+ Interface function: get all Srs instance device IDs managed by StreamNode
+ Input parameters: none
+ Output parameter: List<string?>|ExceptStruct
```json
[
  "22364bc4-5134-494d-8249-51d06777fb7f"
]
```
#### /System/CreateNewSrsInstance
+ Call method: HttpPost
+ Interface function: create a new Srs instance
+ Input parameters:
<details>
<summary>Expand to view</summary>
<pre><code>
{
	"srs": {
		"rtc_server": {
			"enabled": true,
			"listen": 0,
			"candidate": "string",
			"ecdsa": true,
			"sendmmsg": 0,
			"encrypt": true,
			"reuseport": 0,
			"merge_nalus": true,
			"gso": true,
			"padding": 0,
			"perf_stat": true,
			"queue_length": 0,
			"black_hole": {
				"enabled": true,
				"publisher": "string"
			}
		},
		"tcmalloc_release_rate": 0,
		"listen": 0,
		"pid": "string",
		"chunk_size": 0,
		"ff_log_dir": "string",
		"ff_log_level": "string",
		"srs_log_tank": "string",
		"srs_log_level": "string",
		"srs_log_file": "string",
		"max_connections": 0,
		"daemon": true,
		"utc_time": true,
		"pithy_print_ms": 0,
		"work_dir": "string",
		"asprocess": true,
		"empty_ip_ok": true,
		"grace_start_wait": 0,
		"grace_final_wait": 0,
		"force_grace_quit": true,
		"disable_daemon_for_docker": true,
		"inotify_auto_reload": true,
		"auto_reload_for_docker": true,
		"heartbeat": {
			"enabled": true,
			"interval": 0,
			"url": "string",
			"device_id": "string",
			"summaries": true,
			"instanceName": "string"
		},
		"stats": {
			"network": 0,
			"disk": "string"
		},
		"http_api": {
			"enabled": true,
			"listen": 0,
			"crossdomain": true,
			"raw_Api": {
				"enabled": true,
				"allow_reload": true,
				"allow_query": true,
				"allow_update": true
			},
			"instanceName": "string"
		},
		"http_server": {
			"enabled": true,
			"listen": 0,
			"dir": "string",
			"crossdomain": true,
			"instanceName": "string"
		},
		"stream_casters": [{
			"sip": {
				"enabled": true,
				"listen": 0,
				"serial": "string",
				"realm": "string",
				"ack_timeout": 0,
				"keepalive_timeout": 0,
				"auto_play": true,
				"invite_port_fixed": true,
				"query_catalog_interval": 0
			},
			"auto_create_channel": true,
			"enabled": true,
			"caster": "mpegts_over_udp",
			"output": "string",
			"listen": 0,
			"rtp_port_min": 0,
			"rtp_port_max": 0,
			"host": "string",
			"audio_enable": true,
			"wait_keyframe": true,
			"rtp_idle_timeout": 0,
			"instanceName": "string"
		}],
		"srt_server": {
			"default_app": "string",
			"enabled": true,
			"listen": 0,
			"maxbw": 0,
			"connect_timeout": 0,
			"peerlatency": 0,
			"recvlatency": 0,
			"instanceName": "string"
		},
		"kafka": {
			"enabled": true,
			"brokers": "string",
			"topic": "string",
			"instanceName": "string"
		},
		"vhosts": [{
			"vnack": {
				"enabled": true
			},
			"instanceName": "string",
			"vhostDomain": "string",
			"enabled": true,
			"min_latency": true,
			"tcp_nodelay": true,
			"chunk_size": 0,
			"in_ack_size": 0,
			"out_ack_size": 0,
			"rtc": {
				"enabled": true,
				"bframe": "string",
				"acc": "string",
				"stun_timeout": 0,
				"stun_strict_check": true
			},
			"vcluster": {
				"mode": "string",
				"origin": "string",
				"token_traverse": true,
				"vhost": "string",
				"debug_srs_upnode": true,
				"origin_cluster": true,
				"coworkers": "string",
				"instanceName": "string"
			},
			"vforward": {
				"enabled": true,
				"destination": "string"
			},
			"vplay": {
				"mw_msgs": 0,
				"gop_cache": true,
				"queue_length": 0,
				"time_jitter": "full",
				"atc": true,
				"mix_correct": true,
				"atc_auto": true,
				"mw_latency": 0,
				"send_min_interval": 0,
				"reduce_sequence_header": true
			},
			"vpublish": {
				"mr": true,
				"mr_latency": 0,
				"firstpkt_timeout": 0,
				"normal_timeout": 0,
				"parse_sps": true,
				"instanceName": "string"
			},
			"vrefer": {
				"enabled": true,
				"all": "string",
				"publish": "string",
				"play": "string",
				"instanceName": "string"
			},
			"vbandcheck": {
				"enabled": true,
				"key": "string",
				"interval": 0,
				"limit_kbps": 0
			},
			"vsecurity": {
				"enabled": true,
				"seo": [{
					"sem": "allow",
					"set": "publish",
					"rule": "string"
				}]
			},
			"vhttp_static": {
				"enabled": true,
				"mount": "string",
				"dir": "string"
			},
			"vhttp_remux": {
				"enabled": true,
				"fast_cache": 0,
				"mount": "string",
				"hstrs": true
			},
			"vhttp_hooks": {
				"enabled": true,
				"on_connect": "string",
				"on_close": "string",
				"on_publish": "string",
				"on_unpublish": "string",
				"on_play": "string",
				"on_stop": "string",
				"on_dvr": "string",
				"on_hls": "string",
				"on_hls_notify": "string"
			},
			"vexec": {
				"enabled": true,
				"publish": "string"
			},
			"vdash": {
				"enabled": true,
				"dash_fragment": 0,
				"dash_update_period": 0,
				"dash_timeshift": 0,
				"dash_path": "string",
				"dash_mpd_file": "string"
			},
			"vhls": {
				"enabled": true,
				"hls_fragment": 0,
				"hls_td_ratio": 0,
				"hls_aof_ratio": 0,
				"hls_window": 0,
				"hls_on_error": "string",
				"hls_path": "string",
				"hls_m3u8_file": "string",
				"hls_ts_file": "string",
				"hls_ts_floor": true,
				"hls_entry_prefix": "string",
				"hls_acodec": "string",
				"hls_vcodec": "string",
				"hls_cleanup": true,
				"hls_dispose": 0,
				"hls_nb_notify": 0,
				"hls_wait_keyframe": true,
				"hls_keys": true,
				"hls_fragments_per_key": 0,
				"hls_key_file": "string",
				"hls_key_file_path": "string",
				"hls_key_url": "string",
				"hls_dts_directly": true
			},
			"vhds": {
				"enabled": true,
				"hds_fragment": 0,
				"hds_window": 0,
				"hds_path": "string"
			},
			"vdvr": {
				"enabled": true,
				"dvr_apply": "string",
				"dvr_plan": "string",
				"dvr_path": "string",
				"dvr_duration": 0,
				"dvr_wait_keyframe": true,
				"time_Jitter": "full"
			},
			"vingests": [{
				"ingestName": "string",
				"enabled": true,
				"input": {
					"type": "file",
					"url": "string"
				},
				"ffmpeg": "string",
				"engines": [{
					"enabled": true,
					"perfile": {
						"re": "string",
						"rtsp_transport": "string"
					},
					"iformat": "off",
					"vfilter": {
						"i": "string",
						"vf": "string",
						"filter_Complex": "string"
					},
					"vcodec": "string",
					"vbitrate": 0,
					"vfps": 0,
					"vwidth": 0,
					"vheight": 0,
					"vthreads": 0,
					"vprofile": "high",
					"vpreset": "medium",
					"vparams": {
						"t": 0,
						"coder": 0,
						"b_strategy": 0,
						"bf": 0,
						"refs": 0
					},
					"acodec": "string",
					"abitrate": 0,
					"asample_rate": 0,
					"achannels": 0,
					"aparams": {
						"profile_a": "string",
						"bsf_a": "string"
					},
					"oformat": "off",
					"output": "string",
					"engineName": "string",
					"instanceName": "string"
				}],
				"instanceName": "string"
			}],
			"vtranscodes": [{
				"enabled": true,
				"ffmpeg": "string",
				"engines": [{
					"enabled": true,
					"perfile": {
						"re": "string",
						"rtsp_transport": "string"
					},
					"iformat": "off",
					"vfilter": {
						"i": "string",
						"vf": "string",
						"filter_Complex": "string"
					},
					"vcodec": "string",
					"vbitrate": 0,
					"vfps": 0,
					"vwidth": 0,
					"vheight": 0,
					"vthreads": 0,
					"vprofile": "high",
					"vpreset": "medium",
					"vparams": {
						"t": 0,
						"coder": 0,
						"b_strategy": 0,
						"bf": 0,
						"refs": 0
					},
					"acodec": "string",
					"abitrate": 0,
					"asample_rate": 0,
					"achannels": 0,
					"aparams": {
						"profile_a": "string",
						"bsf_a": "string"
					},
					"oformat": "off",
					"output": "string",
					"engineName": "string",
					"instanceName": "string"
				}],
				"instanceName": "string"
			}]
		}],
		"configLines": [
			"string"
		],
		"streamNodeIpAddr": "string",
		"streamNodPort": 0,
		"deviceId": "string",
		"configLinesTrim": [
			"string"
		],
		"confFilePath": "string"
	},
	"srsConfigPath": "string",
	"srsDeviceId": "string",
	"srsWorkPath": "string",
	"srsPidValue": "string",
	"isStopedByUser": true
}
</code></pre>
</details>

+ Output parameters: SrsManage|null|ExceptStruct
+ Note: If it is newly created normally, it will return the SrsManager object, which is basically consistent with the incoming parameters

#### /System/GetSrsInstanceTemplate
+ Call method: HttpGet
+ Interface function: Get a template of SrsManager object, which can be used to create a new one, and the basic settings have been made in the template
+ Input parameters: none
+ Output parameter: object:SrsMansger|ExceptStruct

<details>
<summary>Expand to view</summary>
<pre><code>
{
  "srs": {
    "rtc_server": null,
    "tcmalloc_release_rate": null,
    "listen": 1935,
    "pid": "/root/StreamNode/21629eba-3bcf-42b0-b37e-4502896dcbe1/srs.pid",
    "chunk_size": 6000,
    "ff_log_dir": "/root/StreamNode/21629eba-3bcf-42b0-b37e-4502896dcbe1/ffmpegLog/",
    "ff_log_level": "warning",
    "srs_log_tank": "file",
    "srs_log_level": "verbose",
    "srs_log_file": "/root/StreamNode/21629eba-3bcf-42b0-b37e-4502896dcbe1/srs.log",
    "max_connections": 1000,
    "daemon": true,
    "utc_time": false,
    "pithy_print_ms": null,
    "work_dir": "/root/StreamNode/",
    "asprocess": false,
    "empty_ip_ok": null,
    "grace_start_wait": 2300,
    "grace_final_wait": 3200,
    "force_grace_quit": false,
    "disable_daemon_for_docker": null,
    "inotify_auto_reload": false,
    "auto_reload_for_docker": null,
    "heartbeat": {
      "enabled": true,
      "interval": 5,
      "url": "http://127.0.0.1:5000/api/v1/heartbeat",
      "device_id": "\"21629eba-3bcf-42b0-b37e-4502896dcbe1\"", //The system automatically generates device_id, and all the content about this srs instance is related to device_id.
      "summaries": true,                                       //Two identical device_ids cannot exist in a StreamNode
      "instanceName": null
    },
    "stats": null,
    "http_api": {
      "enabled": true,
      "listen": 8000,
      "crossdomain": true,
      "raw_Api": null,
      "instanceName": ""
    },
    "http_server": {
      "enabled": true,
      "listen": 8001,
      "dir": "/root/StreamNode/21629eba-3bcf-42b0-b37e-4502896dcbe1/wwwroot",
      "crossdomain": true,
      "instanceName": null
    },
    "stream_casters": null,
    "srt_server": null,
    "kafka": null,
    "vhosts": [
      {
        "vnack": null,
        "instanceName": "__defaultVhost__",
        "vhostDomain": "__defaultVhost__",
        "enabled": null,
        "min_latency": null,
        "tcp_nodelay": null,
        "chunk_size": null,
        "in_ack_size": null,
        "out_ack_size": null,
        "rtc": null,
        "vcluster": null,
        "vforward": null,
        "vplay": null,
        "vpublish": null,
        "vrefer": null,
        "vbandcheck": null,
        "vsecurity": null,
        "vhttp_static": null,
        "vhttp_remux": null,
        "vhttp_hooks": null,
        "vexec": null,
        "vdash": null,
        "vhls": null,
        "vhds": null,
        "vdvr": null,
        "vingests": null,
        "vtranscodes": null
      }
    ],
    "configLines": null,
    "streamNodeIpAddr": null,
    "streamNodPort": null,
    "deviceId": null,
    "configLinesTrim": null,
    "confFilePath": null
  },
  "srsConfigPath": "",
  "srsDeviceId": "21629eba-3bcf-42b0-b37e-4502896dcbe1",
  "srsWorkPath": "/root/StreamNode/",
  "srsPidValue": "",
  "isInit": true,
  "isStopedByUser": false,
  "isRunning": false
}
</code></pre>
</details>

#### /System/DelSrsByDevId
+ Call method: HttpGet
+ Interface function: delete an srs instance, if the srs process is running, the system will stop the srs process and delete the configuration file corresponding to srs
+ Input parameter: deviceId: string
+ Output parameters: true|false: bool|ExceptStruct
#### /System/GetSrsInstanceByDeviceId
+ Call method: HttpGet
+ Interface function: get the configuration of a Srs instance through deviceId
+ Input parameter: deviceId: string
+ Output parameters: Object:SrsManager|ExceptStruct, see the previous description for the structure of SrsManager
#### /System/LoadOnvifConfig
+ Call method: HttpGet
+ Interface function: load the configuration file of the Onvif camera, the configuration file mainly stores the ip address, user name, password, rtsp stream address
+ Input parameters: none
+ Output parameters: true|false: bool|ExceptStruct
#### /System/WriteOnvifConfig
+ Call method: HttpGet
+ Interface function: write the Onvif device details in the memory to the configuration file, the configuration file mainly stores the ip address, user name, password, rtsp stream address
+ Input parameters: none
+ Output parameters: true|false: bool|ExceptStruct
#### /System/DelOnvifConfigByIpAddress
+ Call method: HttpGet
+ Interface function: Delete an Onvif device through the ip address of the Onvif device. After deletion, the onvif device list will be automatically reloaded, and the deleted object will be removed.
+ Input parameter: ipAddress: string
+ Output parameters: true|false: bool|ExceptStruct
#### /System/GetSystemInfo
+ Call method: HttpGet
+ Interface function: get system information
+ Input parameters: none
+ Output parameters: info:SystemInfoModule|ExceptStruct
```json
{
  "srsList": [
    {
      "version": "4.0.26",
      "pid": 12135,
      "ppid": 1,
      "argv": "/root/StreamNode/srs -c /root/StreamNode/22364bc4-5134-494d-8249-51d06777fb7f.conf",
      "cwd": "/root/StreamNode",
      "mem_kbyte": 81852,
      "mem_percent": 0,
      "cpu_percent": 0.09,
      "srs_uptime": 3540,
      "srs_DeviceId": "22364bc4-5134-494d-8249-51d06777fb7f"
    }
  ],
  "system": { //Note that if the Srs instance is not running, the system section is null, and the content of this section is obtained from the srs instance
    "cpu_percent": 0.02,
    "disk_read_KBps": 0,
    "disk_write_KBps": 0,
    "disk_busy_percent": 0,
    "mem_ram_kbyte": 8008448,
    "mem_ram_percent": 0.18,
    "mem_swap_kbyte": 16781308,
    "mem_swap_percent": 0,
    "cpus": 8,
    "cpus_online": 8,
    "uptime": 1056007.4,
    "ilde_time": 8292082.5,
    "load_1m": 0.23,
    "load_5m": 0.17,
    "load_15m": 0.15,
    "net_sample_time": 1592376245278,
    "net_recv_bytes": 0,
    "net_send_bytes": 0,
    "net_recvi_bytes": 2600853815680,
    "net_sendi_bytes": 1415715429273,
    "srs_sample_time": 1592376245278,
    "srs_recv_bytes": 8577771876,
    "srs_send_bytes": 87975,
    "conn_sys": 73,
    "conn_sys_et": 26,
    "conn_sys_tw": 17,
    "conn_sys_udp": 9,
    "conn_srs": 8
  },
  "networkInterfaceList": [
    {
      "index": 0,
      "name": "ens160",
      "mac": "01-0C-20-01-1B-60",
      "type": "Ethernet",
      "ipaddr": "192.168.2.42"
    },
    {
      "index": 1,
      "name": "docker0",
      "mac": "0A-42-37-98-C4-0F",
      "type": "Ethernet",
      "ipaddr": "172.17.0.1"
    },
    {
      "index": 2,
      "name": "br-14a99bbbd2d9",
      "mac": "02-40-9B-04-FC-3E",
      "type": "Ethernet",
      "ipaddr": "172.20.0.1"
    }
  ],
  "disksInfo": [
    {
      "devicePath": null,
      "path": "/",
      "size": 325713,
      "free": 260258,
      "format": "xfs",
      "volumeLabel": "/",
      "rootDirectory": "/"
    },
    {
      "devicePath": null,
      "path": "/dev",
      "size": 4088,
      "free": 4088,
      "format": "tmpfs",
      "volumeLabel": "/dev",
      "rootDirectory": "/dev"
    },
    {
      "devicePath": null,
      "path": "/dev/shm",
      "size": 4100,
      "free": 4100,
      "format": "tmpfs",
      "volumeLabel": "/dev/shm",
      "rootDirectory": "/dev/shm"
    },
    {
      "devicePath": null,
      "path": "/run",
      "size": 4100,
      "free": 3715,
      "format": "tmpfs",
      "volumeLabel": "/run",
      "rootDirectory": "/run"
    },
    {
      "devicePath": null,
      "path": "/sys/fs/cgroup",
      "size": 4100,
      "free": 4100,
      "format": "tmpfs",
      "volumeLabel": "/sys/fs/cgroup",
      "rootDirectory": "/sys/fs/cgroup"
    },
    {
      "devicePath": null,
      "path": "/",
      "size": 325713,
      "free": 260258,
      "format": "xfs",
      "volumeLabel": "/",
      "rootDirectory": "/"
    },
    {
      "devicePath": null,
      "path": "/boot",
      "size": 533,
      "free": 337,
      "format": "xfs",
      "volumeLabel": "/boot",
      "rootDirectory": "/boot"
    },
    {
      "devicePath": null,
      "path": "/var/lib/docker/overlay2/ec79f5cb0c9cdc370d5fd5fe75e23905e9b761d2c6d8b691525eb42f8fd1cf73/merged",
      "size": 325713,
      "free": 260258,
      "format": "overlay",
      "volumeLabel": "/var/lib/docker/overlay2/ec79f5cb0c9cdc370d5fd5fe75e23905e9b761d2c6d8b691525eb42f8fd1cf73/merged",
      "rootDirectory": "/var/lib/docker/overlay2/ec79f5cb0c9cdc370d5fd5fe75e23905e9b761d2c6d8b691525eb42f8fd1cf73/merged"
    },
    {
      "devicePath": null,
      "path": "/var/lib/docker/overlay2/c2b4cfeec86dcd9016b34fce83a08b98d8a905bec93f3d69a667a18ce9878fe7/merged",
      "size": 325713,
      "free": 260258,
      "format": "overlay",
      "volumeLabel": "/var/lib/docker/overlay2/c2b4cfeec86dcd9016b34fce83a08b98d8a905bec93f3d69a667a18ce9878fe7/merged",
      "rootDirectory": "/var/lib/docker/overlay2/c2b4cfeec86dcd9016b34fce83a08b98d8a905bec93f3d69a667a18ce9878fe7/merged"
    },
    {
      "devicePath": null,
      "path": "/var/lib/docker/containers/99a5d03de3fbf073f5480ed71543328cdec3df2cb9cb464c86487b85354b00cb/mounts/shm",
      "size": 67,
      "free": 67,
      "format": "tmpfs",
      "volumeLabel": "/var/lib/docker/containers/99a5d03de3fbf073f5480ed71543328cdec3df2cb9cb464c86487b85354b00cb/mounts/shm",
      "rootDirectory": "/var/lib/docker/containers/99a5d03de3fbf073f5480ed71543328cdec3df2cb9cb464c86487b85354b00cb/mounts/shm"
    },
    {
      "devicePath": null,
      "path": "/var/lib/docker/containers/cbd47bd483651421431dd16f8471bdc25ef4c4dbd5dbb8143923534187c923cf/mounts/shm",
      "size": 67,
      "free": 67,
      "format": "tmpfs",
      "volumeLabel": "/var/lib/docker/containers/cbd47bd483651421431dd16f8471bdc25ef4c4dbd5dbb8143923534187c923cf/mounts/shm",
      "rootDirectory": "/var/lib/docker/containers/cbd47bd483651421431dd16f8471bdc25ef4c4dbd5dbb8143923534187c923cf/mounts/shm"
    },
    {
      "devicePath": null,
      "path": "/run/user/0",
      "size": 820,
      "free": 820,
      "format": "tmpfs",
      "volumeLabel": "/run/user/0",
      "rootDirectory": "/run/user/0"
    }
  ],
  "platform": "linux",
  "architecture": "X64",
  "x64": true,
  "hostName": "localhost",
  "cpuCoreSize": 8,
  "version": "Linux 3.10.0-1127.10.1.el7.x86_64 #1 SMP Wed Jun 3 14:28:03 UTC 2020 Unix 3.10.0.1127"
}
```
#### /System​/GetSrsInstanceList
+ Call method: HttpGet
+ Function of the interface: get the list of Srs instances (simple item information)
+ Input parameters: none
+ Output parameter: object:List<SrsInstanceModule?>|ExceptStruct
```json
[
  {
    "deviceId": "22364bc4-5134-494d-8249-51d06777fb7f",
    "isInit": true,
    "isRunning": true,
    "configPath": "/root/StreamNode/22364bc4-5134-494d-8249-51d06777fb7f.conf",
    "pidValue": "12135",
    "srsProcessWorkPath": "/root/StreamNode/srs",
    "srsInstanceWorkPath": "/root/StreamNode/"
  }
]
```
### SRS configuration file operation API (not expanded in detail, classes and configuration files are mapped to each other, you can directly see the source code if necessary)
+ RtcServer

interface name|function|remark
--|:--:|--:
/RtcServer/GetSrsRtcServer|Get the relevant configuration of the Rtc service in Srs|Input is DeviceId
/RtcServer/SetRtcServer|Configure the Rtc service of Srs|Enter as DeviceId
/RtcServer/DelRtcServer|Delete the Rtc service in Srs|Enter as DeviceId

+ SrtServer

interface name|function|remark
--|:--:|--:
/SrtServer/GetSrtServer|Get the relevant configuration of the Srt service in Srs|Input is DeviceId
/SrtServer/SetSrtServer|Configure the Srt service of Srs|Enter as DeviceId
/SrtServer/DelSrtServer|Delete the Srt service in Srs|Enter as DeviceId

+ Stats

interface name|function|remark
--|:--:|--:
/Stats/GetSrsStats|Get the relevant configuration of the Stats service in Srs|Input is DeviceId
/Stats/SetSrsStats|Configure the Stats service of Srs|Enter as DeviceId
/Stats/DelStats|Delete the Stats service in Srs|Enter as DeviceId

+ StreamCaster

interface name|function|remark
--|:--:|--:
/StreamCaster/GetStreamCasterInstanceNameList | Get a list of all StreamCaster instance names | Input is DeviceId
/StreamCaster/GetStreamCasterInstanceList | Get all StreamCaster instances | Input is DeviceId
/StreamCaster/CreateStreamCaster|Create a StreamCaster|Input is DeviceId&StreamCasterConfClass
/StreamCaster/GetStreamCasterTemplate | Get a StreamCaster creation template | Input is CasterType(mpegts_over_udp|rtsp|flv|gb28181)
/StreamCaster/DeleteStreamCasterByInstanceName|Delete a StreamCaster by instance name|Input is DeivceId&StreamCasterInstanceName
/StreamCaster/ChangeStreamCasterInstanceName|Modify the instance name of a StreamCaster|Input is DeviceId&InstanceName&NewInstanceName
/StreamCaster/OnOrOff|Enable name to disable a StreamCaster|Enable as DeviceId&InstanceName&enable:bool
/StreamCaster/SetStreamCaster|Modify a StreamCaster parameter|Input is DeviceId&StreamCasterConfCalss


```js
curl -X GET "http://192.168.2.42:5800/StreamCaster/GetStreamCasterInstanceNameList?deviceId=22364bc4-5134-494d-8249-51d06777fb7f" -H "accept: */*"
```
```json
[
  "gb28181",
  "streamcaster-gb28181-template",
  "streamcaster-gb28181-template2"
]
```
```js
curl -X GET "http://192.168.2.42:5800/StreamCaster/GetStreamCasterInstanceList?deviceId=22364bc4-5134-494d-8249-51d06777fb7f" -H "accept: */*"
```
```json
[
  {
    "sip": {
      "enabled": true,
      "listen": 5060,
      "serial": "34020000002000000001",
      "realm": "3402000000",
      "ack_timeout": 30,
      "keepalive_timeout": 120,
      "auto_play": true,
      "invite_port_fixed": true,
      "query_catalog_interval": 60
    },
    "auto_create_channel": true,
    "enabled": true,
    "caster": "gb28181",
    "output": "rtmp://127.0.0.1/live/[stream]",
    "listen": 9000,
    "rtp_port_min": 58200,
    "rtp_port_max": 58300,
    "host": "*",
    "audio_enable": false,
    "wait_keyframe": false,
    "rtp_idle_timeout": 30,
    "instanceName": "gb28181"
  },
  {
    "sip": {
      "enabled": true,
      "listen": 5060,
      "serial": "34020000002000000001",
      "realm": "3402000000",
      "ack_timeout": 30,
      "keepalive_timeout": 120,
      "auto_play": true,
      "invite_port_fixed": true,
      "query_catalog_interval": 60
    },
    "auto_create_channel": false,
    "enabled": true,
    "caster": "gb28181",
    "output": "rtmp://127.0.0.1/[vhost]/[app]/[stream]",
    "listen": 9001,
    "rtp_port_min": 58200,
    "rtp_port_max": 58300,
    "host": "*",
    "audio_enable": true,
    "wait_keyframe": false,
    "rtp_idle_timeout": 30,
    "instanceName": "streamcaster-gb28181-template"
  },
  {
    "sip": {
      "enabled": true,
      "listen": 5060,
      "serial": "34020000002000000001",
      "realm": "3402000000",
      "ack_timeout": 30,
      "keepalive_timeout": 120,
      "auto_play": true,
      "invite_port_fixed": true,
      "query_catalog_interval": 60
    },
    "auto_create_channel": false,
    "enabled": true,
    "caster": "gb28181",
    "output": "rtmp://127.0.0.1/[vhost]/[app]/[stream]",
    "listen": 9002,
    "rtp_port_min": 58200,
    "rtp_port_max": 58300,
    "host": "*",
    "audio_enable": true,
    "wait_keyframe": false,
    "rtp_idle_timeout": 30,
    "instanceName": "streamcaster-gb28181-template2"
  }
]

```

### The following interface classes are not expanded one by one (not expanded in detail, classes and configuration files are mapped to each other, if necessary, you can directly see the source code and Swagger interface)
<table border="1">
<tr>
<td>Vhost</td>
<td>Vhost related functions</td>
<td>VhostBandcheck</td>
<td>VhostBandcheck related functions</td>
<td>VhostCluster</td>
<td>VhostCluster related functions</td>
</tr>
<tr>
<td>VhostDash</td>
<td>VhostDash related functions</td>
<td>VhostDvr</td>
<td>VhostDvr related functions</td>
<td>VhostExec</td>
<td>VhostExec related functions</td>
</tr>
<tr>
<td>VhostForward</td>
<td>VhostForward related functions</td>
<td>VhostHds</td>
<td>VhostHds related functions</td>
<td>VhostHls</td>
<td>VhostHls related functions</td>
</tr>
<tr>
<td>VhostHttpHooks</td>
<td>VhostHttpHooks related functions</td>
<td>VhostHttpRemux</td>
<td>VhostHttpRemux related functions</td>
<td>VhostHttpStatic</td>
<td>VhostHttpStatic related functions</td>
</tr>
<tr>
<td>VhostIngest</td>
<td>VhostIngest related functions</td>
<td>VhostPlay</td>
<td>VhostPlay related functions</td>
<td>VhostPublish</td>
<td>VhostPublish related functions</td>
</tr>
<tr>
<td>VhostRtc</td>
<td>VhostRtc related functions</td>
<td>Vhost Security</td>
<td>VhostSecurity related functions</td>
<td>VhostTranscode</td>
<td>VhostTranscode related functions</td>
</tr>
</table>

