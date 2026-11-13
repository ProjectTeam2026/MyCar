using Newtonsoft.Json;
using System.Text;

namespace MyCar;

public partial class RegisterPage : ContentPage
{
    public RegisterPage()
    {
        InitializeComponent();
    }

    private async void RegisterButton_Clicked(object sender, EventArgs e)
    {
        ErrorLabel.IsVisible = false;

        string name = NameEntry.Text?.Trim();
        string phone = PhoneEntry.Text?.Trim();
        string email = EmailEntry.Text?.Trim();
        string password = PasswordEntry.Text;
        string confirmPassword = ConfirmPasswordEntry.Text;

        if (string.IsNullOrWhiteSpace(name))
        {
            ShowError("Введите ваше имя");
            return;
        }

        if (string.IsNullOrWhiteSpace(phone))
        {
            ShowError("Введите номер телефона");
            return;
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            ShowError("Введите email");
            return;
        }

        if (!email.Contains("@"))
        {
            ShowError("Введите корректный email");
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

        if (password != confirmPassword)
        {
            ShowError("Пароли не совпадают");
            return;
        }

        try
        {
            var registerButton = (Button)sender;
            registerButton.IsEnabled = false;
            registerButton.Text = "ОТПРАВКА...";

            var request = new RegisterRequest
            {
                Name = name,
                Email = email,
                Password = password,
                Phone = phone
            };

            using var client = new HttpClient();
            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(
                "https://mycarapi-lyubovchernyshova.msk1.amvera.tech/api/Auth/register",
                content);

            if (response.IsSuccessStatusCode)
            {
                await DisplayAlert("Готово!", "Регистрация успешно завершена", "Продолжить");
                await Navigation.PopAsync();
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                ShowError($"Ошибка сервера: {error}");
            }
        }
        catch (Exception ex)
        {
            ShowError($"Ошибка подключения: {ex.Message}");
        }
        finally
        {
            var registerButton = (Button)sender;
            registerButton.IsEnabled = true;
            registerButton.Text = "ЗАРЕГИСТРИРОВАТЬСЯ";
        }
    }

    private async void LoginButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new LoginPage());
    }

    private void ShowError(string message)
    {
        ErrorLabel.Text = message;
        ErrorLabel.IsVisible = true;
    }
}