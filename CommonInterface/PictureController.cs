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
using Newtonsoft.Json;

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
        public async Task<KindResult> UploadByStream(string sName)
        {
            KindResult res = new KindResult();
            try
            {
                Stream PictureStream = await Request.Content.ReadAsStreamAsync();
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
                img.Dispose();

                res.error =0;
                res.url = sFileName;      
            }
            catch (Exception e)
            {
                res.message ="服务器出错:"+e.Message.ToString();
            }
            return res;
        }


        /// <summary>
        /// 以表单的方式上传图片
        /// </summary>
        /// <param name="sName"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpResponseMessage> UploadByFrom(string dir)
        {
            KindResult res = new KindResult();
            try
            {
                var provider = new MultipartFormDataStreamProvider("D:\\Pictures\\Test");      
                await Request.Content.ReadAsMultipartAsync(provider);
                List<string> list = new List<string>();
                foreach (MultipartFileData file in provider.FileData)
                {
                    //可以上传的文件类型
                    string[] sExtension = { ".gif", ".jpg", ".jpeg", ".png", ".bmp" };
                    //获取上传的文件名
                    string FileName = JsonConvert.DeserializeObject(file.Headers.ContentDisposition.FileName).ToString();
                           //获取上传文件的后缀
                    string format = System.IO.Path.GetExtension(FileName);

                    if (!sExtension.Contains(format))
                    {/*上传的文件后缀名格式错误*/
                        res.error = 1;
                        res.message = "上传的文件格式错误!";
                        DeletePicture(file.LocalFileName);//删除该图片
                    }
                    if (file.Headers.ContentDisposition.Size > 2 * 1024 * 1024)
                    {//文件大小超过限制2M
                        res.error = 1;
                        res.message = "上传的文件大小超过限制2M!";
                        DeletePicture(file.LocalFileName);//删除该图片
                    }

                        //1.获取文件流
                    FileStream fs = new FileStream(file.LocalFileName, FileMode.Open, FileAccess.Read);
                        //2.流转图片
                    Image img = Bitmap.FromStream(fs);
                        //3.组装新的文件名
                    string sNewFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".jpg";//文件名
                        //4.保存图片
                    img.Save("D:\\Pictures\\Test\\"+ sNewFileName, ImageFormat.Jpeg);
                        //5.关闭文件流
                    fs.Close();
                        //6.释放资源
                    img.Dispose();
                        //7.删除原来的图片
                    DeletePicture(file.LocalFileName);

                    res.error = 0;
                    res.url ="https://ss0.bdstatic.com/70cFvHSh_Q1YnxGkpoWK1HF6hhy/it/u=2290757533,426974567&fm=116&gp=0.jpg";
                }
            }
            catch (Exception e)
            {
                res.error = 1;
                res.message = e.Message.ToString();
            }
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, "value");
            response.Content = new StringContent(JsonConvert.SerializeObject(res), Encoding.Unicode);
            return  response;
        }

        /// <summary>
        /// 删除本地图片
        /// </summary>
        /// <param name="LocalFileName">本地图片路径和文件名</param>
        private void DeletePicture(string LocalFileName)
        {
            System.IO.File.Delete(LocalFileName);
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

        /// <summary>
        ///  KindEditor专用的返回结果
        /// </summary>
        public class KindResult
        {
            /// <summary>
            /// 0 -成功,1-失败
            /// </summary>
            public int error;

            /// <summary>
            /// 返回图片路径
            /// </summary>
            public string url;

            /// <summary>
            /// 消息提示
            /// </summary>
            public string message;
        }
    }
}
