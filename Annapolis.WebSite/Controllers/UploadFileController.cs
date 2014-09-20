using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Web;
using Annapolis.Abstract.Work;
using Annapolis.Manager;
using Annapolis.Shared.Model;
using Annapolis.WebSite.App;
using Annapolis.WebSite.Application.Base;
using Newtonsoft.Json;

namespace Annapolis.WebSite.Controllers
{
    public class UploadFileController : BaseSiteMvcController
    {

        private static int HashDirectoryLevel = 4;
        private static readonly string ImageFileExtensionKey = "image";
        private static readonly string DocumentFileExtensionKey = "document";
        private static Dictionary<string, HashSet<string>> FileExtensions;
        private readonly IPermissionWork _permissionWork;
        private readonly Dictionary<string, string> _localResources;

        public UploadFileController(IPermissionWork PermissionWork)
        {
            _permissionWork = PermissionWork;
            _localResources = WebSiteConfig.LocaleResources;
        }

        private void LoadFileExtensions()
        {
            if (FileExtensions == null)
            {
                FileExtensions = new Dictionary<string, HashSet<string>>();

                string[] extensions = DefaultSetting.UploadImageFileExtension.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var hashSet = new HashSet<string>();
                foreach (var extension in extensions)
                {
                    hashSet.Add("." + extension);
                }
                FileExtensions.Add(ImageFileExtensionKey, hashSet);

                extensions = DefaultSetting.UploadDocumentFileExtension.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                hashSet = new HashSet<string>();
                foreach (var extension in extensions)
                {
                    hashSet.Add("." + extension);
                }
                FileExtensions.Add(DocumentFileExtensionKey, hashSet);

            }
        }

        private string GetFileDirectory(TokenUser user, string suffix = null)
        {

            char[] hashChars = user.UserId.ToString().Substring(0, HashDirectoryLevel).ToCharArray();
            StringBuilder sb = new StringBuilder();
            sb.Append(DefaultSetting.UploadFileRootPath);
            foreach (var c in hashChars)
            {
                sb.Append(c).Append("/");
            }
            sb.Append(user.UserName).Append("/");
            if (!string.IsNullOrWhiteSpace(suffix))
            {
                sb.Append(suffix);
            }
            return sb.ToString();
        }

        private string GetFilePath(HttpPostedFileBase uploadFile, out string serverDir, out string fileName, string targetExtension = null, string dirSuffix = null)
        {
            string webDir = GetFileDirectory(SecurityManager.CurrentUser, dirSuffix);
            string physicalDir = Server.MapPath(webDir);
            if (!Directory.Exists(physicalDir))
            {
                Directory.CreateDirectory(physicalDir);
            }

            string fname = null;
            if (targetExtension == null)
            {
                fname = string.Format("{0}_{1}", Path.GetFileNameWithoutExtension(Path.GetRandomFileName()), Path.GetFileName(uploadFile.FileName));
            }
            else
            {
                fname = string.Format("{0}_{1}{2}", Path.GetFileNameWithoutExtension(Path.GetRandomFileName()), Path.GetFileNameWithoutExtension(uploadFile.FileName), targetExtension);
            }
            serverDir = webDir;
            fileName = fname;
            return Path.Combine(physicalDir, fname);
        }

        private Image CompressImage(HttpPostedFileBase uploadFile)
        {
            using (Image originalImage = Image.FromStream(uploadFile.InputStream))
            {

                Image targetImage = originalImage;
                if (originalImage.Size.Width > DefaultSetting.UploadImageFileMaxWidth || originalImage.Size.Height > DefaultSetting.UploadImageFileMaxHeight)
                {
                    int targetWidth; 
                    int targetHeight;  

                    if ((double)originalImage.Size.Width / (double)DefaultSetting.UploadImageFileMaxWidth < (double)originalImage.Size.Height / (double)DefaultSetting.UploadImageFileMaxHeight)
                    {
                        targetHeight = DefaultSetting.UploadImageFileMaxHeight;
                        targetWidth = (int)(originalImage.Size.Width * ((double)targetHeight / (double)originalImage.Size.Height));
                    }
                    else
                    {
                        targetWidth = DefaultSetting.UploadImageFileMaxWidth;
                        targetHeight = (int)(originalImage.Size.Height * ((double)targetWidth / (double)originalImage.Size.Width));
                    }

                    targetImage = new System.Drawing.Bitmap(targetWidth, targetHeight);
                    var graphic = Graphics.FromImage(targetImage);
                    graphic.CompositingQuality = CompositingQuality.Default;
                    graphic.SmoothingMode = SmoothingMode.Default;
                    graphic.InterpolationMode = InterpolationMode.Default;
                    graphic.Clear(Color.Transparent);
                    graphic.DrawImage(originalImage, 0, 0, targetWidth, targetHeight);

                }

                return targetImage;
            }

        }

