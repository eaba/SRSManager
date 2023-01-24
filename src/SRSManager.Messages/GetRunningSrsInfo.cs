

namespace SRSManager.Messages
{
    public record struct GetRunningSrsInfo
    { 
        public static GetRunningSrsInfo Instance = new GetRunningSrsInfo();
    }
    public record struct StopSrs
    {
        public static StopSrs Instance = new StopSrs();
    }
    public record struct InitAndStart
    {
        public static InitAndStart Instance = new InitAndStart();
    }
    public record struct GetRunningSrsInfoList
    {
        public static GetRunningSrsInfoList Instance = new GetRunningSrsInfoList();
    }
    public record struct StopAllSrs
    {
        public static StopAllSrs Instance = new StopAllSrs();
    }
    public record struct InitAndStartAllSrs
    {
        public static InitAndStartAllSrs Instance = new InitAndStartAllSrs();
    }
    
}
