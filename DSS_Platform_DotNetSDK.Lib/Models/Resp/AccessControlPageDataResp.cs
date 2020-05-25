using System.Collections.Generic;
namespace DSS_Platform_DotNetSDK.Lib.Models.Resp
{
    public class AccessControlPageDataResp
    {
        public long currentPage{get;set;}

        public List<AccessControlPageData> pageData { get; set; }

        public long pageSize{get;set;}
        public long totalPage{get;set;}
        public long totalRows{get;set;}
      



    }
}