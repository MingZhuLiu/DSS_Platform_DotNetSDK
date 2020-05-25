namespace DSS_Platform_DotNetSDK.Lib.Models.Req {
    public class AcsFaceSyncReq {
        public string[] resouceCodes { get; set; }
        public string[] doorGroupIds { get; set; }
        public string personCode { get; set; }
        public long? timeQuantumId { get; set; }
    }
}