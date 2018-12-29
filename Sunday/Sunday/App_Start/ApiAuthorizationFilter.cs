using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Sunday.App_Start
{
    public class ApiAuthorizationFilter : AuthorizationFilterAttribute
    {
        /// <summary>
        /// 在處理序要求授權時呼叫。
        /// </summary>
        /// <param name="actionContext">動作內容，該內容封裝 <see cref="T:System.Web.Http.Filters.AuthorizationFilterAttribute" /> 的使用資訊。</param>
        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            if (actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Count > 0)
            {
                base.OnAuthorization(actionContext);
                return;
            }

            if (Thread.CurrentPrincipal != null && Thread.CurrentPrincipal.Identity.IsAuthenticated)
            {
                base.OnAuthorization(actionContext);
                return;
            }

            //actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            ApiResultModel result = new ApiResultModel();
            result.code = HttpStatusCode.Unauthorized;
            result.message = "已拒绝为此请求授权。";
            actionContext.Response = actionContext.Request.CreateResponse(result.code, result);
        }
    }
}