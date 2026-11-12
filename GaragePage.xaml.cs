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
                Color = "Белый",
                Year = "2020",            
                BodyType = "Седан"       
            },
            new Car
            {
                Mark = "BMW X5",
                Number = "В456СD 77",
                Color = "Черный",
                Year = "2019",
                BodyType = "Внедорожник"
            }
        };
    }

    private async void OnCarListClicked(object sender, EventArgs e)
    {
        await DisplayAlertAsync("Список авто", "Пока тут пусто", "OK");
    }

    private async void OnAddCarClicked(object sender, EventArgs e)
    {
        
        await Navigation.PushAsync(new AddCarPage());
    }


    private async void GaragePage_Tapped(object sender, TappedEventArgs e)
    {
        
    }

    private async void ServicePage_Tapped(object sender, TappedEventArgs e)
    {
         await Navigation.PushAsync(new ServicePage());
    }

    private async void NewsPage_Tapped(object sender, TappedEventArgs e)
    {
        // await Navigation.PushAsync(new NewsPage());
    }

    private async void ProfilePage_Tapped(object sender, TappedEventArgs e)
    {
        // await Navigation.PushAsync(new ProfilePage());
    }

}


public class Car
{
    public string Mark { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;

    
    public string Year { get; set; } = string.Empty;
    public string BodyType { get; set; } = string.Empty;

   
    public string MarkInitial =>
        string.IsNullOrWhiteSpace(Mark) ? "?" : Mark.Substring(0, 1).ToUpper();
}




