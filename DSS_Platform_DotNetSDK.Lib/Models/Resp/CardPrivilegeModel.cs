using System.Collections.Generic;
namespace DSS_Platform_DotNetSDK.Lib.Models.Resp {
    public class CardPrivilegeModel {
        public string[] cardNumbers { get; set; }
        public string cardNumber { get; set; }
        public string authorizeStatus { get; set; }
        public string deptName { get; set; }
        public string personName { get; set; }
        public string timeQuantumName { get; set; }

        public long? id { get; set; }
        
        public string timeQuantumId { get; set; }
        public List<CardPrivilegeDetailsModel> cardPrivilegeDetails { get; set; }
    }
}