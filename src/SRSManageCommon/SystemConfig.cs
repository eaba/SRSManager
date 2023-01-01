using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FreeSql;
using SRSManageCommon.ManageStructs;

namespace SrsManageCommon
{
    /// <summary>
    /// Profile class
    /// </summary>
    [Serializable]
    public class SystemConfig
    {
        private List<AllowKey> _allowKeys = new List<AllowKey>();
        private ushort _httpPort = 5800;
        private string _password = "password123!@#";
        private string? _db = "data source=" + Common.WorkPath + "SRSWebApi.db";
        private DataType _dbType = DataType.Sqlite;
        private int _dvrPlanExecServiceinterval=1000 * 60;
        private int _srsAdnffmpegLogMonitorServiceinterval=1000 * 60 * 5;
        private int _srsClientManagerServiceinterval=1000 * 5;
        private bool _enableIngestKeeper = true;
        private string? _ffmpegPath = "./ffmpeg";
        private byte? _ffmpegThreadCount = 2;


        /// <summary>
        /// http port
        /// </summary>
        public ushort HttpPort
        {
            get => _httpPort;
            set => _httpPort = value;
        }

        /// <summary>
        /// password
        /// </summary>
        public string Password
        {
            get => _password;
            set => _password = value;
        }

        /// <summary>
        /// Control Access Authorization
        /// </summary>
        public List<AllowKey> AllowKeys
        {
            get => _allowKeys;
            set => _allowKeys = value;
        }

        public string? Db
        {
            get => _db;
            set => _db = value;
        }

        public DataType DbType
        {
            get => _dbType;
            set => _dbType = value;
        }

        public int DvrPlanExecServiceinterval
        {
            get => _dvrPlanExecServiceinterval;
            set => _dvrPlanExecServiceinterval = value;
        }

        public bool EnableIngestKeeper
        {
            get => _enableIngestKeeper;
            set => _enableIngestKeeper = value;
        }

        public int SrsAdnffmpegLogMonitorServiceinterval
        {
            get => _srsAdnffmpegLogMonitorServiceinterval;
            set => _srsAdnffmpegLogMonitorServiceinterval = value;
        }

        public int SrsClientManagerServiceinterval
        {
            get => _srsClientManagerServiceinterval;
            set => _srsClientManagerServiceinterval = value;
        }

        public string? FfmpegPath
        {
            get => _ffmpegPath;
            set => _ffmpegPath = value;
        }

        public byte? FfmpegThreadCount
        {
            get => _ffmpegThreadCount;
            set => _ffmpegThreadCount = value;
        }

        private string[] Getkv(string s, string splitchar)
        {
            return s.Split(splitchar, StringSplitOptions.RemoveEmptyEntries);
        }


