using System.Collections.Generic;

namespace DSS_Platform_DotNetSDK.Lib.Models.Resp
{
    public class PersonQueryResp
    {

        public string errMsg { get; set; }
        public bool? success { get; set; }
        public PersonQueryPaperResp data { get; set; }
    }

    public class PersonQueryPaperResp
    {
        public int pageSize { get; set; }
        public int totalPage { get; set; }
        public int totalRows { get; set; }
        public List<PersonModel> pageData { get; set; }
    }

}