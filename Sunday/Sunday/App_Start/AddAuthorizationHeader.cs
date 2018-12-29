using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace Sunday.App_Start
{
    public class AddAuthorizationHeader : IOperationFilter
    {
        /// <summary>
        /// Adds an authorization header to the given operation in Swagger.
        /// </summary>
        /// <param name="operation">The Swashbuckle operation.</param>
        /// <param name="schemaRegistry">The Swashbuckle schema registry.</param>
        /// <param name="apiDescription">The Swashbuckle api description.</param>
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            if (operation == null) return;

            if (operation.parameters == null)
            {
                operation.parameters = new List<Parameter>();

            }


            var parameter = new Parameter
            {
                //description = "Token",
                description = "jwt认证（先请求接口/api/token），需要在请求headers中加入Authorization:Bearer+空格+获取到的access_token",
                @in = "header",
                name = "Authorization",
                required = true,
                type = "string"
            };


            if (apiDescription.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any())
            {
                //如果Api方法是允许匿名方法，Token不是必填的

                parameter.required = false;
            }

            operation.parameters.Add(parameter);
        }
    }
}