using SrsManageCommon;
using Xunit.Abstractions;

namespace SRSManager.Tests
{
    public class FFmpeg
    {
        private readonly ITestOutputHelper _output;
        public FFmpeg(ITestOutputHelper output)
        {
            _output = output;
        }
        [Fact]
        public void Test_FFmpeg_GetDurationConsole()
        {
            _output.WriteLine("Hello World!");
            var b = FFmpegGetDuration.GetDuration("/usr/local/bin/ffmpeg",
                "/Users/ebere/Downloads/iAETAqNtcDQDAQQABQAG2gAjhAGkC1ijNgKqjQN_hy6n5qQn_APPAAABcjCTN0AEzgAOpOcHzg4ccE0IAA.mp4",
                out long i, out string path);
            _output.WriteLine(
                Path.GetDirectoryName(
                    "/root/StreamNode/eb1d30e2-1c69-4047-a08b-0003b66f430c/wwwroot/dvr/20200528/__defaultVhost__/live/34020000001330000001@34020000001320000001/23/20200528232737___defaultVhost___live_34020000001330000001@34020000001320000001.flv")
            );
            _output.WriteLine("Duration (ms)" + i);
        }
    }
}
