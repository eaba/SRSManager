#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace SRSManageCommon.ControllerStructs.RequestModules
{
    /// <summary>
    /// Initialize the request structure of the onvif device
    /// </summary>
    [Serializable]
    public class ReqInitOnvif
    {
        private List<string> _ipAddrArray = new List<string>();
        private string _ipAddrs = null!;
        private string? _password;
        private string? _username;

        /// <summary>
        /// ip address string, multiple ip addresses are separated by spaces
        /// </summary>
        public string IpAddrs
        {
            get => _ipAddrs;
            set => _ipAddrs = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// username
        /// </summary>
        public string? Username
        {
            get => _username;
            set => _username = value;
        }

        /// <summary>
        /// password
        /// </summary>
        public string? Password
        {
            get => _password;
            set => _password = value;
        }

        [JsonIgnore]

        /// <summary>
        /// No need to pass during initialization, this field is for internal use
        /// </summary>
        public List<string> IpAddrArray
        {
            get => _ipAddrArray;
            set => _ipAddrArray = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Get a list of ip addresses from an ip address string
        /// </summary>
        public void GetIpArray()
        {
            if (!string.IsNullOrEmpty(IpAddrs))
            {
                _ipAddrArray = Regex.Split(_ipAddrs, @"[\s]+").ToList();
                // _ipAddrArray = _ipAddrs.Split(" ", StringSplitOptions.RemoveEmptyEntries).ToList();
            }
        }
    }
}