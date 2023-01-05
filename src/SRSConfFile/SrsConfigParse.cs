using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SrsConfFile.Renders;
using SrsConfFile.SRSConfClass;

namespace SrsConfFile
{
    public class SectionBody
    {
        public string? body;
        private List<string>? bodyList = new List<string>();
        public int? endline;
        public int? startline;
        private List<SectionBody>? subSections = new List<SectionBody>();

        public List<string>? BodyList
        {
            get => bodyList;
            set => bodyList = value;
        }

        public List<SectionBody>? SubSections
        {
            get => subSections;
            set => subSections = value;
        }
    }


    public static class SrsConfigParse
    {
        private static char sectionStartChar = '{';
        private static char sectionEndChar = '}';
        private static SectionBody rootSection = new SectionBody();
        private static List<SectionBody> sectionBody = new List<SectionBody>();

        public static List<SectionBody> SectionBodys
        {
            get => sectionBody;
            set => sectionBody = value;
        }

        public static SectionBody RootSection
        {
            get => rootSection;
            set => rootSection = value;
        }

        public static char SectionStartChar
        {
            get => sectionStartChar;
            set => sectionStartChar = value;
        }

        public static char SectionEndChar
        {
            get => sectionEndChar;
            set => sectionEndChar = value;
        }


        public static void Render(SectionBody scbin, SrsSystemConfClass sccout)
        {
            if (sccout == null) sccout = new SrsSystemConfClass();
            if (Common.IsRoot(scbin))
            {
                RenderRoot.Render(scbin, sccout);
            }

            if (scbin.SubSections != null)
                foreach (var scb in scbin.SubSections)
                {
                    var tmpkv = Common.GetSectionNameAndInstanceNameValue(scb);
                    var cmd = tmpkv.Key;
                    switch (cmd)
                    {
                        case "heartbeat":
                            RenderHeartbeat.Render(scb, sccout, tmpkv.Value);
                            break;
                        case "http_api":
                            RenderHttpApi.Render(scb, sccout, tmpkv.Value);
                            break;
                        case "http_server":
                            RenderHttpServer.Render(scb, sccout, tmpkv.Value);
                            break;
                        case "srt_server":
                            RenderSrtServer.Render(scb, sccout, tmpkv.Value);
                            break;
                        case "stats":
                            RenderStats.Render(scb, sccout, tmpkv.Value);
                            break;
                        case "stream_caster":
                            RenderStreamCaster.Render(scb, sccout, tmpkv.Value);
                            break;
                        case "rtc_server":
                            RenderRtcServer.Render(scb, sccout, tmpkv.Value);
                            break;
                        case "vhost":
                            RenderVHost.Render(scb, sccout, tmpkv.Value);
                            break;
                    }
                }
        }

        /// <summary>
        /// fix for conf file if a line only write {, it will be del this line and upline will add a { to upline ending
        /// </summary>
        /// <param name="scb"></param>
        public static void FixFormat(SectionBody scb)
        {
            if (scb.BodyList != null)
                for (var i = 0; i <= scb.BodyList.Count - 1; i++)
                {
                    var tmp = scb.BodyList[i];
                    if (tmp.Trim() == SectionStartChar.ToString())
                    {
                        scb.BodyList[i - 1] = scb.BodyList[i - 1] + SectionStartChar;
                        scb.BodyList[i] = "";
                    }
                }
        }

        /// <summary>
        /// Parse all conf line to sections and sections subsections.....
        /// </summary>
        public static void Parse()
        {
            FindSection2(RootSection, false);
            if (RootSection.SubSections != null)
                foreach (var scb in RootSection.SubSections)
                {
                    FindSection2(scb, true);
                    if (scb.SubSections != null)
                        foreach (var scbscb in scb.SubSections)
                        {
                            FindSection2(scbscb, true);
                            if (scbscb.SubSections != null)
                                foreach (var scbscbscb in scbscb.SubSections)
                                {
                                    FindSection2(scbscbscb, true);
                                    if (scbscbscb.SubSections != null)
                                        foreach (var scbscbscbscb in scbscbscb.SubSections)
                                        {
                                            FindSection2(scbscbscbscb, true);
                                        }
                                }
                        }
                }
        }


        private static void TrimSpace(SectionBody scbin)
        {
            if (scbin.SubSections != null)
                foreach (var scb in scbin.SubSections)
                {
                    if (scb.BodyList != null)
                    {
                        for (var i = scb.BodyList.Count - 1; i >= 0; i--)
                        {
                            if (String.IsNullOrEmpty(scb.BodyList[i].Trim()))
                            {
                                scb.BodyList.RemoveAt(i);
                            }
                        }

                        scb.body = "";
                        for (var i = 0; i <= scb.BodyList.Count - 1; i++)
                        {
                            scb.body += scb.BodyList[i] + "\r\n";
                        }
                    }
                }
        }

