

using System.Text.Json;

namespace MyCar;

public partial class AddCarPage : ContentPage
{
    public AddCarPage()
    {
        InitializeComponent();
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(EntryPlate.Text))
        {
            await DisplayAlert("Ошибка", "Введите гос. номер", "ОК");
            return;
        }

        if (PickerBrand.SelectedItem == null)
        {
            await DisplayAlert("Ошибка", "Выберите марку", "ОК");
            return;
        }

        if (string.IsNullOrWhiteSpace(EntryModel.Text))
        {
            await DisplayAlert("Ошибка", "Введите модель", "ОК");
            return;
        }

        var newCar = new Car
        {
            Mark = $"{PickerBrand.SelectedItem} {EntryModel.Text}",
            Number = EntryPlate.Text,
            Year = EntryYear.Text,
            Vin = EntryVin.Text,
            Pts = EntryPts.Text,
            BodyType = "Автомобиль",
            Color = "Не указан"
        };

        string json = Preferences.Default.Get("MyCars", string.Empty);
        List<Car> carsList;

        if (string.IsNullOrEmpty(json))
        {
            carsList = new List<Car>();
        }
        else
        {
            carsList = JsonSerializer.Deserialize < List < Car>>(json) ?? new List<Car>();
        }

        carsList.Add(newCar);
        Preferences.Default.Set("MyCars", JsonSerializer.Serialize(carsList));

        await DisplayAlert("Успех", "Автомобиль добавлен!", "ОК");

        if (Application.Current?.Windows.Count > 0)
        {
            Application.Current.Windows[0].Page = new GaragePage();
        }
    }

    private void BackButton_Clicked(object sender, EventArgs e)
    {
        if (Application.Current?.Windows.Count > 0)
        {
            Application.Current.Windows[0].Page = new GaragePage();
        }
    }
}


