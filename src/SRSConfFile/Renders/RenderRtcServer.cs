using System.Collections.Generic;
using SrsConfFile.SRSConfClass;

namespace SrsConfFile.Renders
{
    public static class RenderRtcServer
    {
        public static void Render_BlackHole(SectionBody scbin, SrsSystemConfClass sccout, string instanceName = "")
        {
            if (sccout.Rtc_server?.Black_hole == null)
                if (sccout.Rtc_server != null)
                    sccout.Rtc_server.Black_hole = new BlackHole();
            if (sccout.Rtc_server != null)
            {
                if (sccout.Rtc_server.Black_hole != null)
                {
                    sccout.Rtc_server.Black_hole.SectionsName = "black_hole";
                    if (scbin.BodyList != null)
                        foreach (string s in scbin.BodyList)
                        {
                            if (!s.Trim().EndsWith(";")) continue;
                            KeyValuePair<string, string> tmpkv = Common.GetKV(s);
                            if (string.IsNullOrEmpty(tmpkv.Key)) continue;
                            string cmd = tmpkv.Key.Trim().ToLower();
                            switch (cmd)
                            {
                                case "enabled":
                                    sccout.Rtc_server.Black_hole.Enabled = Common.Str2bool(tmpkv.Value);
                                    break;
                                case "addr":
                                    sccout.Rtc_server.Black_hole.Addr = tmpkv.Value;
                                    break;
                            }
                        }
                }
            }
        }
        private static void Render_Tcp(SectionBody scbin, SrsSystemConfClass sccout)
        {
            if (sccout.Rtc_server?.Tcp == null)
                if (sccout.Rtc_server != null)
                    sccout.Rtc_server.Tcp = new Tcp();
            if (sccout.Rtc_server != null)
            {
                if (sccout.Rtc_server.Tcp != null)
                {                  
                    if (scbin.BodyList != null)
                        foreach (string s in scbin.BodyList)
                        {
                            if (!s.Trim().EndsWith(";")) continue;
                            KeyValuePair<string, string> tmpkv = Common.GetKV(s);
                            if (string.IsNullOrEmpty(tmpkv.Key)) continue;
                            string cmd = tmpkv.Key.Trim().ToLower();
                            switch (cmd)
                            {
                                case "enabled":
                                    sccout.Rtc_server.Tcp.Enabled = Common.Str2bool(tmpkv.Value);
                                    break;
                                case "listen":
                                    sccout.Rtc_server.Tcp.Listen = Common.Str2ushort(tmpkv.Value);
                                    break;
                            }
                        }
                }
            }
        }
        public static void Render(SectionBody scbin, SrsSystemConfClass sccout, string instanceName = "")
        {
            if (sccout.Rtc_server == null) sccout.Rtc_server = new SrsRtcServerConfClass();
            sccout.Rtc_server.SectionsName = "rtc_server";
            if (scbin.BodyList != null)
                foreach (string s in scbin.BodyList)
                {
                    if (!s.Trim().EndsWith(";")) continue;
                    KeyValuePair<string, string> tmpkv = Common.GetKV(s);
                    if (string.IsNullOrEmpty(tmpkv.Key)) continue;

                    string cmd = tmpkv.Key.Trim().ToLower();
                    switch (cmd)
                    {
                        case "enabled":
                            sccout.Rtc_server.Enabled = Common.Str2bool(tmpkv.Value);
                            break;
                        case "listen":
                            sccout.Rtc_server.Listen = Common.Str2ushort(tmpkv.Value);
                            break;
                        case "candidate":
                            sccout.Rtc_server.Candidate = tmpkv.Value;
                            break;
                        case "ecdsa":
                            sccout.Rtc_server.Ecdsa = Common.Str2bool(tmpkv.Value);
                            break;
                        case "protocol":
                            sccout.Rtc_server.Protocol = tmpkv.Value;
                            break;
                        case "encrypt":
                            sccout.Rtc_server.Encrypt = Common.Str2bool(tmpkv.Value);
                            break;
                        case "reuseport":
                            sccout.Rtc_server.Reuseport = Common.Str2ushort(tmpkv.Value);
                            break;
                        case "merge_nalus":
                            sccout.Rtc_server.Merge_nalus = Common.Str2bool(tmpkv.Value);
                            break;
                        case "use_auto_detect_network_ip":
                            sccout.Rtc_server.UseAutoDetectNetworkIp = Common.Str2bool(tmpkv.Value);
                            break;
                        case "ip_family":
                            sccout.Rtc_server.IpFamily = tmpkv.Value;
                            break;
                        case "api_as_candidates":
                            sccout.Rtc_server.ApiAsCandidates = Common.Str2bool(tmpkv.Value);
                            break;
                        case "resolve_api_domain":
                            sccout.Rtc_server.ResolveApiDomain = Common.Str2bool(tmpkv.Value);
                            break;
                        case "keep_api_domain":
                            sccout.Rtc_server.KeepApiDomain = Common.Str2bool(tmpkv.Value);
                            break;
                    }
                }

            if (scbin.SubSections != null && scbin.SubSections.Count > 0)
            {
                foreach (SectionBody scb in scbin.SubSections)
                {
                    Render_BlackHole(scb, sccout);
                    Render_Tcp(scb, sccout);    
                }
            }
        }
    }
}