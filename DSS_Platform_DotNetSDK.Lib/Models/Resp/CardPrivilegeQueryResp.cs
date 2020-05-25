namespace DSS_Platform_DotNetSDK.Lib.Models.Resp
{
    public class CardPrivilegeQueryResp
    {
        public string errMsg{get;set;}
        public bool? success{get;set;}
        public CardPrivilegeQueryPageResp data{get;set;}
    }
}