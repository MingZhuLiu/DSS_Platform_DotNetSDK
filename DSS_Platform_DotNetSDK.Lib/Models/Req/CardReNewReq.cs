namespace DSS_Platform_DotNetSDK.Lib.Models.Req
{
    public class CardReNewReq
    {
        public string newCardNumber{get;set;}
         public string oldCardNumber{get;set;}
        public  long? eventCode{get;set;}
        public  long? personId{get;set;}
    }
}