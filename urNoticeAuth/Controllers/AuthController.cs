using System;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using urNotice.Common.Infrastructure.Common.Constants;
using urNotice.Common.Infrastructure.commonMethods;
using urNotice.Common.Infrastructure.Encryption;
using urNotice.Common.Infrastructure.Model.urNoticeAuthContext;
using urNotice.Common.Infrastructure.Model.urNoticeModel.AssetClass;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;
using urNotice.Common.Infrastructure.Model.urNoticeModel.RequestWrapper;
using urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper;
using urNotice.Common.Infrastructure.Session;
using urNotice.Services.AdminService;
using urNotice.Services.AuthService;
using urNotice.Services.ErrorLogger;
using urNotice.Services.ImageService;
using urNoticeAuth.App_Start;

namespace urNoticeAuth.Controllers
{


    [AllowCrossSiteJson]
    public class AuthController : Controller
    {
        //
        // GET: /Auth/
       
        public delegate void LogoutDelegate(HttpRequestBase requestData);
        //private readonly urnoticeAuthEntities _db = new urnoticeAuthEntities();
        private readonly AuthService _authService = new AuthService();
        private static readonly ILogger log = new Logger(Convert.ToString(MethodBase.GetCurrentMethod().DeclaringType));
        private static string accessKey = ConfigurationManager.AppSettings["AWSAccessKey"];
        private static string secretKey = ConfigurationManager.AppSettings["AWSSecretKey"];

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult CreateAccount(RegisterationRequest req)
        {
            //var returnUrl = "/";
            return Json(_authService.UserRegistration(req, Request, accessKey, secretKey));
        }

        //[HttpPost]
        //public JsonResult Login(LoginRequest req)
        //{
        //    var returnUrl = "/";
        //    //var referral = Request.QueryString["ref"];
        //    //var isMobileFacebookLogin = Request.QueryString["isMobileFacebookLogin"];
        //    var responseData = new LoginResponse();
        //    if (req.Type == "web")
        //    {
        //        responseData = _authService.WebLogin(req.UserName, EncryptionClass.Md5Hash(req.Password), returnUrl, req.KeepMeSignedInCheckBox, accessKey, secretKey);
        //    }

        //    if (responseData.Code == "200")
        //    {
        //        var session = new urNoticeSession(req.UserName, responseData.VertexId);
        //        TokenManager.CreateSession(session);
        //        responseData.UTMZT = session.SessionId;
        //    }
        //    var response = new ResponseModel<LoginResponse> { Status = Convert.ToInt32(responseData.Code), Message = "success", Payload = responseData };
        //    return Json(response);
        //}

        [HttpPost]
        public JsonResult ValidateAccount(ValidateAccountRequest req)
        {
            return Json(new AuthService().ValidateAccountService(req,accessKey,secretKey));
        }

        [HttpPost]
        public JsonResult IsValidSession()
        {
            var headers = new HeaderManager(Request);
            new TokenManager().getSessionInfo(headers.AuthToken, headers);
            var isValidToken = TokenManager.IsValidSession(headers.AuthToken);
            return Json(isValidToken);
        }

        [HttpPost]
        public JsonResult Logout()
        {
            //var isValidToken = ThreadPool.QueueUserWorkItem(new WaitCallback(asyncLogoutThread), Request);
            var logoutServiceDelegate = new LogoutDelegate(AsyncLogoutDelegate);
            logoutServiceDelegate.BeginInvoke(Request, null, null); //invoking the method
            return Json("success");
        }

