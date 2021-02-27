package com.dahua.util;

import java.nio.charset.Charset;
import java.security.KeyManagementException;
import java.security.NoSuchAlgorithmException;
import java.util.Map;
import java.util.Map.Entry;

import javax.net.ssl.SSLContext;
import javax.net.ssl.TrustManager;
import javax.net.ssl.X509TrustManager;

import org.apache.http.HeaderElement;
import org.apache.http.HeaderElementIterator;
import org.apache.http.HttpEntity;
import org.apache.http.HttpResponse;
import org.apache.http.client.config.RequestConfig;
import org.apache.http.client.methods.CloseableHttpResponse;
import org.apache.http.client.methods.HttpDelete;
import org.apache.http.client.methods.HttpEntityEnclosingRequestBase;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.client.methods.HttpPost;
import org.apache.http.client.methods.HttpPut;
import org.apache.http.client.methods.HttpRequestBase;
import org.apache.http.client.protocol.HttpClientContext;
import org.apache.http.config.Registry;
import org.apache.http.config.RegistryBuilder;
import org.apache.http.conn.ConnectionKeepAliveStrategy;
import org.apache.http.conn.socket.ConnectionSocketFactory;
import org.apache.http.conn.socket.PlainConnectionSocketFactory;
import org.apache.http.conn.ssl.SSLConnectionSocketFactory;
import org.apache.http.entity.StringEntity;
import org.apache.http.impl.client.CloseableHttpClient;
import org.apache.http.impl.client.HttpClients;
import org.apache.http.impl.conn.PoolingHttpClientConnectionManager;
import org.apache.http.message.BasicHeaderElementIterator;
import org.apache.http.protocol.HTTP;
import org.apache.http.protocol.HttpContext;
import org.apache.http.util.EntityUtils;

/**
 * 
 *<p>Title:HttpClientPoolUtil</p>
 *<p>Description:httpClient线程池工具类</p>
 *<p>Company:浙江大华技术股份有限公司</p>
 * @author 32174
 * @date 2018年12月15日
 */
public class HttpClientPoolUtil {
	
	public static CloseableHttpClient httpClient = null;
	
	/**
	 * 初始化连接池
	 * @throws NoSuchAlgorithmException 
	 * @throws KeyManagementException 
	 */
	public static synchronized void initPools() throws KeyManagementException, NoSuchAlgorithmException {

		if (httpClient == null) {
			//采用绕过验证的方式处理https请求  
			SSLContext sslcontext = createIgnoreVerifySSL();  
			//设置协议http和https对应的处理socket链接工厂的对象  
			Registry<ConnectionSocketFactory> socketFactoryRegistry = RegistryBuilder.<ConnectionSocketFactory>create()  
					.register("http", PlainConnectionSocketFactory.INSTANCE)  
					.register("https", new SSLConnectionSocketFactory(sslcontext,SSLConnectionSocketFactory.ALLOW_ALL_HOSTNAME_VERIFIER))  
					.build();  
			PoolingHttpClientConnectionManager cm = new PoolingHttpClientConnectionManager(socketFactoryRegistry);
			cm.setDefaultMaxPerRoute(20);
			cm.setMaxTotal(500);
			httpClient = HttpClients.custom().setKeepAliveStrategy(defaultStrategy).setConnectionManager(cm).build();
		}
	}

	/**
	 * Http connection keepAlive 设置
	 */
	public static ConnectionKeepAliveStrategy defaultStrategy = new ConnectionKeepAliveStrategy() {
		public long getKeepAliveDuration(HttpResponse response, HttpContext context) {
			HeaderElementIterator it = new BasicHeaderElementIterator(response.headerIterator(HTTP.CONN_KEEP_ALIVE));
			int keepTime = 30;
			while (it.hasNext()) {
				HeaderElement he = it.nextElement();
				String param = he.getName();
				String value = he.getValue();
				if (value != null && param.equalsIgnoreCase("timeout")) {
					try {
						return Long.parseLong(value) * 1000;
					} catch (Exception e) {
						e.printStackTrace();
					}
				}
			}
			return keepTime * 1000;
		}
	};
	
	/** 
	 * 绕过验证 
	 *   
	 * @return 
	 * @throws NoSuchAlgorithmException  
	 * @throws KeyManagementException  
	 */  
	public static SSLContext createIgnoreVerifySSL() throws NoSuchAlgorithmException, KeyManagementException {  
		SSLContext sc = SSLContext.getInstance("SSLv3");  

		// 实现一个X509TrustManager接口，用于绕过验证，不用修改里面的方法  
		X509TrustManager trustManager = new X509TrustManager() {  
			  
			public void checkClientTrusted(  
					java.security.cert.X509Certificate[] paramArrayOfX509Certificate,  
					String paramString) {  
			}  

			  
			public void checkServerTrusted(  
					java.security.cert.X509Certificate[] paramArrayOfX509Certificate,  
					String paramString) {  
			}  

			  
			public java.security.cert.X509Certificate[] getAcceptedIssuers() {  
				return null;  
			}  
		};  

		sc.init(null, new TrustManager[] { trustManager }, null);  
		return sc;  
	}
	
