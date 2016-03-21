using System;
using System.IO;
using System.Web;
using urNotice.Common.Infrastructure.Common.Config;
using urNotice.Common.Infrastructure.Common.Constants.EmailConstants;
using urNotice.Common.Infrastructure.commonMethods;

namespace urNotice.Services.Email.EmailTemplate
{
    public class ForgetPasswordValidationEmail
    {
        public void SendForgetPasswordValidationEmailMessage(String toMail, String guid, HttpRequestBase request, string id)
        {            
            if (request.Url != null)
            {
                IEmail emailModel = new EmailFromMandrill.EmailFromMandrill();
                emailModel.SendEmail(toMail,
                    SmtpForgetPasswordContants.SenderName,
                    SmtpForgetPasswordContants.EmailTitle,
                    ForgetPasswordEmailBodyContent(request.Url.Authority, toMail, guid),
                    null,
                    null,
                    SmtpForgetPasswordContants.SenderName,
                    SmtpConfig.SmtpEmailFromDoNotReply
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