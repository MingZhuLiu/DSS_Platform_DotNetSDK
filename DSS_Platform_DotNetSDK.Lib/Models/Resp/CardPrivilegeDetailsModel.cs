namespace DSS_Platform_DotNetSDK.Lib.Models.Resp {
    public class CardPrivilegeDetailsModel {
        public string privilegeType { get; set; }
        public string resouceCode { get; set; }

        public string authorizeSource { get; set; }
        public string authorizeStatus { get; set; }
        public string cardNumber { get; set; }
        public bool? compareFlag { get; set; }
        public string deviceStatus { get; set; }
        public long? id { get; set; }
        public string resourceName { get; set; }

    }
}