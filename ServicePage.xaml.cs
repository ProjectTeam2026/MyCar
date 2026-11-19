
using System.Collections.ObjectModel;
using System.Globalization;
using Yandex.MapKit.Maui;

namespace MyCar;

public partial class ServicePage : ContentPage
{
    private readonly ServiceApi _serviceApi = new();
    private readonly ObservableCollection<ServicePlace> _places = new();

    private CancellationTokenSource? _loadCts;
    private string _selectedType = "fuel";

    private static readonly List<ServicePlace> TowServices = new()
    {
        new ServicePlace
        {
            Name = "Российское автомобильное товарищество (РАТ)",
            Type = "tow",
            Phone = "8 800 200-05-60"
        },
        new ServicePlace
        {
            Name = "Российское автомобильное товарищество (РАТ)",
            Type = "tow",
            Phone = "8 800 555-05-60"
        },
        new ServicePlace
        {
            Name = "МИР Ассистанс",
            Type = "tow",
            Phone = "8 800 555-04-57"
        }
    };

    private static readonly Dictionary<string, (double Lat, double Lon)> CityCoordinates = new()
    {
        ["Москва"] = (55.7558, 37.6173),
        ["Санкт-Петербург"] = (59.9343, 30.3351),
        ["Казань"] = (55.7961, 49.1064),
        ["Сочи"] = (43.5855, 39.7231),
        ["Екатеринбург"] = (56.8389, 60.6057)
    };

    public ServicePage()
    {
        InitializeComponent();

        PlacesList.ItemsSource = _places;

        ShowFuelTab();

        Loaded += async (_, _) =>
        {
            await LoadPlacesAsync();
        };
    }

    private async void OnFuelTabClicked(object sender, EventArgs e)
    {
        _selectedType = "fuel";
        ShowSelectedTab(FuelTab);
        await LoadPlacesAsync();
    }

    private async void OnServiceTabClicked(object sender, EventArgs e)
    {
        _selectedType = "workshop";
        ShowSelectedTab(ServiceTab);
        await LoadPlacesAsync();
    }

    private async void OnWashTabClicked(object sender, EventArgs e)
    {
        _selectedType = "wash";
        ShowSelectedTab(WashTab);
        await LoadPlacesAsync();
    }

    private void OnTowTabClicked(object sender, EventArgs e)
    {
        _loadCts?.Cancel();

        _selectedType = "tow";

        ShowSelectedTab(TowTab);

        MapContainer.IsVisible = false;
        MapTitle.IsVisible = false;

        YandexMap.Pins.Clear();

        _places.Clear();

        foreach (var service in TowServices)
        {
            _places.Add(service);
        }

        StatusLabel.Text = "Федеральные службы помощи на дорогах";

        LoadingIndicator.IsRunning = false;
        LoadingIndicator.IsVisible = false;
    }

    private void OnCityChanged(object sender, EventArgs e)
    {
        if (CityPicker.SelectedItem is not string city)
            return;

        if (_selectedType == "tow")
            return;

        _ = LoadPlacesAsync();
    }

    private void ShowFuelTab()
    {
        ShowSelectedTab(FuelTab);
    }

    private void ShowSelectedTab(Button selected)
    {
        Button[] tabs =
        {
            FuelTab,
            ServiceTab,
            WashTab,
            TowTab
        };

        foreach (var tab in tabs)
        {
            bool isSelected = tab == selected;

            tab.BackgroundColor = isSelected
                ? Color.FromArgb("#B17457")
                : Color.FromArgb("#D8D2C2");

            tab.TextColor = isSelected
                ? Colors.White
                : Color.FromArgb("#4A4947");
        }
    }

    private async Task LoadPlacesAsync()
    {
        if (CityPicker.SelectedItem is not string city)
            return;

        if (_selectedType == "tow")
            return;

        _loadCts?.Cancel();

        var currentCts = new CancellationTokenSource();
        _loadCts = currentCts;

        var token = currentCts.Token;

        try
        {
            MapContainer.IsVisible = true;
            MapTitle.IsVisible = true;

            MoveMapToCity(city);

            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            StatusLabel.Text = "Загружаем данные карты...";

            _places.Clear();
            YandexMap.Pins.Clear();

            var result = await _serviceApi.GetPlacesAsync(
                city,
                _selectedType,
                token
            );

            token.ThrowIfCancellationRequested();

            foreach (var place in result)
            {
                _places.Add(place);

                YandexMap.Pins.Add(new MapPin
                {
                    Location = new GeoPoint(
                        place.Latitude,
                        place.Longitude
                    ),

                    Label = place.Name,

                    Color = GetPinColor()
                });
            }

            StatusLabel.Text = result.Count == 0
                ? "Ничего не найдено. Попробуй другой город."
                : $"Найдено объектов: {result.Count}";
        }
        catch (OperationCanceledException)
        {
            // Предыдущий запрос отменён.
        }
        catch (Exception ex)
        {
            if (!token.IsCancellationRequested)
            {
                StatusLabel.Text =
                    "Не удалось загрузить данные. Попробуй ещё раз.";

                await DisplayAlert(
                    "Ошибка API",
                    ex.Message,
                    "OK"
                );
            }
        }
        finally
        {
            if (_loadCts == currentCts)
            {
                LoadingIndicator.IsRunning = false;
                LoadingIndicator.IsVisible = false;

                _loadCts = null;
            }

            currentCts.Dispose();
        }
    }

    private Color GetPinColor()
    {
        return _selectedType switch
        {
            "fuel" => Colors.Orange,
            "workshop" => Colors.Blue,
            "wash" => Colors.Green,
            _ => Colors.Gray
        };
    }

    private void MoveMapToCity(string city)
    {
        if (!CityCoordinates.TryGetValue(city, out var coordinates))
            return;

        YandexMap.MoveTo(
            new GeoPoint(coordinates.Lat, coordinates.Lon),
            zoom: 11
        );
    }

    private async void OnOpenMapClicked(object sender, EventArgs e)
    {
        if (sender is not Button button)
            return;

        if (button.CommandParameter is not ServicePlace place)
            return;

        if (!place.IsMapAvailable)
            return;

        MapContainer.IsVisible = true;
        MapTitle.IsVisible = true;

        YandexMap.MoveTo(
            new GeoPoint(place.Latitude, place.Longitude),
            zoom: 16
        );
    }

    protected override void OnDisappearing()
    {
        _loadCts?.Cancel();

        base.OnDisappearing();
    }

    private void Home_Tapped(object sender, TappedEventArgs e)
    {
        if (Application.Current?.Windows.Count > 0)
        {
            Application.Current.Windows[0].Page =
                new MainScreenPage();
        }
    }

    private void Garage_Tapped(object sender, TappedEventArgs e)
    {
        if (Application.Current?.Windows.Count > 0)
        {
            Application.Current.Windows[0].Page =
                new GaragePage();
        }
    }

    private void Service_Tapped(object sender, TappedEventArgs e)
    {
    }

    private void News_Tapped(object sender, TappedEventArgs e)
    {
        if (Application.Current?.Windows.Count > 0)
        {
            Application.Current.Windows[0].Page =
                new NewsPage();
        }
    }

    private void Profile_Tapped(object sender, TappedEventArgs e)
    {
        if (Application.Current?.Windows.Count > 0)
        {
            Application.Current.Windows[0].Page =
                new ProfilePage();
        }
    }
}
