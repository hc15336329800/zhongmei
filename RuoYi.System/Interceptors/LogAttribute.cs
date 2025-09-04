using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using RuoYi.Common.Enums;
using RuoYi.Common.Utils;
using RuoYi.Data.Models;
using RuoYi.Framework.JsonSerialization;
using RuoYi.Framework.UnifyResult;
using RuoYi.System.Services;
using System.Diagnostics;
using System.Reflection;

namespace RuoYi.System
{
    public class LogAttribute : Attribute, IAsyncActionFilter
    {
#region 参数
        public string Title { get; set; } = "";
        public BusinessType BusinessType { get; set; } = BusinessType.OTHER;
        public OperatorType OperatorType { get; set; } = OperatorType.MANAGE;
        public bool IsSaveRequestData { get; set; } = true;
        public bool IsSaveResponseData { get; set; } = true;
        public string[] ExcludeParamNames { get; set; } = new string[0];

#endregion
        private static string[] EXCLUDE_PROPERTIES =
        {
            "password",
            "oldPassword",
            "newPassword",
            "confirmPassword"
        };
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var actionMethod = (context.ActionDescriptor as ControllerActionDescriptor)?.MethodInfo;
            if (actionMethod == null)
            {
                _ = await next.Invoke();
                return;
            }

            await HandleLogAsync(actionMethod, context.ActionArguments, context, next);
        }

        private async Task HandleLogAsync(MethodInfo actionMethod, IDictionary<string, object> parameterValues, FilterContext context, dynamic next)
        {
            if (context.HttpContext.IsWebSocketRequest())
            {
                _ = await next();
                return;
            }

            LoginUser loginUser = SecurityUtils.GetLoginUser();
            var methodFullName = actionMethod.DeclaringType!.FullName + "." + actionMethod.Name;
            var httpContext = context.HttpContext;
            var httpRequest = httpContext.Request;
            var remoteIPv4 = httpContext.GetRemoteIpAddressToIPv4();
            var operLocation = await AddressUtils.GetRealAddressByIPAsync(remoteIPv4);
            var httpMethod = httpContext.Request.Method;
            var requestUrl = httpRequest.GetRequestUrlAddress();
            var timeOperation = Stopwatch.StartNew();
            var resultContext = await next();
            timeOperation.Stop();
            Exception e = resultContext.Exception;
            var operLog = new SysOperLog
            {
                Status = e != null ? BusinessStatus.FAIL.ToInt() : BusinessStatus.SUCCESS.ToInt(),
                ErrorMsg = e != null ? StringUtils.Substring(e.Message, 0, 2000) : null,
                OperIp = remoteIPv4,
                OperUrl = requestUrl,
                OperName = loginUser != null ? loginUser.UserName : null,
                OperTime = DateTime.Now,
                OperLocation = operLocation,
                Method = methodFullName,
                RequestMethod = httpMethod,
                BusinessType = BusinessType.ToInt(),
                Title = Title,
                OperatorType = OperatorType.ToInt(),
                OperParam = IsSaveRequestData ? GetOperParam(parameterValues, ExcludeParamNames) : null,
                JsonResult = IsSaveRequestData ? GetResponseData(resultContext) : null,
                CostTime = timeOperation.ElapsedMilliseconds
            };
            _ = Task.Factory.StartNew(async () =>
            {
                var sysOperLogService = App.GetService<SysOperLogService>();
                await sysOperLogService.InsertAsync(operLog);
            });
        }

        private string? GetOperParam(IDictionary<string, object> parameterValues, string[] excludeParamNames)
        {
            var values = new Dictionary<string, object>();
            foreach (var parameterValue in parameterValues)
            {
                if (excludeParamNames.Contains(parameterValue.Key))
                {
                    continue;
                }

                var value = parameterValue.Value;
                object rawValue = default;
                if (value is IFormFile || value is List<IFormFile>)
                {
                    if (value is IFormFile formFile)
                    {
                        values.Add(parameterValue.Key, formFile.FileName);
                    }
                    else if (value is List<IFormFile> formFiles)
                    {
                        var fileNames = formFiles.Select(f => f.FileName).ToArray();
                        values.Add(parameterValue.Key, fileNames);
                    }
                }
                else if (value is byte[] byteArray)
                {
                    values.Add(parameterValue.Key, "byte[]");
                }
                else if (value is string || value == null)
                {
                    values.Add(parameterValue.Key, parameterValue.Value);
                }
                else
                {
                    values.Add(parameterValue.Key, parameterValue.Value);
                }
            }

            return JSON.Serialize(values);
        }

        private string? GetResponseData(dynamic resultContext)
        {
            object returnValue = null;
            var result = resultContext.Result as IActionResult;
            if (UnifyContext.CheckVaildResult(result, out var data))
            {
                returnValue = data;
            }
            else if (result is FileResult fresult)
            {
                returnValue = new
                {
                    FileName = fresult.FileDownloadName,
                    fresult.ContentType,
                    Length = fresult is FileContentResult cresult ? (object)cresult.FileContents.Length : null
                };
            }

            return returnValue != null ? JSON.Serialize(returnValue) : null;
        }
    }
}