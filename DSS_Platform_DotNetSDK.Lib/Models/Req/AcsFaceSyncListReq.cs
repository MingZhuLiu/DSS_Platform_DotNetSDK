namespace DSS_Platform_DotNetSDK.Lib.Models.Req
{
    public class AcsFaceSyncListReq
    {
        public long? pageNum { get; set; }
        public long? pageSize { get; set; }

        public string personCode { get; set; }
        public string personName { get; set; }
         public string channelName { get; set; }
          public string operateType { get; set; }
           public string syncFlags { get; set; }

    }
}