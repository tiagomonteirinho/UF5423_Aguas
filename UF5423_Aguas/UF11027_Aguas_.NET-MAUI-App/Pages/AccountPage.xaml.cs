using UF11027_Aguas_.NET_MAUI_App.Services;
using UF11027_Aguas_.NET_MAUI_App.Validations;

namespace UF11027_Aguas_.NET_MAUI_App.Pages
{
    public partial class AccountPage : ContentPage
    {
        private readonly ApiService _apiService;
        private readonly IValidator _validator;
        private bool _loginPageDisplayed = false;

        public AccountPage(ApiService apiService, IValidator validator)
        {
            InitializeComponent();
            _apiService = apiService;
            _validator = validator;
            name_lbl.Text = Preferences.Get("username", string.Empty);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            image_imgBtn.Source = await GetImage();
        }

        private async Task<string?> GetImage()
        {
            string defaultUserImageUrl = AppConfig.DefaultUserImageUrl;

            var (response, errorMessage) = await _apiService.GetImage();
            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return null;
            }

            if (response == null || string.IsNullOrEmpty(response?.ImageUrl))
            {
                return defaultUserImageUrl;
            }

            return response.ImageUrl;
        }

        private async Task DisplayLoginPage()
        {
            _loginPageDisplayed = true;
            await Navigation.PushAsync(new LoginPage(_apiService, _validator));
        }

        private async void image_imgBtn_Clicked(object sender, EventArgs e)
        {
            try
            {
                var imageArray = await ChangeImage();
                if (imageArray == null)
                {
                    await DisplayAlert("Error", "Could not process request.", "OK");
                    return;
                }

                image_imgBtn.Source = ImageSource.FromStream(() => new MemoryStream(imageArray));

                var response = await _apiService.ChangeImage(imageArray);
                if (response.Data)
                {
                    await DisplayAlert("Success", "Image successfully uploaded.", "OK");
                }
                else
                {
                    await DisplayAlert("Error", response.ErrorMessage ?? "Could not process request.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Could not process request: {ex.Message}", "OK");
            }
        }

        private async Task<byte[]?> ChangeImage()
        {
            try
            {
                var imageFile = await MediaPicker.PickPhotoAsync();
                if (imageFile == null) return null;

                using (var stream = await imageFile.OpenReadAsync())
                using (var memoryStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memoryStream);
                    return memoryStream.ToArray();
                }
            }
            catch (FeatureNotSupportedException)
            {
                await DisplayAlert("Error", "That feature is not supported on this platform.", "OK");
            }
            catch (PermissionException)
            {
                await DisplayAlert("Error", "That feature has not been granted required permissions.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Could not process request: {ex.Message}", "OK");
            }

            return null;
        }

        private void logout_imgBtn_Clicked(object sender, EventArgs e)
        {
            Preferences.Set("accesstoken", string.Empty);
            Application.Current!.MainPage = new NavigationPage(new LoginPage(_apiService, _validator));
        }

        private void account_tap_Tapped(object sender, TappedEventArgs e)
        {
            Navigation.PushAsync(new AccountSettingsPage(_apiService, _validator));
        }
    }
}