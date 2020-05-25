namespace DSS_Platform_DotNetSDK.Lib.Models.Req {
    public class AcsCardSyncReq {
        public long? pageNum { get; set; }
        public long? pageSize { get; set; }

        public string cardNumber { get; set; }
        public string channelName { get; set; }
        public string creatStartDate { get; set; }
        public string creatEndDate { get; set; }
        public string personCode { get; set; }
        public string personName { get; set; }
        public string operateType { get; set; }
        public string result { get; set; }
        public string module { get; set; }

    }
}