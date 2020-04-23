using System;
using DSS_Platform_DotNetSDK.Lib;

namespace DSS_Platform_DotNetSDK.Control
{
    class Program
    {
        static void Main(string[] args)
        {
            DSSClient dSSClient=new DSSClient("192.168.1.110");
            // var publicKeyResp=dSSClient.getPublicKey("system");//1.1.1 获取公钥
            var loginResp=dSSClient.login("system","mote12345");//1.1.2 获取 token 的接口
            var organizeList=dSSClient.QueryOrganizeList(new Lib.Models.Req.ApiQuery(){Param=new Lib.Models.Req.BaseParam(){ pagination=new Lib.Models.Req.Base.Pagination(){ currentPage=1,pageSize=2}}});//1.2.1.1 查询组织
            var organize=dSSClient.QueryOrganize(1);//1.2.1.2 查询单个组织详情;
            Console.WriteLine(organizeList);
        }
    }
}
