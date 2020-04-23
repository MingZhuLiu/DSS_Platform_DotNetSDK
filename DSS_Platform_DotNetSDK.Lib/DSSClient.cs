using System;
using System.Collections.Generic;
using System.Text;
using DSS_Platform_DotNetSDK.Lib.Commons;
using DSS_Platform_DotNetSDK.Lib.Models;
using DSS_Platform_DotNetSDK.Lib.Models.Req;
using DSS_Platform_DotNetSDK.Lib.Models.Resp;

namespace DSS_Platform_DotNetSDK.Lib
{
    public class DSSClient
    {
        private QxHttpClient httpClient = null;

        private string baseUrl = null;
        private string apiUrl = null;

        private LoginResponse loginData = null;

        public DSSClient(String ip, int port = 8314)
        {
            baseUrl = "http://" + ip + ":" + port;
            // apiUrl = baseUrl + "/admin/rest/api";
            httpClient = new QxHttpClient();
        }

        #region  登录相关

        /// <summary>
        /// 1.1.1 获取公钥
        /// 用于获取令牌 token 步骤中密码的加密,加密方式为 RSA 加密, 该接口对外开放,不需要认证
        /// </summary>
        /// <param name="loginName">用户名</param>
        /// <returns></returns>
        public BaseModel<PublicKeyResponse> getPublicKey(String loginName)
        {
            BaseModel<PublicKeyResponse> result = new BaseModel<PublicKeyResponse>();
            try
            {
                if (String.IsNullOrWhiteSpace(loginName))
                {
                    result.Failed("The parameter userName is null!");
                }
                else
                {
                    var data = httpClient.PostJson<PublicKeyResponse>(baseUrl + "/WPMS/getPublicKey", new QxHttpPara("loginName", loginName), false);
                    if (data == null)
                    {
                        result.Failed("Server response null!");
                    }
                    else
                    {
                        result.Success(data);
                    }
                }
            }
            catch (Exception ex)
            {
                result.Failed(ex);
            }
            return result;
        }

        /// <summary>
        /// 1.1.2 token 获取
        /// 获取 token 的接口
        /// 每次获取 token 时,publicKey 需要实时获取
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="loginPass"></param>
        /// <returns></returns>
        public BaseModel<LoginResponse> login(string loginName, string loginPass)
        {
            BaseModel<LoginResponse> result = new BaseModel<LoginResponse>();
            try
            {
                if (String.IsNullOrWhiteSpace(loginName))
                {
                    result.Failed("The parameter userName is null!");
                }
                else if (String.IsNullOrWhiteSpace(loginPass))
                {
                    result.Failed("The parameter loginPass is null!");
                }
                else
                {
                    var publicKey = getPublicKey(loginName);
                    if (publicKey.Flag && publicKey.Data.success)
                    {
                        var rsa = new RSAHelper(RSAType.RSA, Encoding.UTF8, null, publicKey.Data.publicKey);
                        var encryptPass = rsa.Encrypt(loginPass);
                        var data = httpClient.PostJson<LoginResponse>(baseUrl + "/WPMS/login",
                            new QxHttpPara("loginName", loginName)
                            .AddPara("loginPass", encryptPass)
                            , false);
                        if (data.success)
                        {
                            loginData = data;
                            apiUrl = baseUrl + "/admin/rest/api?userId=" + data.id + "&username=" + data.loginName + "&token=" + data.token;
                        }
                        result.Success(data);
                    }
                    else
                    {
                        result.Failed(publicKey.Msg);
                    }

                }
            }
            catch (Exception ex)
            {
                result.Failed(ex);
            }
            return result;
        }

        #endregion



        #region  组织相关
        /// <summary>
        /// 1.2.1.1 查询组织
        /// 获取所有组织, pagination 条件不为空为分页查询
        /// </summary>
        /// <param name="apiQuery"></param>
        /// <returns></returns>
        public BaseModel<BaseData<List<OrganizeModel>>> QueryOrganizeList(ApiQuery apiQuery = null)
        {
            if (String.IsNullOrWhiteSpace(apiUrl))
                return new BaseModel<BaseData<List<OrganizeModel>>>().Failed("Please invoke login !");
            if (apiQuery == null)
                apiQuery = new ApiQuery("admin_002_001");
            else
                apiQuery.interfaceId = "admin_002_001";
            return httpClient.Post<BaseData<List<OrganizeModel>>>(apiUrl, apiQuery);
        }

        /// <summary>
        /// 1.2.1.2 查询单个组织详情
        /// 根据组织 id 获取组织的详细信息
        /// </summary>
        /// <param name="id">组织 id </param>
        /// <returns></returns>
        public BaseModel<BaseData<OrganizeModel>> QueryOrganize(int id)
        {
            if (String.IsNullOrWhiteSpace(apiUrl))
                return new BaseModel<BaseData<OrganizeModel>>().Failed("Please invoke login !");
            ApiQuery apiQuery = new ApiQuery();
            apiQuery.AddPara("id", 1);
            apiQuery.interfaceId = "admin_002_003";
            return httpClient.Post<BaseData<OrganizeModel>>(apiUrl, apiQuery);
        }

        
        #endregion

    }
}
