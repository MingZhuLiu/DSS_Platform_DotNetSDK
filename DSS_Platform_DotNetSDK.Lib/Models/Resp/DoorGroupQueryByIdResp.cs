using System.Collections.Generic;
namespace DSS_Platform_DotNetSDK.Lib.Models.Resp {
    public class DoorGroupQueryByIdResp {
        public string success { get; set; }
        public string errMsg { get; set; }

        public List<DoorGroupModel> data { get; set; }

    }
}