using System;

namespace DSS_Platform_DotNetSDK.Lib.Models.Resp {
    public class DeptModel {
        public string name { get; set; }
        public long? id { get; set; }
        public long? parentId { get; set; }

        public string description { get; set; }

        public long? createBy { get; set; }
        public long? updateBy { get; set; }
        public int? authorizeFlag { get; set; }

        public string groupid { get; set; }

        public object ids { get; set; }

        public bool? enable { get; set; }

        public bool? hasChildren { get; set; }

        public bool? cascade { get; set; }

        public long? deptId { get; set; }

        public long? fromOther { get; set; }

        public string argId { get; set; }

    }
}