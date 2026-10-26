

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
            await DisplayAlertAsync("Ошибка", "Введите гос. номер", "ОК");
            return;
        }

        if (PickerBrand.SelectedItem == null)
        {
            await DisplayAlertAsync("Ошибка", "Выберите марку", "ОК");
            return;
        }

        if (string.IsNullOrWhiteSpace(EntryModel.Text))
        {
            await DisplayAlertAsync("Ошибка", "Введите модель", "ОК");
            return;
        }

        await DisplayAlertAsync("Успех", "Данные введены! (Сохранение пока отключено)", "ОК");

  
        await Shell.Current.GoToAsync("..");
    }
}