namespace DSS_Platform_DotNetSDK.Lib.Models.Req
{
    public class PersonDtoReq
    {
        public string birthday { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public string cardNumber { get; set; }
        public string phone { get; set; }
        public string personIdentityId { get; set; }
        public string status { get; set; }

        public string fingerCode{get;set;}
        public string loginPassword{get;set;}
        public string paperNumber{get;set;}

        public string paperType{get;set;}

        public string secfingerCode{get;set;}
        public string sex{get;set;}
        public string deptId{get;set;}

    }
}