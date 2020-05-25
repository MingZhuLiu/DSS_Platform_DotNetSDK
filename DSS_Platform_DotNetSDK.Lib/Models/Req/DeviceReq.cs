namespace DSS_Platform_DotNetSDK.Lib.Models.Req {
    public class DeviceReq {
        public long? pageNum { get; set; }
        public long? pageSize { get; set; }

        public string deviceName { get; set; }
        public string deviceIp { get; set; }
        public string deviceStatus { get; set; }

    }
}