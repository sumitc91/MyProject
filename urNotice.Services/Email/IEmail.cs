using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using urNotice.Common.Infrastructure.Model.urNoticeModel.AssetClass;

namespace urNotice.Services.Email
{
    public interface IEmail
    {
        ResponseModel<String> SendEmail(String toEmailAddrList, String senderName, String subject, String body,String attachmentsFilePathList, String logoPath, String companyDescription, String sendEmailFrom);
    }
}
