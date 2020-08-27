using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;

using api;

/// <summary>
/// 错误统一处理
/// </summary>
public class ApiErrorAttribute: ExceptionFilterAttribute
{
    public override void OnException(System.Web.Http.Filters.HttpActionExecutedContext actionExecutedContext)
    {
        Exception exp = actionExecutedContext.Exception;

        // 取得由 API 返回的状态代码
        //HttpStatusCode StatusCode = actionExecutedContext.ActionContext.Response.StatusCode; 
        HttpStatusCode StatusCode = HttpStatusCode.InternalServerError;

        if (exp is NotImplementedException)
        {
            StatusCode = HttpStatusCode.NotImplemented;
        }
        else if (exp is TimeoutException)
        {
            StatusCode = HttpStatusCode.RequestTimeout;
        }
        else if (exp is UnauthorizedAccessException) {
            StatusCode = HttpStatusCode.Unauthorized;
        }
        else if (exp is NotSupportedException)
        {
            StatusCode = HttpStatusCode.NotImplemented;
        }
        

        // 重新打包回传的讯息
        actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(StatusCode, new AjaxResult
        {
            statusCode = 500,
            message = exp.Message,
            data = exp
        });
        //Logger.Error(exp.Message);

        base.OnException(actionExecutedContext);

        
    }



}