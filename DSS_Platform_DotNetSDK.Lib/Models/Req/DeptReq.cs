
namespace DSS_Platform_DotNetSDK.Lib.Models.Req {
    public class DeptReq {

        public DeptReq () {
            this.parentId = 1L;

        }
        public long? parentId { get; set; }
        public string deptName { get; set; }
        public string description { get; set; }

    }
}