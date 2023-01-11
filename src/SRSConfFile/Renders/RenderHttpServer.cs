﻿using SrsConfFile.SRSConfClass;

namespace SrsConfFile.Renders
{
    public static class RenderHttpServer
    {
        public static void Render(SectionBody scbin, SrsSystemConfClass sccout, string instanceName = "")
        {
            if (sccout.Http_server == null) sccout.Http_server = new SrsHttpServerConfClass();
            sccout.Http_server.SectionsName = "http_server";
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
                            sccout.Http_server.Enabled = Common.Str2bool(tmpkv.Value);
                            break;

                        case "listen":
                            sccout.Http_server.Listen = Common.Str2ushort(tmpkv.Value);
                            break;
                        case "dir":
                            sccout.Http_server.Dir = tmpkv.Value;
                            break;
                        case "crossdomain":
                            sccout.Http_server.Crossdomain = Common.Str2bool(tmpkv.Value);
                            break;
                    }
                }
        }
    }
}