using SrsConfFile.SRSConfClass;

namespace SrsConfFile.Renders
{
    public static class RenderSrtServer
    {
        public static void Render(SectionBody scbin, SrsSystemConfClass sccout, string instanceName = "")
        {
            if (sccout.Srt_server == null) sccout.Srt_server = new SrsSrtServerConfClass();
            sccout.Srt_server.SectionsName = "srt_server";
            if (scbin.BodyList != null)
                foreach (var s in scbin.BodyList)
                {
                    if (!s.Trim().EndsWith(";")) continue;
                    var tmpkv = Common.GetKV(s);
                    if (string.IsNullOrEmpty(tmpkv.Key)) continue;

                    var cmd = tmpkv.Key.Trim().ToLower();
                    switch (cmd)
                    {
                        case "enabled":
                            sccout.Srt_server.Enabled = Common.Str2bool(tmpkv.Value);
                            break;
                        case "listen":
                            sccout.Srt_server.Listen = Common.Str2ushort(tmpkv.Value);
                            break;
                        case "maxbw":
                            sccout.Srt_server.Maxbw = Common.Str2int(tmpkv.Value);
                            break;
                        case "connect_timeout":
                            sccout.Srt_server.Connect_timeout = Common.Str2int(tmpkv.Value);
                            break;
                        case "peerlatency":
                            sccout.Srt_server.Peerlatency = Common.Str2int(tmpkv.Value);
                            break;
                        case "recvlatency":
                            sccout.Srt_server.Recvlatency = Common.Str2int(tmpkv.Value);
                            break;
                        case "default_app":
                            sccout.Srt_server.Default_app = tmpkv.Value;
                            break;
                        case "latency":
                            sccout.Srt_server.Latency = Common.Str2int(tmpkv.Value);
                            break;
                        case "tsbpdmode":
                            sccout.Srt_server.Tsbpdmode = Common.Str2bool(tmpkv.Value);
                            break;
                        case "tlpktdrop":
                            sccout.Srt_server.Tlpktdrop = Common.Str2bool(tmpkv.Value);
                            break;
                        case "sendbuf":
                            sccout.Srt_server.Sendbuf = Common.Str2int(tmpkv.Value);
                            break;
                        case "recvbuf":
                            sccout.Srt_server.Recvbuf = Common.Str2int(tmpkv.Value);
                            break;
                        case "pbkeylen":
                            sccout.Srt_server.Pbkeylen = Common.Str2ushort(tmpkv.Value);
                            break;
                        case "passphrase":
                            sccout.Srt_server.Passphrase = tmpkv.Value;
                            break;
                    }
                }
        }
    }
}