using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using urNotice.Common.Infrastructure.Common.Constants.EmailConstants;
using urNotice.Common.Infrastructure.Model.urNoticeModel.RequestWrapper;

namespace urNotice.Common.Infrastructure.commonMethods.Emails
{
    public class SendContactUsEmail
    {
        public static void SendContactUsEmailMessage(String toMail, ContactUsRequest req)
        {
            var sendEmail = new SendEmail();

            sendEmail.SendEmailMessage(toMail,
               SmtpContactUsEmailContants.SenderName,
                SmtpContactUsEmailContants.EmailTitle,
                ContactUsEmailBodyContent(req),
                null,
                null,
                SmtpContactUsEmailContants.SenderName,
                ConfigurationManager.AppSettings["SmtpEmailFrom"]
                );

        }

        private static string ContactUsEmailBodyContent(ContactUsRequest req)
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
            htmlBody.Append("<h3 style=\"font-weight:normal; margin: 20px 0;\">Contact Us</h3>");
            htmlBody.Append("<p style=\"font-size:12px; line-height:18px;\">");
            htmlBody.Append("User Message. <br /><br />");
            htmlBody.Append("Name : " + req.Name + "<br /><br />");
            htmlBody.Append("Email : " + req.Email + "<br /><br />");
            htmlBody.Append("Phone : " + req.Phone + "<br /><br />");
            htmlBody.Append("Type : " + req.Type + "<br /><br />");
            htmlBody.Append("Message : " + req.Message + "<br /><br />");
            htmlBody.Append("SendMeACopy : " + req.SendMeACopy + "<br /><br />");
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
