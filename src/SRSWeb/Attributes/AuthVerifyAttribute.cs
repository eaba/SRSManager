using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SrsManageCommon;

namespace SRSWeb.Attributes
{
    /// <summary>
    /// Classes for verifying session and allowkey
    /// </summary>
    public class AuthVerifyAttribute : Attribute, IActionFilter
    {
        /// <summary>
        /// after the request
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        /// <summary>
        /// The request is in progress, judging the legitimacy of the user session and allowkey
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (Program.CommonFunctions.IsDebug) return;
            string remoteIpAddr = context.HttpContext.Connection.RemoteIpAddress.ToString();
            string sessionCode = context.HttpContext.Request.Headers["SessionCode"];
            string allowKey = context.HttpContext.Request.Headers["Allowkey"];
            if (sessionCode == null || !Program.CommonFunctions.CheckSession(sessionCode))
            {
                var result = new JsonResult(ErrorMessage.ErrorDic?[ErrorNumber.SystemSessionExcept]);
                result.StatusCode = (int) HttpStatusCode.BadRequest;
                context.Result = result;
            }

            if (allowKey == null || !Program.CommonFunctions.CheckAllow(remoteIpAddr, allowKey))
            {
                var result = new JsonResult(ErrorMessage.ErrorDic?[ErrorNumber.SystemCheckAllowKeyFail]);
                result.StatusCode = (int) HttpStatusCode.BadRequest;
                context.Result = result;
            }
        }
    }
}