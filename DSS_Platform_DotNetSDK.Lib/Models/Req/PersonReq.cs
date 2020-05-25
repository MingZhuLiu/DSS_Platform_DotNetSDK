namespace DSS_Platform_DotNetSDK.Lib.Models.Req
{
    public class PersonQueryReq
    {
        public long? pageNum{get;set;}
        public long? pageSize{get;set;}
        public string deptIdsString{get;set;}
        public string name{get;set;}
        public string code{get;set;}
        public string cardNumber{get;set;}
        public string phone{get;set;}
        public string personIdentityId{get;set;}
        public string status{get;set;}
    }
}