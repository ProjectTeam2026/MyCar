using System.Collections.ObjectModel;
using System.Text.Json;

namespace MyCar;

public partial class GaragePage : ContentPage
{
    public ObservableCollection<Car> Cars { get; set; } = new();

    public GaragePage()
    {
        InitializeComponent();
        CarsCollectionView.ItemsSource = Cars;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadCars();
    }

    private void LoadCars()
    {
        string json = Preferences.Default.Get("MyCars", string.Empty);
        Cars.Clear();

        if (string.IsNullOrEmpty(json))
        {
            
            Cars.Add(new Car { Mark = "Toyota Camry", Number = "А123ВС 77", Color = "Белый", Year = "2020", BodyType = "Седан" });
            Cars.Add(new Car { Mark = "BMW X5", Number = "В456СD 77", Color = "Черный", Year = "2019", BodyType = "Внедорожник" });
        }
        else
        {
            
            var savedCars = JsonSerializer.Deserialize < List < Car>>(json);
            if (savedCars != null)
            {
                foreach (var car in savedCars)
                {
                    Cars.Add(car);
                }
            }
        }
    }

    private async void OnCarListClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Список авто", "Пока тут пусто", "OK");
    }

    private void OnAddCarClicked(object sender, EventArgs e)
    {
        if (Application.Current?.Windows.Count > 0)
        {
            Application.Current.Windows[0].Page = new AddCarPage();
        }
    }

    private void Home_Tapped(object sender, TappedEventArgs e)
    {
        if (Application.Current?.Windows.Count > 0) Application.Current.Windows[0].Page = new MainScreenPage();
    }

    private void Garage_Tapped(object sender, TappedEventArgs e)
    {
        
    }

    private void Service_Tapped(object sender, TappedEventArgs e)
    {
        if (Application.Current?.Windows.Count > 0) Application.Current.Windows[0].Page = new ServicePage();
    }

    private void News_Tapped(object sender, TappedEventArgs e)
    {
        if (Application.Current?.Windows.Count > 0) Application.Current.Windows[0].Page = new NewsPage();
    }

    private void Profile_Tapped(object sender, TappedEventArgs e)
    {
        if (Application.Current?.Windows.Count > 0) Application.Current.Windows[0].Page = new ProfilePage();
    }
}

public class Car
{
    public string Mark { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string Year { get; set; } = string.Empty;
    public string BodyType { get; set; } = string.Empty;
    public string Vin { get; set; } = string.Empty;
    public string Pts { get; set; } = string.Empty;

    public string MarkInitial =>
        string.IsNullOrWhiteSpace(Mark) ? "?" : Mark.Substring(0, 1).ToUpper();
}