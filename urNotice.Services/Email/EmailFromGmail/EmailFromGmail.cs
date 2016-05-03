using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using urNotice.Common.Infrastructure.Common.Config;
using urNotice.Common.Infrastructure.Common.Constants;
using urNotice.Common.Infrastructure.Model.urNoticeModel.AssetClass;

namespace urNotice.Services.Email.EmailFromGmail
{
    public class EmailFromGmail : IEmail
    {
        String _path;
        MailMessage _mail = new MailMessage();

        public ResponseModel<string> SendEmail(string toEmailAddrList, string senderName, string subject, string body,
            string attachmentsFilePathList, string logoPath, string companyDescription, string sendEmailFrom)
        {
            var smtpServer = new SmtpClient
            {
                Credentials =
                    new System.Net.NetworkCredential(SmtpConfig.GmailSmtpEmail,
                        SmtpConfig.GmailSmtpPassword),
                Port = Convert.ToInt32(SmtpConfig.SmtpPort.ToString(CultureInfo.InvariantCulture)),
                Host = SmtpConfig.GmailSmtpHost.ToString(CultureInfo.InvariantCulture),
                EnableSsl = Convert.ToBoolean(SmtpConfig.SmtpEnableSsl.ToString(CultureInfo.InvariantCulture))
            };
            _mail = new MailMessage();
            var addr = toEmailAddrList.Split(',');
           
            if(sendEmailFrom == null)
                sendEmailFrom = SmtpConfig.GmailSmtpEmailFromDoNotReply;

            _mail.From = new MailAddress(sendEmailFrom, senderName, System.Text.Encoding.UTF8);
            Byte i;
            for (i = 0; i < addr.Length; i++)
                _mail.To.Add(addr[i]);
            _mail.Subject = subject;
            _mail.Body = body;
            if (attachmentsFilePathList != null)
            {
                var attachments = attachmentsFilePathList.Split(',');
                for (i = 0; i < attachments.Length; i++)
                    _mail.Attachments.Add(new Attachment(attachments[i]));
            }
            _path = logoPath;
            if (_path != null)
            {
                var logo = new LinkedResource(_path) { ContentId = "Logo" };
                string htmlview = "<html><body><table border=2><tr width=100%><td><img src=cid:Logo alt=companyname /></td><td>" + companyDescription + "</td></tr></table><hr/></body></html>";
                var alternateView1 = AlternateView.CreateAlternateViewFromString(htmlview + body, null, MediaTypeNames.Text.Html);
                alternateView1.LinkedResources.Add(logo);
                _mail.AlternateViews.Add(alternateView1);
            }
            else
            {
                var alternateView1 = AlternateView.CreateAlternateViewFromString(body, null, MediaTypeNames.Text.Html);
                _mail.AlternateViews.Add(alternateView1);
            }
            _mail.IsBodyHtml = true;
            _mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;            
            smtpServer.Send(_mail);

            return OrbitPageResponseModel.SetOk("Email sent successfully.", String.Empty);
        }
    }
}
