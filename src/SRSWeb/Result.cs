using SRSManageCommon.ManageStructs;
using SrsManageCommon;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace SRSWeb
{
    public class Result
    {
        /// <summary>
        /// Unified processing of apis return results
        /// </summary>
        /// <param name="rt">return value</param>
        /// <param name="rs">ResponseStruct</param>
        /// <returns></returns>
        public static JsonResult DelApisResult(object rt, ResponseStruct rs)
        {
            if (rs.Code != (int)ErrorNumber.None)
            {
                return new JsonResult(rs) { StatusCode = (int)HttpStatusCode.BadRequest };
            }

            return new JsonResult(rt) { StatusCode = (int)HttpStatusCode.OK };
        }
    }
}
