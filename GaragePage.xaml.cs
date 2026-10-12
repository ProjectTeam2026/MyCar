namespace MyCar;

public partial class GaragePage : ContentPage
{
    public GaragePage()
    {
        InitializeComponent();

        
        CarsCollectionView.ItemsSource = new List<Car>
        {
            new Car
            {
                Mark = "Toyota Camry",
                Number = "А123ВС 77",
                Color = "Белый"
            }
        };

        
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

public class Car
{
    public string Mark { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;

    
    public string MarkInitial =>
        string.IsNullOrWhiteSpace(Mark) ? "?" : Mark.Substring(0, 1).ToUpper();
}