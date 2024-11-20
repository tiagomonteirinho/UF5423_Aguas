namespace UF11027_Aguas_.NET_MAUI_App.Validations
{
    public interface IValidator
    {
        string NameError { get; set; }

        string EmailError { get; set; }

        string PasswordError { get; set; }

        Task<bool> Validate(string name, string email, string password);
    }
}
