using SRSManager.Tests.db;
using Xunit.Abstractions;

namespace SRSManager.Tests
{
    public class SqlSugar
    {
        private readonly ITestOutputHelper _output;
        public SqlSugar(ITestOutputHelper output)
        {
            _output = output;
        }
        [Fact]
        public void StreamDvrPlan()
        {
            DBManager.db.DbMaintenance.CreateDatabase(); //Build database
            var dt = DBManager.db.Ado.GetDataTable("select 1"); //test
            DBManager.db.CodeFirst.InitTables(typeof(DvrDayTimeRange), typeof(StreamDvrPlan));//build table

            /*DBManager.db.Insertable<>()*/

            _output.WriteLine(dt.ToString());
            _output.WriteLine("Hello World!");
            Assert.True(dt != null);
        }
    }
}
