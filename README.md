# DSS_Platform
## 大华智慧平台对接
## 大华智慧园区综合管理平台对外开放接口

### [Gitub项目地址](https://github.com/MingZhuLiu/DSS_Platform_DotNetSDK)


### 大华的个别接口行为和文档描述并不一致,小伙伴们祝好运.
### 按照大华的接口规范对接很可能会踩到发卡慢,无法刷卡的大坑.

> 核心类库DSS_Platform_DotNetSDK.Lib

> 调用测试DSS_Platform_DotNetSDK.Control

> 接口过多,基本调用范例参考DSS_Platform_DotNetSDK.Control.Program.cs

> 其余接口参考大华文档以及DSS_Platform_DotNetSDK.Lib.DSSClient.cs中的实现

> 所有接口均在大华的返回值后又包装了一层泛型BaseModel,用于捕获异常.

> Nuget 引用 dotnet add package DSS_Platform_DotNetSDK.Lib

##### (注:大华卡片密码当前是写死的,具体RSA算法还不清楚,要求100位以内的长度,有清楚的大神可以给我提供建议)