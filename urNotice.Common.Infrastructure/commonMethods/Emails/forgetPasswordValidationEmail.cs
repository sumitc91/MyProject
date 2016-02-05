using System;
using System.Configuration;
using System.IO;
using System.Web;
using urNotice.Common.Infrastructure.Common.Constants.EmailConstants;

namespace urNotice.Common.Infrastructure.commonMethods.Emails
{
    public class ForgetPasswordValidationEmail
    {
        public void SendForgetPasswordValidationEmailMessage(String toMail, String guid, HttpRequestBase request, string id)
        {
            var sendEmail = new SendEmail();
            if (request.Url != null)
            {
                sendEmail.SendEmailMessage(toMail,
                    SmtpForgetPasswordContants.SenderName,
                    SmtpForgetPasswordContants.EmailTitle,
                    ForgetPasswordEmailBodyContent(request.Url.Authority, toMail, guid),
                    null,
                    null,
                    SmtpForgetPasswordContants.SenderName,
                    ConfigurationManager.AppSettings["SmtpEmailFromDoNotReply"]
                    );
            }
        }

        private string ForgetPasswordEmailBodyContent(String requestUrlAuthority, String toMail, String guid)
        {
            var template = File.ReadAllText(HttpContext.Current.Server.MapPath("~/EmailTemplate/ForgetPasswordEmail.html"));
            string messageBody =template.Replace("{1}","http://" + SmtpForgetPasswordContants.AccountDomain + "/#" + "resetpassword/" + toMail + "/" + guid);                             
            return messageBody;            
        }

    }
}