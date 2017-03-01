using log4net;
using Lumos.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace AnXinSdk
{

    public class AnXin
    {
        static ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static IdentityCardVO GetIdentityCardByImageBase64(string imageBase64)
        {
            IdentityCardVO model = new IdentityCardVO();

            try
            {
                imageBase64 = HttpUtility.UrlEncode(imageBase64, Encoding.UTF8);

                //log.Info("安心接口imageBase64:" + imageBase64);
                string xml = GetCertInfoByImageBase64("identityCard", imageBase64);
                //log.Info("安心接口:" + xml);
                StringBuilder sb = new StringBuilder();

                //sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                //sb.Append("<CertificateIdentificationSystemResponse>");
                //sb.Append("<ResponseHead>");
                //sb.Append("<responseCode>0</responseCode>");
                //sb.Append("<requestType>01</requestType>");
                //sb.Append("<errorCode></errorCode>");
                //sb.Append("<errorMessage></errorMessage>");
                //sb.Append("<esbCode>00</esbCode>");
                //sb.Append("<esbMessage>成功</esbMessage>");
                //sb.Append("<signData></signData>");
                //sb.Append("</ResponseHead>");
                //sb.Append("<CertificateIdentificationSystemResponseMain>");
                //sb.Append("<!-- 证件信息  -->");
                //sb.Append("<LicenceInfoVO>");
                //sb.Append("<!-- 识别类型 -->");
                //sb.Append("<type>identityCard</type>");
                //sb.Append("<!-- 错误信息 -->");
                //sb.Append("<errorMessage></errorMessage>");
                //sb.Append("<IdentityCardVO>");
                //sb.Append("<!-- 住址 -->");
                //sb.Append("<address>湖北省洪湖市曹市镇天井村3-27</address>");
                //sb.Append("<!-- 出生日期-->");
                //sb.Append("<birthday>1985年9月8日</birthday>");
                //sb.Append("<!-- 身份证号 -->");
                //sb.Append("<idNumber>421083198509081218</idNumber>");
                //sb.Append("<!-- 姓名 -->");
                //sb.Append("<name>李佳</name>");
                //sb.Append("<!-- 民族 -->");
                //sb.Append("<people>汉</people>");
                //sb.Append("<!-- 性别 -->");
                //sb.Append("<sex>男</sex>");
                //sb.Append("<!-- 证件类型 -->");
                //sb.Append("<type>第二代身份证</type>");
                //sb.Append("<!-- 发证机关 -->");
                //sb.Append("<issueAuthority></issueAuthority>");
                //sb.Append("<!-- 有效期 -->");
                //sb.Append("<validity></validity>");
                //sb.Append("</IdentityCardVO>");
                //sb.Append("</LicenceInfoVO>");
                //sb.Append("</CertificateIdentificationSystemResponseMain>");
                //sb.Append("</CertificateIdentificationSystemResponse>");


                //string xml = sb.ToString();


                IdentityCardSystemResponse response = XmlUtils.Deserialize(typeof(IdentityCardSystemResponse), xml) as IdentityCardSystemResponse;

                log.Info(Newtonsoft.Json.JsonConvert.SerializeObject(response));

                if (response == null)
                    return model;
                if (response.CertificateIdentificationSystemResponseMain == null)
                    return model;
                if (response.CertificateIdentificationSystemResponseMain.LicenceInfoVO == null)
                    return model;

                //  Console.Write(string.Format("名字:{0},年龄:{1}", stu2.Name, stu2.Age));
                //log.Info("姓名" + model.name);

                if (response.CertificateIdentificationSystemResponseMain.LicenceInfoVO.IdentityCardVO == null)
                    return model;

                model = response.CertificateIdentificationSystemResponseMain.LicenceInfoVO.IdentityCardVO;

                return model;
            }
            catch (Exception ex)
            {
                log.Error("调用安心接口报错", ex);
                return model;
            }




        }


        public static DrivingLicenceVO GetDrivingLicenceByImageBase64(string imageBase64)
        {
            DrivingLicenceVO model = new DrivingLicenceVO();

            try
            {
                imageBase64 = HttpUtility.UrlEncode(imageBase64, Encoding.UTF8);

                string xml = GetCertInfoByImageBase64("drivingLicence", imageBase64);
                log.Info("安心接口:" + xml);
                StringBuilder sb = new StringBuilder();

                //sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                //sb.Append("<CertificateIdentificationSystemResponse>");
                //sb.Append("<ResponseHead>");
                //sb.Append("<responseCode>0</responseCode>");
                //sb.Append("<requestType>01</requestType>");
                //sb.Append("<errorCode></errorCode>");
                //sb.Append("<errorMessage></errorMessage>");
                //sb.Append("<esbCode>00</esbCode>");
                //sb.Append("<esbMessage>成功</esbMessage>");
                //sb.Append("<signData></signData>");
                //sb.Append("</ResponseHead>");
                //sb.Append("<CertificateIdentificationSystemResponseMain>");
                //sb.Append("<!-- 证件信息  -->");
                //sb.Append("<LicenceInfoVO>");
                //sb.Append("<!-- 识别类型 -->");
                //sb.Append("<type>identityCard</type>");
                //sb.Append("<!-- 错误信息 -->");
                //sb.Append("<errorMessage></errorMessage>");
                //sb.Append("<IdentityCardVO>");
                //sb.Append("<!-- 住址 -->");
                //sb.Append("<address>湖北省洪湖市曹市镇天井村3-27</address>");
                //sb.Append("<!-- 出生日期-->");
                //sb.Append("<birthday>1985年9月8日</birthday>");
                //sb.Append("<!-- 身份证号 -->");
                //sb.Append("<idNumber>421083198509081218</idNumber>");
                //sb.Append("<!-- 姓名 -->");
                //sb.Append("<name>李佳</name>");
                //sb.Append("<!-- 民族 -->");
                //sb.Append("<people>汉</people>");
                //sb.Append("<!-- 性别 -->");
                //sb.Append("<sex>男</sex>");
                //sb.Append("<!-- 证件类型 -->");
                //sb.Append("<type>第二代身份证</type>");
                //sb.Append("<!-- 发证机关 -->");
                //sb.Append("<issueAuthority></issueAuthority>");
                //sb.Append("<!-- 有效期 -->");
                //sb.Append("<validity></validity>");
                //sb.Append("</IdentityCardVO>");
                //sb.Append("</LicenceInfoVO>");
                //sb.Append("</CertificateIdentificationSystemResponseMain>");
                //sb.Append("</CertificateIdentificationSystemResponse>");


                //   string xml = sb.ToString();


                DrivingLicenceSystemResponse response = XmlUtils.Deserialize(typeof(DrivingLicenceSystemResponse), xml) as DrivingLicenceSystemResponse;

                log.Info(Newtonsoft.Json.JsonConvert.SerializeObject(response));

                if (response == null)
                    return model;
                if (response.CertificateIdentificationSystemResponseMain == null)
                    return model;
                if (response.CertificateIdentificationSystemResponseMain.LicenceInfoVO == null)
                    return model;


                if (response.CertificateIdentificationSystemResponseMain.LicenceInfoVO.DrivingLicenceVO == null)
                    return model;

                model = response.CertificateIdentificationSystemResponseMain.LicenceInfoVO.DrivingLicenceVO;

                return model;
            }
            catch (Exception ex)
            {
                log.Error("调用安心接口报错", ex);
                return model;
            }


        }

        //public static void GetDrivingLicenceByImageBase64(string imageBase64)
        //{
        //    GetCertInfoByImageBase64("drivingLicence", imageBase64);
        //}


        /// <summary>
        /// 证件识别（安心接口）
        /// </summary>
        /// <param name="cImgTyp">证件类型：identityCard身份证;drivingLicence驾驶证;bankcard银行卡</param>
        /// <param name="imagePath">图像路径</param>
        /// <returns></returns>
        public static string GetCertInfoByImageBase64(string cImgTyp, string imageBase64)
        {
            ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

            string result = "";
            try
            {
                ServicePointManager.DefaultConnectionLimit = 120000;
                CookieContainer cookieContainer = new CookieContainer();
                HttpWebRequest request = null;
                HttpWebResponse SendSMSResponse = null;
                Stream dataStream = null;
                StreamReader SendSMSResponseStream = null;
                request = WebRequest.Create("https://antx11.answern.com/carchannel?comid=AEC16110001") as HttpWebRequest;
                request.Method = "POST";
                request.KeepAlive = false;
                request.ServicePoint.ConnectionLimit = 120000;
                request.AllowAutoRedirect = true;
                request.Timeout = 120000;
                request.ReadWriteTimeout = 120000;
                request.ContentType = "text/xml;charset=UTF-8";
                request.Accept = "application/xml";
                request.Headers.Add("X-Auth-Token", HttpUtility.UrlEncode("openstack"));
                request.Proxy = null;
                request.CookieContainer = cookieContainer;
                byte[] bytes = CreateRequestBytesByImageBase64(cImgTyp, imageBase64);//生成加密报文
                request.ContentLength = bytes.Length;
                using (dataStream = request.GetRequestStream())
                {
                    dataStream.Write(bytes, 0, bytes.Length);
                }
                SendSMSResponse = (HttpWebResponse)request.GetResponse();
                if (SendSMSResponse.StatusCode == HttpStatusCode.RequestTimeout)
                {
                    if (SendSMSResponse != null)
                    {
                        SendSMSResponse.Close();
                        SendSMSResponse = null;
                    }
                    if (request != null)
                    {
                        request.Abort();
                    }
                    return null;
                }
                SendSMSResponseStream = new StreamReader(SendSMSResponse.GetResponseStream(), Encoding.GetEncoding("utf-8"));
                string strRespone = SendSMSResponseStream.ReadToEnd();
                //log.InfoFormat("图片识别返回结果:{0}", strRespone);
                result = AESDecrypt(strRespone, "123456");//解密XML
                log.InfoFormat("图片识别返回解密结果:{0}", result);
            }
            catch (Exception ex)
            {
                log.Error("调用安心接口报错", ex);
            }

            return result;

        }

        public static string GetCertInfoByImagePath(string cImgTyp, string imagePath)
        {

            string base64Image = ImgToBase64String(imagePath);

            return GetCertInfoByImageBase64(cImgTyp, base64Image);



        }


        /// <summary>
        /// 生成图文识别加密报文
        /// </summary>
        /// <param name="cImgTyp">证件类型：identityCard身份证;drivingLicence驾驶证;bankcard银行卡</param>
        /// <param name="imagePath">图像路径</param>
        /// <returns></returns>
        public static byte[] CreateRequestBytesByImageBase64(string cImgTyp, string imageBase64)
        {
            //加载xml模版文件
            XmlDocument xmlDoc = new XmlDocument();
            string xmlFilePath = System.Web.HttpContext.Current.Server.MapPath("~/xmlModel/图文识别.xml");//读模版
            xmlDoc.Load(xmlFilePath);
            //读取图片


            string base64Image = imageBase64;

            //修改图片内容
            XmlNode imageContentNode = xmlDoc.SelectSingleNode("/CertificateIdentificationSystemRequest/CertificateIdentificationSystemRequestMain/ImageVO/imageContent");
            if (imageContentNode != null)
            {
                imageContentNode.InnerText = base64Image;
            }
            //修改改证件类型
            XmlNode cImgTypNode = xmlDoc.SelectSingleNode("/CertificateIdentificationSystemRequest/CertificateIdentificationSystemRequestMain/ImageVO/cImgTyp");
            if (cImgTypNode != null)
            {
                cImgTypNode.InnerText = cImgTyp;
            }
            //对报文加密
            string requestXmlStr = AESEncrypt(xmlDoc.InnerXml, "123456");
            return Encoding.UTF8.GetBytes(requestXmlStr);
        }


        public static byte[] CreateRequestBytesByImagePath(string cImgTyp, string imagePath)
        {

            string base64Image = ImgToBase64String(imagePath);

            return CreateRequestBytesByImageBase64(cImgTyp, base64Image);

        }

        /// <summary>
        /// 读取图片文件并转换为Base64
        /// </summary>
        /// <param name="Imagefilename">图片文件路径</param>
        /// <returns></returns>
        public static string ImgToBase64String(string Imagefilename)
        {
            try
            {
                Bitmap bmp = new Bitmap(Imagefilename);
                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                return HttpUtility.UrlEncode(Convert.ToBase64String(arr), Encoding.UTF8);
            }
            catch (Exception ex)
            {
                return null;
            }
        }



        /// <summary>  
        /// AES加密(无向量)  
        /// </summary>  
        /// <param name="plainBytes">被加密的明文</param>  
        /// <param name="key">密钥</param>  
        /// <returns>密文</returns>  
        public static string AESEncrypt(String Data, String Key)
        {
            MemoryStream mStream = new MemoryStream();
            RijndaelManaged aes = new RijndaelManaged();
            byte[] plainBytes = Encoding.UTF8.GetBytes(Data);
            Byte[] bKey = new Byte[16];//32
            Array.Copy(MD5Encrypt(Key), bKey, bKey.Length);//对密钥进行MD5加密（长度为16字节)
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;
            aes.KeySize = 128;
            aes.Key = bKey;
            CryptoStream cryptoStream = new CryptoStream(mStream, aes.CreateEncryptor(), CryptoStreamMode.Write);
            try
            {
                cryptoStream.Write(plainBytes, 0, plainBytes.Length);
                cryptoStream.FlushFinalBlock();
                return Convert.ToBase64String(mStream.ToArray());
            }
            finally
            {
                cryptoStream.Close();
                mStream.Close();
                aes.Clear();
            }
        }

        /// <summary>  
        /// AES解密(无向量)  
        /// </summary>  
        /// <param name="encryptedBytes">被加密的明文</param>  
        /// <param name="key">密钥</param>  
        /// <returns>明文</returns>  
        public static string AESDecrypt(String Data, String Key)
        {
            Byte[] encryptedBytes = Convert.FromBase64String(Data);
            Byte[] bKey = new Byte[16];
            Array.Copy(MD5Encrypt(Key), bKey, bKey.Length);//对密钥进行MD5加密（长度为16字节)
            MemoryStream mStream = new MemoryStream(encryptedBytes);
            RijndaelManaged aes = new RijndaelManaged();
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;
            aes.KeySize = 128;
            aes.Key = bKey;
            CryptoStream cryptoStream = new CryptoStream(mStream, aes.CreateDecryptor(), CryptoStreamMode.Read);
            try
            {
                byte[] tmp = new byte[encryptedBytes.Length + Key.Length];

                int len = cryptoStream.Read(tmp, 0, encryptedBytes.Length + Key.Length);
                byte[] ret = new byte[len];
                Array.Copy(tmp, 0, ret, 0, len);
                return Encoding.UTF8.GetString(ret);
            }
            finally
            {
                cryptoStream.Close();
                mStream.Close();
                aes.Clear();
            }
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="data"></param>
        /// <returns>16字节</returns>
        public static byte[] MD5Encrypt(string data)
        {
            byte[] temp = Encoding.UTF8.GetBytes(data);    //tbPass为输入密码的文本框
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(temp);
            return result;
        }

    }
}
