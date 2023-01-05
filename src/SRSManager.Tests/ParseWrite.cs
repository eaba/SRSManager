using SrsConfFile.SRSConfClass;
using SrsConfFile;
using Xunit.Abstractions;

namespace SRSManager.Tests
{
    public class ParseWrite
    {
        private readonly ITestOutputHelper _output;
        public ParseWrite(ITestOutputHelper output)
        {
            _output = output;
        }
        [Fact]
        public void TestParse() 
        {
            /*Package and publish into a single executable file and run it manually in the project directory：dotnet publish -r win-x64 -c Release /p:PublishSingleFile=true /p:PublishTrimmed=true  
               win-x64 is the target platform, such as osx-x64, linux-x64, etc.
               PublishSingleFile true is published as a single file, PublishTrimmed true, compression can prevent cracking*/
            SrsConfigParse.LoadSrsConfObject("/Users/ebere/test.conf"); //load configuration file
            SrsConfigParse.Parse(); //Analysis Profile 
            SrsConfigParse.Trim(); //Configuration deduplication
            var srs = new SrsSystemConfClass(); //Create an SRS configuration instance
            SrsConfigParse.Render(SrsConfigParse.RootSection, srs); //Write SRS configuration instance
            SrsConfigParse.Render(SrsConfigParse.RootSection, srs); //Write SRS configuration instance
            _output.WriteLine(SrsConfigBuild.Build(srs, "conf/full1.conf")); //Rebuild configuration file, output file
            Console.Read();
            _output.WriteLine("end!");
        }
    }
}
