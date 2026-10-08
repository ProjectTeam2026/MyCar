namespace MyCar;

public partial class GaragePage : ContentPage
{
	public GaragePage()
	{
        InitializeComponent();
	}

    private async void OnCarListClicked(object sender, EventArgs e)
    {
        await DisplayAlertAsync("Список авто", "Пока тут пусто", "OK");
    }

    private async void OnAddCarClicked(object sender, EventArgs e)
    {
        await DisplayAlertAsync("Добавить авто", "Форма в разработке", "OK");
    }
}