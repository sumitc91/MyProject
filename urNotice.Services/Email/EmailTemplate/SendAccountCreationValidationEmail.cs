using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;
using urNotice.Common.Infrastructure.Common.Config;
using urNotice.Common.Infrastructure.Common.Constants.EmailConstants;
using urNotice.Common.Infrastructure.commonMethods;
using urNotice.Common.Infrastructure.Model.urNoticeModel.RequestWrapper;
using urNotice.Services.ErrorLogger;

namespace urNotice.Services.Email.EmailTemplate
{
    public class SendAccountCreationValidationEmail
    {
        private ILogger _logger = new Logger(Convert.ToString(MethodBase.GetCurrentMethod().DeclaringType));
        
        public SendAccountCreationValidationEmail(ILogger logger)
        {
            _logger = logger;
        }

        public static void SendAccountCreationValidationEmailMessage(RegisterationRequest req, String guid, HttpRequestBase request)
        {
            
            if (request.Url != null)
            {
                IEmail emailModel = new EmailFromMandrill.EmailFromMandrill();
                emailModel.SendEmail(req.EmailId,
                    SmptCreateAccountConstants.SenderName,
                    SmptCreateAccountConstants.EmailTitle,
                    CreateAccountEmailBodyContent(request.Url.Authority, req, guid),
                    null,
                    null,
                    SmptCreateAccountConstants.SenderName,
                    SmtpConfig.SmtpEmailFromDoNotReply
                    );
            }
        }

        public static void SendAccountValidationEmailMessage(String toMail, String guid, HttpRequestBase request)
        {
            if (request != null && request.Url != null)
            {
                //IEmail emailModel = new EmailFromGmail();
                IEmail emailModel = new EmailFromMandrill.EmailFromMandrill();
                emailModel.SendEmail(toMail,
                    SmptCreateAccountConstants.SenderName,
                    SmptCreateAccountConstants.EmailTitle,
                    CreateAccountEmailBodyContent(request.Url.Authority,toMail, guid),
                    null,
                    null,
                    SmptCreateAccountConstants.SenderName,
                    SmtpConfig.SmtpEmailFromDoNotReply
                    );
            }
        }
        
        private static string CreateAccountEmailBodyContent(String requestUrlAuthority, RegisterationRequest req, String guid)
        {
            var template = File.ReadAllText(HttpContext.Current.Server.MapPath("~/EmailTemplate/CreateAccountEmail.html"));

            var encryptedUserInfo = new Dictionary<String, String>();
            encryptedUserInfo["EMAIL"] = req.EmailId;
            encryptedUserInfo["KEY"] = guid;

            string messageBody = template.Replace("{1}", "http://" + SmptCreateAccountConstants.AccountDomain + "/#/" + "validate/" + encryptedUserInfo["EMAIL"] + "/" + encryptedUserInfo["KEY"]);
            return messageBody;   
        }

        private static string CreateAccountEmailBodyContent(String requestUrlAuthority, String email, String guid)
        {
            var template = File.ReadAllText(HttpContext.Current.Server.MapPath("~/EmailTemplate/CreateAccountEmail.html"));

            var encryptedUserInfo = new Dictionary<String, String>();
            encryptedUserInfo["EMAIL"] = email;
            encryptedUserInfo["KEY"] = guid;

            string messageBody = template.Replace("{1}", "http://" + SmptCreateAccountConstants.AccountDomain + "/#/" + "validate/" + encryptedUserInfo["EMAIL"] + "/" + encryptedUserInfo["KEY"]);
            return messageBody;
        }

