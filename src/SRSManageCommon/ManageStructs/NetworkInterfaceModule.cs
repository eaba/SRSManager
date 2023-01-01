#nullable enable
namespace SRSManageCommon.ManageStructs
{
    public class NetworkInterfaceModule
    {
        private ushort? _index;

        //  private string? speed; //linux not Supported
        private string? _ipaddr;
        private string? _mac;
        private string? _name;

        private string? _type;

        public ushort? Index
        {
            get => _index;
            set => _index = value;
        }

        public string? Name
        {
            get => _name;
            set => _name = value;
        }

        public string? Mac
        {
            get => _mac;
            set => _mac = value;
        }

        public string? Type
        {
            get => _type;
            set => _type = value;
        }

        /*public string? Speed //linux not Supported
        {
            get => speed;
            set => speed = value;
        }*/

        public string? Ipaddr
        {
            get => _ipaddr;
            set => _ipaddr = value;
        }
    }
}