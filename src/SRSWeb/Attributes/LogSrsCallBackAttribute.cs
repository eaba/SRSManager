using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using SrsManageCommon;

namespace SRSWeb.Attributes
{
    /// <summary>
    /// logging
    /// </summary>
    public class LogSrsCallBackAttribute : Attribute, IActionFilter
    {
        /// <summary>
        /// 请求后
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        /// <summary>
        /// after request
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            string remoteIpAddr = context.HttpContext.Connection.RemoteIpAddress.ToString();
            if (context.HttpContext.Request.Method.Equals("get", StringComparison.InvariantCultureIgnoreCase))
            {
                LogWebApiWriter.WriteWebApiLog(
                    $@"INPUT    {remoteIpAddr}    {context.HttpContext.Request.Method}    {context.HttpContext.Request.Path}",
                    $@"{JsonConvert.SerializeObject(context.ActionArguments)}", ConsoleColor.Gray);
            }
            else
            {
                LogWebApiWriter.WriteWebApiLog(
                    $@"INPUT    {remoteIpAddr}    {context.HttpContext.Request.Method}    {context.HttpContext.Request.Path}",
                    $@"{JsonConvert.SerializeObject(context.ActionArguments)}", ConsoleColor.Gray);
            }
        }
    }
}