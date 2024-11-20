using Microsoft.Extensions.Logging;
using System.Net;
using System.Text;
using System.Text.Json;
using UF11027_Aguas_.NET_MAUI_App.Models;

namespace UF11027_Aguas_.NET_MAUI_App.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://fcbdb5dh-5001.uks1.devtunnels.ms/";
        private readonly ILogger<ApiService> _logger;

        JsonSerializerOptions _serializerOptions;
        public ApiService(HttpClient httpClient, ILogger<ApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<ApiResponse<bool>> Login(string email, string password)
        {
            try
            {
                var login = new Login()
                {
                    Email = email,
                    Password = password
                };

                var json = JsonSerializer.Serialize(login, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await PostRequest("api/AccountApi/Login", content);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Could not process HTTP request: {response.StatusCode}");
                    return new ApiResponse<bool>
                    {
                        ErrorMessage = $"Could not process HTTP request: {response.StatusCode}"
                    };
                }

                var jsonResult = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Token>(jsonResult, _serializerOptions);

                Preferences.Set("accesstoken", result!.AccessToken);
                Preferences.Set("userid", result.UserId);
                Preferences.Set("username", result.UserName);

                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not log in: {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
        }

        private async Task<HttpResponseMessage> PostRequest(string uri, HttpContent content)
        {
            var url = _baseUrl + uri;
            try
            {
                var result = await _httpClient.PostAsync(url, content);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not process HTTP request to {uri}: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }
    }
}