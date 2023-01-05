using SRSManager.Tests.db;
using Xunit.Abstractions;

namespace SRSManager.Tests
{
    public class FreeSql
    {
        private readonly ITestOutputHelper _output;
        public FreeSql(ITestOutputHelper output)
        {
            _output = output;
        }
        [Fact]
        public void Free_Sql()
        {
            DBManager.fsql.Delete<SRSManageCommon.DBMoudle.StreamDvrPlan>().Where("1=1").ExecuteAffrows();
            DBManager.fsql.Delete<SRSManageCommon.DBMoudle.DvrDayTimeRange>().Where("1=1").ExecuteAffrows();
            var a = new SRSManageCommon.DBMoudle.StreamDvrPlan();
            a.App = "live";
            a.Enable = true;
            a.Stream = "stream";
            a.DeviceId = "device_id123";
            a.LimitDays = 10;
            a.LimitSpace = 99999999;
            a.VhostDomain = "vhost";
            a.OverStepPlan = SRSManageCommon.DBMoudle.OverStepPlan.DeleteFile;
            a.TimeRangeList = new List<SRSManageCommon.DBMoudle.DvrDayTimeRange>();
            var b = new SRSManageCommon.DBMoudle.DvrDayTimeRange();
            b.EndTime = DateTime.Now.AddDays(10);
            b.StartTime = DateTime.Now;
            b.WeekDay = DateTime.Now.DayOfWeek;
            a.TimeRangeList.Add(b);
            var c = new SRSManageCommon.DBMoudle.DvrDayTimeRange();
            c.EndTime = DateTime.Now.AddDays(15);
            c.StartTime = DateTime.Now.AddDays(-5);
            c.WeekDay = DayOfWeek.Monday;
            a.TimeRangeList.Add(c);
            /*Insert with subclasses*/
            var repo = DBManager.fsql.GetRepository<SRSManageCommon.DBMoudle.StreamDvrPlan>();
            repo.DbContextOptions.EnableAddOrUpdateNavigateList = true; //Need to open manually
            repo.Insert(a);
            /*Insert with subclasses*/
            /* Check out together with subclasses */
            var ret = DBManager.fsql.Select<SRSManageCommon.DBMoudle.StreamDvrPlan>().IncludeMany(a => a.TimeRangeList)
                .ToList();
            /* Check out together with subclasses */

            _output.WriteLine("Hello World!");
            Assert.True(ret.Any());
        }
    }
}
