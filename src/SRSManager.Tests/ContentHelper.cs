using SrsApis.SrsManager;
using System.Text;
using System.Text.Json;

namespace SRSManager.Tests
{
    public static class ContentHelper
    {
        public static StringContent StringContent(this List<SrsManager> obj)
            => new StringContent(JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true }), Encoding.Default, "application/json");
    }
}
