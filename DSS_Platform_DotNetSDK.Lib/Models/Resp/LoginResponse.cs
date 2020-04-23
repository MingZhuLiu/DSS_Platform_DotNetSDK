namespace DSS_Platform_DotNetSDK.Lib.Models.Resp
{
    public class LoginResponse
    {
        /// <summary>
        /// 
        /// </summary>
        public bool success { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string loginName { get; set; }
        /// <summary>
        /// 登陆失败:[unknown block type]
        /// </summary>
        public string errMsg { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string token { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string cmsIp { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string cmsPort { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string orgCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string publicKey { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string nonce { get; set; }

    }
}