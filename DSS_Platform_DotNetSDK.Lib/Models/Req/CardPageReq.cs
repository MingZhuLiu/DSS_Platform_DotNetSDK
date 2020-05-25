namespace DSS_Platform_DotNetSDK.Lib.Models.Req {
    public class CardPageReq {
        public string personCode { get; set; }
        public string cardNumber { get; set; }
        public string cardStatus { get; set; }
        public long? pageSize { get; set; }
        public long? pageNum { get; set; }
    }
}