using System.Collections.Generic;
using DSS_Platform_DotNetSDK.Lib.Models.Req.Base;

namespace DSS_Platform_DotNetSDK.Lib.Models.Req
{
    public class BaseParam
    {
        public BaseParam()
        {
            param=new Dictionary<string, object>();
            // locale="zh-CN";
        }

        public Dictionary<string, object> param { get; set; }

        public Authorinize authorinize { get; set; }

        public Pagination pagination{get;set;}

        public List<Order> orders{get;set;}

        public string locale{get;set;}

    }

}