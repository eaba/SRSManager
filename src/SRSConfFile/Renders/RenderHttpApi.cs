﻿using SrsConfFile.SRSConfClass;

namespace SrsConfFile.Renders
{
    public static class RenderHttpApi
    {
        public static void Render_RawApi(SectionBody scbin, SrsSystemConfClass sccout, string instanceName = "")
        {
            if (sccout.Http_api?.Raw_Api == null)
                if (sccout.Http_api != null)
                    sccout.Http_api.Raw_Api = new RawApi();
            if (sccout.Http_api != null)
            {
                if (sccout.Http_api.Raw_Api != null)
                {
                    sccout.Http_api.Raw_Api.SectionsName = "raw_api";
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
                                    sccout.Http_api.Raw_Api.Enabled = Common.Str2bool(tmpkv.Value);
                                    break;
                                case "allow_reload":
                                    sccout.Http_api.Raw_Api.Allow_reload = Common.Str2bool(tmpkv.Value);
                                    break;
                            }
                        }
                }
            }
        }

        public static void Render(SectionBody scbin, SrsSystemConfClass sccout, string instanceName = "")
        {
            if (sccout.Http_api == null) sccout.Http_api = new SrsHttpApiConfClass();
            sccout.Http_api.SectionsName = "http_api";
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
                            sccout.Http_api.Enabled = Common.Str2bool(tmpkv.Value);
                            break;
                        case "listen":
                            sccout.Http_api.Listen = Common.Str2ushort(tmpkv.Value);
                            break;
                        case "crossdomain":
                            sccout.Http_api.Crossdomain = Common.Str2bool(tmpkv.Value);
                            break;
                        case "device_id":
                            break;
                    }
                }

            if (scbin.SubSections != null && scbin.SubSections.Count > 0)
            {
                foreach (var scb in scbin.SubSections)
                {
                    Render_RawApi(scb, sccout);
                }
            }
        }
    }
}