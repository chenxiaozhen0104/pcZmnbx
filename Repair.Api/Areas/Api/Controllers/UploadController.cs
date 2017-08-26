using LZY.BX.Model;
using LZY.BX.Model.Enum;
using LZY.BX.Service.Mb;
using Repair.Api.Areas.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WeiXin;

namespace Repair.Api.Areas.Api.Controllers
{

    public class UploadController : ControllerApiBase
    {
        //
        // GET: /Api/Upload/

        public ActionResult Index()
        {
            return View();
        }
        public static byte[] ReadToEnd(System.IO.Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[10240000];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UploadImg(PhotoType Type)
        {
            try
            {
                HttpFileCollectionBase uploadFile = Request.Files;
                List<Picture> pInfo = new List<Picture>();
                if (uploadFile.Count > 0)
                {
                    for (int i = 0; i < uploadFile.Count; i++)
                    {
                        HttpPostedFileBase file = uploadFile[i];
                        string path = ConfigurationManager.AppSettings["repairImgPath"].ToString() + "/" + Type.ToString() + "/";

                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        string fileName = Guid.NewGuid().ToString("N") + "." + file.ContentType.ToString().Split('/')[1];
                        file.SaveAs(path + "/" + fileName);

                        pInfo.Add(new Picture
                        {
                            Type = Type,
                            Url = Type.ToString() + "/" + fileName,
                            CreateTime = DateTime.Now,
                            Note = file.FileName
                        });
                    }
                }
                else
                {
                    string imgFile = Guid.NewGuid().ToString("N") + ".jpg";
                    string filePath = ConfigurationManager.AppSettings["repairImgPath"].ToString() + "/" + Type.ToString() + "/";
                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }
                    byte[] bytes = Request.BinaryRead(Request.TotalBytes);
                    FileStream fStream = new FileStream(filePath + imgFile, FileMode.Create, FileAccess.Write);
                    BinaryWriter bw = new BinaryWriter(fStream);
                    bw.Write(bytes);
                    bw.Close();
                    fStream.Close();

                    pInfo.Add(new Picture
                    {
                        Type = Type,
                        Url = Type.ToString() + "/" + imgFile,
                        CreateTime = DateTime.Now,
                        Note = imgFile
                    });
                }
                if (pInfo.Count != 0)
                {
                    using (var db = new MbContext())
                    {
                        int i = 0;
                        foreach (Picture item in pInfo)
                        {
                            item.CreateTime = DateTime.Now;
                            item.SortField = i;
                            i++;
                            db.Picture.Add(item);
                            db.SaveChanges();
                        }
                       
                    }
                    return Json(pInfo.Select(x => new { PictureId = x.PictureId, Name = x.Note }).ToArray());
                }
                else
                {
                    return Json(new { error = "图片数量不能0" });
                }

            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        public ActionResult WxUploadImg(string serviceId)
        {

            var token = WeiXinMedia.GetAccessToken();
            var downUrl = WeiXinMedia.DownLoadImgUrl(serviceId, token);
            try
            {

                string imgFile = Guid.NewGuid().ToString("N") + ".jpg";
                string filePath = ConfigurationManager.AppSettings["repairImgPath"].ToString() + "/" + PhotoType.MainOrder.ToString() + "/";
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                var web = new WebClient();
                web.DownloadFile(downUrl, filePath + imgFile);

                using (var db = new MbContext())
                {
                    var pic = new Picture
                     {
                         Type = PhotoType.MainOrder,
                         Url = PhotoType.MainOrder.ToString() + "/" + imgFile,
                         CreateTime = DateTime.Now,
                         Note = imgFile
                     };
                    var reuslt = db.Picture.Add(pic);

                    db.SaveChanges();

                    return Json(new List<object> { new { PictureId = pic.PictureId, Name = pic.Note } });
                }
            }
            catch (Exception e)
            {
                Logger.Error("微信上传图片异常", e);

                return Json(new { error = "服务器繁忙，请稍后再试试" });
            }
        }

        [HttpPost]
        public ActionResult UploadBase64Image(PhotoType Type, string strImage)
        {
            //Logger.InfoFormat("UploadBase64Image:strImage{0}====Type{1}", strImage, Type);
            strImage = strImage.Replace(' ', '+').Substring(strImage.IndexOf(',') + 1);
            strImage = strImage.Trim('\0');
            byte[] arr = Convert.FromBase64String(strImage);
            using (MemoryStream ms = new MemoryStream(arr))
            {
                string path = ConfigurationManager.AppSettings["repairImgPath"].ToString() + "/" + Type.ToString() + "/";

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string imgName = Guid.NewGuid().ToString("N") + ".jpeg";
                Bitmap bmp = new Bitmap(ms);
                bmp.Save(path + imgName);
                List<Picture> pInfo = new List<Picture>();

                pInfo.Add(new Picture
                {
                    Type = Type,
                    Url = Type.ToString() + "/" + imgName,
                    CreateTime = DateTime.Now,
                    Note = imgName
                });

                if (pInfo.Count != 0)
                {
                    using (var db = new MbContext())
                    {
                        int i = 0;
                        foreach (Picture item in pInfo)
                        {
                            item.CreateTime = DateTime.Now;
                            item.SortField = i;
                            i++;
                            db.Picture.Add(item);
                            db.SaveChanges();
                        }
                        
                    }
                    return Json(pInfo.Select(x => new { PictureId = x.PictureId, Name = x.Note }).ToArray());
                }
                else
                {
                    return Json(new { error = "图片数量不能0" });
                }
            }
        }
    }
}
