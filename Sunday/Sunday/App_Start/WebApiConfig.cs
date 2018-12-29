using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using Sunday.App_Start;

namespace Sunday
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 配置和服务
            // 将 Web API 配置为仅使用不记名令牌身份验证。
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // Web API 路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // api json返回键值统一为小写，null为‘’，时间为yyyy-MM-dd HH:mm:ss
            FormatApi.Init(config);

            config.Filters.Add(new ApiAuthorizationFilter());
            // api 统一返回值模板
            config.Filters.Add(new ApiResultAttribute());
            // api 发生例外返回值模板
            config.Filters.Add(new ApiErrorHandleAttribute());
            //config.Filters.Add(new LogFilterAttribute());
            //config.Filters.Add(new AbnormalFilterAttribute());
        }
    }
}
