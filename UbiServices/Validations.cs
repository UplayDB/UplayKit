using System.Text.RegularExpressions;

namespace UbiServices
{
    public class Validations
    {
        public static bool EmailValidation(string email)
        {
            Regex regex = new Regex(@"^([\w\.\-\+]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(email);
            if (match.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IdValidation(string Id)
        {
            Regex regex = new Regex(@"((\w){8})((\-(\w){4})){3}(\-(\w){12})");
            Match match = regex.Match(Id);
            if (match.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
