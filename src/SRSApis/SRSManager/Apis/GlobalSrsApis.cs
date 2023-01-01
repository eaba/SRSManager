using SrsManageCommon;
using SRSManageCommon.ManageStructs;
using Common = SRSApis.Common;

namespace SrsApis.SrsManager.Apis
{
    public static class GlobalSrsApis
    {
        /// <summary>
        /// Check if the SRS instance is running
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool IsRunning(string deviceId, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            var ret = Common.SrsManagers.FindLast(x =>
                x.SrsDeviceId.Trim().ToUpper().Equals(deviceId.Trim().ToUpper()));
            if (ret != null)
            {
                return ret.IsRunning;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.SrsObjectNotInit,
                Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
            };
            return false;
        }

        /// <summary>
        /// Check if SrsManager is initialized
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool IsInit(string deviceId, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            var ret = Common.SrsManagers.FindLast(x =>
                x.SrsDeviceId.Trim().ToUpper().Equals(deviceId.Trim().ToUpper()));
            if (ret != null)
            {
                return ret.IsInit;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.SrsObjectNotInit,
                Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
            };
            return false;
        }

        /// <summary>
        /// Start the SRS instance
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool StartSrs(string deviceId, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            var ret = Common.SrsManagers.FindLast(x =>
                x.SrsDeviceId.Trim().ToUpper().Equals(deviceId.Trim().ToUpper()));
            if (ret != null)
            {
                if (ret.IsRunning)
                {
                    return true;
                }

                return ret.Start(out rs);
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.SrsObjectNotInit,
                Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
            };
            return false;
        }

        /// <summary>
        /// Stop the SRS instance
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool StopSrs(string deviceId, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            var ret = Common.SrsManagers.FindLast(x =>
                x.SrsDeviceId.Trim().ToUpper().Equals(deviceId.Trim().ToUpper()));
            if (ret != null)
            {
                return ret.Stop(out rs);
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.SrsObjectNotInit,
                Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
            };
            return false;
        }

        /// <summary>
        /// Restart the SRS instance
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool RestartSrs(string deviceId, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            var ret = Common.SrsManagers.FindLast(x =>
                x.SrsDeviceId.Trim().ToUpper().Equals(deviceId.Trim().ToUpper()));
            if (ret != null)
            {
                return ret.Restart(out rs);
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.SrsObjectNotInit,
                Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
            };
            return false;
        }

        /// <summary>
        /// Reload the configuration in the SRS instance
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool ReloadSrs(string deviceId, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            var ret = Common.SrsManagers.FindLast(x =>
                x.SrsDeviceId.Trim().ToUpper().Equals(deviceId.Trim().ToUpper()));
            if (ret != null)
            {
                return ret.Reload(out rs);
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.SrsObjectNotInit,
                Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
            };
            return false;
        }


        /// <summary>
        /// Modify the global Chunksize
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="size"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool GlobalChangeChunksize(string deviceId, ushort size, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            var ret = Common.SrsManagers.FindLast(x =>
                x.SrsDeviceId.Trim().ToUpper().Equals(deviceId.Trim().ToUpper()));
            if (ret != null && ret.Srs != null)
            {
                ret.Srs.Chunk_size = size;
                return true;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.SrsObjectNotInit,
                Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
            };
            return false;
        }

        /// <summary>
        /// Modify the Listen port of HttpApi
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="port"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool GlobalChangeHttpApipListen(string deviceId, ushort port, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            var ret = Common.SrsManagers.FindLast(x =>
                x.SrsDeviceId.Trim().ToUpper().Equals(deviceId.Trim().ToUpper()));
            if (ret != null && ret.Srs.Http_api != null)
            {
                ret.Srs.Http_api.Listen = port;
                return true;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.SrsObjectNotInit,
                Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
            };
            return false;
        }

        /// <summary>
        /// Modify whether Http_Api is available
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="enable"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool GlobalChangeHttpApiEnable(string deviceId, bool enable, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            var ret = Common.SrsManagers.FindLast(x =>
                x.SrsDeviceId.Trim().ToUpper().Equals(deviceId.Trim().ToUpper()));
            if (ret != null && ret.Srs.Http_api != null)
            {
                ret.Srs.Http_api.Enabled = enable;
                return true;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.SrsObjectNotInit,
                Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
            };
            return false;
        }

        /// <summary>
        /// Modify the MaxConnections value
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="maxconnections"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool GlobalChangeMaxConnections(string deviceId, ushort maxconnections, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            var ret = Common.SrsManagers.FindLast(x =>
                x.SrsDeviceId.Trim().ToUpper().Equals(deviceId.Trim().ToUpper()));
            if (ret != null && ret.Srs != null)
            {
                ret.Srs.Max_connections = maxconnections;
                return true;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.SrsObjectNotInit,
                Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
            };
            return false;
        }

