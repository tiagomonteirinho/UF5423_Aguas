using System.Threading.Tasks;

namespace UF5423_Aguas.Helpers
{
    public interface IMailHelper
    {
        public bool SendEmail(string receiver, string title, string body);
    }
}
