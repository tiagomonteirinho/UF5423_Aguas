using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using UF11027_Aguas_.NET_MAUI_App.Models;

namespace UF11027_Aguas_.NET_MAUI_App.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = AppConfig.BaseUrl;
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

                var response = await PostRequest("api/AccountApi/login", content);
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
                Preferences.Set("useremail", result.UserEmail);
                Preferences.Set("username", result.UserName);

                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not log in: {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> RequestWaterMeter(string name, string email, string phoneNumber,
            string address, string postalCode, string serialNumber)
        {
            try
            {
                if (!int.TryParse(serialNumber, out int parsedSerialNumber))
                {
                    _logger.LogError($"Invalid serial number: {serialNumber}");
                    return new ApiResponse<bool>
                    {
                        ErrorMessage = "Invalid serial number format."
                    };
                }

                var requestWaterMeter = new RequestWaterMeter()
                {
                    FullName = name,
                    Email = email,
                    PhoneNumber = phoneNumber,
                    Address = address,
                    PostalCode = postalCode,
                    SerialNumber = parsedSerialNumber,
                };

                var json = JsonSerializer.Serialize(requestWaterMeter, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await PostRequest("api/MetersApi/RequestWaterMeter", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Could not process HTTP request: {response.StatusCode}, Content: {errorContent}");
                    return new ApiResponse<bool>
                    {
                        ErrorMessage = $"Could not process HTTP request: {response.StatusCode}, Content: {errorContent}"
                    };
                }

                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not register: {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> RecoverPassword(string email)
        {
            try
            {
                var recoverPassword = new { Email = email };
                var json = JsonSerializer.Serialize(recoverPassword, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await PostRequest("api/accountApi/recoverPassword", content);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Could not process HTTP request: {response.StatusCode}");
                    return new ApiResponse<bool>
                    {
                        ErrorMessage = $"Could not process HTTP request: {response.StatusCode}"
                    };
                }

                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not log in: {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> ChangeImage(byte[] imageArray)
        {
            try
            {
                var content = new MultipartFormDataContent();
                content.Add(new ByteArrayContent(imageArray), "image", "image.png");
                var response = await PostRequest("api/accountApi/ChangeImage", content);
                if (!response.IsSuccessStatusCode)
                {
                    string errorMessage = response.StatusCode == HttpStatusCode.Unauthorized ? "Unauthorized" : $"Could not process request: {response.ReasonPhrase}";
                    _logger.LogError($"Could not process request: {response.StatusCode}");
                    return new ApiResponse<bool> { ErrorMessage = errorMessage };
                }

                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not upload image: {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> ChangeInfo(string name)
        {
            try
            {
                var changeInfo = new { Name = name };
                var json = JsonSerializer.Serialize(changeInfo, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await PostRequest("api/accountApi/changeInfo", content);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Could not process HTTP request: {response.StatusCode}");
                    return new ApiResponse<bool>
                    {
                        ErrorMessage = $"Could not process HTTP request: {response.StatusCode}"
                    };
                }

                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not update info: {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> ChangePassword(string oldPassword, string newPassword, string confirmNewPassword)
        {
            try
            {
                var changePassword = new ChangePassword()
                {
                    OldPassword = oldPassword,
                    NewPassword = newPassword,
                    ConfirmNewPassword = confirmNewPassword,
                };

                var json = JsonSerializer.Serialize(changePassword, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await PostRequest("api/accountApi/changePassword", content);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Could not process HTTP request: {response.StatusCode}");
                    return new ApiResponse<bool>
                    {
                        ErrorMessage = $"Could not process HTTP request: {response.StatusCode}"
                    };
                }

                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not update password: {ex.Message}");
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

        public async Task<(List<Meter>? meters, string? errorMessage)> GetMeters()
        {
            return await GetAsync<List<Meter>>("api/metersApi/getMeters");
        }

        public async Task<(MeterDetails? meterDetails, string? errorMessage)> GetMeterDetails(int id)
        {
            string endpoint = $"api/metersApi/getMeterDetails/{id}";
            return await GetAsync<MeterDetails>(endpoint);
        }

        public async Task<(List<Consumption>? consumptions, string? errorMessage)> GetConsumptions()
        {
            return await GetAsync<List<Consumption>>("api/metersApi/getConsumptions");
        }

        public async Task<(Consumption? consumptionDetails, string? errorMessage)> GetConsumptionDetails(int id)
        {
            string endpoint = $"api/metersApi/getConsumptionDetails/{id}";
            return await GetAsync<Consumption>(endpoint);
        }

        public async Task<(Invoice? invoiceDetails, string? errorMessage)> GetInvoiceDetails(int consumptionId)
        {
            string endpoint = $"api/metersApi/getInvoiceDetails/{consumptionId}";
            return await GetAsync<Invoice>(endpoint);
        }

        public async Task<(List<Notification>? notifications, string? errorMessage)> GetNotifications()
        {
            return await GetAsync<List<Notification>>("api/accountApi/getNotifications");
        }

        public async Task<(Notification? notificationDetails, string? errorMessage)> GetNotificationDetails(int id)
        {
            string endpoint = $"api/accountApi/getNotificationDetails/{id}";
            return await GetAsync<Notification>(endpoint);
        }

        public async Task<(UserImage? userImage, string? ErrorMessage)> GetImage()
        {
            string endpoint = "api/accountApi/getImage";
            return await GetAsync<UserImage>(endpoint);
        }

        public async Task<(List<Tier>? tiers, string? errorMessage)> GetTiers()
        {
            return await GetUnauthorizedAsync<List<Tier>>("api/tiersApi/getTiers");
        }

        private async Task<(T? data, string? errorMessage)> GetAsync<T>(string endpoint)
        {
            try
            {
                AddAuthorizationHeader();
                var response = await _httpClient.GetAsync(AppConfig.BaseUrl + endpoint);
                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<T>(responseString, _serializerOptions);
                    return (data ?? Activator.CreateInstance<T>(), null);
                }
                else
                {
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        string errorMessage = "Unauthorized";
                        _logger.LogWarning(errorMessage);
                        return (default, errorMessage);
                    }

                    string generalErrorMessage = $"Could not process HTTP request: {response.ReasonPhrase}";
                    _logger.LogError(generalErrorMessage);
                    return (default, generalErrorMessage);
                }
            }
            catch (HttpRequestException ex)
            {
                string errorMessage = $"Could not process HTTP request: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return (default, errorMessage);
            }
            catch (JsonException ex)
            {
                string errorMessage = $"Could not deserialize JSON: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return (default, errorMessage);
            }
            catch (Exception ex)
            {
                string errorMessage = $"Could not process request: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return (default, errorMessage);
            }
        }

        private async Task<(T? data, string? errorMessage)> GetUnauthorizedAsync<T>(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync(AppConfig.BaseUrl + endpoint);
                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<T>(responseString, _serializerOptions);
                    return (data ?? Activator.CreateInstance<T>(), null);
                }
                else
                {
                    string errorMessage = $"Could not process HTTP request: {response.ReasonPhrase}";
                    _logger.LogError(errorMessage);
                    return (default, errorMessage);
                }
            }
            catch (HttpRequestException ex)
            {
                string errorMessage = $"Could not process HTTP request: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return (default, errorMessage);
            }
            catch (JsonException ex)
            {
                string errorMessage = $"Could not deserialize JSON: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return (default, errorMessage);
            }
            catch (Exception ex)
            {
                string errorMessage = $"Could not process request: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return (default, errorMessage);
            }
        }

        public async Task<string?> MarkNotificationAsRead(int id)
        {
            try
            {
                var content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
                var response = await PutRequest($"api/accountApi/markNotificationAsRead/{id}", content);
                if (response.IsSuccessStatusCode)
                {
                    return null;
                }
                else
                {
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        string errorMessage = "Unauthorized";
                        _logger.LogWarning(errorMessage);
                        return errorMessage;
                    }

                    string generalErrorMessage = $"Could not process HTTP request: {response.ReasonPhrase}";
                    _logger.LogError(generalErrorMessage);
                    return generalErrorMessage;
                }
            }
            catch (HttpRequestException ex)
            {
                string errorMessage = $"Could not process HTTP request: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return errorMessage;
            }
            catch (JsonException ex)
            {
                string errorMessage = $"Could not deserialize JSON: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return errorMessage;
            }
            catch (Exception ex)
            {
                string errorMessage = $"Could not process request: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return errorMessage;
            }
        }

        public async Task<string?> BuyConsumption(int id)
        {
            try
            {
                var content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
                var response = await PutRequest($"api/metersApi/buyConsumption/{id}", content);
                if (response.IsSuccessStatusCode)
                {
                    return null;
                }
                else
                {
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        string errorMessage = "Unauthorized";
                        _logger.LogWarning(errorMessage);
                        return errorMessage;
                    }

                    string generalErrorMessage = $"Could not process HTTP request: {response.ReasonPhrase}";
                    _logger.LogError(generalErrorMessage);
                    return generalErrorMessage;
                }
            }
            catch (HttpRequestException ex)
            {
                string errorMessage = $"Could not process HTTP request: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return errorMessage;
            }
            catch (JsonException ex)
            {
                string errorMessage = $"Could not deserialize JSON: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return errorMessage;
            }
            catch (Exception ex)
            {
                string errorMessage = $"Could not process request: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return errorMessage;
            }
        }

        private async Task<HttpResponseMessage> PutRequest(string uri, HttpContent content)
        {
            var url = AppConfig.BaseUrl + uri;

            try
            {
                AddAuthorizationHeader();
                var result = await _httpClient.PutAsync(url, content);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not process HTTP request to {uri}: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }

        private void AddAuthorizationHeader()
        {
            var token = Preferences.Get("accesstoken", string.Empty);
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
    }
}