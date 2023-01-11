#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using SrsConfFile.SRSConfClass;

namespace SrsConfFile
{
    public static class SrsConfigBuild
    {
        private static KeyValuePair<string, string> PaddingSegment(int segmentLevel)
        {
            var segmentSpace = "";
            var segmentSpace_head = "";
            for (var i = 0; i < segmentLevel; i++)
            {
                segmentSpace += "\t";
                if (i < segmentLevel - 1)
                {
                    segmentSpace_head += "\t";
                }
            }

            return new KeyValuePair<string, string>(segmentSpace_head, segmentSpace);
        }

        private static void Write_HttpApi(SrsHttpApiConfClass? o, out string output, int segmentLevel,
            List<Type> types = null!)
        {
            var output_raw_api = "";
            output = "";
            var segmentSpace_head = PaddingSegment(segmentLevel).Key;
            var segmentSpace = PaddingSegment(segmentLevel).Value;
            output += segmentSpace_head + o?.SectionsName?.ToLower().Trim() + " { \r\n";
            foreach (var p in o?.GetType().GetProperties()!)
            {
                var obj = p.GetValue(o);
                if (obj == null) continue;
                if (p.Name.ToLower().Trim() == "sectionsname" || p.Name.ToLower().Trim() == "instancename")
                {
                    continue;
                }

                if ((p.PropertyType == typeof(string) || p.PropertyType == typeof(int?) ||
                     p.PropertyType == typeof(ushort?) ||
                     p.PropertyType == typeof(byte?)
                     || p.PropertyType == typeof(float?) || p.PropertyType == typeof(bool?))
                    || ((types != null) && types.Contains(p.PropertyType)))
                {
                    if (obj != null)
                    {
                        if (p.PropertyType == typeof(bool?))
                        {
                            var s = "";
                            s = Common.GetBoolStr(p, o);
                            var sTmp = segmentSpace + p.Name.Trim().ToLower() + "\t" + s + ";";
                            output += (sTmp + "\r\n");
                        }
                        else
                        {
                            var sTmp = segmentSpace + p.Name.Trim().ToLower() + "\t" + obj + ";";
                            output += (sTmp + "\r\n");
                        }
                    }
                }
            }

            foreach (var p in o.GetType().GetProperties()) //Loop non-basic type data, in order to ensure that the basic type data is on top
            {
                var obj = p.GetValue(o);
                if (obj == null) continue;
                if (p.Name.ToLower().Trim() == "sectionsname" || p.Name.ToLower().Trim() == "instancename")
                {
                    continue;
                }

                if (p.PropertyType == typeof(RawApi))
                {
                    Write_SubOnly(o.Raw_Api, out output_raw_api, 2);
                    output += output_raw_api;
                }
            }

            output += segmentSpace_head + "}\r\n";
        }

        private static void Write_RtcServer(SrsRtcServerConfClass? o, out string output, int segmentLevel,
            List<Type> types = null!)
        {
            var output_black_hole = "";
            output = "";
            var segmentSpace_head = PaddingSegment(segmentLevel).Key;
            var segmentSpace = PaddingSegment(segmentLevel).Value;
            output += segmentSpace_head + o?.SectionsName?.ToLower().Trim() + " { \r\n";
            foreach (var p in o?.GetType().GetProperties()!)
            {
                var obj = p.GetValue(o);
                if (obj == null) continue;
                if (p.Name.ToLower().Trim() == "sectionsname" || p.Name.ToLower().Trim() == "instancename")
                {
                    continue;
                }

                if ((p.PropertyType == typeof(string) || p.PropertyType == typeof(int?) ||
                     p.PropertyType == typeof(ushort?) ||
                     p.PropertyType == typeof(byte?)
                     || p.PropertyType == typeof(float?) || p.PropertyType == typeof(bool?))
                    || ((types != null) && types.Contains(p.PropertyType)))
                {
                    if (obj != null)
                    {
                        if (p.PropertyType == typeof(bool?))
                        {
                            var s = "";
                            s = Common.GetBoolStr(p, o);
                            var sTmp = segmentSpace + p.Name.Trim().ToLower() + "\t" + s + ";";
                            output += (sTmp + "\r\n");
                        }
                        else
                        {
                            var sTmp = segmentSpace + p.Name.Trim().ToLower() + "\t" + obj + ";";
                            output += (sTmp + "\r\n");
                        }
                    }
                }
            }

            foreach (var p in o.GetType().GetProperties()) //Loop non-basic type data, in order to ensure that the basic type data is on top
            {
                var obj = p.GetValue(o);
                if (obj == null) continue;
                if (p.Name.ToLower().Trim() == "sectionsname" || p.Name.ToLower().Trim() == "instancename")
                {
                    continue;
                }

                if (p.PropertyType == typeof(BlackHole))
                {
                    Write_SubOnly(o.Black_hole, out output_black_hole, 2);
                    output += output_black_hole;
                }
            }

            output += segmentSpace_head + "}\r\n";
        }

