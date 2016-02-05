using System;
using System.Configuration;
using System.Globalization;
using System.Net.Mail;
using System.Net.Mime;
using System.Reflection;
using urNotice.Common.Infrastructure.Common.Logger;

namespace urNotice.Common.Infrastructure.commonMethods
{
    public class SendEmail
    {
        private readonly ILogger _logger = new Logger(Convert.ToString(MethodBase.GetCurrentMethod().DeclaringType));        

        String _path;
        MailMessage _mail = new MailMessage();

        public void SendEmailMessage(String toEmailAddrList,String senderName,String subject,String body,String attachmentsFilePathList,String logoPath, String companyDescription,String sendEmailFrom)
        {
            var smtpServer = new SmtpClient
            {
                Credentials =
                    new System.Net.NetworkCredential(ConfigurationManager.AppSettings["SmtpEmail"],
                        ConfigurationManager.AppSettings["SmtpPassword"]),
                Port = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"].ToString(CultureInfo.InvariantCulture)),
                Host = ConfigurationManager.AppSettings["SmtpHost"].ToString(CultureInfo.InvariantCulture),
                EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["SmtpEnableSsl"].ToString(CultureInfo.InvariantCulture))
            };
            _mail = new MailMessage();
            var addr = toEmailAddrList.Split(',');
            try
            {
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
                    var logo = new LinkedResource(_path) {ContentId = "Logo"};
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
                //mail.ReplyToList = new MailAddress(ConfigurationManager.AppSettings["SmtpEmail"].ToString());
                smtpServer.Send(_mail);                
            }
            catch (Exception ex)
            {
                _logger.Error("Exception occured while sending email",ex);                
            }
            
        }

        public void SendEmailMessageFromGmail(String toEmailAddrList, String senderName, String subject, String body, String attachmentsFilePathList, String logoPath, String companyDescription, String sendEmailFrom)
        {
            var smtpServer = new SmtpClient
            {
                Credentials =
                    new System.Net.NetworkCredential(ConfigurationManager.AppSettings["GmailSmtpEmail"],
                        ConfigurationManager.AppSettings["GmailSmtpPassword"]),
                Port = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"].ToString(CultureInfo.InvariantCulture)),
                Host = ConfigurationManager.AppSettings["GmailSmtpHost"].ToString(CultureInfo.InvariantCulture),
                EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["SmtpEnableSsl"].ToString(CultureInfo.InvariantCulture))
            };
            _mail = new MailMessage();
            var addr = toEmailAddrList.Split(',');
            try
            {
                sendEmailFrom = ConfigurationManager.AppSettings["GmailSmtpEmailFromDoNotReply"].ToString(CultureInfo.InvariantCulture);
                
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
                //mail.ReplyToList = new MailAddress(ConfigurationManager.AppSettings["SmtpEmail"].ToString());
                smtpServer.Send(_mail);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception occured while sending email", ex);
            }

        }
    }
}