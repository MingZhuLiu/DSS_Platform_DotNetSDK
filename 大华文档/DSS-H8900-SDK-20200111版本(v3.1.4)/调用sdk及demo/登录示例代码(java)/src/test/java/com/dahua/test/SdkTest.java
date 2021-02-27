package com.dahua.test;

import java.util.HashMap;
import java.util.Map;

import org.junit.Test;

import com.alibaba.fastjson.JSON;
import com.dahua.client.SdkClient;

/**
 * 
 *<p>Title:SdkTest</p>
 *<p>Description:测试类</p>
 *<p>Company:浙江大华技术股份有限公司</p>
 * @author 32174
 * @date 2018年12月18日
 */
public class SdkTest {

	@Test
	public void login() throws Exception {
		
		String ip="10.35.120.95";
		String port="80";
		String userName="system";
		String password="dahua2006";
		Map<String, String> headMap = new HashMap<String, String>();
		headMap.put("Content-Type", "application/json;charset=UTF-8");
		System.out.println(SdkClient.login(ip, port, userName, password,headMap));		
	}
	
	@Test
	public void get() throws Exception {
		String url="http://10.35.120.95/face/groupInfo/page?searchType=1&searchKey=&pageNum=1&pageSize=12&reset=false&token=1e466c3d0968b97baa6c51a143b9695b";
		Map<String, String> headMap = new HashMap<String, String>();
		headMap.put("content-Type", "application/json");
		System.out.println(SdkClient.get(url,headMap));
	}
	
	@Test
	public void post() throws Exception {
		String url="http://10.35.120.95/face/groupInfo/add?token=1e466c3d0968b97baa6c51a143b9695b";
		Map<String,Object> param = new HashMap<String,Object>();
		param.put("groupdetail", "test");
		param.put("groupname", "test1");
		param.put("grouptype", 1);
		Map<String, String> headMap = new HashMap<String, String>();
		headMap.put("content-Type", "application/json");
		System.out.println(SdkClient.post(url, JSON.toJSONString(param),headMap));
	}
	
	@Test
	public void put() throws Exception {
		
	}
	
	@Test
	public void delete() throws Exception {
		
	}

}
