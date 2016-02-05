namespace urNotice.Common.Infrastructure.Model.urNoticeModel.RequestWrapper
{
    public class LoginRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string KeepMeSignedInCheckBox { get; set; }
        public string Type { get; set; }
    }
}