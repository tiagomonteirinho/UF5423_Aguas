namespace UF11027_Aguas_.NET_MAUI_App.Services;

public class ApiResponse<T>
{
    public T? Data { get; set; }

    public string? ErrorMessage { get; set; }

    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);
}