        private bool ParseConfig(List<string> lines)
        {
            foreach (var s in lines)
            {
                string tmps = s.Trim();
                if (!tmps.EndsWith(";") || tmps.StartsWith("#")) continue;
                tmps = tmps.TrimEnd(';');
                string[] kv = Getkv(tmps, "::");
                if (kv.Length != 2) continue;
                kv[0] = kv[0].Trim().ToLower();
                switch (kv[0])
                {
                    case "ffmpegthreadcount":
                        if (kv.Length == 2)
                        {
                            if (!string.IsNullOrEmpty(kv[1]))
                            {
                                FfmpegThreadCount = byte.Parse(kv[1].Trim());
                                Common.FFmpegThreadCount = byte.Parse(kv[1].Trim());
                            }
                        }
                        break;
                    case "ffmpegpath":
                        if (kv.Length == 2)
                        {
                            if (!string.IsNullOrEmpty(kv[1]))
                            {
                                FfmpegPath = kv[1].Trim();
                                Common.FFmpegBinPath = FfmpegPath;
                            }
                        }
                        break;
                    case "auto_cleintmanagerinterval":
                        if (kv.Length == 2)
                        {
                            if (!string.IsNullOrEmpty(kv[1]))
                            {
                                SrsClientManagerServiceinterval = int.Parse(kv[1].Trim());
                            }
                        }

                        break;
                    case "auto_logmonitorinterval":
                        if (kv.Length == 2)
                        {
                            if (!string.IsNullOrEmpty(kv[1]))
                            {
                                SrsAdnffmpegLogMonitorServiceinterval = int.Parse(kv[1].Trim());
                            }
                        }
                        break;
                    case "enableingestkeeper":
                        if (kv.Length == 2)
                        {
                            if (!string.IsNullOrEmpty(kv[1]))
                            {
                                EnableIngestKeeper = bool.Parse(kv[1].Trim());
                            }
                        }
                        break;
                    case "auto_dvrplaninterval":
                        if (kv.Length == 2)
                        {
                            if (!string.IsNullOrEmpty(kv[1]))
                            {
                                DvrPlanExecServiceinterval = int.Parse(kv[1].Trim());
                            }
                        }
                        break;
                    case "db":
                        if (kv.Length == 2)
                        {
                            if (!string.IsNullOrEmpty(kv[1]))
                            {
                                Db = kv[1].Trim();
                            }
                        }

                        break;
                    case "dbtype":
                        if (kv.Length == 2)
                        {
                            if (!string.IsNullOrEmpty(kv[1]))
                            {
                                string tmpStr = kv[1].Trim();
                                if (tmpStr.ToLower().Contains("mysql"))
                                {
                                    DbType = DataType.MySql;
                                }

                                if (tmpStr.ToLower().Contains("sqlite"))
                                {
                                    DbType = DataType.Sqlite;
                                }

                                if (tmpStr.ToLower().Contains("oracle"))
                                {
                                    DbType = DataType.Oracle;
                                }

                                if (tmpStr.ToLower().Contains("dameng"))
                                {
                                    DbType = DataType.Dameng;
                                }

                                if (tmpStr.ToLower().Contains("sqlserver"))
                                {
                                    DbType = DataType.SqlServer;
                                }

                                if (tmpStr.ToLower().Contains("postagre"))
                                {
                                    DbType = DataType.PostgreSQL;
                                }

                                if (tmpStr.ToLower().Contains("access"))
                                {
                                    DbType = DataType.MsAccess;
                                }

                                if (tmpStr.ToLower().Contains("odbc"))
                                {
                                    DbType = DataType.Odbc;
                                }
                            }
                        }

                        break;
                    case "httpport":

                        if (kv.Length == 2)
                        {
                            ushort v;
                            if (ushort.TryParse(kv[1], out v))
                            {
                                HttpPort = v;
                            }
                        }

                        break;
                    case "password":

                        if (kv.Length == 2)
                        {
                            if (!string.IsNullOrEmpty(kv[1]))
                            {
                                Password = kv[1].Trim();
                            }
                        }

                        break;
                    case "allowkey":
                        string[] kv1 = Getkv(kv[1], "\t");
                        if (kv1.Length > 1)
                        {
                            AllowKey ak = new AllowKey();
                            ak.Key = kv1[0];
                            for (int i = 1; i <= kv1.Length - 1; i++)
                            {
                                ak.IpArray.Add(kv1[i].Trim());
                            }

                            AllowKeys.Add(ak);
                        }

                        break;
                }
            }

            return true;
        }

        /// <summary>
        /// write configuration file
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public bool RebuidConfig(string filePath)
        {
            List<string> writeFile = new List<string>();
            writeFile.Add("httpport::" + HttpPort + ";");
            writeFile.Add("password::" + Password + ";");
            writeFile.Add("ffmpegpath::" + Common.FFmpegBinPath+ ";");
            writeFile.Add("ffmpegthreadcount::" + Common.FFmpegThreadCount+ ";");
            writeFile.Add("db::" + Db + ";");
            writeFile.Add("dbtype::" + Enum.GetName(typeof(DataType), this.DbType)!.ToLower() + ";");
            writeFile.Add("auto_cleintmanagerinterval::" + SrsClientManagerServiceinterval + ";");
            writeFile.Add("auto_logmonitorinterval::" + SrsAdnffmpegLogMonitorServiceinterval + ";");
            writeFile.Add("enableingestkeeper::" + EnableIngestKeeper + ";");
            writeFile.Add("auto_dvrplaninterval::" + DvrPlanExecServiceinterval + ";");
            
            foreach (var ak in _allowKeys)
            {
                if (ak != null)
                {
                    string tmps = "";
                    foreach (string ip in ak.IpArray)
                    {
                        if (!string.IsNullOrEmpty(ip))
                        {
                            tmps += ip + "\t";
                        }
                    }

                    writeFile.Add("allowkey=" + ak.Key + "\t" + tmps + ";");
                }
            }

            try
            {
                File.WriteAllLines(filePath, writeFile);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// load configuration file
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public bool LoadConfig(string filePath)
        {
            if (File.Exists(filePath))
            {
                List<string> tmp_sl = File.ReadAllLines(filePath).ToList();
                if (tmp_sl != null && tmp_sl.Count > 0)
                {
                    if (ParseConfig(tmp_sl))
                    {
                        return true;
                    }

                    return false;
                }

                return false;
            }
            else
            {
                return false;
            }
        }
    }
}