using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sunday.App_Start
{
    public class CommonClass
    {
    }

    /// <summary>
    /// 公共分页数据返回对象
    /// </summary>
    public class CommonClassPaged
    {
        /// <summary>
        /// 总数
        /// </summary>
        public int totalCount { get; set; }

        /// <summary>
        /// 数据对象
        /// </summary>
        public object data { get; set; }
    }
}