        /// <summary>
        /// Modify the global rtmp port
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="port"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool GlobalChangeRtmpListen(string deviceId, ushort port, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            var ret = Common.SrsManagers.FindLast(x =>
                x.SrsDeviceId.Trim().ToUpper().Equals(deviceId.Trim().ToUpper()));
            if (ret != null && ret.Srs != null)
            {
                ret.Srs.Listen = port;
                return true;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.SrsObjectNotInit,
                Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
            };
            return false;
        }

        /// <summary>
        /// Modify the port of Httpserver
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="port"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool GlobalChangeHttpServerListen(string deviceId, ushort port, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            var ret = Common.SrsManagers.FindLast(x =>
                x.SrsDeviceId.Trim().ToUpper().Equals(deviceId.Trim().ToUpper()));
            if (ret != null && ret.Srs.Http_server != null)
            {
                ret.Srs.Http_server.Listen = port;
                return true;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.SrsObjectNotInit,
                Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
            };
            return false;
        }

        /// <summary>
        /// Modify the webroot of httpserver
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="path"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool GlobalChangeHttpServerPath(string deviceId, string path, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            var ret = Common.SrsManagers.FindLast(x =>
                x.SrsDeviceId.Trim().ToUpper().Equals(deviceId.Trim().ToUpper()));
            if (ret != null && ret.Srs.Http_server != null)
            {
                ret.Srs.Http_server.Dir = path;
                return true;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.SrsObjectNotInit,
                Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
            };
            return false;
        }

        /// <summary>
        /// Modify whether httpserver is available
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="enable"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool GlobalChangeHttpServerEnable(string deviceId, bool enable, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            var ret = Common.SrsManagers.FindLast(x =>
                x.SrsDeviceId.Trim().ToUpper().Equals(deviceId.Trim().ToUpper()));
            if (ret != null && ret.Srs.Http_server != null)
            {
                ret.Srs.Http_server.Enabled = enable;
                return true;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.SrsObjectNotInit,
                Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
            };
            return false;
        }

        /// <summary>
        /// Get the global modifiable parameter object of the SRS instance
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static GlobalModule GetGlobalParams(string deviceId, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            var ret = Common.SrsManagers.FindLast(x =>
                x.SrsDeviceId.Trim().ToUpper().Equals(deviceId.Trim().ToUpper()));
            if (ret != null && ret.Srs.Http_server != null)
            {
                GlobalModule result = new GlobalModule()
                {
                    Listen = ret.Srs.Listen,
                    HttpApiListen = ret.Srs.Http_api!.Listen,
                    MaxConnections = ret.Srs.Max_connections,
                    HttpApiEnable = ret.Srs.Http_api!.Enabled,
                    HttpServerEnable = ret.Srs.Http_server!.Enabled,
                    HttpServerPath = ret.Srs.Http_server!.Dir,
                    HttpServerListen = ret.Srs.Http_server!.Listen,
                    HeartbeatEnable = ret.Srs.Heartbeat!.Enabled,
                    HeartbeatUrl = ret.Srs.Heartbeat!.Url,
                    HeartbeatSummariesEnable = ret.Srs.Heartbeat!.Summaries,
                };
                return result;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.SrsObjectNotInit,
                Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
            };
            return null!;
        }

        /// <summary>
        /// Integral modification of global variable parameters
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="bm"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool ChangeGlobalParams(string deviceId, GlobalModule bm, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            var ret = Common.SrsManagers.FindLast(x =>
                x.SrsDeviceId.Trim().ToUpper().Equals(deviceId.Trim().ToUpper()));
            if (ret != null && ret.Srs.Http_server != null)
            {
                if (bm.Listen != null) ret.Srs.Listen = bm.Listen;
                if (bm.MaxConnections != null) ret.Srs.Max_connections = bm.MaxConnections;
                if (bm.HttpApiEnable != null) ret.Srs.Http_api!.Enabled = bm.HttpApiEnable;
                if (bm.HttpApiListen != null) ret.Srs.Http_api!.Listen = bm.HttpApiListen;
                if (bm.HttpServerPath != null) ret.Srs.Http_server!.Dir = bm.HttpServerPath;
                if (bm.HttpServerEnable != null) ret.Srs.Http_server!.Enabled = bm.HttpServerEnable;
                if (bm.HttpServerListen != null) ret.Srs.Http_server!.Listen = bm.HttpServerListen;
                if (bm.HeartbeatEnable != null) ret.Srs.Heartbeat!.Enabled = bm.HeartbeatEnable;
                if (bm.HeartbeatUrl != null) ret.Srs.Heartbeat!.Url = bm.HeartbeatUrl;
                if (bm.HeartbeatSummariesEnable != null) ret.Srs.Heartbeat!.Summaries = bm.HeartbeatSummariesEnable;
                return true;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.SrsObjectNotInit,
                Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
            };
            return false;
        }
    }
}