	/**
	 * 创建请求
	 *
	 * @param uri 请求url
	 * @param methodName 请求的方法类型
	 * @param headMap 请求头
	 * @return
	 * @throws NoSuchAlgorithmException 
	 * @throws KeyManagementException 
	 */
	public static HttpRequestBase getRequest(String url, String methodName,Map<String, String> headMap) 
			throws KeyManagementException, NoSuchAlgorithmException {
		if (httpClient == null) {
			initPools();
		}
		HttpRequestBase method = null;     
		RequestConfig requestConfig = RequestConfig.custom().setSocketTimeout(30 * 1000).setConnectTimeout(30 * 1000)
				.setConnectionRequestTimeout(30 * 1000).setExpectContinueEnabled(false).build();

		if (HttpPut.METHOD_NAME.equalsIgnoreCase(methodName)) {
			method = new HttpPut(url);
		} else if (HttpPost.METHOD_NAME.equalsIgnoreCase(methodName)) {
			method = new HttpPost(url);
		} else if (HttpGet.METHOD_NAME.equalsIgnoreCase(methodName)) {
			method = new HttpGet(url);
		} else if (HttpDelete.METHOD_NAME.equalsIgnoreCase(methodName)) {
			method = new HttpDelete(url);
		} else {
			method = new HttpPost(url);
		}            
		if(!headMap.isEmpty()){
			for(Entry<String, String> value:headMap.entrySet()){
				method.addHeader(value.getKey(), value.getValue());
			}
		}
		method.setConfig(requestConfig);
		return method;
	}
	

	/**
	 * 执行GET 请求
	 *
	 * @param uri
	 * @return
	 */
	public static String get(String url,Map<String, String> headMap) throws Exception{
		HttpEntity httpEntity = null;
		HttpRequestBase method = null;
		String responseBody = "";
		try {
			if (httpClient == null) {
				initPools();
			}
			method = getRequest(url, HttpGet.METHOD_NAME,headMap);
			HttpContext context = HttpClientContext.create();
			CloseableHttpResponse httpResponse = httpClient.execute(method, context);
			httpEntity = httpResponse.getEntity();
			if (httpEntity != null) {
				responseBody = EntityUtils.toString(httpEntity, "UTF-8");
			}
		} catch (Exception e) {
			if(method != null){
				method.abort();
			}
			throw e;
		} finally {
			if (httpEntity != null) {
				try {
					EntityUtils.consumeQuietly(httpEntity);
				} catch (Exception e) {				
					throw e;
				}
			}
		}
		return responseBody;
	}

	/**
	 * 执行http post请求 默认采用Content-Type：application/json，Accept：application/json
	 *
	 * @param uri 请求地址
	 * @param data  请求数据
	 * @param data  请求头
	 * @return
	 */
	public static String post(String url, String data,Map<String, String> headMap) throws Exception{
		HttpEntity httpEntity = null;
		HttpEntityEnclosingRequestBase method = null;
		String responseBody = "";
		try {
			if (httpClient == null) {
				initPools();
			}
			method = (HttpEntityEnclosingRequestBase) getRequest(url, HttpPost.METHOD_NAME,headMap);
			method.setEntity(new StringEntity(data,Charset.forName("UTF-8")));
			HttpContext context = HttpClientContext.create();
			CloseableHttpResponse httpResponse = httpClient.execute(method, context);
			httpEntity = httpResponse.getEntity();
			if (httpEntity != null) {
				responseBody = EntityUtils.toString(httpEntity, "UTF-8");
			}

		} catch (Exception e) {
			if(method != null){
				method.abort();
			}
			throw e;
		} finally {
			if (httpEntity != null) {
				try {
					EntityUtils.consumeQuietly(httpEntity);
				} catch (Exception e) {
					throw e;
				}
			}
		}
		return responseBody;
	}
	
	/**
	 * 执行http put请求 默认采用Content-Type：application/json，Accept：application/json
	 *
	 * @param uri 请求地址
	 * @param data  请求数据
	 * @param data  请求头
	 * @return
	 */
	public static String put(String url, String data,Map<String, String> headMap) throws Exception{
		HttpEntity httpEntity = null;
		HttpEntityEnclosingRequestBase method = null;
		String responseBody = "";
		try {
			if (httpClient == null) {
				initPools();
			}
			method = (HttpEntityEnclosingRequestBase) getRequest(url, HttpPut.METHOD_NAME,headMap);
			method.setEntity(new StringEntity(data,Charset.forName("UTF-8")));
			HttpContext context = HttpClientContext.create();
			CloseableHttpResponse httpResponse = httpClient.execute(method, context);
			httpEntity = httpResponse.getEntity();
			if (httpEntity != null) {
				responseBody = EntityUtils.toString(httpEntity, "UTF-8");
			}

		} catch (Exception e) {
			if(method != null){
				method.abort();
			}			
			throw e;
		} finally {
			if (httpEntity != null) {
				try {
					EntityUtils.consumeQuietly(httpEntity);
				} catch (Exception e) {					
					throw e;
				}
			}
		}
		return responseBody;
	}
		
	/**
	 * 执行DELETE 请求
	 *
	 * @param uri
	 * @return
	 */
	public static String delete(String url,Map<String, String> headMap) throws Exception{
		HttpEntity httpEntity = null;
		HttpRequestBase method = null;
		String responseBody = "";
		try {
			if (httpClient == null) {
				initPools();
			}
			method = getRequest(url, HttpDelete.METHOD_NAME,headMap);
			HttpContext context = HttpClientContext.create();
			CloseableHttpResponse httpResponse = httpClient.execute(method, context);
			httpEntity = httpResponse.getEntity();
			if (httpEntity != null) {
				responseBody = EntityUtils.toString(httpEntity, "UTF-8");
			}
		} catch (Exception e) {
			if(method != null){
				method.abort();
			}			
			throw e;
		} finally {
			if (httpEntity != null) {
				try {
					EntityUtils.consumeQuietly(httpEntity);
				} catch (Exception e) {					
					throw e;
				}
			}
		}
		return responseBody;
	}

}
