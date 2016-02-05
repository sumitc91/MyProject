using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Facebook;

namespace urNotice.Services.SocialAuthService.Facebook
{
    public class FacebookService
    {
        public string getFacebookAuthToken(string returnUrl, string scope, string code, string app_id, string app_secret)
        {

            Dictionary<string, string> tokens = new Dictionary<string, string>();
            string url = string.Format(
                "https://graph.facebook.com/oauth/access_token?client_id={0}&redirect_uri={1}&scope={2}&code={3}&client_secret={4}",
                app_id, returnUrl, scope, code, app_secret);

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {

                StreamReader reader = new StreamReader(response.GetResponseStream());

                string vals = reader.ReadToEnd();

                foreach (string token in vals.Split('&'))
                {
                    tokens.Add(token.Substring(0, token.IndexOf("=")),
                        token.Substring(token.IndexOf("=") + 1, token.Length - token.IndexOf("=") - 1));
                }

            }

            string access_token = tokens["access_token"];
            return access_token;
        }

        public static string GetPictureUrl(string faceBookId)
        {
            WebResponse response = null;
            string pictureUrl = string.Empty;
            try
            {
                WebRequest request = WebRequest.Create(string.Format("http://graph.facebook.com/{0}/picture?type=large", faceBookId));
                response = request.GetResponse();
                pictureUrl = response.ResponseUri.ToString();
            }
            catch (Exception ex)
            {
                //? handle
            }
            finally
            {
                if (response != null) response.Close();
            }
            return pictureUrl;
        }


        internal static void GetUserFriendList(string access_token)
        {

            //string myAccessToken = "something";
            FacebookClient client = new FacebookClient(access_token);

            //var friendListData = client.Get("me?access_token=" + access_token + "&debug=all&fields=id%2Cname%2Cfriends{gender%2Ccover%2Cemail%2Cpicture{url}}&format=json&method=get&pretty=0&suppress_http_code=1");
            //JObject friendListJson = JObject.Parse(friendListData.ToString());

            //var model=null;
            String url = "https://graph.facebook.com/v2.5/me?access_token=" + access_token + "&fields=id%2Cname%2Cfriends&format=json&method=get&pretty=0&suppress_http_code=1";
            var client2 = new HttpClient();
            String response2 = "";
            var task = client2.GetAsync(url)
              .ContinueWith((taskwithresponse) =>
              {
                  var response = taskwithresponse.Result;
                  var jsonString = response.Content.ReadAsStringAsync();
                  jsonString.Wait();
                  //model = JsonConvert.DeserializeObject<GeoCodeResponse>(jsonString.Result);
                  response2 = jsonString.Result;
              });
            task.Wait();

            /////////////////////////
            /// 
            /// 

        }
    }
}
