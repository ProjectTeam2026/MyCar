namespace MyCar;

public class ServicePlace
{
    public long Id { get; set; }

    public string Name { get; set; } = "Без названия";

    public string Type { get; set; } = "";

    public string Address { get; set; } = "Адрес не указан";

    public string Phone { get; set; } = "";

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public string Coordinates =>
        $"{Latitude:F5}, {Longitude:F5}";

    public bool IsPhoneService =>
        Type == "tow";

    public bool IsMapAvailable =>
        Type != "tow";

    public bool HasAddress =>
        Type != "tow";

    public string DisplayType =>
        Type switch
        {
            "fuel" => "Заправка",
            "workshop" => "СТО",
            "wash" => "Автомойка",
            "tow" => "Помощь на дорогах",
            _ => "Сервис"
        };
}