        private static void Write_SubOnly(SrsConfBase? o, out string output, int segmentLevel, List<Type> types = null!)
        {
            output = "";
            var segmentSpace_head = PaddingSegment(segmentLevel).Key;
            var segmentSpace = PaddingSegment(segmentLevel).Value;
            output += segmentSpace_head + o?.SectionsName?.ToLower().Trim() + " { \r\n";
            foreach (var p in o?.GetType().GetProperties()!)
            {
                var obj = p.GetValue(o);
                if (obj == null) continue;
                if (p.Name.ToLower().Trim() == "sectionsname" || p.Name.ToLower().Trim() == "instancename")
                {
                    continue;
                }

                if ((p.PropertyType == typeof(string) || p.PropertyType == typeof(int?) ||
                     p.PropertyType == typeof(ushort?) ||
                     p.PropertyType == typeof(byte?)
                     || p.PropertyType == typeof(float?) || p.PropertyType == typeof(bool?))
                    || ((types != null) && types.Contains(p.PropertyType)))
                {
                    if (obj != null)
                    {
                        if (p.PropertyType == typeof(bool?))
                        {
                            var s = "";
                            s = Common.GetBoolStr(p, o);
                            var sTmp = segmentSpace + p.Name.Trim().ToLower() + "\t" + s + ";";
                            output += (sTmp + "\r\n");
                        }
                        else
                        {
                            var sTmp = segmentSpace + p.Name.Trim().ToLower() + "\t" + obj + ";";
                            output += (sTmp + "\r\n");
                        }
                    }
                }
            }

            output += segmentSpace_head + "}\r\n";
        }

        private static void Write_StreamCaster(SrsStreamCasterConfClass o, out string output, int segmentLevel,
            List<Type> types = null!)
        {
            var output_sip = "";
            output = "";
            var segmentSpace_head = PaddingSegment(segmentLevel).Key;
            var segmentSpace = PaddingSegment(segmentLevel).Value;
            output += segmentSpace_head + o.SectionsName?.ToLower().Trim() + " " + o.InstanceName + " { \r\n";
            foreach (var p in o.GetType().GetProperties())
            {
                var obj = p.GetValue(o);
                if (obj == null) continue;
                if (p.Name.ToLower().Trim() == "sectionsname" || p.Name.ToLower().Trim() == "instancename")
                {
                    continue;
                }

                if ((p.PropertyType == typeof(string) || p.PropertyType == typeof(int?) ||
                     p.PropertyType == typeof(ushort?) ||
                     p.PropertyType == typeof(byte?)
                     || p.PropertyType == typeof(float?) || p.PropertyType == typeof(bool?))
                    || ((types != null) && types.Contains(p.PropertyType)))
                {
                    if (obj != null)
                    {
                        if (p.PropertyType == typeof(bool?))
                        {
                            var s = "";
                            s = Common.GetBoolStr(p, o);
                            var sTmp = segmentSpace + p.Name.Trim().ToLower() + "\t" + s + ";";
                            output += (sTmp + "\r\n");
                        }
                        else
                        {
                            var sTmp = segmentSpace + p.Name.Trim().ToLower() + "\t" + obj + ";";
                            output += (sTmp + "\r\n");
                        }
                    }
                }
            }

            foreach (var p in o.GetType().GetProperties()) //Loop non-basic type data, in order to ensure that the basic type data is on top
            {
                var obj = p.GetValue(o);
                if (obj == null) continue;
                if (p.Name.ToLower().Trim() == "sectionsname" || p.Name.Trim() == "instancename")
                {
                    continue;
                }

                if (p.PropertyType == typeof(Sip))
                {
                    Write_SubOnly(o.sip, out output_sip, 2);
                    output += output_sip;
                }
            }

            output += segmentSpace_head + "}\r\n";
        }


