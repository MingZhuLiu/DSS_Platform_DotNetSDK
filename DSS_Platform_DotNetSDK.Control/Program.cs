using System;
using System.IO;
using System.Linq;
using System.Text;
using DSS_Platform_DotNetSDK.Lib;
using DSS_Platform_DotNetSDK.Lib.Commons;
using DSS_Platform_DotNetSDK.Lib.Models.Req;
using DSS_Platform_DotNetSDK.Lib.Models.Resp;

namespace DSS_Platform_DotNetSDK.Control
{
    class Program
    {
        static void Main(string[] args)
        {
            DSSClient dSSClient = new DSSClient("192.168.19.180");
            // var publicKeyResp=dSSClient.getPublicKey("system");//1.1.1 获取公钥
            var loginResp = dSSClient.login("system", "mote12345"); //1.1.2 获取 token 的接口
            if (loginResp.Flag && loginResp.Data.success)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("登录成功!");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("登录失败!");
                return;
            }



            PersonModel personModel = new PersonModel();
            var departments = dSSClient.QueryDept();
            var deptIdsArr = departments.Data.data.Select(p => p.id).ToArray();
            var deptIdsStr = String.Join(',', deptIdsArr);
            deptIdsStr = departments.Data.data.Where(p => p.name == "综合计划部").FirstOrDefault()?.id.ToString();



            // var zs = dSSClient.QueryPersonList(new Lib.Models.Req.PersonQueryReq() { pageNum = 1, pageSize = 1000, deptIdsString = deptIdsStr, phone = "18115647321" }).Data.data.pageData.FirstOrDefault();

            while (true)
            {




                PersonDtoReq person = new PersonDtoReq();
                person.birthday = "2019-02-12";
                person.cardNumber = 320204198402140027.ToString();
                person.code = "00000442";
                person.deptId = deptIdsStr;
                person.name = "法外狂徒-云龙";
                person.paperNumber = "BY10010";
                person.paperType = "学生证";
                person.personIdentityId = "-99";
                person.phone = "13912345678";
                person.sex = "男";
                person.status = "在职";
                var persons = dSSClient.QueryPersonList(new Lib.Models.Req.PersonQueryReq() { pageNum = 1, pageSize = 1000, deptIdsString = deptIdsStr, cardNumber = person.cardNumber });
                if (persons.Data.data.totalRows != 0)
                {

                    PersonModel pm = persons.Data.data.pageData.FirstOrDefault();
                    person.code = pm.code;
                }
                else
                {
                    var resp = dSSClient.AddPerson(person);
                }





                System.IO.FileStream fileStream = new System.IO.FileStream("/Users/mingzhuliu/Downloads/13.jpg", FileMode.Open);
                byte[] buffer = new byte[fileStream.Length];
                fileStream.Read(buffer, 0, buffer.Length);
                fileStream.Close();

                ImageReq imageReq = new ImageReq();
                imageReq.personCode = "00000442";
                imageReq.base64file = Convert.ToBase64String(buffer);
                var imageResp = dSSClient.SaveImage(imageReq);


                var cardNum = "57944AF0";

                //先退卡

                var returnCardResp = dSSClient.ReturnCard(new CardReq() { cardIds = new string[] { cardNum } });



                var existCard = dSSClient.Combined(new CardPageReq() { pageNum = 1, pageSize = 100, cardNumber = cardNum }).Data.data.pageData?.FirstOrDefault();
                if (existCard != null)
                {
                    existCard.cardStatus = "WITHDRAWN";
                    dSSClient.UpdateCard(existCard);
                }

                var cardModel = new CardModel();
                cardModel.personId = resp.Data.data;
                cardModel.personName = person.name;
                cardModel.category = "0";
                cardModel.cardNumber = cardNum;
                cardModel.cardType = "0";
                cardModel.cardStatus = "ACTIVE";
                cardModel.startDate = DateTime.Now.ToString("yyyy-MM-dd");
                cardModel.endDate = DateTime.Now.AddDays(60).ToString("yyyy-MM-dd");

                cardModel.subSystems = "1,3,4,5,6";
                cardModel.cardSubsidy = "0";
                cardModel.cardCash = "0";
                cardModel.cardCost = "0";
                cardModel.cardDeposit = "0";

                // var publicKey = dSSClient.GetPubKey();
                // var rsa = new RSAHelper(RSAType.RSA, Encoding.UTF8, null, publicKey.Data.data);
                cardModel.cardPassword = "S+TxzkUL77ftcG4nPB+X81aMLPh4CeLi5IUnik+mhWX6DI+rljSzlkt5ZpDZVaECkbn5b+v5N9YUo+aGjR1fYENw0BNVQqF5IBMFwvXdHbyRUyCKFQ4ssssNV7klFrcwf3OI0vpGwXbGTpHk4FGur6R2qraWOMOzFwdv/5VM8xg=";//rsa.Encrypt("1");
                var len = cardModel.cardPassword.Length;

                var openCardResp = dSSClient.OpenCard(new CardModel[] { cardModel }.ToList());


                //开门计划
                var timeConditions = dSSClient.QueryConditions(new ConditionReq() { pageNum = 1, pageSize = 9999, });

                //开门通道
                var chanels = dSSClient.QueryChannels(new ChannelReq() { pageNum = 1, pageSize = 9999, });

                //
                CardPrivilegeModel cardAuthReq = new CardPrivilegeModel();
                cardAuthReq.cardNumbers = new string[] { cardNum };
                cardAuthReq.timeQuantumId = timeConditions.Data.data.pageData.FirstOrDefault()?.id;
                cardAuthReq.cardPrivilegeDetails = chanels.Data.data.pageData.Select(p => new CardPrivilegeDetailsModel() { privilegeType = "1", resouceCode = p.channelCode }).ToList();

                var authResp = dSSClient.AddCardPrivilege(cardAuthReq);




            }

            Console.WriteLine("Oveer");

            //var organizeListResp=dSSClient.QueryOrganizeList();//1.2.1.1 查询组织
            //var organizeResp=dSSClient.QueryOrganize(1);//1.2.1.2 查询单个组织详情;
            //var addOrganizeResp = dSSClient.AddOrganize (new Lib.Models.Resp.OrganizeModel () { orgName = "测试组织", orgType = "1", orgPreCode = "001", orgSn = "1003" });//1.2.1.3 组织新增
            // var editOrganizeResp = dSSClient.EditOrganize (new Lib.Models.Resp.OrganizeModel () { orgName = "测试组织111", id = "7" });//1.2.1.3 组织修改
            //var deletedOrganizeResp=dSSClient.DeleteOrganize(new String[]{"9"}); 1.2.1.3组织删除

            //1.3.1.1.1添加部门信息
            //var addDeptResp=dSSClient.AddDept(new Lib.Models.Resp.DeptModel(){name="测试部门",parentId=1});
            //1.3.1.1.2 修改部门信息
            // var UpdateDeptResp=dSSClient.UpdateDept(new Lib.Models.Resp.DeptModel(){id=5, name="测试部门1",parentId=1});
            //1.3.1.1.4 查询部门列表  获取所有的部门信息列表
            //  var DeptResp = dSSClient.QueryDept();

            Console.WriteLine("End...");
        }
    }
}