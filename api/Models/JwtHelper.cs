using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

/// <summary>
/// 
/// </summary>
public sealed class JwtHelper
{
    #region Json 助手

    /// <summary>
    /// Json 助手
    /// </summary>
    private sealed class JsonHelper
    {

        /// <summary>
        /// 将对象序列化为 Json 字符串
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>json字符串</returns>
        public static string Serialize(object obj)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            return JsonConvert.SerializeObject(obj, settings);
        }

        /// <summary>
        /// 将对象序列化为 Json 字符串
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>json字符串</returns>
        public static string Serialize(object obj, JsonSerializerSettings settings)
        {
            return JsonConvert.SerializeObject(obj, settings);
        }

        /// <summary>
        /// 解析 Json 字符串，生成T对象实体
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="json">json字符串。例如：{"ID":"1","Name":"2"} </param>
        /// <returns>T对象实体</returns>
        public static T Deserialize<T>(string json) where T : class
        {
            JsonSerializer serializer = new JsonSerializer();
            System.IO.StringReader sr = new System.IO.StringReader(json);
            return serializer.Deserialize<T>(new JsonTextReader(sr));
        }

        /// <summary>
        /// 反序列化JSON到给定的匿名对象.
        /// </summary>
        /// <typeparam name="T">匿名对象类型</typeparam>
        /// <param name="json">json字符串</param>
        /// <param name="anonymousTypeObject">匿名对象</param>
        /// <returns>匿名对象</returns>
        public static T DeserializeAnonymousType<T>(string json, T anonymousTypeObject)
        {
            return JsonConvert.DeserializeAnonymousType(json, anonymousTypeObject);
        }

    }

    #endregion

    #region Base64 编码助手


    /// <summary>
    /// Base64 编码助手
    /// </summary>
    public sealed class Base64Helper
    {
        private static string _ToBase64String(string str)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
        }
        private static string _FromBase64String(string str64)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(str64));
        }
        /// <summary>
        /// Base64 编码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Encode(byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }
        /// <summary>
        /// Base64 编码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Encode(string str)
        {
            return _ToBase64String(str);
        }
        /// <summary>
        /// Base64 解码
        /// </summary>
        /// <param name="str64"></param>
        /// <returns></returns>
        public static string Decode(string str64)
        {
            return _FromBase64String(str64);
        }
        /// <summary>
        /// Base64URL 编码（可用于在网络中安全顺畅传输）
        /// </summary>
        /// <param name="bytes)"></param>
        /// <returns></returns>
        public static string EncodeUrl(byte[] bytes)
        {
            /*
             1、明文使用Base64进行加密 
             2、在Base64的基础上进行以下编码：
               2.1、去除尾部的"="
               2.2、把"+"替换成"-"
               2.3、把"/"替换成"_"
             */
            return Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }
        /// <summary>
        /// Base64URL 编码（可用于在网络中安全顺畅传输）
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string EncodeUrl(string str)
        {
            /*
             1、明文使用Base64进行加密 
             2、在Base64的基础上进行以下编码：
               2.1、去除尾部的"="
               2.2、把"+"替换成"-"
               2.3、把"/"替换成"_"
             */
            return _ToBase64String(str).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }
        /// <summary>
        /// Base64URL 解码
        /// </summary>
        /// <param name="str64"></param>
        /// <returns></returns>
        public static string DecodeUrl(string str64)
        {
            /*
             1、把 Base64URL 的编码做如下解码：
                   1.1、把"-"替换成"+"
                   1.2、把"_"替换成"/"
                   1.3、 result=(计算BASE64URL编码长度)%4 
                      result为0，不做处理 
                      result为2，字符串添加"==" 
                      result为3，字符串添加"="

                  2、使用Base64解码密文，得到原始的明文

             */
            var b64 = str64.Replace('_', '/').Replace('-', '+');
            switch (b64.Length % 4)
            {
                case 2:
                    b64 += "==";
                    break;
                case 3:
                    b64 += "=";
                    break;
            }
            return _FromBase64String(b64);
        }


    }

    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <param name="header"></param>
    /// <param name="payload"></param>
    /// <param name="secret"></param>
    /// <returns></returns>
    private static byte[] HS256(string header, string payload,string secret)
    {
        using (var hasher = new System.Security.Cryptography.HMACSHA256(Encoding.UTF8.GetBytes(secret)))
        {
            var bytes = Encoding.UTF8.GetBytes(string.Format("{0}.{1}", header, payload));
            return hasher.ComputeHash(bytes);
        }
    }




    /// <summary>
    /// 头部
    /// </summary>
    [Serializable]
    private class JwtHeader
    {
        /// <summary>
        /// 声明令牌的类型，JWT令牌默认统一写为JWT
        /// </summary>
        public string typ = "JWT";
        /// <summary>
        /// 声明加密的算法 通常直接使用 HMAC SHA256 默认为HMAC SHA256（写为HS256）
        /// </summary>
        public string alg = "HS256";
    }

    /// <summary>
    /// 
    /// </summary>
    private static string JwtSecret { get { return "jwt"; } }
    
    /// <summary>
    /// 标准负载
    /// </summary>
    [Serializable]
    public class JwtStandardPayload {
        /// <summary>
        /// 签发者
        /// </summary>
        public string iss = null;
        /// <summary>
        /// 所面向的用户
        /// </summary>
        public string sub = null;
        /// <summary>
        /// 接收jwt的一方
        /// </summary>
        public string aud = null;
        /// <summary>
        /// 过期时间，这个过期时间必须要大于签发时间
        /// </summary>
        public DateTime exp;
        /// <summary>
        /// 定义在什么时间之前，该jwt都是不可用的.
        /// </summary>
        public string nbf = null;
        /// <summary>
        /// 签发时间
        /// </summary>
        public DateTime iat;
        /// <summary>
        /// 唯一身份标识，主要用来作为一次性token,从而回避重放攻击。
        /// </summary>
        public string jti = null;

    }

    /// <summary>
    /// 生成 Json Web Token
    /// </summary>
    /// <returns></returns>
    public static string CreateJWT(object payloadObj, string secret = null) {
        /*
         payload 标准中注册的声明 (建议但不强制使用) ：
            iss: jwt签发者
            sub: jwt所面向的用户
            aud: 接收jwt的一方
            exp: jwt的过期时间，这个过期时间必须要大于签发时间
            nbf: 定义在什么时间之前，该jwt都是不可用的.
            iat: jwt的签发时间
            jti: jwt的唯一身份标识，主要用来作为一次性token,从而回避重放攻击。
         */

        string header = Base64Helper.EncodeUrl(JsonHelper.Serialize(new JwtHeader
        {
            typ = "JWT",
            alg = "HS256"
        }));
        string payload = Base64Helper.EncodeUrl(JsonHelper.Serialize(payloadObj));

        string signature = Base64Helper.EncodeUrl(HS256(header, payload, string.IsNullOrEmpty(secret) ? secret : JwtSecret));

        return string.Format(@"{0}.{1}.{2}", header, payload, signature);
    }
    /// <summary>
    /// 生成 Json Web Token
    /// </summary>
    /// <returns></returns>
    public static string CreateJwtDictionary(Dictionary<string, object> payloadDIC)
    {
      return CreateJWT(payloadDIC);
    }

    /// <summary>
    /// 验证 Json Web Token
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public static bool VerifyJWT(string token)
    {
        if (string.IsNullOrEmpty(token)) {
            return false;
        }

        var split = token.Split('.');
        if (split.Length != 3) {
            return false;
        }

        string signature = Base64Helper.EncodeUrl(HS256(split[0], split[1], JwtSecret));
        return signature == split[2];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public static T DecodeJWT<T>(string token, string secret = null) where T:class,new()
    {
        if (string.IsNullOrEmpty(token))
        {
            return default;
        }

        var split = token.Split('.');
        if (split.Length != 3)
        {
            return default;
        }

        string signature = Base64Helper.EncodeUrl(HS256(split[0], split[1],string.IsNullOrEmpty(secret)? secret: JwtSecret));
        return signature == split[2] ? JsonHelper.Deserialize<T>(Base64Helper.DecodeUrl(split[1])) : default;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public static Dictionary<string, object> DecodeJwtDictionary(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return null;
        }

        var split = token.Split('.');
        if (split.Length != 3)
        {
            return null;
        }

        string signature = Base64Helper.EncodeUrl(HS256(split[0], split[1], JwtSecret));
        return signature == split[2] ? JsonHelper.Deserialize<Dictionary<string, object>>(Base64Helper.DecodeUrl(split[1])) : null;
    }





}
