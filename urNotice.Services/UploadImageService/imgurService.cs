﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Web;
using Newtonsoft.Json;
using urNotice.Common.Infrastructure.Common.Config;
using urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper;

namespace urNotice.Services.UploadImageService
{
    public class imgurService
    {
        public string CreateImgurAlbum()
        {
            byte[] response;
            using (var w = new WebClient())
            {
                var values = new NameValueCollection
                {
                {"ids[]", "5FBgDJI"},
                {"cover", "5FBgDJI"},
                {"layout", "grid"},
                {"title", "Title"},
                {"description", "This is the caption of the image"}
            };

                w.Headers.Add("Authorization", "Client-ID " + ImgurConfig.ImgurClientId);
                response = w.UploadValues("https://api.imgur.com/3/album/{0}", values);                
                
            }
            System.Text.Encoding enc = System.Text.Encoding.ASCII;
            string myString = enc.GetString(response);
            return ("uploaded : " + myString);
        }

        public string GetImgurAlbumDetails(string albumId)
        {

            string myString=string.Empty;
            using (var w = new WebClient())
            {
                var values = new NameValueCollection
                {
                {"id", albumId}
                
            };
                w.Headers.Add("Authorization", "Client-ID " + ImgurConfig.ImgurClientId);
                byte[] response = w.UploadValues("https://api.imgur.com/3/album/{0}", values);
                System.Text.Encoding enc = System.Text.Encoding.ASCII;
                myString = enc.GetString(response);                
            }
            return ("uploaded : " + myString);
        }

        public ImgurImageResponse UploadMultipleImagesToImgur(IEnumerable<HttpPostedFileBase> files, string albumid)
        {    
            System.Text.Encoding enc = System.Text.Encoding.ASCII;
            imgurUploadImageResponse imgurImageResponseData = new imgurUploadImageResponse();
            var imgurImage = new ImgurImageResponse();
            foreach (HttpPostedFileBase file in files)
            {                
                using (var w = new WebClient())
                {
                    byte[] binaryData;
                    binaryData = new Byte[file.InputStream.Length];
                    long bytesRead = file.InputStream.Read(binaryData, 0, (int)file.InputStream.Length);
                    file.InputStream.Close();
                    string base64String = System.Convert.ToBase64String(binaryData, 0, binaryData.Length);

                    var values = new NameValueCollection
                    {
                        {"image",  base64String},
                        {"album", albumid}
                    };

                    w.Headers.Add("Authorization", "Client-ID " + ImgurConfig.ImgurClientId);
                    byte[] response = w.UploadValues("https://api.imgur.com/3/upload", values);
                    
                    imgurImageResponseData = JsonConvert.DeserializeObject<imgurUploadImageResponse>(enc.GetString(response));
                    
                    imgurImage.data = new imgurData();
                    if (imgurImageResponseData == null && imgurImageResponseData.data == null)
                        return imgurImage;
                    imgurImage.data.deletehash = imgurImageResponseData.data.deletehash;
                    imgurImage.data.link = imgurImageResponseData.data.link;
                    imgurImage.data.link_s = imgurImageResponseData.data.link.Split('/')[0] + "//" + imgurImageResponseData.data.link.Split('/')[2] + "/" + imgurImageResponseData.data.link.Split('/')[3].Split('.')[0] + 's' + "." + imgurImageResponseData.data.link.Split('/')[3].Split('.')[1];
                    imgurImage.data.link_m = imgurImageResponseData.data.link.Split('/')[0] + "//" + imgurImageResponseData.data.link.Split('/')[2] + "/" + imgurImageResponseData.data.link.Split('/')[3].Split('.')[0] + 'm' + "." + imgurImageResponseData.data.link.Split('/')[3].Split('.')[1];
                    imgurImage.data.link_l = imgurImageResponseData.data.link.Split('/')[0] + "//" + imgurImageResponseData.data.link.Split('/')[2] + "/" + imgurImageResponseData.data.link.Split('/')[3].Split('.')[0] + 'l' + "." + imgurImageResponseData.data.link.Split('/')[3].Split('.')[1];
                    imgurImage.data.copyText = "";
                }
            }
            return imgurImage;
        }

        public ImgurImageResponse UploadSingleImageToImgur(HttpPostedFileBase file, string albumid)
        {
            System.Text.Encoding enc = System.Text.Encoding.ASCII;
            imgurUploadImageResponse imgurImageResponseData = new imgurUploadImageResponse();
            var imgurImage = new ImgurImageResponse();
                using (var w = new WebClient())
                {
                    byte[] binaryData;
                    binaryData = new Byte[file.InputStream.Length];
                    long bytesRead = file.InputStream.Read(binaryData, 0, (int)file.InputStream.Length);
                    file.InputStream.Close();
                    string base64String = System.Convert.ToBase64String(binaryData, 0, binaryData.Length);

                    var values = new NameValueCollection
                    {
                        {"image",  base64String},
                        {"album", albumid}
                    };

                    w.Headers.Add("Authorization", "Client-ID " + ImgurConfig.ImgurClientId);
                    byte[] response = w.UploadValues("https://api.imgur.com/3/upload", values);

                    imgurImageResponseData = JsonConvert.DeserializeObject<imgurUploadImageResponse>(enc.GetString(response));
                    
                    
                    imgurImage.data = new imgurData();
                    if (imgurImageResponseData == null && imgurImageResponseData.data == null)
                        return imgurImage;
                    imgurImage.data.deletehash = imgurImageResponseData.data.deletehash;
                    imgurImage.data.link = imgurImageResponseData.data.link;
                    imgurImage.data.link_s = imgurImageResponseData.data.link.Split('/')[0] + "//" + imgurImageResponseData.data.link.Split('/')[2] + "/" + imgurImageResponseData.data.link.Split('/')[3].Split('.')[0] + 's' + "." + imgurImageResponseData.data.link.Split('/')[3].Split('.')[1];
                    imgurImage.data.link_m = imgurImageResponseData.data.link.Split('/')[0] + "//" + imgurImageResponseData.data.link.Split('/')[2] + "/" + imgurImageResponseData.data.link.Split('/')[3].Split('.')[0] + 'm' + "." + imgurImageResponseData.data.link.Split('/')[3].Split('.')[1];
                    imgurImage.data.link_l = imgurImageResponseData.data.link.Split('/')[0] + "//" + imgurImageResponseData.data.link.Split('/')[2] + "/" + imgurImageResponseData.data.link.Split('/')[3].Split('.')[0] + 'l' + "." + imgurImageResponseData.data.link.Split('/')[3].Split('.')[1];
                    imgurImage.data.copyText = "";
                }

                return imgurImage;
        }
    }
}