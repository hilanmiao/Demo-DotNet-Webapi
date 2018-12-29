using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sunday.Models
{
    /// <summary>
    /// 日志表
    /// </summary>
    public class SysLog
    {
        public int Id { get; set; }

        public string Application { get; set; }

        public DateTime? Logged { get; set; }

        public string Level { get; set; }

        public string Message { get; set; }

        public string UserName { get; set; }

        public string ServerName { get; set; }

        public string Port { get; set; }

        public string Url { get; set; }

        public string Https { get; set; }

        public string ServerAddress { get; set; }

        public string RemoteAddress { get; set; }

        public string Logger { get; set; }

        public string Callsite { get; set; }

        public string Exception { get; set; }
    }
}