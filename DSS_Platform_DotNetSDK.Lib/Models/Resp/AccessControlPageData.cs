using System.Collections.Generic;
namespace DSS_Platform_DotNetSDK.Lib.Models.Resp {
    public class AccessControlPageData {
        public long? accessChnNum { get; set; }

        public List<AccessControlChannelModel> accessControlChannelList { get; set; }

        public string accessThirdPartyOpenDoor { get; set; }
        public List<object> alarmInputChannelList { get; set; }
        public long? alarmInputChnNum { get; set; }
        public List<object> alarmOutputChannelList { get; set; }
        public long? alarmOutputChnNum { get; set; }
        public long? category { get; set; }
        public long? channelAmount { get; set; }
        public string deviceCode { get; set; }
        public string deviceIp { get; set; }
        public string deviceModel { get; set; }
        public string deviceModelName { get; set; }
        public string deviceName { get; set; }
        public string devicePassword { get; set; }
        public string devicePort { get; set; }
        public string deviceStatus { get; set; }
        public string deviceType { get; set; }
        public string deviceUser { get; set; }
        public long? id { get; set; }
        public string loginType { get; set; }
        public string manufacturer { get; set; }
        public string orgCode { get; set; }
        public string orgName { get; set; }
        public long? proxyPort { get; set; }
        public string serviceId { get; set; }
        public long? sort { get; set; }

    }
}