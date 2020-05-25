using System.Collections.Generic;

namespace DSS_Platform_DotNetSDK.Lib.Models.Resp {
    public class CardQueryResp {
         public string errMsg { get; set; }
        public bool? success { get; set; }
        public CardQueryPaperResp data { get; set; }
    }

        public class CardQueryPaperResp
    {
        public int pageSize { get; set; }
        public int totalPage { get; set; }
        public int totalRows { get; set; }
        public List<CardModel> pageData { get; set; }
    }
}