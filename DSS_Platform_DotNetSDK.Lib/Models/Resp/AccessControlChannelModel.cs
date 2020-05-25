using System.Collections.Generic;
namespace DSS_Platform_DotNetSDK.Lib.Models.Resp {
    public class AccessControlChannelModel {
        public List<CardReaderModel> cardReaderList { get; set; }

        public string channelCode { get; set; }
        public string channelName { get; set; }
        public long? channelSeq { get; set; }

        public long? delayTime { get; set; }
        public string deviceCode { get; set; }

        public string deviceModel { get; set; }
        public string deviceName { get; set; }
        public string deviceType { get; set; }
        public long? id { get; set; }
        public string onlineStatus { get; set; }

        public string orgName { get; set; }
        public long? status { get; set; }
        public long? validFlag { get; set; }

    }
}