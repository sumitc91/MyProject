using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper;
using urNotice.Services.UploadImageService;
using S3ImageUploadServices = urNotice.Services.ImageService.S3ImageUploadServices;

namespace OrbitPageWorkgraphy.Controllers
{
    public class UploadController : Controller
    {
        public ActionResult UploadMultipleImages(IEnumerable<HttpPostedFileBase> files)
        {
            const string rootfolder = @"~/Upload/Images/";
            const string startingDir = rootfolder; //@"c:\Temp";

            foreach (HttpPostedFileBase file in files)
            {
                string filePath = Path.Combine(startingDir, file.FileName);

                System.IO.File.WriteAllBytes(Server.MapPath(filePath), ReadData(file.InputStream));
            }

            return Json("All files have been successfully stored.");
        }

        [System.Web.Http.HttpPost]
        public ContentResult UploadAngularFile(HttpPostedFileBase file)
        {
            var filename = Path.GetFileName(file.FileName);
            const string rootfolder = @"~/Upload/UserID/";
            const string startingDir = rootfolder; //@"c:\Temp";
            string filePath = Path.Combine(startingDir, file.FileName);

            file.SaveAs(Server.MapPath(filePath));

            return new ContentResult
            {
                ContentType = "text/plain",
                Content = filename,
                ContentEncoding = Encoding.UTF8
            };
        }

        [System.Web.Http.HttpPost]
        public ActionResult UploadAngularFileOnImgUr(HttpPostedFileBase file)
        {
            bool isDummy = false;


            if (isDummy)
            {
                var dummyImgurImage = new ImgurImageResponse();
                dummyImgurImage.data = new imgurData();
                dummyImgurImage.data.link_s = "http://i.imgur.com/FdU2YRFs.jpg";
                dummyImgurImage.data.link = "http://i.imgur.com/FdU2YRF.jpg";
                return Json(dummyImgurImage);
            }

            //const string albumid = "Xlh72LgTBw6Tzs1";
            //var imgurService = new imgurService();
            //var uploadedImagesId = imgurService.UploadSingleImageToImgur(file, albumid);

            var folderName = "OrbitPage/User/Sumit/WallPost";

            var uploadedImagesId = S3ImageUploadServices.UploadSingleImageToImgur(file, folderName);

            return Json(uploadedImagesId);
        }

        [System.Web.Http.HttpPost]
        public ActionResult UploadDropZoneFilesImgUr(IEnumerable<HttpPostedFileBase> files)
        {

            //const string albumid = "Xlh72LgTBw6Tzs1";

            /*var imgurService = new imgurService();
            var uploadedImagesId = imgurService.UploadMultipleImagesToImgur(files, albumid);*/
            var folderName = "urjobgraphy";

            var uploadedImagesId = S3ImageUploadServices.UploadMultipleImagesToImgur(files, folderName);

            return Json(uploadedImagesId);
        }

        public ActionResult CreateImgurAlbum()
        {
            var imgurService = new imgurService();
            return Json(imgurService.CreateImgurAlbum(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetImgurAlbumDetails()
        {
            var albumId = Request.QueryString["albumId"];
            var imgurService = new imgurService();
            return Json(imgurService.GetImgurAlbumDetails(albumId), JsonRequestBehavior.AllowGet);
        }

        private byte[] ReadData(Stream stream)
        {
            var buffer = new byte[16 * 1024];

            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }

                return ms.ToArray();
            }
        }
    }
}