        public JsonResult ForgetPassword()
        {
            var id = Request.QueryString["id"];
            return Json(new AuthService().ForgetPasswordService(id, Request, accessKey, secretKey), JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateDesignations()
        {
            //var reader = new StreamReader(System.IO.File.OpenRead(@"C:\code\svn\AllDesignations.csv"));
            ////var list = new List<Location>();

            //while (!reader.EndOfStream)
            //{
            //    var designationName = reader.ReadLine();
            //    new AdminService().CreateNewDesignation(designationName, "orbitpage@gmail.com", accessKey, secretKey);
            //}
            
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateCompanyDesignationSalaries()
        {
            var reader = new StreamReader(System.IO.File.OpenRead(@"C:\code\svn\salary.csv"));

            while (!reader.EndOfStream)
            {
                var readLine = reader.ReadLine();
                if (readLine != null)
                {
                    string[] line = readLine.Split(',');
                    //Console.WriteLine(line);
                    new AdminService().CreateNewCompanyDesignationSalary(line[1],line[0],line[2], "orbitpage@gmail.com", accessKey, secretKey);
                }
                //new AdminService().CreateNewCompanyDesignationSalary(designationName, "orbitpage@gmail.com", accessKey, secretKey);
            }

            return Json("success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateCompaniesFromCsv()
        {

            //using (CsvReader reader = new CsvReader(@"C:\code\svn\final.csv"))
            //{
            //    int flag = 0;
            //    foreach (string[] values in reader.RowEnumerator)
            //    {
            //        if (values[0] == "Id") continue;
            //        if (values[1] == "ISGN")
            //        {
            //            flag = 1;
            //        }
            //        if (flag == 0) continue;

            //        string imageUrl = @values[3];
            //        string saveLocation = @Server.MapPath("~/Downloads/") + values[1].Replace(" ","_")+"_image.png";
            //        S3ImageUploadServices.SaveImageOnServer(imageUrl, saveLocation);
            //        //S3ImageUploadServices.UploadSingleImageToS3FromPath(saveLocation,"company","png");

            //        ImgurImageResponse img = S3ImageUploadServices.UploadSingleImageToS3FromPath(saveLocation, "OrbitPageCompanies", values[1]);

            //        OrbitPageCompany newCompany = new OrbitPageCompany
            //        {
            //            CompanyName = values[1].Trim(),
            //            averageRating = (float) Convert.ToDecimal(values[2]),
            //            logoUrl = img.data.link_m,
            //            squareLogoUrl = CommonConstants.CompanySquareLogoNotAvailableImage,
            //            DisplayName = values[5].Trim(),
            //            website = values[6],
            //            workLifeBalanceRating = (float) Convert.ToDecimal(values[7]),
            //            salaryRating = (float) Convert.ToDecimal(values[8]),
            //            companyCultureRating = (float) Convert.ToDecimal(values[9]),
            //            careerGrowthRating = (float) Convert.ToDecimal(values[10]),
            //            specialities = values[11],
            //            founded = values[12],
            //            founder = values[13],
            //            turnover=values[14],
            //            headquarter=values[15],
            //            employees = values[16],
            //            competitors = values[17],
            //            description=values[18]

            //        };
            //        new AdminService().CreateNewCompany(newCompany, "orbitpage@gmail.com",accessKey,secretKey);
            //        Console.WriteLine("Row {0} has {1} values.", reader.RowIndex, values.Length);
            //    }
            //}

            return Json("success", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ContactUs(ContactUsRequest req)
        {

            var contactUsService = new AuthService();
            var response = contactUsService.ContactUsService(req);
            return Json(response);
        }

        public void AsyncLogoutDelegate(HttpRequestBase requestData)
        {
            var headers = new HeaderManager(requestData);
            urNoticeSession session = TokenManager.getLogoutSessionInfo(headers.AuthToken);
            //if (session != null)
            //{
            //    var user = _db.Users.SingleOrDefault(x => x.username == session.UserName);
            //    if (user != null) user.keepMeSignedIn = "false";
            //    try
            //    {
            //        _db.SaveChanges();
            //    }
            //    catch (DbEntityValidationException e)
            //    {
            //        DbContextException.LogDbContextException(e);
            //    }
            //}
            new TokenManager().Logout(headers.AuthToken);
        }


        [HttpPost]
        public JsonResult ResendValidationCode(ValidateAccountRequest req)
        {
            return Json(new AuthService().ResendValidationCodeService(req, Request, accessKey, secretKey));
        }

        //swagger done
        [HttpPost]
        public JsonResult ResetPassword(ResetPasswordRequest req)
        {
            return Json(new AuthService().ResetPasswordService(req, accessKey, secretKey));
        }
    }
}
