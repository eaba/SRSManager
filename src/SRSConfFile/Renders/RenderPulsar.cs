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
                foreach (var s in scbin.BodyList)
                {
                    if (!s.Trim().EndsWith(";")) continue;
                    var tmpkv = Common.GetKV(s);
                    if (string.IsNullOrEmpty(tmpkv.Key)) continue;

                    var cmd = tmpkv.Key.Trim().ToLower();
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