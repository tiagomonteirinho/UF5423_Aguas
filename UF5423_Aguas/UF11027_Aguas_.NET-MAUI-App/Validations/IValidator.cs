namespace UF11027_Aguas_.NET_MAUI_App.Validations;

public interface IValidator
{
    string NameError { get; set; }

    string EmailError { get; set; }

    string PhoneNumberError { get; set; }

    string AddressError { get; set; }

    string SerialNumberError { get; set; }

    string PostalCodeError { get; set; }

    Task<bool> Validate(string name, string email, string phoneNumber, string address, string serialNumber, string postalCode);
}
