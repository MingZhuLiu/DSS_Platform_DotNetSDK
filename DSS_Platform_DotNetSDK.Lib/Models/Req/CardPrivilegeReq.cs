namespace DSS_Platform_DotNetSDK.Lib.Models.Req {
    public class CardPrivilegeReq {
        public string personName { get; set; }
        public string cardNumber { get; set; }
        public string personCode { get; set; }
        public string authorizeStatus { get; set; }
        public string taskStatus { get; set; }
        public long? pageSize { get; set; }
        public long? pageNum { get; set; }
    }
}