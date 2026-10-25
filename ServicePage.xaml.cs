using System.Collections.ObjectModel;
using System.Globalization;

namespace MyCar;

public partial class ServicePage : ContentPage
{
    private readonly ServiceApi _serviceApi = new();
    private readonly ObservableCollection<ServicePlace> _places = new();

    private CancellationTokenSource? _loadCts;
    private string _selectedType = "fuel";

    public ServicePage()
    {
        InitializeComponent();

        PlacesList.ItemsSource = _places;

        ShowFuelTab();

        Loaded += async (_, _) => await LoadPlacesAsync();
    }

    private async void OnFuelTabClicked(object sender, EventArgs e)
    {
        _selectedType = "fuel";
        ShowFuelTab();
        await LoadPlacesAsync();
    }

    private async void OnServiceTabClicked(object sender, EventArgs e)
    {
        _selectedType = "workshop";
        ShowServiceTab();
        await LoadPlacesAsync();
    }

    private async void OnCityChanged(object sender, EventArgs e)
    {
        if (CityPicker.SelectedItem is not null)
            await LoadPlacesAsync();
    }

    private void ShowFuelTab()
    {
        FuelTab.BackgroundColor = Color.FromArgb("#B17457");
        FuelTab.TextColor = Colors.White;

        ServiceTab.BackgroundColor = Color.FromArgb("#D8D2C2");
        ServiceTab.TextColor = Color.FromArgb("#4A4947");
    }

    private void ShowServiceTab()
    {
        ServiceTab.BackgroundColor = Color.FromArgb("#B17457");
        ServiceTab.TextColor = Colors.White;

        FuelTab.BackgroundColor = Color.FromArgb("#D8D2C2");
        FuelTab.TextColor = Color.FromArgb("#4A4947");
    }

    private async Task LoadPlacesAsync()
    {
        if (CityPicker.SelectedItem is not string city)
            return;

        _loadCts?.Cancel();
        _loadCts?.Dispose();
        _loadCts = new CancellationTokenSource();

        var token = _loadCts.Token;

        try
        {
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;
            StatusLabel.Text = "Загружаем данные карты...";
            _places.Clear();

            var result = await _serviceApi.GetPlacesAsync(
                city, _selectedType, token);

            token.ThrowIfCancellationRequested();

            foreach (var place in result)
                _places.Add(place);

            StatusLabel.Text = result.Count == 0
                ? "Ничего не найдено. Попробуй другой город."
                : $"Найдено объектов: {result.Count}";
        }
        catch (OperationCanceledException)
        {
            // Запрос отменён, например при выборе другой вкладки.
        }
        catch (Exception ex)
        {
            StatusLabel.Text = "Не удалось загрузить данные.";

            await DisplayAlert(
                "Ошибка API",
                ex.Message,
                "OK");
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
        }
    }

    private async void OnOpenMapClicked(object sender, EventArgs e)
    {
        if (sender is not Button button ||
            button.CommandParameter is not ServicePlace place)
            return;

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
            await DisplayAlert("Ошибка", ex.Message, "OK");
        }
    }

    protected override void OnDisappearing()
    {
        _loadCts?.Cancel();
        base.OnDisappearing();
    }
}