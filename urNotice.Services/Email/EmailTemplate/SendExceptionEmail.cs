﻿using System;
using System.Globalization;
using System.Reflection;
using System.Text;
using urNotice.Common.Infrastructure.Common.Config;
using urNotice.Common.Infrastructure.Common.Constants.EmailConstants;
using urNotice.Common.Infrastructure.commonMethods;
using urNotice.Services.ErrorLogger;

namespace urNotice.Services.Email.EmailTemplate
{
    public class SendExceptionEmail
    {        
        public delegate void exceptionEmailSend_Delegate(String toEmailAddrList, String senderName, String subject, String body, String attachmentsFilePathList, String logoPath, String companyDescription, String sendEmailFrom);


        public static void SendExceptionEmailMessage(String toMail, String exceptionMessage)
        {            
            IEmail emailModel = new EmailFromGmail.EmailFromGmail();
            //exceptionEmailSend_Delegate exceptionEmail_delegate = null;
            //exceptionEmail_delegate = new exceptionEmailSend_Delegate(emailModel.SendEmail);
            //IAsyncResult CallAsynchMethod = null;
            String emailFrom = SmtpConfig.GmailSmtpEmailFromDoNotReply;
            //CallAsynchMethod = exceptionEmail_delegate.BeginInvoke(toMail, SmtpSendExceptionEmailContants.SenderName, SmtpSendExceptionEmailContants.EmailTitle, ExceptionEmailBodyContent(exceptionMessage), null, null, SmtpSendExceptionEmailContants.SenderName,emailFrom, null, null); //invoking the method
            emailModel.SendEmail(toMail, SmtpSendExceptionEmailContants.SenderName, SmtpSendExceptionEmailContants.EmailTitle, ExceptionEmailBodyContent(exceptionMessage), null, null, SmtpSendExceptionEmailContants.SenderName, emailFrom);
        }

        private static string ExceptionEmailBodyContent(String exceptionMessage)
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
            htmlBody.Append("Exception occured. <br /><br />");
            htmlBody.Append("Date :" + DateTime.Now.ToLongDateString() + "<br /><br />");
            htmlBody.Append("time : " + DateTime.Now.ToLongTimeString() + "<br /><br />");
            htmlBody.Append("Exception : " + exceptionMessage + "<br /><br />");
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