        private Image CompressThumbnail(HttpPostedFileBase uploadFile)
        {
          
            using (Image originalImage = Image.FromStream(uploadFile.InputStream))
            {
                Bitmap targetImage;

                int targetWidth = DefaultSetting.UploadThumbnailWidth;
                int targetHeight = DefaultSetting.UploadThumbnailHeight;

                targetImage = new System.Drawing.Bitmap(targetWidth, targetHeight);
                var graphic = Graphics.FromImage(targetImage);
                graphic.CompositingQuality = CompositingQuality.Default;
                graphic.SmoothingMode = SmoothingMode.Default;
                graphic.InterpolationMode = InterpolationMode.Default;
                graphic.Clear(Color.Transparent);
                graphic.DrawImage(originalImage, 0, 0, targetWidth, targetHeight);
                FillPngWhite(targetImage);

                return targetImage;
            }
        }

        private void FillPngWhite(Bitmap bmp)
        {
            if (bmp.PixelFormat != PixelFormat.Format32bppArgb) return;
               
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat);

            IntPtr ptr = bmpData.Scan0;

            int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
            byte[] rgbaValues = new byte[bytes];

           
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbaValues, 0, bytes);

        
            for (int counter = 0; counter < rgbaValues.Length; counter += 4)
            {
                double t = rgbaValues[counter + 3] / 255.0; 
                double rt = 1 - t; 

                rgbaValues[counter] = (byte)(rgbaValues[counter] * t + 255 * rt); // R color
                rgbaValues[counter + 1] = (byte)(rgbaValues[counter + 1] * t + 255 * rt); // G color
                rgbaValues[counter + 2] = (byte)(rgbaValues[counter + 2] * t + 255 * rt); // B color

                rgbaValues[counter + 3] = 255; 
            }
           
            System.Runtime.InteropServices.Marshal.Copy(rgbaValues, 0, ptr, bytes);

