
using System.Globalization;
using System.Text.Json;

namespace MyCar;

public class ServiceApi
{
    private static readonly HttpClient Http = CreateHttpClient();

    private const string Endpoint =
        "https://overpass-api.de/api/interpreter";

    private static HttpClient CreateHttpClient()
    {
        var client = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(45)
        };

        client.DefaultRequestHeaders.UserAgent.ParseAdd(
            "MyCar/1.0");

        return client;
    }

    private static readonly Dictionary<string, (double Lat, double Lon)>
        Cities = new()
        {
            ["Москва"] = (55.7558, 37.6173),
            ["Санкт-Петербург"] = (59.9343, 30.3351),
            ["Казань"] = (55.7961, 49.1064),
            ["Сочи"] = (43.5855, 39.7231),
            ["Екатеринбург"] = (56.8389, 60.6057)
        };

    public async Task<List<ServicePlace>> GetPlacesAsync(
        string city,
        string type,
        CancellationToken cancellationToken = default)
    {
        if (!Cities.TryGetValue(city, out var center))
            throw new ArgumentException(
                $"Город не поддерживается: {city}");

        if (type != "fuel" && type != "workshop")
            throw new ArgumentException("Неизвестный тип объектов.");

        string query = BuildQuery(center.Lat, center.Lon, type);

        using var content = new FormUrlEncodedContent(
            new Dictionary<string, string>
            {
                ["data"] = query
            });

        using var response = await Http.PostAsync(
            Endpoint, content, cancellationToken);

        response.EnsureSuccessStatusCode();

        await using var stream =
            await response.Content.ReadAsStreamAsync(cancellationToken);

        using var document = await JsonDocument.ParseAsync(
            stream, cancellationToken: cancellationToken);

        var places = new List<ServicePlace>();
        var seen = new HashSet<string>();

        if (!document.RootElement.TryGetProperty(
                "elements", out var elements))
            return places;

        foreach (var element in elements.EnumerateArray())
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!element.TryGetProperty("tags", out var tags))
                continue;

            double lat = GetNumber(element, "lat");
            double lon = GetNumber(element, "lon");

            if (element.TryGetProperty("center", out var centerElement))
            {
                lat = GetNumber(centerElement, "lat");
                lon = GetNumber(centerElement, "lon");
            }

            if (lat == 0 && lon == 0)
                continue;

            long id = GetLong(element, "id");
            string osmType = GetString(element, "type");

            if (!seen.Add($"{osmType}/{id}"))
                continue;

            string name = GetTag(tags, "name");

            if (string.IsNullOrWhiteSpace(name))
                name = type == "fuel"
                    ? "Заправка без названия"
                    : "СТО без названия";

            places.Add(new ServicePlace
            {
                Id = id,
                Name = name,
                Type = type,
                Address = BuildAddress(tags),
                Latitude = lat,
                Longitude = lon
            });
        }

        return places.OrderBy(p => p.Name).ToList();
    }

    private static string BuildQuery(
        double lat, double lon, string type)
    {
        string latitude = lat.ToString(CultureInfo.InvariantCulture);
        string longitude = lon.ToString(CultureInfo.InvariantCulture);

        string selectors;

        if (type == "fuel")
        {
            selectors = """
                node["amenity"="fuel"](around:15000,LAT,LON);
                way["amenity"="fuel"](around:15000,LAT,LON);
                relation["amenity"="fuel"](around:15000,LAT,LON);
                """;
        }
        else
        {
            selectors = """
                node["shop"="car_repair"](around:15000,LAT,LON);
                way["shop"="car_repair"](around:15000,LAT,LON);
                relation["shop"="car_repair"](around:15000,LAT,LON);
                node["amenity"="car_repair"](around:15000,LAT,LON);
                way["amenity"="car_repair"](around:15000,LAT,LON);
                relation["amenity"="car_repair"](around:15000,LAT,LON);
                """;
        }

        selectors = selectors
            .Replace("LAT", latitude)
            .Replace("LON", longitude);

        return $"""
            [out:json][timeout:35];
            (
                {selectors}
            );
            out center tags;
            """;
    }

    private static string GetTag(JsonElement tags, string key)
    {
        if (tags.ValueKind == JsonValueKind.Object &&
            tags.TryGetProperty(key, out var value))
            return value.ToString();

        return "";
    }

    private static string BuildAddress(JsonElement tags)
    {
        string street = GetTag(tags, "addr:street");
        string house = GetTag(tags, "addr:housenumber");
        string city = GetTag(tags, "addr:city");

        string streetAddress = string.Join(
            " ",
            new[] { street, house }
                .Where(s => !string.IsNullOrWhiteSpace(s)));

        string address = string.Join(
            ", ",
            new[] { streetAddress, city }
                .Where(s => !string.IsNullOrWhiteSpace(s)));

        return string.IsNullOrWhiteSpace(address)
            ? "Адрес не указан в OpenStreetMap"
            : address;
    }

    private static string GetString(
        JsonElement element, string key)
    {
        return element.TryGetProperty(key, out var value)
            ? value.ToString()
            : "";
    }

    private static double GetNumber(
        JsonElement element, string key)
    {
        return element.TryGetProperty(key, out var value) &&
               value.TryGetDouble(out double number)
            ? number
            : 0;
    }

    private static long GetLong(
        JsonElement element, string key)
    {
        return element.TryGetProperty(key, out var value) &&
               value.TryGetInt64(out long number)
            ? number
            : 0;
    }
}

