
namespace MyCar;

public class ServicePlace
{
    public long Id { get; set; }
    public string Name { get; set; } = "Без названия";
    public string Type { get; set; } = "";
    public string Address { get; set; } = "Адрес не указан";
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Coordinates =>
        $"{Latitude:F5}, {Longitude:F5}";
    public string DisplayType =>
        Type == "fuel" ? "Заправка" : "СТО";
}