        /// <summary>
        /// clear the spaceline
        /// </summary>
        public static void Trim()
        {
            if (RootSection.BodyList != null)
            {
                for (var i = RootSection.BodyList.Count - 1; i >= 0; i--)
                {
                    if (String.IsNullOrEmpty(RootSection.BodyList[i].Trim()))
                    {
                        RootSection.BodyList.RemoveAt(i);
                    }
                }

                RootSection.body = "";
                for (var i = 0; i <= RootSection.BodyList.Count - 1; i++)
                {
                    RootSection.body += RootSection.BodyList[i] + "\r\n";
                }
            }

            TrimSpace(RootSection);
            if (RootSection.SubSections != null)
                foreach (var scb in RootSection.SubSections)
                {
                    TrimSpace(scb);
                    if (scb.SubSections != null)
                        foreach (var scbscb in scb.SubSections)
                        {
                            TrimSpace(scbscb);
                            if (scbscb.SubSections != null)
                                foreach (var scbscbscb in scbscb.SubSections)
                                {
                                    TrimSpace(scbscbscb);
                                    if (scbscbscb.SubSections != null)
                                        foreach (var scbscbscbscb in scbscbscb.SubSections)
                                        {
                                            TrimSpace(scbscbscbscb);
                                        }
                                }
                        }
                }
        }

        private static void FindSection2(SectionBody scbin, bool withoutSelf = false)
        {
            var startcount = 0;
            var linenostart = 0;
            var linenoend = 0;

            var i = 0;
            if (scbin.BodyList != null)
            {
                var endline = scbin.BodyList.Count - 1;
                if (withoutSelf)
                {
                    i = 1;
                    endline = scbin.BodyList.Count - 2;
                }

                for (; i <= endline; i++)
                {
                    var tmpStr = scbin.BodyList[i];
                    if (tmpStr.Contains(SectionStartChar))
                    {
                        if (startcount == 0)
                        {
                            linenostart = i;
                        }

                        startcount++;
                    }
                    else if (tmpStr.Contains(SectionEndChar))
                    {
                        linenoend = i;
                        startcount--;
                        if (startcount == 0)
                        {
                            var scb = new SectionBody();
                            scb.startline = linenostart;
                            scb.endline = linenoend;
                            for (var k = linenostart; k <= linenoend; k++)
                            {
                                scb.body += scbin.BodyList[k] + "\r\n";
                                scb.BodyList?.Add(scbin.BodyList[k]);
                                scbin.BodyList[k] = "";
                            }

                            scbin.SubSections?.Add(scb);
                            scbin.body = "";
                            for (var f = 0; f <= scbin.BodyList.Count - 1; f++)
                            {
                                scbin.body += scbin.BodyList[f] + "\r\n";
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// load the conf file,with out # and break the spaceline
        /// </summary>
        /// <param name="srsconf"></param>
        /// <returns></returns>
        public static bool LoadSrsConfObject(SrsSystemConfClass srsconf)
        {
            if (!File.Exists(srsconf.ConfFilePath)) return false;
            RootSection.BodyList?.Clear();
            if (srsconf.ConfigLines != null)
                foreach (var str in srsconf.ConfigLines)
                {
                    var tmp_str = "";
                    if (string.IsNullOrEmpty(str.Trim())) continue;
                    if (str.Trim().StartsWith('#')) continue;
                    if (str.Trim().Contains("#"))
                    {
                        foreach (var c in str.Trim())
                        {
                            if (c != '#')
                            {
                                tmp_str += c;
                            }
                            else
                            {
                                break;
                            }
                        }

                        rootSection.BodyList?.Add(tmp_str);
                    }
                    else
                    {
                        rootSection.BodyList?.Add(str);
                    }
                }

            return true;
        }

        public static bool LoadSrsConfObject(string filepath)
        {
            if (!File.Exists(filepath)) return false;
            var lines = new List<string>();
            foreach (var str in File.ReadAllLines(filepath, Encoding.Default))
            {
                var str_tmp = str.Trim();
                if (!string.IsNullOrEmpty(str_tmp) && !str_tmp.StartsWith('#'))
                {
                    lines.Add(str_tmp);
                }
            }

            RootSection.BodyList?.Clear();
            if (lines != null)
                foreach (var str in lines)
                {
                    var tmp_str = "";
                    if (string.IsNullOrEmpty(str.Trim())) continue;
                    if (str.Trim().StartsWith('#')) continue;
                    if (str.Trim().Contains("#"))
                    {
                        foreach (var c in str.Trim())
                        {
                            if (c != '#')
                            {
                                tmp_str += c;
                            }
                            else
                            {
                                break;
                            }
                        }

                        rootSection.BodyList?.Add(tmp_str);
                    }
                    else
                    {
                        rootSection.BodyList?.Add(str);
                    }
                }

            return true;
        }

        public static bool LoadSrsConfObject(List<string> lines)
        {
            if (lines == null) return false;
            RootSection.BodyList?.Clear();
            if (lines != null)
                foreach (var str in lines)
                {
                    var tmp_str = "";
                    if (string.IsNullOrEmpty(str.Trim())) continue;
                    if (str.Trim().StartsWith('#')) continue;
                    if (str.Trim().Contains("#"))
                    {
                        foreach (var c in str.Trim())
                        {
                            if (c != '#')
                            {
                                tmp_str += c;
                            }
                            else
                            {
                                break;
                            }
                        }

                        rootSection.BodyList?.Add(tmp_str);
                    }
                    else
                    {
                        rootSection.BodyList?.Add(str);
                    }
                }

            return true;
        }
    }
}