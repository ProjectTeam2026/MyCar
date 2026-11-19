using System.Collections.ObjectModel;
using System.Globalization;

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

    private async void OnFuelTabClicked(
        object sender,
        EventArgs e)
    {
        _selectedType = "fuel";

        ShowSelectedTab(FuelTab);

        await LoadPlacesAsync();
    }

    private async void OnServiceTabClicked(
        object sender,
        EventArgs e)
    {
        _selectedType = "workshop";

        ShowSelectedTab(ServiceTab);

        await LoadPlacesAsync();
    }

    private async void OnWashTabClicked(
        object sender,
        EventArgs e)
    {
        _selectedType = "wash";

        ShowSelectedTab(WashTab);

        await LoadPlacesAsync();
    }

    private void OnTowTabClicked(
        object sender,
        EventArgs e)
    {
        _loadCts?.Cancel();

        _selectedType = "tow";

        ShowSelectedTab(TowTab);

        _places.Clear();

        foreach (var service in TowServices)
        {
            _places.Add(service);
        }

        StatusLabel.Text =
            "Федеральные службы помощи на дорогах";

        LoadingIndicator.IsRunning = false;
        LoadingIndicator.IsVisible = false;
    }

    private void OnCityChanged(
        object sender,
        EventArgs e)
    {
        if (CityPicker.SelectedItem is not null &&
            _selectedType != "tow")
        {
            _ = LoadPlacesAsync();
        }
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
        _loadCts?.Dispose();

        var currentCts = new CancellationTokenSource();

        _loadCts = currentCts;

        var token = currentCts.Token;

        try
        {
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            StatusLabel.Text = "Загружаем данные карты...";

            _places.Clear();

            var result = await _serviceApi.GetPlacesAsync(
                city,
                _selectedType,
                token);

            token.ThrowIfCancellationRequested();

            foreach (var place in result)
            {
                _places.Add(place);
            }

            StatusLabel.Text = result.Count == 0
                ? "Ничего не найдено. Попробуй другой город."
                : $"Найдено объектов: {result.Count}";
        }
        catch (OperationCanceledException)
        {
            // Запрос отменён при переключении вкладки или города.
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
                    "OK");
            }
        }
        finally
        {
            if (_loadCts == currentCts)
            {
                LoadingIndicator.IsRunning = false;
                LoadingIndicator.IsVisible = false;
            }

            currentCts.Dispose();
        }
    }

    private async void OnOpenMapClicked(
        object sender,
        EventArgs e)
    {
        if (sender is not Button button ||
            button.CommandParameter is not ServicePlace place)
        {
            return;
        }

        string lat = place.Latitude.ToString(
            CultureInfo.InvariantCulture);

        string lon = place.Longitude.ToString(
            CultureInfo.InvariantCulture);

        string url =
            $"https://www.openstreetmap.org/?mlat={lat}&mlon={lon}#map=17/{lat}/{lon}";

        try
        {
            await Launcher.Default.OpenAsync(url);
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Ошибка",
                ex.Message,
                "OK");
        }
    }

    protected override void OnDisappearing()
    {
        _loadCts?.Cancel();

        base.OnDisappearing();
    }

    private void Home_Tapped(
        object sender,
        TappedEventArgs e)
    {
        if (Application.Current?.Windows.Count > 0)
        {
            Application.Current.Windows[0].Page =
                new MainScreenPage();
        }
    }

    private void Garage_Tapped(
        object sender,
        TappedEventArgs e)
    {
        if (Application.Current?.Windows.Count > 0)
        {
            Application.Current.Windows[0].Page =
                new GaragePage();
        }
    }

    private void Service_Tapped(
        object sender,
        TappedEventArgs e)
    {
    }

    private void News_Tapped(
        object sender,
        TappedEventArgs e)
    {
        if (Application.Current?.Windows.Count > 0)
        {
            Application.Current.Windows[0].Page =
                new NewsPage();
        }
    }

    private void Profile_Tapped(
        object sender,
        TappedEventArgs e)
    {
        if (Application.Current?.Windows.Count > 0)
        {
            Application.Current.Windows[0].Page =
                new ProfilePage();
        }
    }
}