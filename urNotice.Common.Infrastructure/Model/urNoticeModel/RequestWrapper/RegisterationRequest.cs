namespace urNotice.Common.Infrastructure.Model.urNoticeModel.RequestWrapper
{
    public class RegisterationRequest
    {    
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string EmailId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }        
        public string Source { get; set; }
        public string Referral { get; set; }
    }
}