using System;

namespace DSS_Platform_DotNetSDK.Lib.Models.Resp
{
    public class OrganizeModel
    {
        public OrganizeModel()
        {
            orgType="1";
        }
        public String id{get;set;}
        public String orgCode{get;set;}
        public String orgName{get;set;}
        public String orgSn{get;set;}

        public String orgPreCode{get;set;}
        public String orgType{get;set;}
    }
}