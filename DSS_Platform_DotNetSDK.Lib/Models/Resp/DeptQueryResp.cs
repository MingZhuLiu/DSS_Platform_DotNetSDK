using System.Collections.Generic;

namespace DSS_Platform_DotNetSDK.Lib.Models.Resp
{
    public class DeptQueryResp
    {
        public string code;
        public string errMsg;
        public bool? success;
        public List<DeptModel> data;  
    }
}