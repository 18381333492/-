using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.IO;
using System.Drawing;
using System.Web.Http.Cors;
using System.Drawing.Imaging;
using System.Net.Http;
using System.Net;
using System.Web;

namespace CommonInterface
{
    /// <summary>
    /// 图片的相关接口
    /// </summary>
    [EnableCors(origins: "http://localhost:26673/", headers: "*", methods: "GET,POST")]//跨域的设置
    public class PictureController:ApiController
    {

        /// <summary>
        /// 以流的方式上传图片到服务器
        /// </summary>
        /// <param name="sName">项目名称/或者文件夹名称</param>
        /// <returns></returns>
        [HttpPost]
        public result UploadByStream(string sName)
        {
            result res = new result();
            try
            {
                //Stream PictureStream = HttpContext.Current.Request.InputStream;  // Request.Content.ReadAsStreamAsync().Result;
                Stream PictureStream =  Request.Content.ReadAsStreamAsync().Result;
                //流转图片
                Image img = Bitmap.FromStream(PictureStream);
                string sFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".jpg";//文件名
                sName = string.IsNullOrEmpty(sName) ? "Default\\" : sName + "\\";
                string sPath = "D:\\Pictures\\" + sName;//保存的路径
                if (!Directory.Exists(sPath))
                {
                    Directory.CreateDirectory(sPath);
                }
                img.Save(sPath + sFileName, ImageFormat.Jpeg);
                res.success = true;
                res.data = sFileName;
            }
            catch (Exception e)
            {
                res.info ="服务器出错:"+e.Message.ToString();
            }
            return res;
        }


        /// <summary>
        /// 以表单的方式上传图片
        /// </summary>
        /// <param name="sName"></param>
        /// <returns></returns>
        [HttpPost]
        public result UploadByFrom(string dir)
        {
            result res = new result();
            try
            {
                var provider = new MultipartFormDataStreamProvider("D:\\Pictures\\Test");      
                Request.Content.ReadAsMultipartAsync(provider);
                List<string> list = new List<string>();
                foreach (MultipartFileData file in provider.FileData)
                {
                    //图片文件上传之后可以在处理图片

                    //if (file != null && file.ContentLength > 0)
                    //{
                    //    /*图片保存路径*/
                    //    dir = string.IsNullOrEmpty(dir) ? "Default\\" : dir + "\\";
                    //    string sPath = "D:\\Pictures\\" + dir;
                    //    if (!Directory.Exists(sPath))
                    //    {
                    //        Directory.CreateDirectory(sPath);
                    //    }
                    //    string format = System.IO.Path.GetExtension(file.FileName);//后缀名

                    //    string[] sExtension = { ".gif", ".jpg", ".jpeg", ".png", ".bmp" };
                    //    if (!sExtension.Contains(format))
                    //    {//上传文件的格式错误
                    //        res.info = "上传文件的格式错误!";
                    //        return res;
                    //    }
                    //    if (file.ContentLength > (2 * 1024 * 1024))//2M
                    //    {//上传图片大小超过限制
                    //        res.info = "上传图片大小超过限制(2M)!";
                    //        return res;
                    //    }
                    //    Image img = Bitmap.FromStream(file.InputStream);
                    //    string sFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".jpg";//文件名
                    //    img.Save(sPath + sFileName);
                    //    list.Add(sFileName);
                    //}
                }
                res.success = true;
                res.data = string.Join(",", list.ToArray());
                return res;
            }
            catch (Exception e)
            {
                res.success = false;
                res.info = e.Message.ToString();
            }
            return res;
        }


        /// <summary>
        /// 返回结果集
        /// </summary>
        public class result
        {
            public bool success=false;
            public string info=string.Empty;
            public object data=null;
        }
 
    }
}
