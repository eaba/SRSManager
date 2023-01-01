using System;
using System.Net;
using System.Web.Mvc;

namespace SRSManageCommon.ControllerStructs.ResponseModules
{
    /// <summary>
    /// webapi return structure
    /// </summary>
    [Serializable]
    public class BaseResponseModule
    {
        private HttpStatusCode _code;
        private JsonResult _msg = null!;

        /// <summary>
        /// return http status code
        /// </summary>
        public HttpStatusCode Code
        {
            get => _code;
            set => _code = value;
        }

        /// <summary>
        /// returned message
        /// </summary>
        public JsonResult Msg
        {
            get => _msg;
            set => _msg = value;
        }
    }
}