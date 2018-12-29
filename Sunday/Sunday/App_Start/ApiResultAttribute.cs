using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;

namespace Sunday.App_Start
{
    public class ApiResultAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            // 若发生例外则不在这边处理
            if (actionExecutedContext.Exception != null)
                return;

            base.OnActionExecuted(actionExecutedContext);

            ApiResultModel result = new ApiResultModel();

            // 取得由 API 返回的状态代码
            result.code = actionExecutedContext.ActionContext.Response.StatusCode;
            // 取得由 API 返回的资料
            result.data = actionExecutedContext.ActionContext.Response.Content.ReadAsAsync<object>().Result;
            // TODO: 测试，如果是字符串或字符直接返回
            //if (result.data is String || result.data is Char) {
            //    result.data = result.data.ToString();
            //    result.message = result.data.ToString();
            //}
            // 重新封装回传格式
            actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(result.code, result);
        }
    }
}