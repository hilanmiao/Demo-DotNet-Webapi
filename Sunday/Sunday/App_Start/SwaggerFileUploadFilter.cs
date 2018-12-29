using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Description;

namespace Sunday.App_Start
{
    public class SwaggerFileUploadFilter : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            // 通过备注中是否显示upload字眼来显示上传按钮
            if (!string.IsNullOrWhiteSpace(operation.summary) && operation.summary.Contains("upload")) {
                operation.consumes.Add("application/form-data");
                operation.parameters.Add(new Parameter {
                    name = "file",
                    @in = "formData",
                    required = true,
                    type = "file"
                });
            }
        }
    }

    //public class SwaggerFileUploadFilter : IOperationFilter
    //{
    //    public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
    //    {
    //        var parameters = apiDescription.ActionDescriptor.GetParameters();
    //        foreach (HttpParameterDescriptor parameterDesc in parameters)
    //        {
    //            var fileUploadAttr = parameterDesc.GetCustomAttributes<SwaggerFileUploadAttribute>().FirstOrDefault();
    //            if (fileUploadAttr != null)
    //            {
    //                operation.consumes.Add("multipart/form-data");

    //                operation.parameters.Add(new Parameter
    //                {
    //                    name = parameterDesc.ParameterName + "_file",
    //                    @in = "formData",
    //                    description = "file to upload",
    //                    required = fileUploadAttr.Required,
    //                    type = "file"
    //                });
    //            }
    //        }
    //    }
    //}

    //[AttributeUsage(AttributeTargets.Parameter)]
    //public class SwaggerFileUploadAttribute : Attribute
    //{
    //    public bool Required { get; private set; }

    //    public SwaggerFileUploadAttribute(bool Required = true)
    //    {
    //        this.Required = Required;
    //    }
    //}
}