        private static void Write_Vhost_Ingest(Ingest o, out string output, int segmentLevel, List<Type> types = null!)
        {
            output = "";
            var segmentSpace_head = PaddingSegment(segmentLevel).Key;
            var segmentSpace = PaddingSegment(segmentLevel).Value;
            output += segmentSpace_head + o.SectionsName?.ToLower().Trim() + " " + o.IngestName + " { \r\n";
            foreach (var p in o.GetType().GetProperties())
            {
                var obj = p.GetValue(o);
                if (obj == null) continue;
                if (p.Name.ToLower().Trim() == "sectionsname" || p.Name.Trim().ToLower() == "instancename" ||
                    p.Name.ToLower().Trim() == "ingestname")
                {
                    continue;
                }

                if ((p.PropertyType == typeof(string) || p.PropertyType == typeof(int?) ||
                     p.PropertyType == typeof(ushort?) ||
                     p.PropertyType == typeof(byte?)
                     || p.PropertyType == typeof(float?) || p.PropertyType == typeof(bool?))
                    || ((types != null) && types.Contains(p.PropertyType)))
                {
                    if (obj != null)
                    {
                        if (p.PropertyType == typeof(bool?))
                        {
                            var s = "";
                            s = Common.GetBoolStr(p, o);
                            var sTmp = segmentSpace + p.Name.Trim().ToLower() + "\t" + s + ";";
                            output += (sTmp + "\r\n");
                        }
                        else
                        {
                            var sTmp = segmentSpace + p.Name.Trim().ToLower() + "\t" + obj + ";";
                            output += (sTmp + "\r\n");
                        }
                    }
                }
            }

            foreach (var p in o.GetType().GetProperties()) //Loop non-basic type data, in order to ensure that the basic type data is on top
            {
                var obj = p.GetValue(o);
                if (obj == null) continue;
                if (p.Name.ToLower().Trim() == "sectionsname" || p.Name.ToLower().Trim() == "instancename")
                {
                    continue;
                }

                if (p.PropertyType == typeof(IngestInput))
                {
                    var s = "";
                    var types2 = new List<Type>();
                    types2.Add(typeof(IngestInputType?));
                    Write_SubOnly(o.Input, out s, 3, types2);
                    output += s;
                }

                if (p.PropertyType == typeof(List<IngestTranscodeEngine>))
                {
                    var types1 = new List<Type>();
                    types1.Add(typeof(IngestEngineIoformat?));
                    types1.Add(typeof(IngestEngineVprofile?));
                    types1.Add(typeof(IngestEngineVpreset?));
                    if (o.Engines != null)
                        foreach (var i in o.Engines)
                        {
                            var s = "";
                            Write_Vhost_Ingest_Engine(i, out s, 3, types1);
                            output += s;
                        }
                }
            }

            output += segmentSpace_head + "}\r\n";
        }

