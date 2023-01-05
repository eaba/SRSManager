using System;
using System.Collections.Generic;
using SrsConfFile.SRSConfClass;

namespace SrsConfFile.Renders
{
    public static class RenderStreamCaster
    {
        public static void Render_Sip(SectionBody scbin, SrsStreamCasterConfClass sccout, string instanceName = "")
        {
            if (sccout.sip == null)
            {
                sccout.sip = new Sip();
            }
            else
            {
                return; //only one
            }

            sccout.sip.SectionsName = "sip";
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
                            sccout.sip.Enabled = Common.Str2bool(tmpkv.Value);
                            break;
                        case "listen":
                            sccout.sip.Listen = Common.Str2ushort(tmpkv.Value);
                            break;
                        case "serial":
                            sccout.sip.Serial = tmpkv.Value;
                            break;
                        case "realm":
                            sccout.sip.Realm = tmpkv.Value;
                            break;
                        case "ack_timeout":
                            sccout.sip.Ack_timeout = Common.Str2ushort(tmpkv.Value);
                            break;
                        case "keepalive_timeout":
                            sccout.sip.Keepalive_timeout = Common.Str2ushort(tmpkv.Value);
                            break;
                        case "auto_play":
                            sccout.sip.Auto_play = Common.Str2bool(tmpkv.Value);
                            break;
                        case "invite_port_fixed":
                            sccout.sip.Invite_port_fixed = Common.Str2bool(tmpkv.Value);
                            break;
                        case "query_catalog_interval":
                            sccout.sip.Query_catalog_interval = Common.Str2ushort(tmpkv.Value);
                            break;
                    }
                }
        }

        public static void Render(SectionBody scbin, SrsSystemConfClass sccout, string instanceName = "")
        {
            if (sccout.Stream_casters == null) sccout.Stream_casters = new List<SrsStreamCasterConfClass>();
         
            if (null != sccout.Stream_casters.Find(s => s.InstanceName == instanceName))
                return; //filter the same streamcaster instance
            var sccc = new SrsStreamCasterConfClass();
            if (scbin.BodyList != null)
                foreach (var s in scbin.BodyList)
                {
                    if (!s.Trim().EndsWith(";")) continue;
                    var tmpkv = Common.GetKV(s);
                    if (string.IsNullOrEmpty(tmpkv.Key)) continue;
                    sccc.InstanceName = instanceName;
                    sccc.SectionsName = "stream_caster";
                    var cmd = tmpkv.Key.Trim().ToLower();
                    switch (cmd)
                    {
                        case "enabled":
                            sccc.Enabled = Common.Str2bool(tmpkv.Value);
                            break;
                        case "caster":
                            sccc.Caster = (CasterEnum) Enum.Parse(typeof(CasterEnum), tmpkv.Value);
                            break;
                        case "output":
                            sccc.Output = tmpkv.Value;
                            break;
                        case "listen":
                            sccc.Listen = Common.Str2ushort(tmpkv.Value);
                            break;
                        case "rtp_port_min":
                            sccc.Rtp_port_min = Common.Str2ushort(tmpkv.Value);
                            break;
                        case "rtp_port_max":
                            sccc.Rtp_port_max = Common.Str2ushort(tmpkv.Value);
                            break;
                        case "host":
                            sccc.Host = tmpkv.Value;
                            break;
                        case "audio_enable":
                            sccc.Audio_enable = Common.Str2bool(tmpkv.Value);
                            break;
                        case "wait_keyframe":
                            sccc.Wait_keyframe = Common.Str2bool(tmpkv.Value);
                            break;
                        case "rtp_idle_timeout":
                            sccc.Rtp_idle_timeout = Common.Str2ushort(tmpkv.Value);
                            break;
                        case "auto_create_channel":
                            sccc.Auto_create_channel = Common.Str2bool(tmpkv.Value);
                            break;
                        case "jitterbuffer_enable":
                            sccc.Jitterbuffer_Enable=Common.Str2bool(tmpkv.Value);
                            break;
                            
                    }
                }

            if (scbin.SubSections != null)
                foreach (var scb in scbin.SubSections)
                {
                    Render_Sip(scb, sccc);
                }

            sccout.Stream_casters.Add(sccc);
        }
    }
}