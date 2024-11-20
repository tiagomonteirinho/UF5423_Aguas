using System.Text.RegularExpressions;

namespace UF11027_Aguas_.NET_MAUI_App.Validations
{
    public class Validator : IValidator
    {
        public string NameError { get; set; } = "";
        public string EmailError { get; set; } = "";
        public string PhoneNumberError { get; set; } = "";
        public string PasswordError { get; set; } = "";

        private const string EmptyNameErrorMessage = "Name is required.";
        private const string InvalidNameErrorMessage = "Name is invalid.";
        private const string EmptyEmailErrorMessage = "Email is required.";
        private const string InvalidEmailErrorMessage = "Email is invalid.";
        private const string EmptyPasswordErrorMessage = "Password is required.";
        private const string InvalidPasswordErrorMessage = "Password must contain at least 8 characters and include letters and digits.";

        public Task<bool> Validate(string name, string email, string password)
        {
            var isNameValid = ValidateName(name);
            var isEmailValid = ValidateEmail(email);
            var isPasswordValid = ValidatePassword(password);
            return Task.FromResult(isNameValid && isEmailValid && isPasswordValid);
        }

        private bool ValidateName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                NameError = EmptyNameErrorMessage;
                return false;
            }

            if (name.Length < 3)
            {
                NameError = InvalidNameErrorMessage;
                return false;
            }

            NameError = "";
            return true;
        }

        private bool ValidateEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                EmailError = EmptyEmailErrorMessage;
                return false;
            }

            if (!Regex.IsMatch(email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"))
            {
                EmailError = InvalidEmailErrorMessage;
                return false;
            }

            EmailError = "";
            return true;
        }

        private bool ValidatePassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                PasswordError = EmptyPasswordErrorMessage;
                return false;
            }

            if (password.Length < 8 || !Regex.IsMatch(password, @"[a-zA-Z]") || !Regex.IsMatch(password, @"\d"))
            {
                PasswordError = InvalidPasswordErrorMessage;
                return false;
            }

            PasswordError = "";
            return true;
        }
    }
}
