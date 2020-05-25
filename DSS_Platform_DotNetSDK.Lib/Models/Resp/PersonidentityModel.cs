using System;

namespace DSS_Platform_DotNetSDK.Lib.Models.Resp {
        public class PersonidentityModel {

            public String id { get; set; }
            public string name { get; set; }
            public long? isCashRecharge { get; set; }

            public long? isMachineRecharge { get; set; }

            public double? subsidyAmount { get; set; }

        }
}