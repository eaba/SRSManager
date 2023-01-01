using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using SrsManageCommon;
using SRSManageCommon.ManageStructs;

namespace SrsWebApi.Attributes
{
    /// <summary>
    /// logging
    /// </summary>
    public class LogAttribute : Attribute, IActionFilter
    {
        /// <summary>
        /// after request
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuted(ActionExecutedContext context)
        {
            try
            {
                string info = $@"StatusCode:{((context.Result as JsonResult)!).StatusCode}";
                string remoteIpAddr = context.HttpContext.Connection.RemoteIpAddress.ToString();
                if (((context.Result as JsonResult)!).StatusCode != (int) HttpStatusCode.OK)
                {
                    info =
                        $@"{info}  Body: {JsonConvert.SerializeObject(((context.Result as JsonResult)!).Value as ResponseStruct)}";
                    LogWebApiWriter.WriteWebApiLog(
                        $@"OUTPUT    {remoteIpAddr}    {context.HttpContext.Request.Method}    {context.HttpContext.Request.Path}",
                        info,
                        ConsoleColor.Yellow);
                }
                else
                {
                    info = $@"{info}  Body: {JsonConvert.SerializeObject(((context.Result as JsonResult)!).Value)}";
                    LogWebApiWriter.WriteWebApiLog(
                        $@"OUTPUT    {remoteIpAddr}    {context.HttpContext.Request.Method}    {context.HttpContext.Request.Path}",
                        info,
                        ConsoleColor.Gray);
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// requesting
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            try
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
            catch
            {
            }
        }
    }
}