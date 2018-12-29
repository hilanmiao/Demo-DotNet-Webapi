using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Sunday.Models;
using Sunday.App_Start;
using System.Web;
using System.IO;

namespace Sunday.Controllers
{
    /// <summary>
    /// 测试用接口
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Test")]
    public class TestController : ApiController
    {
        // NLog
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        //private ApplicationDbContext context = new ApplicationDbContext();

        /// <summary>
        /// 项目开始前先初始化数据
        /// </summary>
        /// <returns></returns>
        //[AllowAnonymous]
        //[Route("InitData")]
        //public IQueryable<SysRole> GetInitData()
        //{
        //    return context.SysRoles;
        //}

        /// <summary>
        /// 这个接口仅仅用来做测试用，请参照webapi使用说明文档自行实现access_token的获取！！！
        /// 测试账号：admin 密码：123456
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("Login")]
        public async Task<IHttpActionResult> GetLogin(string username, string password)
        {
            var clientId = "1234";
            var clientSecret = "5678";

            var parameters = new Dictionary<string, string>();
            parameters.Add("grant_type", "password");
            parameters.Add("username", username);
            parameters.Add("password", password);

            HttpClient _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://localhost:55930");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(clientId + ":" + clientSecret)));
            var response = await _httpClient.PostAsync("/api/token", new FormUrlEncodedContent(parameters));
            var responseValue = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                //return "Bearer " + JObject.Parse(responseValue)["access_token"].Value<string>();
                return Content<string>(HttpStatusCode.OK, "Bearer " + JObject.Parse(responseValue)["access_token"].Value<string>());
            }
            else
            {
                //Console.WriteLine(responseValue);
                return Content<string>(HttpStatusCode.BadRequest, "用户名或密码不正确");
            }
        }

        // GET api/<controller>
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        /// <summary>
        /// 测试json键值对转为小写，null转换为‘’，时间格式化，
        /// 参考旧值{"timeNow": "2018-07-04T16:31:38.5375755+08:00",  "userName": "admin@123.com",  "role": null}
        /// </summary>
        /// <returns></returns>
        [Route("Test")]
        public TempObject GetTest()
        {
            return new TempObject
            {
                userName = User.Identity.Name,
                timeNow = DateTime.Now,
                role = null
            };
        }

        /// <summary>
        /// 测试例外情况返回值
        /// </summary>
        /// <returns></returns>
        [Route("TestError")]
        public TempObject GetTestError()
        {
            return new TempObject
            {
                userName = Convert.ToInt32(User.Identity.Name).ToString(),
                timeNow = DateTime.Now,
                role = null
            };
        }

        /// <summary>
        /// 测试日志
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("TestLog")]
        public string GetTestLog()
        {
            try
            {
                //var logger = NLog.LogManager.GetCurrentClassLogger();
                //logger.Trace("Sample trace message");
                //logger.Debug("Sample debug message");
                //logger.Info("Sample informational message");
                //logger.Warn("Sample warning message");
                //logger.Error("Sample error message");
                //logger.Fatal("Sample fatal error message");

                // alternatively you can call the Log() method
                // and pass log level as the parameter.
                //logger.Log(LogLevel.Info, "Sample informational message");

                var temp = Convert.ToInt32("string").ToString();
                return "log";
            }
            catch (Exception ex)
            {
                logger.Error(ex, "测试exception!"); // render the exception with ${exception}
                throw;
            }

        }

        /// <summary>
        /// 测试自定义message
        /// </summary>
        /// <returns></returns>
        [Route("TestMessage")]
        public IHttpActionResult GetTestMessage()
        {
            var tempObject = new TempObject
            {
                timeNow = DateTime.Now,
            };
            return Content<string>(HttpStatusCode.OK, "OK");
            //return Content<TempObject>(HttpStatusCode.OK, tempObject);
        }

        /// <summary>
        /// [upload] 这个接口仅仅用来做测试用
        /// </summary>
        /// <returns></returns>
        //[Route("TestUpload")]
        //public HttpResponseMessage PostTestUpload([SwaggerFileUpload] string file1=null, [SwaggerFileUpload] string file =null)
        //{
        //    return Request.CreateResponse(HttpStatusCode.OK, 1);
        //}
        [Route("TestUpload")]
        public object PostTestUpload()
        {
            var _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://localhost:55930");
            var content = new MultipartFormDataContent();

            var file1 = HttpContext.Current.Request.Files[0];

            byte[] bytes = null;
            using (var binaryReader = new BinaryReader(file1.InputStream))
            {
                bytes = binaryReader.ReadBytes(file1.ContentLength);
            }

            var fileContent1 = new ByteArrayContent(bytes);
            fileContent1.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = "Sample.png"
            };

            content.Add(fileContent1);

            var response = _httpClient.PostAsync("/api/upload/UploadFile",content).Result;
            return response;

            //if (response.StatusCode == System.Net.HttpStatusCode.OK)
            //{
            //    return Request.CreateResponse(HttpStatusCode.OK, response.Content);
            //}
            //else
            //{
            //    return Request.CreateResponse(HttpStatusCode.InternalServerError, "上传出错");
            //}
        }


        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }

        public class TempObject
        {
            public DateTime timeNow { get; set; }
            public string userName { get; set; }
            public string role { get; set; }
        }
    }
}