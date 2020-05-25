using System;
using System.Collections.Generic;
using System.Text;
using DSS_Platform_DotNetSDK.Lib.Commons;
using DSS_Platform_DotNetSDK.Lib.Models;
using DSS_Platform_DotNetSDK.Lib.Models.Req;
using DSS_Platform_DotNetSDK.Lib.Models.Resp;
using Newtonsoft.Json;
namespace DSS_Platform_DotNetSDK.Lib {
    public class DSSClient {
        private QxHttpClient httpClient = null;

        private Newtonsoft.Json.JsonSerializerSettings settings = new Newtonsoft.Json.JsonSerializerSettings ();
        private string baseUrl = null;
        private string apiUrl = null;
        private string orgUrl = "/admin/rest/api";
        private string CardSolutionUrl = "/CardSolution/card/department";
        private string PersonidentityUrl = "/CardSolution/card/person/personidentity";

        private string PersonUrl = "/CardSolution/card/person";

        private LoginResponse loginData = null;

        public DSSClient (String ip, int port = 8314) {
            baseUrl = "http://" + ip + ":" + port;
            // apiUrl = baseUrl + "/admin/rest/api";
            httpClient = new QxHttpClient ();
        }

        #region  登录相关

        /// <summary>
        /// 1.1.1 获取公钥
        /// 用于获取令牌 token 步骤中密码的加密,加密方式为 RSA 加密, 该接口对外开放,不需要认证
        /// </summary>
        /// <param name="loginName">用户名</param>
        /// <returns></returns>
        public BaseModel<PublicKeyResponse> getPublicKey (String loginName) {
            BaseModel<PublicKeyResponse> result = new BaseModel<PublicKeyResponse> ();
            try {
                if (String.IsNullOrWhiteSpace (loginName)) {
                    result.Failed ("The parameter userName is null!");
                } else {
                    var data = httpClient.PostJson<PublicKeyResponse> (baseUrl + "/WPMS/getPublicKey", new QxHttpPara ("loginName", loginName), false);
                    if (data == null) {
                        result.Failed ("Server response null!");
                    } else {
                        result.Success (data);
                    }
                }
            } catch (Exception ex) {
                result.Failed (ex);
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
        public BaseModel<LoginResponse> login (string loginName, string loginPass) {
            BaseModel<LoginResponse> result = new BaseModel<LoginResponse> ();
            try {
                if (String.IsNullOrWhiteSpace (loginName)) {
                    result.Failed ("The parameter userName is null!");
                } else if (String.IsNullOrWhiteSpace (loginPass)) {
                    result.Failed ("The parameter loginPass is null!");
                } else {
                    var publicKey = getPublicKey (loginName);
                    if (publicKey.Flag && publicKey.Data.success) {
                        var rsa = new RSAHelper (RSAType.RSA, Encoding.UTF8, null, publicKey.Data.publicKey);
                        var encryptPass = rsa.Encrypt (loginPass);
                        var data = httpClient.PostJson<LoginResponse> (baseUrl + "/WPMS/login",
                            new QxHttpPara ("loginName", loginName)
                            .AddPara ("loginPass", encryptPass), false);
                        if (data.success) {
                            loginData = data;
                            apiUrl = "?userId=" + data.id + "&username=" + data.loginName + "&token=" + data.token;
                        }
                        result.Success (data);
                    } else {
                        result.Failed (publicKey.Msg);
                    }

                }
            } catch (Exception ex) {
                result.Failed (ex);
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
        public BaseModel<BaseData<List<OrganizeModel>>> QueryOrganizeList (ApiQuery apiQuery = null) {

            if (String.IsNullOrWhiteSpace (apiUrl))
                return new BaseModel<BaseData<List<OrganizeModel>>> ().Failed ("Please invoke login !");
            if (apiQuery == null)
                apiQuery = new ApiQuery ("admin_002_001");
            else
                apiQuery.interfaceId = "admin_002_001";

            if (apiQuery.Param.param == null) {
                apiQuery.Param.param = new Object ();
            }
            return httpClient.Post<BaseData<List<OrganizeModel>>> (baseUrl + orgUrl + apiUrl, apiQuery);
        }

        /// <summary>
        /// 1.2.1.2 查询单个组织详情
        /// 根据组织 id 获取组织的详细信息
        /// </summary>
        /// <param name="id">组织 id </param>
        /// <returns></returns>
        public BaseModel<BaseData<OrganizeModel>> QueryOrganize (int id) {
            if (String.IsNullOrWhiteSpace (apiUrl))
                return new BaseModel<BaseData<OrganizeModel>> ().Failed ("Please invoke login !");
            ApiQuery apiQuery = new ApiQuery ();
            var reqPara = new Dictionary<String, Object> ();
            reqPara.Add ("id", 1);
            apiQuery.Param.param = reqPara;
            apiQuery.interfaceId = "admin_002_003";
            return httpClient.Post<BaseData<OrganizeModel>> (baseUrl + orgUrl + apiUrl, apiQuery);
        }

        /// <summary>
        /// 1.2.1.3 组织新增
        /// 新增设备组织
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public BaseModel<OrganizeEditResp> AddOrganize (OrganizeModel model) {
            if (String.IsNullOrWhiteSpace (apiUrl))
                return new BaseModel<OrganizeEditResp> ().Failed ("Please invoke login !");
            BaseModel<OrganizeEditResp> result = new BaseModel<OrganizeEditResp> ();
            if (model == null) {
                result.Failed ("OrganizeModel is null!");
            } else if (String.IsNullOrWhiteSpace (model.orgName)) {
                result.Failed ("orgName is null or empty!");
            } else if (String.IsNullOrWhiteSpace (model.orgPreCode)) {
                result.Failed ("orgPreCode is null or empty!");
            } else {
                var reqPara = new ApiQuery ();
                reqPara.interfaceId = "admin_002_002";
                reqPara.Param.param = model;
                result = httpClient.Post<OrganizeEditResp> (baseUrl + orgUrl + apiUrl, reqPara);
            }
            return result;
        }

        /// <summary>
        /// 1.2.1.3 组织修改
        /// 修改设备组织信息,主要是组织名称
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public BaseModel<OrganizeEditResp> EditOrganize (OrganizeModel model) {
            if (String.IsNullOrWhiteSpace (apiUrl))
                return new BaseModel<OrganizeEditResp> ().Failed ("Please invoke login !");
            BaseModel<OrganizeEditResp> result = new BaseModel<OrganizeEditResp> ();
            if (model == null) {
                result.Failed ("OrganizeModel is null!");
            } else if (String.IsNullOrWhiteSpace (model.id)) {
                result.Failed ("id is null or empty!");
            } else {
                var reqPara = new ApiQuery ();
                reqPara.interfaceId = "admin_002_005";
                reqPara.Param.param = model;
                result = httpClient.Post<OrganizeEditResp> (baseUrl + orgUrl + apiUrl, reqPara);
            }
            return result;
        }
        /// <summary>
        /// 1.2.1.3组织删除
        /// 删除设备组织信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public BaseModel<OrganizeDeletedResp> DeleteOrganize (String[] ids) {
            if (String.IsNullOrWhiteSpace (apiUrl))
                return new BaseModel<OrganizeDeletedResp> ().Failed ("Please invoke login !");
            BaseModel<OrganizeDeletedResp> result = new BaseModel<OrganizeDeletedResp> ();
            if (ids == null || ids.Length == 0) {
                result.Failed ("ids is null!");
            } else {
                ApiQuery apiQuery = new ApiQuery ();
                var reqPara = new Dictionary<String, Object> ();
                reqPara.Add ("ids", String.Join (",", ids));
                apiQuery.Param.param = reqPara;
                apiQuery.interfaceId = "admin_002_004";
                result = httpClient.Post<OrganizeDeletedResp> (baseUrl + orgUrl + apiUrl, apiQuery);
            }
            return result;
        }

        #endregion

        #region  一卡通服务

        #region 1.3.1 一卡通基础

        #region 1.3.1.1 部门管理
        /**
         * @description: 1.3.1.1.1添加部门信息
         * @param {type} 
         * @return: 
         */

        public BaseModel<BaseReturnData> AddDept (DeptModel model) {
            if (String.IsNullOrWhiteSpace (apiUrl))
                return new BaseModel<BaseReturnData> ().Failed ("Please invoke login !");
            BaseModel<BaseReturnData> result = new BaseModel<BaseReturnData> ();
            if (model == null) {
                result.Failed ("DeptModel is null!");
            } else if (String.IsNullOrWhiteSpace (model.name)) {
                result.Failed ("deptName is null or empty!");
            } else {

                settings.NullValueHandling = NullValueHandling.Ignore;
                var jsonParam = Newtonsoft.Json.JsonConvert.SerializeObject (model, settings);
                result = httpClient.Post<BaseReturnData> (baseUrl + CardSolutionUrl + apiUrl, jsonParam);
            }
            return result;

        }
        /**
         * @description: 1.3.1.1.2 修改部门信息
         * @param {type} 
         * @return: 
         */
        public BaseModel<DeptUpdateResp> UpdateDept (DeptModel model) {
            if (String.IsNullOrWhiteSpace (apiUrl))
                return new BaseModel<DeptUpdateResp> ().Failed ("Please invoke login !");
            BaseModel<DeptUpdateResp> result = new BaseModel<DeptUpdateResp> ();
            if (model == null) {
                result.Failed ("OrganizeModel is null!");
            } else if (String.IsNullOrWhiteSpace (model.name)) {
                result.Failed ("deptName is null or empty!");
            } else {
                settings.NullValueHandling = NullValueHandling.Ignore;
                var jsonParam = Newtonsoft.Json.JsonConvert.SerializeObject (model, settings);
                result = httpClient.Post<DeptUpdateResp> (baseUrl + CardSolutionUrl + "/update" + apiUrl, jsonParam);
            }
            return result;
        }
        /**
         * @description: 1.3.1.1.3 删除部门信息
         * @param {type} 
         * @return: 
         */
        public BaseModel<DeptUpdateResp> DeleteDept (String id) {
            if (String.IsNullOrWhiteSpace (apiUrl))
                return new BaseModel<DeptUpdateResp> ().Failed ("Please invoke login !");
            BaseModel<DeptUpdateResp> result = new BaseModel<DeptUpdateResp> ();
            if (id == null || id.Equals ("")) {
                result.Failed ("id is null!");
            } else {

                result = httpClient.Get<DeptUpdateResp> (baseUrl + CardSolutionUrl + "/" + id + apiUrl);
            }
            return result;
        }

        /**
         * @description: 1.3.1.1.4 查询部门列表  获取所有的部门信息列表
         * @param {type} 
         * @return: 
         */
        public BaseModel<DeptQueryResp> QueryDept () {
            if (String.IsNullOrWhiteSpace (apiUrl))
                return new BaseModel<DeptQueryResp> ().Failed ("Please invoke login !");
            BaseModel<DeptQueryResp> result = new BaseModel<DeptQueryResp> ();
            result = httpClient.Get<DeptQueryResp> (baseUrl + CardSolutionUrl + apiUrl);
            return result;
        }

        #endregion

        #region 1.3.1.2 身份管理

        /**
         * @description: 1.3.1.2.1 查询身份列表 获取所有的人员身份信息列表
         * @param {type} 
         * @return: 
         */
        public BaseModel<PersonidentityQueryResp> QueryPersonidentity () {
            if (String.IsNullOrWhiteSpace (apiUrl))
                return new BaseModel<PersonidentityQueryResp> ().Failed ("Please invoke login !");
            BaseModel<PersonidentityQueryResp> result = new BaseModel<PersonidentityQueryResp> ();
            result = httpClient.Get<PersonidentityQueryResp> (baseUrl + PersonidentityUrl + apiUrl);
            return result;
        }

        #endregion

        #region 1.3.1.3 人员管理
        /**
         * @description: 1.3.1.3.1 添加人员信息
         * @param {type} 
         * @return: 
         */
        public BaseModel<BaseReturnData> AddPerson (PersonDtoReq model) {
            if (String.IsNullOrWhiteSpace (apiUrl))
                return new BaseModel<BaseReturnData> ().Failed ("Please invoke login !");
            BaseModel<BaseReturnData> result = new BaseModel<BaseReturnData> ();
            if (model == null) {
                result.Failed ("OrganizeModel is null!");
            } else if (String.IsNullOrWhiteSpace (model.name)) {
                result.Failed ("name is null or empty!");
            } else if (String.IsNullOrWhiteSpace (model.code)) {
                result.Failed ("code is null or empty!");
            } else if (String.IsNullOrWhiteSpace (model.paperNumber)) {
                result.Failed ("paperNumber is null or empty!");
            } else if (String.IsNullOrWhiteSpace (model.paperType)) {
                result.Failed ("paperType is null or empty!");
            } else if (String.IsNullOrWhiteSpace (model.personIdentityId)) {
                result.Failed ("personIdentityId is null or empty!");
            } else if (String.IsNullOrWhiteSpace (model.sex)) {
                result.Failed ("sex is null or empty!");
            } else {
                var reqJson=Newtonsoft.Json.JsonConvert.SerializeObject(model);
                result = httpClient.Post<BaseReturnData> (baseUrl + PersonUrl + apiUrl, reqJson);
            }
            return result;

        }
        /**
         * @description: 1.3.1.3.2 修改人员信息
         * @param {type} 
         * @return: 
         */
        public BaseModel<BaseReturnData> UpdatePerson (PersonDtoReq model) {
            if (String.IsNullOrWhiteSpace (apiUrl))
                return new BaseModel<BaseReturnData> ().Failed ("Please invoke login !");
            BaseModel<BaseReturnData> result = new BaseModel<BaseReturnData> ();
            if (model == null) {
                result.Failed ("OrganizeModel is null!");
            } else if (String.IsNullOrWhiteSpace (model.name)) {
                result.Failed ("name is null or empty!");
            } else if (String.IsNullOrWhiteSpace (model.code)) {
                result.Failed ("code is null or empty!");
            } else if (String.IsNullOrWhiteSpace (model.paperNumber)) {
                result.Failed ("paperNumber is null or empty!");
            } else if (String.IsNullOrWhiteSpace (model.paperType)) {
                result.Failed ("paperType is null or empty!");
            } else if (String.IsNullOrWhiteSpace (model.personIdentityId)) {
                result.Failed ("personIdentityId is null or empty!");
            } else if (String.IsNullOrWhiteSpace (model.sex)) {
                result.Failed ("sex is null or empty!");
            } else {
                var reqPara = new ApiQuery ();
                reqPara.Param.param = model;
                result = httpClient.Post<BaseReturnData> (baseUrl + PersonUrl + "/update" + apiUrl, reqPara);
            }
            return result;

        }

        /**
         * @description: 1.3.1.3.3 删除人员信息
         * @param {type} 
         * @return: 
         */
        public BaseModel<BaseReturnData> DeletePerson (String[] ids) {
            if (String.IsNullOrWhiteSpace (apiUrl))
                return new BaseModel<BaseReturnData> ().Failed ("Please invoke login !");
            BaseModel<BaseReturnData> result = new BaseModel<BaseReturnData> ();
            if (ids == null || ids.Length == 0) {
                result.Failed ("ids is null!");
            } else {
                ApiQuery apiQuery = new ApiQuery ();
                var reqPara = new Dictionary<String, Object> ();
                reqPara.Add ("ids", String.Join (",", ids));
                apiQuery.Param.param = reqPara;
                result = httpClient.Post<BaseReturnData> (baseUrl + PersonUrl + "/delete" + apiUrl, apiQuery);
            }
            return result;
        }

        /**  
         * @description: 1.3.1.3.4 查询人员列表信息  分页返回
         * @param {type} 
         * @return: 
         */
        public BaseModel<PersonQueryResp> QueryPersonList (PersonQueryReq model) {
            if (String.IsNullOrWhiteSpace (apiUrl))
                return new BaseModel<PersonQueryResp> ().Failed ("Please invoke login !");
            BaseModel<PersonQueryResp> result = new BaseModel<PersonQueryResp> ();
            if (model == null) {
                result.Failed ("PersonReq is null!");
            } else if (String.IsNullOrWhiteSpace (model.deptIdsString)) {
                result.Failed ("deptIdsString is null or empty!");
            } else {
                settings.NullValueHandling = NullValueHandling.Ignore;
                var jsonParam = Newtonsoft.Json.JsonConvert.SerializeObject (model, settings);
                result = httpClient.Post<PersonQueryResp> (baseUrl + PersonUrl + "/bycondition/combined" + apiUrl, jsonParam);
            }
            return result;
        }

        /**
         * @description: 1.3.1.3.5 人员图片上传
         * @param {type} 
         * @return: 
         */
        public BaseModel<BaseReturnData> SaveImage (ImageReq model) {
            if (String.IsNullOrWhiteSpace (apiUrl))
                return new BaseModel<BaseReturnData> ().Failed ("Please invoke login !");
            BaseModel<BaseReturnData> result = new BaseModel<BaseReturnData> ();
            if (model == null) {
                result.Failed ("ImageReq is null!");
            } else if (String.IsNullOrWhiteSpace (model.personCode)) {
                result.Failed ("personCode is null or empty!");
            } else if (String.IsNullOrWhiteSpace (model.base64file)) {
                result.Failed ("base64file is null or empty!");
            } else {
                settings.NullValueHandling = NullValueHandling.Ignore;
                var jsonParam = Newtonsoft.Json.JsonConvert.SerializeObject (model, settings);
                result = httpClient.Post<BaseReturnData> (baseUrl + "/CardSolution/common/saveMobileBase64ImageToByte" + apiUrl, jsonParam);
            }
            return result;
        }
        /**
         * @description: 1.3.1.3.6 批量添加人员
         * @param {type} 
         * @return: 
         */
        public BaseModel<PersonListEditResp> AddPersonList (List<PersonModel> models) {
            if (String.IsNullOrWhiteSpace (apiUrl))
                return new BaseModel<PersonListEditResp> ().Failed ("Please invoke login !");
            BaseModel<PersonListEditResp> result = new BaseModel<PersonListEditResp> ();

            if (models == null || models.Count <= 0) {
                result.Failed ("OrganizeModel is null!");
            } else {

                foreach (var model in models) {
                    if (model == null) {
                        result.Failed ("OrganizeModel is null!");
                    } else if (String.IsNullOrWhiteSpace (model.name)) {
                        result.Failed ("name is null or empty!");
                    } else if (String.IsNullOrWhiteSpace (model.code)) {
                        result.Failed ("code is null or empty!");
                    } else if (String.IsNullOrWhiteSpace (model.paperNumber)) {
                        result.Failed ("paperNumber is null or empty!");
                    } else if (String.IsNullOrWhiteSpace (model.paperType)) {
                        result.Failed ("paperType is null or empty!");
                    } else if (String.IsNullOrWhiteSpace (model.personIdentityId)) {
                        result.Failed ("personIdentityId is null or empty!");
                    } else if (String.IsNullOrWhiteSpace (model.sex)) {
                        result.Failed ("sex is null or empty!");
                    }
                }

            }

            settings.NullValueHandling = NullValueHandling.Ignore;
            var jsonParam = Newtonsoft.Json.JsonConvert.SerializeObject (models, settings);
            result = httpClient.Post<PersonListEditResp> (baseUrl + "/CardSolution/card/person/batch/third" + apiUrl, jsonParam);

            return result;

        }
        /**
         * @description: 1.3.1.3.7 根据人员编码删除人员
         * @param {type} 
         * @return: 
         */
        public BaseModel<BaseReturnData> DelPersonByCode (String personCode) {
            if (String.IsNullOrWhiteSpace (apiUrl))
                return new BaseModel<BaseReturnData> ().Failed ("Please invoke login !");
            BaseModel<BaseReturnData> result = new BaseModel<BaseReturnData> ();
            result = httpClient.Get<BaseReturnData> (baseUrl + "/CardSolution/card/person/deleteByCode/" + personCode + apiUrl);
            return result;

        }
        /**
         * @description: 1.3.1.3.8 人员详情查看
         * @param {type} 
         * @return: 
         */
        public BaseModel<PersonQueryByIdResp> QueryPersonById (long id) {
            if (String.IsNullOrWhiteSpace (apiUrl))
                return new BaseModel<PersonQueryByIdResp> ().Failed ("Please invoke login !");
            BaseModel<PersonQueryByIdResp> result = new BaseModel<PersonQueryByIdResp> ();
            var json = "{ \"id\":" + id + "}";
            result = httpClient.Post<PersonQueryByIdResp> (baseUrl + "/CardSolution/card/person/queryById" + apiUrl, json);
            return result;

        }
        /**
         * @description: 1.3.1.3.10 获取人员照片
         * @param {type} 
         * @return: 
         */
        public BaseModel<byte[]> getImage (long id) {
            if (String.IsNullOrWhiteSpace (apiUrl))
                return new BaseModel<byte[]> ().Failed ("Please invoke login !");
            BaseModel<byte[]> result = new BaseModel<byte[]> ();
            result = httpClient.Get<byte[]> (baseUrl + "/CardSolution/common/getImageByte/" + id + apiUrl);
            return result;

        }

        #endregion

        #region  1.3.1.4 卡片管理
        /**
         * @description: 1.3.1.4.1 人员开卡 开卡并且将人员与卡片相绑定

         * @param {type} 
         * @return: 
         */
        public BaseModel<BaseReturnData> OpenCard (List<CardModel> models) {
            if (String.IsNullOrWhiteSpace (apiUrl))
                return new BaseModel<BaseReturnData> ().Failed ("Please invoke login !");
            BaseModel<BaseReturnData> result = new BaseModel<BaseReturnData> ();
            if (models == null || models.Count <= 0) {
                result.Failed ("OrganizeModel is null!");
            } else {

                foreach (var model in models) {
                    if (model.personId == 0) {
                        result.Failed ("personId is null or empty!");
                    }

                }

                var reqPara = new Dictionary<string, List<CardModel>> ();
                reqPara.Add ("objectList", models);
                settings.NullValueHandling = NullValueHandling.Ignore;
                var jsonParam = Newtonsoft.Json.JsonConvert.SerializeObject (reqPara, settings);
                //  jsonParam="{ \"objectList\": [{ \"cardNumber\": \"B7BB4AF0\", \"cardType\": \"0\", \"category\": \"0\", \"cardStatus\": \"ACTIVE\", \"startDate\": \"2020-05-20\", \"endDate\": \"2020-07-19\", \"cardPassword\": \"S+TxzkUL77ftcG4nPB+X81aMLPh4CeLi5IUnik+mhWX6DI+rljSzlkt5ZpDZVaECkbn5b+v5N9YUo+aGjR1fYENw0BNVQqF5IBMFwvXdHbyRUyCKFQ4ssssNV7klFrcwf3OI0vpGwXbGTpHk4FGur6R2qraWOMOzFwdv/5VM8xg=\", \"subSystems\": \"1,3,4,5,6\", \"cardSubsidy\": \"0\", \"cardDeposit\": \"0\", \"cardCash\": \"0\", \"cardCost\": \"0\", \"personId\": 214, \"personName\": \"张三\" }] }";

                result = httpClient.Post<BaseReturnData> (baseUrl + "/CardSolution/card/card/open/batch" + apiUrl, jsonParam);
            }
            return result;

        }
        /**
         * @description: 1.3.1.4.2 退卡
         * @param {type} 
         * @return: 
         */
        public BaseModel<BaseReturnData> ReturnCard (CardReq model) {
            if (String.IsNullOrWhiteSpace (apiUrl))
                return new BaseModel<BaseReturnData> ().Failed ("Please invoke login !");
            BaseModel<BaseReturnData> result = new BaseModel<BaseReturnData> ();
            if (model == null) {
                result.Failed ("CardReq is null!");
            } else if (model.cardIds == null) {
                result.Failed ("ids is null!");
            } else {

                settings.NullValueHandling = NullValueHandling.Ignore;
                var jsonParam = Newtonsoft.Json.JsonConvert.SerializeObject (model, settings);
                result = httpClient.Post<BaseReturnData> (baseUrl + "/CardSolution/card/card/return" + apiUrl, jsonParam);
            }

            return result;

        }
        /**
         * @description: 1.3.1.4.3 挂失
         * @param {type} 
         * @return: 
         */
        public BaseModel<BaseReturnData> LostCard (CardReq model) {
            if (String.IsNullOrWhiteSpace (apiUrl))
                return new BaseModel<BaseReturnData> ().Failed ("Please invoke login !");
            BaseModel<BaseReturnData> result = new BaseModel<BaseReturnData> ();
            if (model == null) {
                result.Failed ("CardReq is null!");
            } else if (model.cardIds == null) {
                result.Failed ("ids is null!");
            } else if (model.eventCode == 0) {
                result.Failed ("eventCode is null!");
            } else {

                settings.NullValueHandling = NullValueHandling.Ignore;
                var jsonParam = Newtonsoft.Json.JsonConvert.SerializeObject (model, settings);
                result = httpClient.Post<BaseReturnData> (baseUrl + "/CardSolution/card/card/lose" + apiUrl, jsonParam);
            }

            return result;

        }
        /**
         * @description: 1.3.1.4.4 解挂
         * @param {type} 
         * @return: 
         */
        public BaseModel<BaseReturnData> RelieveCard (CardReq model) {
            if (String.IsNullOrWhiteSpace (apiUrl))
                return new BaseModel<BaseReturnData> ().Failed ("Please invoke login !");
            BaseModel<BaseReturnData> result = new BaseModel<BaseReturnData> ();
            if (model == null) {
                result.Failed ("CardReq is null!");
            } else if (model.cardIds == null) {
                result.Failed ("ids is null!");
            } else {

                settings.NullValueHandling = NullValueHandling.Ignore;
                var jsonParam = Newtonsoft.Json.JsonConvert.SerializeObject (model, settings);
                result = httpClient.Post<BaseReturnData> (baseUrl + "/CardSolution/card/card/relieve" + apiUrl, jsonParam);
            }

            return result;

        }
        /**
         * @description: 1.3.1.4.5 补卡
         * @param {type} 
         * @return: 
         */
        public BaseModel<BaseReturnData> ReNewCard (CardReNewReq model) {
            if (String.IsNullOrWhiteSpace (apiUrl))
                return new BaseModel<BaseReturnData> ().Failed ("Please invoke login !");
            BaseModel<BaseReturnData> result = new BaseModel<BaseReturnData> ();
            if (model == null) {
                result.Failed ("CardReq is null!");
            } else if (String.IsNullOrWhiteSpace (model.newCardNumber)) {
                result.Failed ("ids is null!");
            } else if (String.IsNullOrWhiteSpace (model.oldCardNumber)) {
                result.Failed ("oldCardNumber is null!");
            } else {

                settings.NullValueHandling = NullValueHandling.Ignore;
                var jsonParam = Newtonsoft.Json.JsonConvert.SerializeObject (model, settings);
                result = httpClient.Post<BaseReturnData> (baseUrl + "/CardSolution/card/card/renew" + apiUrl, jsonParam);
            }

            return result;

        }
        /**
         * @description: 1.3.1.4.6 换卡
         * @param {type} 
         * @return: 
         */
        public BaseModel<BaseReturnData> ChangeCard (CardReNewReq model) {
            if (String.IsNullOrWhiteSpace (apiUrl))
                return new BaseModel<BaseReturnData> ().Failed ("Please invoke login !");
            BaseModel<BaseReturnData> result = new BaseModel<BaseReturnData> ();
            if (model == null) {
                result.Failed ("CardReq is null!");
            } else if (String.IsNullOrWhiteSpace (model.newCardNumber)) {
                result.Failed ("ids is null!");
            } else if (String.IsNullOrWhiteSpace (model.oldCardNumber)) {
                result.Failed ("oldCardNumber is null!");
            } else {

                settings.NullValueHandling = NullValueHandling.Ignore;
                var jsonParam = Newtonsoft.Json.JsonConvert.SerializeObject (model, settings);
                result = httpClient.Post<BaseReturnData> (baseUrl + "/CardSolution/card/card/changeCard" + apiUrl, jsonParam);
            }

            return result;

        }
        /**
         * @description: 1.3.1.4.7 卡片修改 修改卡片开始与结束时间
         * @param {type} 
         * @return: 
         */
        public BaseModel<BaseReturnData> UpdateCard (CardModel model) {
            if (String.IsNullOrWhiteSpace (apiUrl))
                return new BaseModel<BaseReturnData> ().Failed ("Please invoke login !");
            BaseModel<BaseReturnData> result = new BaseModel<BaseReturnData> ();
            if (model == null) {
                result.Failed ("CardReq is null!");
            } else if (String.IsNullOrWhiteSpace (model.cardPassword)) {
                result.Failed ("cardPassword is null!");
            } else if (String.IsNullOrWhiteSpace (model.cardNumber)) {
                result.Failed ("cardNumber is null!");
            } else if (String.IsNullOrWhiteSpace (model.endDate)) {
                result.Failed ("endDate is null!");
            } else {

                settings.NullValueHandling = NullValueHandling.Ignore;
                var jsonParam = Newtonsoft.Json.JsonConvert.SerializeObject (model, settings);
                result = httpClient.Post<BaseReturnData> (baseUrl + "/CardSolution/card/card/update" + apiUrl, jsonParam);
            }
            return result;
        }
        /**
         * @description: 1.3.1.4.8 卡片查询
         * @param {type} 
         * @return: 
         */
        public BaseModel<CardQueryResp> Combined (CardPageReq model) {
            if (String.IsNullOrWhiteSpace (apiUrl))
                return new BaseModel<CardQueryResp> ().Failed ("Please invoke login !");
            BaseModel<CardQueryResp> result = new BaseModel<CardQueryResp> ();
            if (model == null) {
                result.Failed ("CardReq is null!");
            } else if (model.pageNum == 0) {
                result.Failed ("pageNum is null!");
            } else if (model.pageSize == 0) {
                result.Failed ("pageSize is null!");
            } else {

                settings.NullValueHandling = NullValueHandling.Ignore;
                var jsonParam = Newtonsoft.Json.JsonConvert.SerializeObject (model, settings);
                result = httpClient.Post<CardQueryResp> (baseUrl + "/CardSolution/card/card/bycondition/combined" + apiUrl, jsonParam);
            }

            return result;

        }
        /**
         * @description: 1.3.1.5 获取加密公钥
         *      用于一卡通服务中部分敏感字段加密,eg:卡片密码。
         *      加密方式,RSA 加密,具体加密方法可以详见 demo 用例
         * @param {type} 
         * @return: 
         */
        public BaseModel<BaseReturnStringData> GetPubKey () {
            if (String.IsNullOrWhiteSpace (apiUrl))
                return new BaseModel<BaseReturnStringData> ().Failed ("Please invoke login !");
            BaseModel<BaseReturnStringData> result = new BaseModel<BaseReturnStringData> ();

            result = httpClient.Get<BaseReturnStringData> (baseUrl + "/CardSolution/rsa/getPubKey" + apiUrl);

            return result;

        }

        #endregion

        #endregion

        #region  1.3.2 一卡通门禁
        #region 1.3.2.1 门禁设备
        /**
         * @description: 1.3.2.1.1 查询门禁设备信息
         * @param {type} 
         * @return: 
         */
        public BaseModel<AccessControlPageResp> QueryDevices (DeviceReq model) {
            if (String.IsNullOrWhiteSpace (apiUrl))
                return new BaseModel<AccessControlPageResp> ().Failed ("Please invoke login !");
            BaseModel<AccessControlPageResp> result = new BaseModel<AccessControlPageResp> ();
            if (model == null) {
                result.Failed ("PageReq is null!");
            } else if (model.pageNum == 0) {
                result.Failed ("pageNum is null!");
            } else if (model.pageNum == 0) {
                result.Failed ("pageNum is null!");
            } else {
                settings.NullValueHandling = NullValueHandling.Ignore;
                var jsonParam = Newtonsoft.Json.JsonConvert.SerializeObject (model, settings);
                result = httpClient.Post<AccessControlPageResp> (baseUrl + "/CardSolution/card/accessControl/device/bycondition/combined" + apiUrl, jsonParam);
            }
            return result;

        }
        /**
         * @description: 1.3.2.1.2 查询门禁通道信息
         * @param {type} 
         * @return: 
         */
        public BaseModel<ChannelQueryResp> QueryChannels (ChannelReq model) {
            if (String.IsNullOrWhiteSpace (apiUrl))
                return new BaseModel<ChannelQueryResp> ().Failed ("Please invoke login !");
            BaseModel<ChannelQueryResp> result = new BaseModel<ChannelQueryResp> ();
            if (model == null) {
                result.Failed ("PageReq is null!");
            } else if (model.pageNum == 0) {
                result.Failed ("pageNum is null!");
            } else if (model.pageNum == 0) {
                result.Failed ("pageNum is null!");
            } else {
                settings.NullValueHandling = NullValueHandling.Ignore;
                var jsonParam = Newtonsoft.Json.JsonConvert.SerializeObject (model, settings);
                result = httpClient.Post<ChannelQueryResp> (baseUrl + "/CardSolution/card/accessControl/channel/bycondition/combined" + apiUrl, jsonParam);
            }
            return result;

        }

        #endregion
        #endregion

        #region 1.3.2.2 开门计划
        /**
         * @description: 1.3.2.2.1 开门计划查询
         * @param {type} 
         * @return: 
         */
        public BaseModel<ConditionQueryResq> QueryConditions (ConditionReq model) {
            if (String.IsNullOrWhiteSpace (apiUrl))
                return new BaseModel<ConditionQueryResq> ().Failed ("Please invoke login !");
            BaseModel<ConditionQueryResq> result = new BaseModel<ConditionQueryResq> ();
            if (model == null) {
                result.Failed ("PageReq is null!");
            } else if (model.pageNum == 0) {
                result.Failed ("pageNum is null!");
            } else if (model.pageNum == 0) {
                result.Failed ("pageNum is null!");
            } else {
                settings.NullValueHandling = NullValueHandling.Ignore;
                var jsonParam = Newtonsoft.Json.JsonConvert.SerializeObject (model, settings);
                result = httpClient.Post<ConditionQueryResq> (baseUrl + "/CardSolution/card/accessControl/timeQuantum/1/page" + apiUrl, jsonParam);
            }
            return result;

        }
        /**
         * @description: 1.3.2.2.2 新增开门计划
         * @param {type} 
         * @return: 
         */
        public BaseModel<BaseReturnData> AddConditions (ConditionModel model) {
            if (String.IsNullOrWhiteSpace (apiUrl))
                return new BaseModel<BaseReturnData> ().Failed ("Please invoke login !");
            BaseModel<BaseReturnData> result = new BaseModel<BaseReturnData> ();
            if (model == null) {
                result.Failed ("PageReq is null!");
            } else if (String.IsNullOrWhiteSpace (model.detail)) {
                result.Failed ("detail is null!");
            } else if (String.IsNullOrWhiteSpace (model.name)) {
                result.Failed ("name is null!");
            } else {
                settings.NullValueHandling = NullValueHandling.Ignore;
                var jsonParam = Newtonsoft.Json.JsonConvert.SerializeObject (model, settings);
                result = httpClient.Post<BaseReturnData> (baseUrl + "/CardSolution/card/accessControl/timeQuantum" + apiUrl, jsonParam);
            }
            return result;

        }
        /**
         * @description: 1.3.2.2.3 修改计划查询
         * @param {type} 
         * @return: 
         */
        public BaseModel<BaseReturnData> UpdateConditions (ConditionModel model) {
            if (String.IsNullOrWhiteSpace (apiUrl))
                return new BaseModel<BaseReturnData> ().Failed ("Please invoke login !");
            BaseModel<BaseReturnData> result = new BaseModel<BaseReturnData> ();
            if (model == null) {
                result.Failed ("PageReq is null!");
            } else if (String.IsNullOrWhiteSpace (model.detail)) {
                result.Failed ("detail is null!");
            } else if (String.IsNullOrWhiteSpace (model.name)) {
                result.Failed ("name is null!");
            } else {
                settings.NullValueHandling = NullValueHandling.Ignore;
                var jsonParam = Newtonsoft.Json.JsonConvert.SerializeObject (model, settings);
                result = httpClient.Post<BaseReturnData> (baseUrl + "/CardSolution/card/accessControl/timeQuantum/update" + apiUrl, jsonParam);
            }
            return result;

        }

        /**
         * @description: 1.3.2.2.4 删除开门计划
         * @param {type} 
         * @return: 
         */
        public BaseModel<BaseReturnData> DelConditions (string[] ids) {
            if (String.IsNullOrWhiteSpace (apiUrl))
                return new BaseModel<BaseReturnData> ().Failed ("Please invoke login !");
            BaseModel<BaseReturnData> result = new BaseModel<BaseReturnData> ();
            if (ids == null || ids.Length == 0) {
                result.Failed ("ids is null!");
            } else {
                ApiQuery apiQuery = new ApiQuery ();
                var reqPara = new Dictionary<String, Object> ();
                reqPara.Add ("ids", String.Join (",", ids));
                apiQuery.Param.param = reqPara;
                result = httpClient.Post<BaseReturnData> (baseUrl + "/CardSolution/card/accessControl/timeQuantum/delete" + apiUrl, apiQuery);
            }
            return result;

        }
        #endregion

        #region 1.3.2.3 门组
        /**
         * @description: 1.3.2.3.1 添加门组
         * @param {type} 
         * @return: 
         */
        public BaseModel<BaseReturnData> AddDoorGroup (DoorGroupModel model) {
            if (String.IsNullOrWhiteSpace (apiUrl))
                return new BaseModel<BaseReturnData> ().Failed ("Please invoke login !");
            BaseModel<BaseReturnData> result = new BaseModel<BaseReturnData> ();
            if (model == null) {
                result.Failed ("PageReq is null!");
            } else if (model.doorGroupDetail == null || model.doorGroupDetail.Count == 0) {
                result.Failed ("doorGroupDetail is null!");
            } else if (String.IsNullOrWhiteSpace (model.name)) {
                result.Failed ("name is null!");
            } else {
                settings.NullValueHandling = NullValueHandling.Ignore;
                var jsonParam = Newtonsoft.Json.JsonConvert.SerializeObject (model, settings);
                result = httpClient.Post<BaseReturnData> (baseUrl + "/CardSolution/card/accessControl/doorGroup" + apiUrl, jsonParam);
            }
            return result;

        }
        /**
         * @description: 1.3.2.3.2 修改门组
         * @param {type} 
         * @return: 
         */
        public BaseModel<BaseReturnData> UpdateDoorGroup (DoorGroupModel model) {
            if (String.IsNullOrWhiteSpace (apiUrl))
                return new BaseModel<BaseReturnData> ().Failed ("Please invoke login !");
            BaseModel<BaseReturnData> result = new BaseModel<BaseReturnData> ();
            if (model == null) {
                result.Failed ("PageReq is null!");
            } else if (model.doorGroupDetail == null || model.doorGroupDetail.Count == 0) {
                result.Failed ("doorGroupDetail is null!");
            } else if (String.IsNullOrWhiteSpace (model.name)) {
                result.Failed ("name is null!");
            } else {
                settings.NullValueHandling = NullValueHandling.Ignore;
                var jsonParam = Newtonsoft.Json.JsonConvert.SerializeObject (model, settings);
                result = httpClient.Post<BaseReturnData> (baseUrl + "/CardSolution/card/accessControl/doorGroup" + apiUrl, jsonParam);
            }
            return result;

        }

        /**
         * @description: 1.3.2.3.3 删除门组
         * @param {type} 
         * @return: 
         */
        public BaseModel<BaseReturnData> DelDoorGroup (string[] ids) {
            if (String.IsNullOrWhiteSpace (apiUrl))
                return new BaseModel<BaseReturnData> ().Failed ("Please invoke login !");
            BaseModel<BaseReturnData> result = new BaseModel<BaseReturnData> ();
            if (ids == null || ids.Length == 0) {
                result.Failed ("PageReq is null!");
            } else {

                var jsonParam = "{\"doorGroupIds\":" + ids + "}";
                result = httpClient.Post<BaseReturnData> (baseUrl + "/CardSolution/card/accessControl/doorGroup/delete/batch" + apiUrl, jsonParam);
            }
            return result;

        }
        /**
         * @description: 1.3.2.3.4 查看门组权限
         * @param {type} 
         * @return: 
         */
        public BaseModel<DoorGroupQueryByIdResp> DelDoorGroup (string id) {
            if (String.IsNullOrWhiteSpace (apiUrl))
                return new BaseModel<DoorGroupQueryByIdResp> ().Failed ("Please invoke login !");
            BaseModel<DoorGroupQueryByIdResp> result = new BaseModel<DoorGroupQueryByIdResp> ();
            if (String.IsNullOrWhiteSpace (id)) {
                result.Failed ("id is null!");
            } else {
                result = httpClient.Get<DoorGroupQueryByIdResp> (baseUrl + "/CardSolution/card/accessControl/doorGroup/" + id + apiUrl);
            }
            return result;

        }
        /**
         * @description: 1.3.2.3.5 门组查询
         * @param {type} 
         * @return: 
         */
        public BaseModel<ConditionQueryResq> QueryDoorGroups (ConditionReq model) {
            if (String.IsNullOrWhiteSpace (apiUrl))
                return new BaseModel<ConditionQueryResq> ().Failed ("Please invoke login !");
            BaseModel<ConditionQueryResq> result = new BaseModel<ConditionQueryResq> ();
            if (model == null) {
                result.Failed ("ConditionReq is null!");
            } else if (model.pageNum == 0) {
                result.Failed ("pageNum is null!");
            } else if (model.pageSize == 0) {
                result.Failed ("pageSize is null!");
            } else {
                result = httpClient.Get<ConditionQueryResq> (baseUrl + "/CardSolution/card/accessControl/doorGroup/bycondition/combined" + apiUrl);
            }
            return result;

        }

        #endregion

        #region 1.3.2.4 卡片授权
        /**
         * @description: 1.3.2.4.1 添加卡片授权
         * @param {type} 
         * @return: 
         */
        public BaseModel<BaseReturnData> AddCardPrivilege (CardPrivilegeModel model) {
            if (String.IsNullOrWhiteSpace (apiUrl))
                return new BaseModel<BaseReturnData> ().Failed ("Please invoke login !");
            BaseModel<BaseReturnData> result = new BaseModel<BaseReturnData> ();
            if (model == null) {
                result.Failed ("CardPrivilegeModel is null!");
            } else if (model.cardNumbers == null || model.cardNumbers.Length == 0) {
                result.Failed ("cardNumbers is null!");
            } else if (String.IsNullOrWhiteSpace (model.timeQuantumId)) {
                result.Failed ("timeQuantumId is null!");
            } else if (model.cardPrivilegeDetails == null || model.cardPrivilegeDetails.Count == 0) {
                result.Failed ("cardPrivilegeDetails is null!");
            } else {
                settings.NullValueHandling = NullValueHandling.Ignore;
                var jsonParam = Newtonsoft.Json.JsonConvert.SerializeObject (model, settings);
                result = httpClient.Post<BaseReturnData> (baseUrl + "/CardSolution/card/accessControl/doorAuthority" + apiUrl, jsonParam);
            }
            return result;

        }
        /**
         * @description: 1.3.2.4.2 修改卡片授权
         * @param {type} 
         * @return: 
         */
        public BaseModel<BaseReturnData> UpdateCardPrivilege (CardPrivilegeModel model) {
            if (String.IsNullOrWhiteSpace (apiUrl))
                return new BaseModel<BaseReturnData> ().Failed ("Please invoke login !");
            BaseModel<BaseReturnData> result = new BaseModel<BaseReturnData> ();
            if (model == null) {
                result.Failed ("CardPrivilegeModel is null!");
            } else if (model.cardNumbers == null || model.cardNumbers.Length == 0) {
                result.Failed ("cardNumbers is null!");
            } else if (String.IsNullOrWhiteSpace (model.timeQuantumId)) {
                result.Failed ("timeQuantumId is null!");
            } else if (model.cardPrivilegeDetails == null || model.cardPrivilegeDetails.Count == 0) {
                result.Failed ("cardPrivilegeDetails is null!");
            } else {
                settings.NullValueHandling = NullValueHandling.Ignore;
                var jsonParam = Newtonsoft.Json.JsonConvert.SerializeObject (model, settings);
                result = httpClient.Post<BaseReturnData> (baseUrl + " /CardSolution/card/accessControl/doorAuthority/update" + apiUrl, jsonParam);
            }
            return result;

        }
        /**
         * @description: 1.3.2.4.3 删除卡片授权
         * @param {type} 
         * @return: 
         */
        public BaseModel<BaseReturnData> DelCardPrivilege (string[] ids) {
            if (String.IsNullOrWhiteSpace (apiUrl))
                return new BaseModel<BaseReturnData> ().Failed ("Please invoke login !");
            BaseModel<BaseReturnData> result = new BaseModel<BaseReturnData> ();
            if (ids == null || ids.Length == 0) {
                result.Failed ("PageReq is null!");
            } else {

                var jsonParam = "{\"cardNums\":" + ids + "}";
                result = httpClient.Post<BaseReturnData> (baseUrl + "/CardSolution/card/accessControl/doorAuthority/delete/batch" + apiUrl, jsonParam);
            }
            return result;

        }

        /**
         * @description: 1.3.2.4.4 查询卡片授权
         * @param {type} 
         * @return: 
         */
        public BaseModel<CardPrivilegeQueryResp> QueryCardPrivilege (CardPrivilegeReq model) {
            if (String.IsNullOrWhiteSpace (apiUrl))
                return new BaseModel<CardPrivilegeQueryResp> ().Failed ("Please invoke login !");
            BaseModel<CardPrivilegeQueryResp> result = new BaseModel<CardPrivilegeQueryResp> ();
            if (model == null) {
                result.Failed ("CardPrivilegeModel is null!");
            } else if (model.pageSize == null || model.pageSize == 0) {
                result.Failed ("cardNumbers is null!");
            } else if (model.pageNum == null || model.pageNum == 0) {
                result.Failed ("cardNumbers is null!");
            } else {
                settings.NullValueHandling = NullValueHandling.Ignore;
                var jsonParam = Newtonsoft.Json.JsonConvert.SerializeObject (model, settings);
                result = httpClient.Post<CardPrivilegeQueryResp> (baseUrl + " /CardSolution/card/accessControl/doorAuthority/bycondition/combined" + apiUrl, jsonParam);
            }
            return result;

        }
        /**
         * @description: 1.3.2.4.5 根据卡号查询授权详情
         * @param {type} 
         * @return: 
         */
        public BaseModel<CardPrivilegeQueryResp> QueryCardPrivilegeByCode (string cardNum) {
            if (String.IsNullOrWhiteSpace (apiUrl))
                return new BaseModel<CardPrivilegeQueryResp> ().Failed ("Please invoke login !");
            BaseModel<CardPrivilegeQueryResp> result = new BaseModel<CardPrivilegeQueryResp> ();
            if (String.IsNullOrWhiteSpace (cardNum)) {
                result.Failed ("cardNum is null!");
            } else {
                var apiQuery = new ApiQuery ();
                result = httpClient.Post<CardPrivilegeQueryResp> (baseUrl + " /CardSolution/card/accessControl/doorAuthority/" + cardNum + apiUrl, apiQuery);
            }
            return result;

        }
        /**
         * @description: 1.3.2.4.6 卡片授权任务查询
         * @param {type} 
         * @return: 
         */
        public BaseModel<AcsCardSyncResp> GetAcsCardSyncList (AcsCardSyncReq model) {
            if (String.IsNullOrWhiteSpace (apiUrl))
                return new BaseModel<AcsCardSyncResp> ().Failed ("Please invoke login !");
            BaseModel<AcsCardSyncResp> result = new BaseModel<AcsCardSyncResp> ();

            if (model == null) {
                result.Failed ("CardPrivilegeModel is null!");
            } else if (model.pageSize == null || model.pageSize == 0) {
                result.Failed ("cardNumbers is null!");
            } else if (model.pageNum == null || model.pageNum == 0) {
                result.Failed ("cardNumbers is null!");
            } else {
                settings.NullValueHandling = NullValueHandling.Ignore;
                var jsonParam = Newtonsoft.Json.JsonConvert.SerializeObject (model, settings);
                result = httpClient.Post<AcsCardSyncResp> (baseUrl + " /CardSolution/card/accessControl/doorAuthority/getAcsCardSyncList" + apiUrl, jsonParam);
            }
            return result;

        }

        #endregion

        #region 1.3.2.5 人脸授权(不推荐,直接调用卡片授权接口即可)

        /**
         * @description: 1.3.2.5.1 添加人脸授权  1.3.2.5.2 修改人脸授权
         * @param {type} 
         * @return: 
         */
        public BaseModel<BaseReturnData> AddOrUpdateAcsFaceSync (AcsFaceSyncReq model) {
            if (String.IsNullOrWhiteSpace (apiUrl))
                return new BaseModel<BaseReturnData> ().Failed ("Please invoke login !");
            BaseModel<BaseReturnData> result = new BaseModel<BaseReturnData> ();
            if (model == null) {
                result.Failed ("PageReq is null!");
            } else if ((model.resouceCodes == null || model.resouceCodes.Length == 0) && ((model.doorGroupIds == null || model.doorGroupIds.Length == 0))) {
                result.Failed ("resouceCodes and doorGroupIds is null!");
            } else if (String.IsNullOrWhiteSpace (model.personCode)) {
                result.Failed ("personCode is null!");
            } else {
                settings.NullValueHandling = NullValueHandling.Ignore;
                var jsonParam = Newtonsoft.Json.JsonConvert.SerializeObject (model, settings);
                result = httpClient.Post<BaseReturnData> (baseUrl + "/CardSolution/card/accessControl/doorAuthority/updateForThirdParty" + apiUrl, jsonParam);
            }
            return result;

        }
        /**
         * @description: 1.3.2.5.3 删除人脸授权

         * @param {type} 
         * @return: 
         */
        public BaseModel<BaseReturnData> DelAcsFaceSync (AcsFaceSyncReq model) {
            if (String.IsNullOrWhiteSpace (apiUrl))
                return new BaseModel<BaseReturnData> ().Failed ("Please invoke login !");
            BaseModel<BaseReturnData> result = new BaseModel<BaseReturnData> ();
            if (model == null) {
                result.Failed ("PageReq is null!");
            } else if (String.IsNullOrWhiteSpace (model.personCode)) {
                result.Failed ("personCode is null!");
            } else {
                settings.NullValueHandling = NullValueHandling.Ignore;
                var jsonParam = Newtonsoft.Json.JsonConvert.SerializeObject (model, settings);
                result = httpClient.Post<BaseReturnData> (baseUrl + "/CardSolution/card/accessControl/doorAuthority/faceDelete" + apiUrl, jsonParam);
            }
            return result;

        }
        #endregion

        /**
         * @description: 1.3.2.5.4 查询人脸授权任务
         * @param {type} 
         * @return: 
         */
        public BaseModel<AcsCardSyncResp> GetAcsFaceSyncList (AcsFaceSyncListReq model) {
            if (String.IsNullOrWhiteSpace (apiUrl))
                return new BaseModel<AcsCardSyncResp> ().Failed ("Please invoke login !");
            BaseModel<AcsCardSyncResp> result = new BaseModel<AcsCardSyncResp> ();
            if (model == null) {
                result.Failed ("PageReq is null!");
            } else if (model.pageNum == 0) {
                result.Failed ("pageNum is null!");
            } else if (model.pageSize == 0) {
                result.Failed ("pageSize is null!");
            } else {
                settings.NullValueHandling = NullValueHandling.Ignore;
                var jsonParam = Newtonsoft.Json.JsonConvert.SerializeObject (model, settings);
                result = httpClient.Post<AcsCardSyncResp> (baseUrl + "/CardSolution/card/accessControl/doorAuthority/getAcsFaceSyncList" + apiUrl, jsonParam);
            }
            return result;
        }

        #endregion
    }

}