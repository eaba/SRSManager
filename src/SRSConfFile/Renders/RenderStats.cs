using SrsConfFile.SRSConfClass;

namespace SrsConfFile.Renders
{
    public static class RenderStats
    {
        public static void Render(SectionBody scbin, SrsSystemConfClass sccout, string instanceName = "")
        {
            if (sccout.Stats == null) sccout.Stats = new SrsStatsConfClass();
            sccout.Stats.SectionsName = "stats";
            if (scbin.BodyList != null)
                foreach (var s in scbin.BodyList)
                {
                    if (!s.Trim().EndsWith(";")) continue;
                    var tmpkv = Common.GetKV(s);
                    if (string.IsNullOrEmpty(tmpkv.Key)) continue;

                    var cmd = tmpkv.Key.Trim().ToLower();
                    switch (cmd)
                    {
                        case "network":
                            sccout.Stats.Network = Common.Str2byte(tmpkv.Value);
                            break;

                        case "disk":
                            sccout.Stats.Disk = tmpkv.Value;
                            break;
                    }
                }
        }
    }
}