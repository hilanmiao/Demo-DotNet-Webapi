using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;

namespace Sunday.App_Start
{
    public class ApiErrorHandleAttribute: ExceptionFilterAttribute
    {
        public override void OnException(System.Web.Http.Filters.HttpActionExecutedContext actionExecutedContext)
        {
            base.OnException(actionExecutedContext);

            // 取得发生例外时的错误讯息
            var errorMessage = actionExecutedContext.Exception.Message;

            var result = new ApiResultModel()
            {
                code = HttpStatusCode.BadRequest,
                message = errorMessage
            };

            // 重新打包回传的讯息
            actionExecutedContext.Response = actionExecutedContext.Request
                .CreateResponse(result.code, result);
        }
    }
}