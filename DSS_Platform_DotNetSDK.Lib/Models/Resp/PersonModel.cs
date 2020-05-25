using System.Collections.Generic;
namespace DSS_Platform_DotNetSDK.Lib.Models.Resp {
    public class PersonModel {
        public long id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public string fingerCode { get; set; }
        public string loginPassword { get; set; }
        public string paperNumber { get; set; }
        public string paperType { get; set; }
        public string personIdentityId { get; set; }
        public string phone { get; set; }
        public string secfingerCode { get; set; }
        public string sex { get; set; }
        public string status { get; set; }
        public long? deptId { get; set; }
        public string deptName { get; set; }
        public string faceData { get; set; }
        public string hongWaiFaceData { get; set; }
        public string personIdentity { get; set; }
        public string syncToFace { get; set; }

        public string entranceTime { get; set; }
        public string from { get; set; }
        public bool? hasOpenDoorPassword { get; set; }
        public string[] personIds { get; set; }
         public string personType { get; set; }

        public List<CardModel> cards{get;set;}

    }
}