        private static void Write_Vhost_Transcode(Transcode o, out string output, int segmentLevel,
            List<Type> types = null!)
        {
            output = "";
            var segmentSpace_head = PaddingSegment(segmentLevel).Key;
            var segmentSpace = PaddingSegment(segmentLevel).Value;
            output += segmentSpace_head + o.SectionsName?.ToLower().Trim() + " " + o.InstanceName + " { \r\n";
            foreach (var p in o.GetType().GetProperties())
            {
                var obj = p.GetValue(o);
                if (obj == null) continue;
                if (p.Name.ToLower().Trim() == "sectionsname" || p.Name.ToLower().Trim() == "instancename" ||
                    p.Name.ToLower().Trim() == "ingestname")
                {
                    continue;
                }

                if ((p.PropertyType == typeof(string) || p.PropertyType == typeof(int?) ||
                     p.PropertyType == typeof(ushort?) ||
                     p.PropertyType == typeof(byte?)
                     || p.PropertyType == typeof(float?) || p.PropertyType == typeof(bool?))
                    || ((types != null) && types.Contains(p.PropertyType)))
                {
                    if (obj != null)
                    {
                        if (p.PropertyType == typeof(bool?))
                        {
                            var s = "";
                            s = Common.GetBoolStr(p, o);
                            var sTmp = segmentSpace + p.Name.Trim().ToLower() + "\t" + s + ";";
                            output += (sTmp + "\r\n");
                        }
                        else
                        {
                            var sTmp = segmentSpace + p.Name.Trim().ToLower() + "\t" + obj + ";";
                            output += (sTmp + "\r\n");
                        }
                    }
                }
            }

            foreach (var p in o.GetType().GetProperties()) //Loop non-basic type data, in order to ensure that the basic type data is on top
            {
                var obj = p.GetValue(o);
                if (obj == null) continue;
                if (p.Name.ToLower().Trim() == "sectionsname" || p.Name.ToLower().Trim() == "instancename")
                {
                    continue;
                }

                if (p.PropertyType == typeof(List<IngestTranscodeEngine>))
                {
                    var types1 = new List<Type>();
                    types1.Add(typeof(IngestEngineIoformat?));
                    types1.Add(typeof(IngestEngineVprofile?));
                    types1.Add(typeof(IngestEngineVpreset?));
                    if (o.Engines != null)
                        foreach (var i in o.Engines)
                        {
                            var s = "";
                            Write_Vhost_Ingest_Engine(i, out s, 3, types1);
                            output += s;
                        }
                }
            }

            output += segmentSpace_head + "}\r\n";
        }

        private static void Write_Vhost_Ingest_Engine(IngestTranscodeEngine o, out string output, int segmentLevel,
            List<Type> types = null!)
        {
            output = "";
            var segmentSpace_head = PaddingSegment(segmentLevel).Key;
            var segmentSpace = PaddingSegment(segmentLevel).Value;
            output += segmentSpace_head + o.SectionsName?.ToLower().Trim() + " " + o.EngineName + " { \r\n";
            foreach (var p in o.GetType().GetProperties())
            {
                var obj = p.GetValue(o);
                if (obj == null) continue;
                if (p.Name.ToLower().Trim() == "sectionsname" || p.Name.ToLower().Trim() == "instancename" ||
                    p.Name.ToLower().Trim() == "enginename")
                {
                    continue;
                }

                if ((p.PropertyType == typeof(string) || p.PropertyType == typeof(int?) ||
                     p.PropertyType == typeof(ushort?) ||
                     p.PropertyType == typeof(byte?)
                     || p.PropertyType == typeof(float?) || p.PropertyType == typeof(bool?))
                    || ((types != null) && types.Contains(p.PropertyType)))
                {
                    if (obj != null)
                    {
                        if (p.PropertyType == typeof(bool?))
                        {
                            var s = "";
                            s = Common.GetBoolStr(p, o);
                            var sTmp = segmentSpace + p.Name.Trim().ToLower() + "\t" + s + ";";
                            output += (sTmp + "\r\n");
                        }
                        else
                        {
                            var sTmp = segmentSpace + p.Name.Trim().ToLower() + "\t" + obj + ";";
                            output += (sTmp + "\r\n");
                        }
                    }
                }
            }

            foreach (var p in o.GetType().GetProperties()) //Loop non-basic type data, in order to ensure that the basic type data is on top
            {
                var obj = p.GetValue(o);
                if (obj == null) continue;
                if (p.Name.ToLower().Trim() == "sectionsname" || p.Name.ToLower().Trim() == "instancename")
                {
                    continue;
                }

                if (p.PropertyType == typeof(IngestEnginePerfile))
                {
                    var s = "";
                    Write_SubOnly(o.Perfile, out s, 4);
                    var sArr = s.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
                    var sss = "";
                    foreach (var ss in sArr)
                    {
                        if (ss.ToLower().Contains("re;"))
                        {
                            var i = 0;
                            foreach (var c in ss)
                            {
                                if (c == '\t' && i < 4) sss += c;
                                i++;
                            }

                            sss += "re;\r\n";
                        }
                        else
                        {
                            sss += ss + "\r\n";
                        }
                    }

                    output += sss;
                }

                if (p.PropertyType == typeof(IngestEngineVfilter))
                {
                    var s = "";
                    Write_SubOnly(o.Vfilter, out s, 4);
                    output += s;
                }

                if (p.PropertyType == typeof(IngestEngineVparams))
                {
                    var s = "";
                    Write_SubOnly(o.Vparams, out s, 4);
                    output += s;
                }

                if (p.PropertyType == typeof(IngestEngineAparams))
                {
                    var s = "";
                    Write_SubOnly(o.Aparams, out s, 4);
                    output += s;
                }
            }

            output += segmentSpace_head + "}\r\n";
        }

