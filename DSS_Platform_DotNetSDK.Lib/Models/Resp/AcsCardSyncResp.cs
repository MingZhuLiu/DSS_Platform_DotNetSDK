namespace DSS_Platform_DotNetSDK.Lib.Models.Resp {
    public class AcsCardSyncResp {
        public string errMsg { get; set; }
        public bool? success { get; set; }
        public AcsCardSyncPageModel data { get; set; }
    }
}