namespace DSS_Platform_DotNetSDK.Lib.Models.Req
{
    public class ConditionReq
    {
        public long? pageNum { get; set; }
        public long? pageSize { get; set; }

        public string singleCondition { get; set; }
    }
}