using Newtonsoft.Json;
using System.Text;

namespace MyCar;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private async void LoginButton_Clicked(object sender, EventArgs e)
    {
        ErrorLabel.IsVisible = false;

        string login = LoginEntry.Text?.Trim();
        string password = PasswordEntry.Text;

        if (string.IsNullOrWhiteSpace(login))
        {
            ShowError("Введите телефон или email");
            return;
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            ShowError("Введите пароль");
            return;
        }

        if (password.Length < 6)
        {
            ShowError("Пароль должен содержать минимум 6 символов");
            return;
        }

        try
        {
            var loginButton = (Button)sender;
            loginButton.IsEnabled = false;
            loginButton.Text = "ВХОД...";

            var request = new
            {
                Email = login,
                Password = password
            };

            using var client = new HttpClient();
            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(
                "https://mycarapi-lyubovchernyshova.msk1.amvera.tech/api/Auth/login",
                content);

            if (response.IsSuccessStatusCode)
            {
                if (Application.Current?.Windows.Count > 0)
                {
                    Application.Current.Windows[0].Page = new MainScreenPage();
                }
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                ShowError($"Ошибка: {error}");
            }
        }
        catch (Exception ex)
        {
            ShowError($"Ошибка подключения: {ex.Message}");
        }
        finally
        {
            var loginButton = (Button)sender;
            loginButton.IsEnabled = true;
            loginButton.Text = "ВОЙТИ";
        }
    }

    private async void RegisterButton_Clicked(object sender, EventArgs e)
    {
        if (Application.Current?.Windows.Count > 0)
        {
            Application.Current.Windows[0].Page = new RegisterPage();
        }
    }

    private void ShowError(string message)
    {
        ErrorLabel.Text = message;
        ErrorLabel.IsVisible = true;
    }
}