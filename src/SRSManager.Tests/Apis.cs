using SrsApis.SrsManager.Apis;
using SrsApis.SrsManager;
using SrsConfFile.SRSConfClass;
using SrsConfFile;
using SRSManageCommon.ManageStructs;
using SrsManageCommon;
using Common = SRSApis.Common;
using Xunit.Abstractions;

namespace SRSManager.Tests
{
    public class Apis
    {
        private readonly ITestOutputHelper _output;
        public Apis(ITestOutputHelper output)
        {
            _output = output;
        }
        [Fact]
        public void GetAllSrsManagerDeviceId()
        {
            var stream =
                "{\"code\":0,\"server\":87846,\"data\":{\"ok\":true,\"now_ms\":1591068638439,\"self\":{\"version\":\"4.0.23\",\"pid\":29282,\"ppid\":1,\"argv\":\"/root/StreamNode/srs -c /root/StreamNode/22364bc4-5134-494d-8249-51d06777fb7f.conf\",\"cwd\":\"/root/StreamNode\",\"mem_kbyte\":71448,\"mem_percent\":0.00,\"cpu_percent\":0.09,\"srs_uptime\":214},\"system\":{\"cpu_percent\":0.02,\"disk_read_KBps\":0,\"disk_write_KBps\":0,\"disk_busy_percent\":0.00,\"mem_ram_kbyte\":16266040,\"mem_ram_percent\":0.06,\"mem_swap_kbyte\":8257532,\"mem_swap_percent\":0.00,\"cpus\":8,\"cpus_online\":8,\"uptime\":162062.71,\"ilde_time\":1275660.46,\"load_1m\":0.12,\"load_5m\":0.22,\"load_15m\":0.19,\"net_sample_time\":1591068632439,\"net_recv_bytes\":0,\"net_send_bytes\":0,\"net_recvi_bytes\":458866896997,\"net_sendi_bytes\":218579639053,\"srs_sample_time\":1591068638439,\"srs_recv_bytes\":447805521,\"srs_send_bytes\":33944,\"conn_sys\":55,\"conn_sys_et\":29,\"conn_sys_tw\":10,\"conn_sys_udp\":4,\"conn_srs\":10}}}";
            var a = JsonHelper.FromJson<SrsSystemInfo>(stream);
            _output.WriteLine(a.ToString());
            _output.WriteLine("Hello World!");
            _output.WriteLine(AppDomain.CurrentDomain.BaseDirectory);
            _output.WriteLine(JsonHelper.ToJson(SystemApis.GetSystemInfo()));
            Common.Init_SrsServer();
            Common.StartServers();
            var srsdevidlist = SystemApis.GetAllSrsManagerDeviceId();
            _output.WriteLine(JsonHelper.ToJson(srsdevidlist));
            foreach (var s in srsdevidlist)
            {
                var put = JsonHelper.ToJson(SystemApis.GetSrsManagerInstanceByDeviceId(s));
                _output.WriteLine(put);
                Assert.True(put != null);
            }

            if (srsdevidlist.Count > 0)
            {
                var srsm = SystemApis.GetSrsManagerInstanceByDeviceId(srsdevidlist[0]);
                Assert.True(srsm != null);
                var d = "www.test1cn.tyz";

                _output.WriteLine("pid:" + srsm.IsRunning);
                ResponseStruct rs;
                var vhost = VhostApis.GetVhostTemplate(VhostIngestInputType.Stream, out rs);
                vhost.VhostDomain = d;
                VhostApis.SetVhost(srsm.SrsDeviceId, vhost, out rs);
                var rtc = new Rtc();
                rtc.KeepBFrame = false;
                rtc.Enabled = true;
                //VhostRtcApis.SetVhostRtc(srsm.SrsDeviceId, d, rtc, out rs);
                var dvr = new Dvr();
                dvr.Enabled = true;
                dvr.Dvr_path = "/dvr/path/";
                VhostDvrApis.SetVhostDvr(srsm.SrsDeviceId, d, dvr, out rs);
                var hds = new Hds();
                hds.Enabled = true;
                hds.Hds_window = 50;

                //VhostHdsApis.SetVhostHds(srsm.SrsDeviceId, d, hds, out rs);
                SrtServerApis.DeleteSrtServer(srsm.SrsDeviceId, out rs);
                var srt = new SrsSrtServerConfClass();
                srt = SrtServerApis.GetSrtServerTemplate(out rs);

                srt.Enabled = true;
                SrtServerApis.SetSrtServer(srsm.SrsDeviceId, srt, out rs);

                VhostApis.DeleteVhostByDomain(srsm.SrsDeviceId, "__defaultvhost__", out rs);
                //VhostRtcApis.DeleteVhostRtc(srsm.SrsDeviceId, d, out rs);
                //VhostHdsApis.DeleteVhostHds(srsm.SrsDeviceId, d, out rs);

                var b = SrsConfigBuild.Build(srsm.Srs, srsm.SrsConfigPath);
                Assert.True(b != null);
                if (srsm.IsRunning)
                {
                    var ret = srsm.Reload(out rs);
                }
            }
        }
    }
}