        private static void Write_Vhost(SrsvHostConfClass o, out string output, int segmentLevel,
            List<Type> types = null!)
        {
            output = "";
            var segmentSpace_head = PaddingSegment(segmentLevel).Key;
            var segmentSpace = PaddingSegment(segmentLevel).Value;
            output += segmentSpace_head + o.SectionsName?.ToLower().Trim() + " " + o.VhostDomain + " { \r\n";
            foreach (var p in o.GetType().GetProperties())
            {
                var obj = p.GetValue(o);
                if (obj == null) continue;
                if (p.Name.ToLower().Trim() == "sectionsname" || p.Name.ToLower().Trim() == "instancename" ||
                    p.Name.ToLower().Trim() == "vhostdomain")
                {
                    continue;
                }

                if ((p.PropertyType == typeof(string) || p.PropertyType == typeof(int?) ||
                     p.PropertyType == typeof(ushort?) ||
                     p.PropertyType == typeof(byte?)
                     || p.PropertyType == typeof(float?) || p.PropertyType == typeof(bool?))
                    || ((types != null) && types.Contains(p.PropertyType)))
                {
                    if (obj != null)
                    {
                        if (p.PropertyType == typeof(bool?))
                        {
                            var s = "";
                            s = Common.GetBoolStr(p, o);
                            var sTmp = segmentSpace + p.Name.Trim().ToLower() + "\t" + s + ";";
                            output += (sTmp + "\r\n");
                        }
                        else
                        {
                            var sTmp = segmentSpace + p.Name.Trim().ToLower() + "\t" + obj + ";";
                            output += (sTmp + "\r\n");
                        }
                    }
                }
            }

            foreach (var p in o.GetType().GetProperties()) //Loop non-basic type data, in order to ensure that the basic type data is on top
            {
                var obj = p.GetValue(o);
                if (obj == null) continue;
                if (p.Name.ToLower().Trim() == "sectionsname" || p.Name.ToLower().Trim() == "instancename")
                {
                    continue;
                }

                if (p.PropertyType == typeof(Rtc))
                {
                    var output_rtc = "";
                    Write_SubOnly(o.Rtc, out output_rtc, 2);
                    output += output_rtc;
                }

                if (p.PropertyType == typeof(Cluster))
                {
                    var output_cluster = "";
                    Write_SubOnly(o.Vcluster, out output_cluster, 2);
                    output += output_cluster;
                }

                if (p.PropertyType == typeof(Forward))
                {
                    var output_forward = "";
                    Write_SubOnly(o.Vforward, out output_forward, 2);
                    output += output_forward;
                }

                if (p.PropertyType == typeof(Play))
                {
                    var output_play = "";
                    var types1 = new List<Type>();
                    types1.Add(typeof(PlayTimeJitter?));
                    Write_SubOnly(o.Vplay, out output_play, 2, types1);
                    output += output_play;
                }

                if (p.PropertyType == typeof(Publish))
                {
                    var output_publish = "";
                    Write_SubOnly(o.Vpublish, out output_publish, 2);
                    output += output_publish;
                }

                if (p.PropertyType == typeof(Refer))
                {
                    var output_refer = "";
                    Write_SubOnly(o.Vrefer, out output_refer, 2);
                    output += output_refer;
                }

                if (p.PropertyType == typeof(Bandcheck))
                {
                    var output_bandcheck = "";
                    Write_SubOnly(o.Vbandcheck, out output_bandcheck, 2);
                    output += output_bandcheck;
                }

                if (p.PropertyType == typeof(Security))
                {
                    var segmentSpace_head1 = PaddingSegment(2).Key;
                    var segmentSpace1 = PaddingSegment(2).Value;
                    output += segmentSpace_head1 + "security" + " { \r\n";
                    if (o.Vsecurity?.Enabled != null)
                    {
                        if (o.Vsecurity.Enabled == true)
                        {
                            output += segmentSpace1 + "enabled\t" + "on;\r\n";
                        }
                        else
                        {
                            output += segmentSpace1 + "enabled\t" + "off;\r\n";
                        }
                    }

                    if (o.Vsecurity?.Seo != null)
                    {
                        foreach (var s in o.Vsecurity.Seo)
                        {
                            output += segmentSpace1 + s.Sem.ToString() + "\t" + s.Set.ToString() + "\t" + s.Rule +
                                      ";\r\n";
                        }
                    }

                    output += segmentSpace_head1 + "}\r\n";
                }

                if (p.PropertyType == typeof(HttpStatic))
                {
                    var output_http_static = "";
                    Write_SubOnly(o.Vhttp_static, out output_http_static, 2);
                    output += output_http_static;
                }

                if (p.PropertyType == typeof(HttpRemux))
                {
                    var output_http_remux = "";
                    Write_SubOnly(o.Vhttp_remux, out output_http_remux, 2);
                    output += output_http_remux;
                }

                if (p.PropertyType == typeof(HttpHooks))
                {
                    var output_http_hooks = "";
                    Write_SubOnly(o.Vhttp_hooks, out output_http_hooks, 2);
                    output += output_http_hooks;
                }

                if (p.PropertyType == typeof(Exec))
                {
                    var output_exec = "";
                    Write_SubOnly(o.Vexec, out output_exec, 2);
                    output += output_exec;
                }

                if (p.PropertyType == typeof(Dash))
                {
                    var output_dash = "";
                    Write_SubOnly(o.Vdash, out output_dash, 2);
                    output += output_dash;
                }

                if (p.PropertyType == typeof(Hls))
                {
                    var output_hls = "";
                    Write_SubOnly(o.Vhls, out output_hls, 2);
                    output += output_hls;
                }

                if (p.PropertyType == typeof(Hds))
                {
                    var output_hds = "";
                    Write_SubOnly(o.Vhds, out output_hds, 2);
                    output += output_hds;
                }

                if (p.PropertyType == typeof(Dvr))
                {
                    var output_dvr = "";
                    var types2 = new List<Type>();
                    types2.Add(typeof(PlayTimeJitter?));
                    Write_SubOnly(o.Vdvr, out output_dvr, 2, types2);
                    output += output_dvr;
                }

                if (p.PropertyType == typeof(Nack))
                {
                    var output_nack = "";
                    Write_SubOnly(o.Vnack, out output_nack, 2);
                    output += output_nack;
                }


                if (p.PropertyType == typeof(List<Ingest>))
                {
                    if (o.Vingests != null)
                        foreach (var i in o.Vingests)
                        {
                            var s = "";
                            var types3 = new List<Type>();
                            types3.Add(typeof(IngestInputType?));
                            Write_Vhost_Ingest(i, out s, 2, types3);
                            output = output + s;
                        }
                }

                if (p.PropertyType == typeof(List<Transcode>))
                {
                    if (o.Vtranscodes != null)
                        foreach (var i in o.Vtranscodes)
                        {
                            var s = "";

                            Write_Vhost_Transcode(i, out s, 2);
                            output = output + s;
                        }
                }
            }

            output += segmentSpace_head + "}\r\n";
        }