            bmp.UnlockBits(bmpData);
        }

        private void ReportError(string errorMessage)
        {
            Hashtable hash = new Hashtable();
            hash["error"] = 1;
            hash["message"] = errorMessage;
            Response.Write(JsonConvert.SerializeObject(hash));
            Response.End();
        }

        private void CheckLoginUser()
        {
            if (!SecurityManager.IsCurrentUserValid())
            {
                ReportError(_localResources["Membership.OperationStatus.NotValidUser"]);
            }
        }

        private void CheckUploadFile()
        {
            if (Request.Files.Count == 0 || Request.Files[0] == null)
            {
                ReportError(_localResources["Upload.GenericError"]);
            }
        }

        private Guid CheckThreadId(string threadId)
        {
            Guid id;
            if (!Guid.TryParse(threadId, out id))
            {
                ReportError(_localResources["Thread.InvalidThread"]);
            }
            return id;
        }

        private string CheckFileExtension()
        {
            LoadFileExtensions();
            var uploadFile = Request.Files[0]; 
            string extension = Path.GetExtension(uploadFile.FileName);
            if (string.IsNullOrWhiteSpace(extension) ||
                (!FileExtensions[ImageFileExtensionKey].Contains(extension) && !FileExtensions[DocumentFileExtensionKey].Contains(extension)))
            {
                ReportError(_localResources["Upload.InValidFileExtension"]);
            }

            return extension;
        }

        private void UploadDocument()
        {

            var uploadFile = Request.Files[0];

            if (uploadFile.ContentLength > DefaultSetting.UploadDocumentFileMaxByteSize)
            {
                ReportError(_localResources["Upload.DocumentExceedLimit"]);
            }
            string filePath = null;
            string serverDir = null;
            string fileName = null;

            filePath = GetFilePath(uploadFile, out serverDir, out fileName);
            uploadFile.SaveAs(filePath);

            Hashtable hash = new Hashtable();
            hash["error"] = 0;
            if (!string.IsNullOrEmpty(serverDir) && !string.IsNullOrEmpty(fileName))
            {
                hash["url"] = Path.Combine(serverDir, fileName);
                hash["title"] = fileName;
            }
            Response.Write(JsonConvert.SerializeObject(hash));
            Response.End();
        }

        private void UploadImage()
        {
            var uploadFile = Request.Files[0];

            string filePath = null;
            string serverDir = null;
            string fileName = null;

            if (uploadFile.ContentLength > DefaultSetting.UploadImageFileOrginalMaxByteSize)
            {
                ReportError(_localResources["Upload.ImageExceedLimit"]);
            }
            else
            {
                using (var image = CompressImage(uploadFile))
                {
                    filePath = GetFilePath(uploadFile, out serverDir, out fileName, ".jpeg");
                    image.Save(filePath, ImageFormat.Jpeg);
                }
                FileInfo info = new FileInfo(filePath);
                if (info.Length > DefaultSetting.UploadImageFileMaxByteSize)
                {
                    info.Delete();
                    ReportError(_localResources["Upload.ImageExceedLimit"]);
                }
            }

            Hashtable hash = new Hashtable();
            hash["error"] = 0;
            if (!string.IsNullOrEmpty(serverDir) && !string.IsNullOrEmpty(fileName))
            {
                hash["url"] = Path.Combine(serverDir, fileName);
                hash["title"] = fileName;
            }
            Response.Write(JsonConvert.SerializeObject(hash));
            Response.End();

        }

        [System.Web.Http.HttpPost]
        public void TopicUpload(string key)
        {
            try
            {
                Response.AddHeader("Content-Type", "text/html; charset=UTF-8");
                CheckLoginUser();
                CheckUploadFile();
                Guid threadId = CheckThreadId(key);
                var extension = CheckFileExtension();

                #region Upload File

                if (FileExtensions[ImageFileExtensionKey].Contains(extension))
                {
                    if (!_permissionWork.IsPermissionGranted(SecurityManager.CurrentUser.RoleId, PermissionConstants.Topic_Upload_Image, threadId))
                    {
                        ReportError(_localResources["Upload.NoPermissionToUploadImage"]);
                    }
                    else
                    {
                        UploadImage();
                    }
                }
                else if (FileExtensions[DocumentFileExtensionKey].Contains(extension))
                {
                    if (!_permissionWork.IsPermissionGranted(SecurityManager.CurrentUser.RoleId, PermissionConstants.Topic_Upload_Document, threadId))
                    {
                        ReportError(_localResources["Upload.NoPermissionToUploadDocument"]); 
                    }
                    else
                    {
                        UploadDocument();
                    }
                }
                else
                {
                    ReportError(_localResources["Upload.InValidFileExtension"]);
                }

                #endregion
            }
            catch
            {
                ReportError(_localResources["Upload.GenericError"]);
            }
        }

        [System.Web.Http.HttpPost]
        public void CommentUpload(string key)
        {
            try
            {
                Response.AddHeader("Content-Type", "text/html; charset=UTF-8");
                CheckLoginUser();
                CheckUploadFile();
                Guid threadId = CheckThreadId(key);
                var extension = CheckFileExtension();

                #region Upload File

                if (FileExtensions[ImageFileExtensionKey].Contains(extension))
                {
                    if (!_permissionWork.IsPermissionGranted(SecurityManager.CurrentUser.RoleId, PermissionConstants.Comment_Upload_Image, threadId))
                    {
                       ReportError(_localResources["Upload.NoPermissionToUploadImage"]);
                    }
                    else
                    {
                        UploadImage();
                    }
                }
                else if (FileExtensions[DocumentFileExtensionKey].Contains(extension))
                {
                    if (!_permissionWork.IsPermissionGranted(SecurityManager.CurrentUser.RoleId, PermissionConstants.Comment_Upload_Document, threadId))
                    {
                        ReportError(_localResources["Upload.NoPermissionToUploadDocument"]); 
                    }
                    else
                    {
                        UploadDocument();
                    }
                }
                else
                {
                    ReportError(_localResources["Upload.InValidFileExtension"]);
                }

                #endregion
            }
            catch
            {
                ReportError(_localResources["Upload.GenericError"]);
            }

        }

        [System.Web.Http.HttpPost]
        public void UploadThumbnail()
        {
            try
            {
                Response.AddHeader("Content-Type", "text/html; charset=UTF-8");
                CheckLoginUser();
                CheckUploadFile();
                var extension = CheckFileExtension();
                if (string.IsNullOrWhiteSpace(extension) || !FileExtensions["image"].Contains(extension))
                {
                    ReportError(_localResources["Upload.InValidFileExtension"]);
                }

                #region Upload

                var uploadFile = Request.Files[0];
                if (uploadFile.ContentLength > DefaultSetting.UploadThumbnailFileMaxByteSize)
                {
                    ReportError(_localResources["Upload.ImageExceedLimit"]);
                }
                string filePath = null;
                string serverDir = null;
                string fileName = null;
                using (var image = CompressThumbnail(uploadFile))
                {
                    filePath = GetFilePath(uploadFile, out serverDir, out fileName, ".jpeg", DefaultSetting.UploadThumbnailPath);
                    image.Save(filePath, ImageFormat.Jpeg);
                }
                Hashtable hash = new Hashtable();
                hash["error"] = 0;
                if (!string.IsNullOrEmpty(serverDir) && !string.IsNullOrEmpty(fileName))
                {
                    hash["url"] = Path.Combine(serverDir, fileName);
                    hash["title"] = fileName;
                }
                Response.Write(JsonConvert.SerializeObject(hash));
                Response.End();

                #endregion
            }
            catch
            {
                ReportError(_localResources["Upload.GenericError"]);
            }

        }
    }
}
