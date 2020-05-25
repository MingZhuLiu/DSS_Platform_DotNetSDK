using System.Collections.Generic;
namespace DSS_Platform_DotNetSDK.Lib.Models.Resp {
    public class DoorGroupModel {
        public long? id { get; set; }
        public string name { get; set; }
        public string memo { get; set; }
        public bool? hasChildChannel { get; set; }
        public List<object> doorGroupDetail { get; set; }
        public List<object> addDoorGroupRelDoorList { get; set; }
        public List<object> deleteDoorGroupRelDoorList { get; set; }

    }
}