        public static string Build(SrsSystemConfClass sccout, string filepath = "")
        {
      
            var output_heartbeat = "";
            var output_httpserver = "";
            var output_httpapi = "";
            var output_pulsar = "";
            var output_rtcserver = "";
            var output_srtserver = "";
            var output_stats = "";
            var output_streamcaster = "";
            var output_vhost = "";
            var output = "";
            var segmentSpace = PaddingSegment(0).Value;
            foreach (var p in sccout.GetType().GetProperties())
            {
                var obj = p.GetValue(sccout);
                if (obj == null) continue;
                if (p.Name.ToLower().Trim() == "sectionsname" || p.Name.ToLower().Trim() == "instancename")
                {
                    continue;
                }

                if (p.PropertyType == typeof(string) || p.PropertyType == typeof(int?) ||
                    p.PropertyType == typeof(ushort?) ||
                    p.PropertyType == typeof(byte?)
                    || p.PropertyType == typeof(float?) || p.PropertyType == typeof(bool?))
                {
                    if (obj != null)
                    {
                        if (p.PropertyType == typeof(bool?))
                        {
                            var s = "";
                            s = Common.GetBoolStr(p, sccout);
                            var sTmp = segmentSpace + p.Name.ToLower().Trim() + "\t" + s + ";";
                            output += (sTmp + "\r\n");
                        }
                        else
                        {
                            var sTmp = segmentSpace + p.Name.ToLower().Trim() + "\t" + obj + ";";
                            output += (sTmp + "\r\n");
                        }
                    }
                }
            }

            foreach (var p in sccout.GetType().GetProperties()) //Loop non-basic type data, in order to ensure that the basic type data is on top
            {
                var obj = p.GetValue(sccout);
                if (obj == null) continue;
                if (p.Name.ToLower().Trim() == "sectionsname" || p.Name.ToLower().Trim() == "instancename")
                {
                    continue;
                }

                if (p.PropertyType == typeof(SrsHeartbeatConfClass))
                {
                    Write_SubOnly(sccout.Heartbeat, out output_heartbeat, 1);
                    output = output + output_heartbeat;
                }

                if (p.PropertyType == typeof(SrsHttpApiConfClass))
                {
                    Write_HttpApi(sccout.Http_api, out output_httpapi, 1);
                    output = output + output_httpapi;
                }

                if (p.PropertyType == typeof(SrsHttpServerConfClass))
                {
                    Write_SubOnly(sccout.Http_server, out output_httpserver, 1);
                    output = output + output_httpserver;
                }

                if (p.PropertyType == typeof(SrsPulsarConfClass))
                {
                    Write_SubOnly(sccout.Pulsar, out output_pulsar, 1);
                    output = output + output_pulsar;
                }

                if (p.PropertyType == typeof(SrsRtcServerConfClass))
                {
                    Write_RtcServer(sccout.Rtc_server, out output_rtcserver, 1);
                    output = output + output_rtcserver;
                }

                if (p.PropertyType == typeof(SrsSrtServerConfClass))
                {
                    Write_SubOnly(sccout.Srt_server, out output_srtserver, 1);
                    output = output + output_srtserver;
                }

                if (p.PropertyType == typeof(SrsStatsConfClass))
                {
                    Write_SubOnly(sccout.Stats, out output_stats, 1);
                    output = output + output_stats;
                }

                if (p.PropertyType == typeof(List<SrsStreamCasterConfClass>))
                {
                    if (sccout.Stream_casters != null)
                        foreach (var s in sccout.Stream_casters)
                        {
                            
                            var types = new List<Type>();
                            types.Add(typeof(CasterEnum?));
                            Write_StreamCaster(s, out output_streamcaster, 1, types);
                            output = output + output_streamcaster;
                        }
                }

                if (p.PropertyType == typeof(List<SrsvHostConfClass>))
                {
                    if (sccout.Vhosts != null)
                        foreach (var s in sccout.Vhosts)
                        {
                            Write_Vhost(s, out output_vhost, 1);
                            output = output + output_vhost;
                        }
                }
            }

            if (!string.IsNullOrEmpty(filepath))
            {
                File.WriteAllText(filepath, output);
            }

            return output;
        }
    }
}