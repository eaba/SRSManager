﻿using SrsConfFile.SRSConfClass;

namespace SrsConfFile.Renders
{
    public static class RenderRoot
    {
        public static void Render(SectionBody scbin,
            SrsSystemConfClass sccout, string instanceName = "")
        {
            sccout.SectionsName = "root";
            if (scbin.BodyList != null)
                foreach (var s in scbin.BodyList)
                {
                    if (!s.Trim().EndsWith(";")) continue;
                    var tmpkv = Common.GetKV(s);
                    if (string.IsNullOrEmpty(tmpkv.Key)) continue;

                    var cmd = tmpkv.Key.Trim().ToLower();
                    switch (cmd)
                    {
                        /*case "deviceid":
                            sccout.DeviceId = tmpkv.Value;
                            break;
                         srs-ie193id;
                        in_docker off;
                        query_latest_version on;
                        first_wait_for_qlv 300;
                        is_full on;
                        include str
                         */
                        case "server_id":
                            sccout.Server_Id = tmpkv.Value;
                            break;
                        case "in_docker":
                            sccout.In_Docker = Common.Str2bool(tmpkv.Value);
                            break;
                        case "query_latest_version":
                            sccout.Query_Latest_Version = Common.Str2bool(tmpkv.Value);
                            break;
                        case "first_wait_for_qlv":
                            sccout.First_Wait_For_Glv = Common.Str2ushort(tmpkv.Value);
                            break;
                        case "is_full":
                            sccout.Is_Full = Common.Str2bool(tmpkv.Value);
                            break;
                        case "include":
                            sccout.Include = tmpkv.Value;
                            break;
                        case "listen":
                            sccout.Listen = Common.Str2ushort(tmpkv.Value);
                            break;
                        case "pid":
                            sccout.Pid = tmpkv.Value;
                            break;
                        case "chunk_size":
                            sccout.Chunk_size = Common.Str2ushort(tmpkv.Value);
                            break;
                        case "ff_log_dir":
                            sccout.Ff_log_dir = tmpkv.Value;
                            break;
                        case "ff_log_level":
                            sccout.Ff_log_level = tmpkv.Value;
                            break;
                        case "srs_log_tank":
                            sccout.Srs_log_tank = tmpkv.Value;
                            break;
                        case "srs_log_level":
                            sccout.Srs_log_level = tmpkv.Value;
                            break;
                        case "srs_log_file":
                            sccout.Srs_log_file = tmpkv.Value;
                            break;
                        case "max_connections":
                            sccout.Max_connections = Common.Str2ushort(tmpkv.Value);
                            break;
                        case "daemon":
                            sccout.Daemon = Common.Str2bool(tmpkv.Value);
                            break;
                        case "utc_time":
                            sccout.Utc_time = Common.Str2bool(tmpkv.Value);
                            break;
                        case "pithy_print_ms":
                            sccout.Pithy_print_ms = Common.Str2int(tmpkv.Value);
                            break;
                        case "work_dir":
                            sccout.Work_dir = tmpkv.Value;
                            break;
                        case "asprocess":
                            sccout.Asprocess = Common.Str2bool(tmpkv.Value);
                            break;
                        case "empty_ip_ok":
                            sccout.Empty_ip_ok = Common.Str2bool(tmpkv.Value);
                            break;
                        case "grace_start_wait":
                            sccout.Grace_start_wait = Common.Str2int(tmpkv.Value);
                            break;
                        case "grace_final_wait":
                            sccout.Grace_final_wait = Common.Str2int(tmpkv.Value);
                            break;
                        case "force_grace_quit":
                            sccout.Force_grace_quit = Common.Str2bool(tmpkv.Value);
                            break;
                        case "disable_daemon_for_docker":
                            sccout.Disable_daemon_for_docker = Common.Str2bool(tmpkv.Value);
                            break;
                        case "inotify_auto_reload":
                            sccout.Inotify_auto_reload = Common.Str2bool(tmpkv.Value);
                            break;
                        case "auto_reload_for_docker":
                            sccout.Auto_reload_for_docker = Common.Str2bool(tmpkv.Value);
                            break;
                        case "tcmalloc_release_rate":
                            sccout.Tcmalloc_release_rate = Common.Str2float(tmpkv.Value);
                            break;
                    }
                }
        }
    }
}