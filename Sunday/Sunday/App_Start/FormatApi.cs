using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;

namespace Sunday.App_Start
{
    public class FormatApi
    {
        public static void Init(HttpConfiguration config)
        {
            //HttpConfiguration config = GlobalConfiguration.Configuration;
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            
            // 解决json序列化时的循环引用问题
            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            // 对 JSON 数据使用混合大小写。驼峰式,但是是javascript 首字母小写形式.
            //config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new  CamelCasePropertyNamesContractResolver();
            // 对 JSON 数据使用混合大小写。跟属性名同样的大小.输出
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new UnderlineSplitContractResolver();
        }
    }

    public class UnderlineSplitContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            //return CamelCaseToUnderlineSplit(propertyName);
            return propertyName.ToLower();
        }
        private string CamelCaseToUnderlineSplit(string name)
        {
            return name.ToLower();
        }

        //解决API NULL 和时间格式问题
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            return type.GetProperties()
                   .Select(p =>
                   {
                       var jp = base.CreateProperty(p, memberSerialization);
                       if (jp.PropertyType == typeof(System.String))
                           jp.ValueProvider = new NullToEmptyStringValueProvider(p);
                       if (jp.PropertyType.ToString().Contains("System.DateTime"))
                           jp.Converter = new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
                       return jp;
                   }).ToList();
        }
    }

    /// <summary>
    /// 解决返回json数据null问题
    /// </summary>
    public class NullToEmptyStringValueProvider : IValueProvider
    {
        PropertyInfo _MemberInfo;
        public NullToEmptyStringValueProvider(PropertyInfo memberInfo)
        {
            _MemberInfo = memberInfo;
        }
        public object GetValue(object target)
        {
            object result = _MemberInfo.GetValue(target);
            //if (result == null) 
            //    result = "";
            if (_MemberInfo.PropertyType == typeof(string) && result == null)
                result = "";
            return result;
        }
        public void SetValue(object target, object value)
        {
            _MemberInfo.SetValue(target, value);
        }
    }
}