namespace Water_Services.Helpers
{
    public interface IMailHelper
    {
        public bool SendEmail(string receiver, string title, string body);
    }
}
