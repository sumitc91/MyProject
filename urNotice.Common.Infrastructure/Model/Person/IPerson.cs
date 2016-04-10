using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using urNotice.Common.Infrastructure.Model.urNoticeModel.AssetClass;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;
using urNotice.Common.Infrastructure.Model.urNoticeModel.GraphModel;
using urNotice.Common.Infrastructure.Model.urNoticeModel.RequestWrapper;
using urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper;
using urNotice.Common.Infrastructure.Session;

namespace urNotice.Common.Infrastructure.Model.Person
{
    public interface IPerson
    {
        ResponseModel<LoginResponse> RegisterMe(RegisterationRequest req, HttpRequestBase request);
        ResponseModel<LoginResponse> SocialRegisterMe(RegisterationRequest req, HttpRequestBase request);
        ResponseModel<LoginResponse> Login(string userName, string password,bool decryptPassword);
        ResponseModel<OrbitPageUser> GetFullUserDetail(string userEmail);
        ResponseModel<ClientDetailsModel> GetPersonDetails(string username);
        ResponseModel<UserPostVertexModel> CreateNewUserPost(urNoticeSession session, string message, string image,string userWallVertexId, out Dictionary<string, string> sendNotificationResponse);
        
        
        //admin services
        Dictionary<string, string> CreateNewCompanyDesignationEdge(urNoticeSession session, string designation,
            string salary, string jobFromYear, string jobToYear, string companyVertexId);
        bool CreateNewDesignation(string designationName, string createdBy);
        bool CreateNewCompanyDesignationSalary(string companyName, string designationName, string salary,
            string createdBy);

        bool CreateNewCompany(OrbitPageCompany company, string createdBy);
    }
}
