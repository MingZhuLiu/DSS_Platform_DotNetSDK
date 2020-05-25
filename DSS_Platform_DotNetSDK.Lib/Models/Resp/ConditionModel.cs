namespace DSS_Platform_DotNetSDK.Lib.Models.Resp {
    public class ConditionModel {

        public ConditionModel () {
            this.type = 1;
        }
        public string id { get; set; }
        public long? index { get; set; }
        public string detail { get; set; }
        public string name { get; set; }
        public long? syncStatus { get; set; }
        public long? type { get; set; }
        public string memo { get; set; }

        public bool? hasChildChannel { get; set; }
    }
}