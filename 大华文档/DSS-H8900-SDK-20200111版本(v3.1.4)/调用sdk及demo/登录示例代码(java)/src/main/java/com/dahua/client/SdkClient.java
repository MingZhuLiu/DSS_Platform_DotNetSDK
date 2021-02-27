package com.dahua.client;

import java.text.MessageFormat;
import java.util.HashMap;
import java.util.Map;

import com.alibaba.fastjson.JSON;
import com.alibaba.fastjson.JSONObject;
import com.dahua.constant.Constants;
import com.dahua.util.HttpClientPoolUtil;
import com.dahua.util.RSAUtils;

/**
 * 
 *<p>Title:SdkClient</p>
 *<p>Description:客户端调用工具类</p>
 *<p>Company:浙江大华技术股份有限公司</p>
 * @author 32174
 * @date 2018年12月18日
 */
public class SdkClient {
	

	/**
	 * 
	 * @Title:get
	 * @Description:get请求
	 * @param:@param url
	 * @param:@param headMap
	 * @param:@return
	 * @param:@throws Exception
	 * @return:String
	 * @throws
	 *
	 */
	public static String get(String url,Map<String, String> headMap) throws Exception{

		return HttpClientPoolUtil.get(url, headMap);
	}

	/**
	 * 
	 * @Title:post
	 * @Description:post请求
	 * @param:@param url
	 * @param:@param data JSON序列化后的参数 JSON.toJSONString(xxx)
	 * @param:@param headMap
	 * @param:@return
	 * @param:@throws Exception
	 * @return:String
	 * @throws
	 *
	 */
	public static String post(String url,String data,Map<String, String> headMap) throws Exception{


		return HttpClientPoolUtil.post(url, data, headMap);
	}

	/**
	 * 
	 * @Title:put
	 * @Description:put请求
	 * @param:@param url
	 * @param:@param data JSON序列化后的参数 JSON.toJSONString(xxx)
	 * @param:@param headMap
	 * @param:@return
	 * @param:@throws Exception
	 * @return:String
	 * @throws
	 *
	 */
	public static String put(String url,String data,Map<String, String> headMap) throws Exception{
		return HttpClientPoolUtil.put(url, data, headMap);
	}

	/**
	 * 
	 * @Title:delete
	 * @Description:delete请求
	 * @param:@param url
	 * @param:@param data JSON序列化后的参数 JSON.toJSONString(xxx)
	 * @param:@param headMap
	 * @param:@return
	 * @param:@throws Exception
	 * @return:String
	 * @throws
	 *
	 */
	public static String delete(String url,String data,Map<String, String> headMap) throws Exception{
		return HttpClientPoolUtil.delete(url, headMap);
	}


	/**
	 * 
	 * @Title:login
	 * @Description:模拟登录调用,获取token
	 * @param:@param ip
	 * @param:@param port
	 * @param:@param userName  用户名              用户名及密码由大华区域技术提供
	 * @param:@param password 密码(明文)
	 * @param:@param headMap
	 * @param:@return
	 * @param:@throws Exception
	 * @return:String
	 * @throws
	 *
	 */
	public static String login(String ip,String port,String userName,String password,Map<String, String> headMap) throws Exception{
		String result="";
		String publicKeyUrl =MessageFormat.format(Constants.GET_PUBLIC_KEY, ip,port);
		String loginUrl =MessageFormat.format(Constants.LOGIN, ip,port);
		try {
			Map<String, Object> paramMap = new HashMap<String, Object>();
			paramMap.put("loginName", userName);
			String publicKeyResult =HttpClientPoolUtil.post(publicKeyUrl, JSON.toJSONString(paramMap), headMap);
			if((publicKeyResult != null && !"".equals(publicKeyResult))){
				JSONObject publicKeyResultObject=JSON.parseObject(publicKeyResult);
				String publicKey =publicKeyResultObject.getString("publicKey");
				if(publicKey == null || "".equals(publicKey)){
					throw new Exception("获取token失败");
				}
				String entryPassword =RSAUtils.encryptBASE64(RSAUtils.encryptByPublicKey(password.getBytes(), publicKey));
				paramMap.put("loginPass", entryPassword);
				result = HttpClientPoolUtil.post(loginUrl, JSON.toJSONString(paramMap),headMap);
			}			
		} catch (Exception e) {
			throw e;
		}
		return result;
	}

}
