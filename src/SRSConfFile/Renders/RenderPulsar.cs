using System.Collections.Generic;
using SrsConfFile.SRSConfClass;

namespace SrsConfFile.Renders
{
    public static class RenderPulsar
    {
        public static void Render(SectionBody scbin, SrsSystemConfClass sccout, string instanceName = "")
        {
            if (sccout.Pulsar == null) sccout.Pulsar = new SrsPulsarConfClass();
            sccout.Pulsar.SectionsName = "pulsar";
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
                            sccout.Pulsar.Enabled = Common.Str2bool(tmpkv.Value);
                            break;
                        case "brokers":
                            sccout.Pulsar.Brokers = tmpkv.Value;
                            break;
                        case "topic":
                            sccout.Pulsar.Topic = tmpkv.Value;
                            break;
                    }
                }
        }
    }
}