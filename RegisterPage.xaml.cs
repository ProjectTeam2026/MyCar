namespace DriverApp;

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

        await DisplayAlert(
            "Готово!",
            "Регистрация успешно завершена",
            "Продолжить"
        );

        await Navigation.PopAsync();
    }

    private async void LoginButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private void ShowError(string message)
    {
        ErrorLabel.Text = message;
        ErrorLabel.IsVisible = true;
    }
}