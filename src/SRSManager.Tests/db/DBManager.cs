using System.Diagnostics;
using FreeSql;
using SqlSugar;

namespace SRSManager.Tests.db
{
    public static class DBManager
    {
        public static SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
        {
            DbType = DbType.Sqlite,
            ConnectionString = "DataSource=/Users/qiuzhouwei/mytest.db",
            InitKeyType = InitKeyType.Attribute,
            IsAutoCloseConnection = true,
            AopEvents = new AopEvents
            {
                OnLogExecuting = (sql, p) =>
                {
                    Console.WriteLine("pringSql:" + sql);
                    Console.WriteLine("printJson:" + string.Join(",", p?.Select(it => it.ParameterName + ":" + it.Value)));
                }
            }
        });
        public static IFreeSql fsql = new FreeSqlBuilder()
           .UseConnectionString(DataType.MySql, "Data Source=192.168.2.35;Port=3306;User ID=root;Password=cdtnb...; Initial Catalog=srswebapi;Charset=utf8; SslMode=none;Min pool size=1")
           .UseMonitorCommand(cmd => Trace.WriteLine($"thread：{cmd.CommandText}\r\n"))
           .UseAutoSyncStructure(true)//Automatically create and migrate entity table structures
           .UseNoneCommandParameter(true)
           .Build();
    }
}