
using SrsApis.SrsManager;
using SRSManager.Tests.Fixtures;
using Xunit.Abstractions;
using System.Text.Json;
using SRSManager.Shared;

namespace SRSManager.Tests
{
    public class WebApis : IntegrationTest
    {
        public static CommonFunctions CommonFunctions = new CommonFunctions();
        private readonly ITestOutputHelper _output;
        public WebApis(ITestOutputHelper output, ApiWebApplicationFactory fixture)
            : base(fixture) 

        {
            _output = output;
        }
        [Fact]
        public async ValueTask Test_Allow_GetSession()
        {
            var srs = new List<SrsManager>();   
            var s  = await _client.PostAsync("/Allow/GetSession", srs.StringContent());
            _output.WriteLine(JsonSerializer.Serialize(s, options: new JsonSerializerOptions { WriteIndented = true}));
            Assert.NotNull(s);  
        }
    }
}