        private static string ValidateAccountEmailBodyContent(String requestUrlAuthority, String toMail, String guid)
        {
            var htmlBody = new StringBuilder();

            htmlBody.Append("<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" bgcolor=\"#368ee0\">");
            htmlBody.Append("<tr>");
            htmlBody.Append("<td align=\"center\">");
            htmlBody.Append("<center>");
            htmlBody.Append("<table border=\"0\" width=\"600\" cellpadding=\"0\" cellspacing=\"0\">");
            htmlBody.Append("<tr>");
            htmlBody.Append("<td style=\"color:#ffffff !important; font-size:24px; font-family: Arial, Verdana, sans-serif; padding-left:10px;\" height=\"40\"></td>");
            htmlBody.Append("<td align=\"right\" width=\"50\" height=\"45\"></td>");
            htmlBody.Append("</tr>");
            htmlBody.Append("</table>");
            htmlBody.Append("</center>");
            htmlBody.Append("</td>");
            htmlBody.Append("</tr>");
            htmlBody.Append("</table>");

            htmlBody.Append("<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" bgcolor=\"#ffffff\">");
            htmlBody.Append("<tr>");
            htmlBody.Append("<td align=\"center\">");
            htmlBody.Append("<center>");
            htmlBody.Append("<table border=\"0\" width=\"600\" cellpadding=\"0\" cellspacing=\"0\">");
            htmlBody.Append("<tr>");
            htmlBody.Append("<td style=\"color:#333333 !important; font-size:20px; font-family: Arial, Verdana, sans-serif; padding-left:10px;\" height=\"40\">");
            htmlBody.Append("<h3 style=\"font-weight:normal; margin: 20px 0;\">Account verification</h3>");
            htmlBody.Append("<p style=\"font-size:12px; line-height:18px;\">");
            htmlBody.Append("Message for User. <br /><br />");
            htmlBody.Append("Email: " + toMail + "");
            htmlBody.Append("</p>");
            htmlBody.Append("<p style=\"font-size:12px; line-height:18px;\">");
            htmlBody.Append("<a href=\"http://" + requestUrlAuthority + "/#/" + "validate/" + toMail + "/" + guid + "\"> Click here to validate your account </a>");
            htmlBody.Append("</p>");
            htmlBody.Append("<br> OR <br><p style=\"font-size:12px; line-height:18px;\">");
            htmlBody.Append("copy given URL in your browser <br>http://" + requestUrlAuthority + "/#/" + "validate/" + toMail + "/" + guid + " <br>");
            htmlBody.Append("</p>");
            htmlBody.Append("</td>");
            htmlBody.Append("</tr>");
            htmlBody.Append("</table>");
            htmlBody.Append("</center>");
            htmlBody.Append("</td>");
            htmlBody.Append("</tr>");
            htmlBody.Append("</table>");
            htmlBody.Append("<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" bgcolor=\"#ffffff\">");
            htmlBody.Append("<tr>");
            htmlBody.Append("<td align=\"center\">");
            htmlBody.Append("<center>");
            htmlBody.Append("<table border=\"0\" width=\"600\" cellpadding=\"0\" cellspacing=\"0\">");
            htmlBody.Append("<tr>");
            htmlBody.Append("<td style=\"color:#333333 !important; font-size:20px; font-family: Arial, Verdana, sans-serif; padding-left:10px;\" height=\"40\">");
            htmlBody.Append("<h3 style=\"font-weight:normal; margin: 20px 0;\">Security</h3>");
            htmlBody.Append("<p style=\"font-size:12px; line-height:18px;\">");
            htmlBody.Append("Some details for user<br />");
            htmlBody.Append("<br />");
            htmlBody.Append("<br />More details for user.");
            htmlBody.Append("</p>");
            htmlBody.Append("<p style=\"font-size:12px; line-height:18px;\">");
            htmlBody.Append("<a href=\"#\">Check security settings</a>");
            htmlBody.Append("</p>");
            htmlBody.Append(" </td>");
            htmlBody.Append("</tr>");
            htmlBody.Append("</table>");
            htmlBody.Append("</center>");
            htmlBody.Append("</td>");
            htmlBody.Append("</tr>");
            htmlBody.Append("</table>");

            htmlBody.Append("<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" bgcolor=\"#368ee0\">");
            htmlBody.Append("<tr>");
            htmlBody.Append("<td align=\"center\">");
            htmlBody.Append("<center>");
            htmlBody.Append("<table border=\"0\" width=\"600\" cellpadding=\"0\" cellspacing=\"0\">");
            htmlBody.Append("<tr>");
            htmlBody.Append("<td style=\"color:#ffffff !important; font-size:20px; font-family: Arial, Verdana, sans-serif; padding-left:10px;\" height=\"40\">");
            htmlBody.Append("<center>");
            htmlBody.Append("<p style=\"font-size:12px; line-height:18px;\">");
            htmlBody.Append("If you don't want to get system emails from FLAT please change your email settings.");
            htmlBody.Append("<br />");
            htmlBody.Append("<a href=\"#\" style=\"color:#ffffff !important;\">Click here to change email settings</a>");
            htmlBody.Append("</p>");
            htmlBody.Append("</center>");
            htmlBody.Append("</td>");
            htmlBody.Append("</tr>");
            htmlBody.Append("</table>");
            htmlBody.Append("</center>");
            htmlBody.Append("</td>");
            htmlBody.Append("</tr>");
            htmlBody.Append("</table>");


            return htmlBody.ToString();
        }

    }
}