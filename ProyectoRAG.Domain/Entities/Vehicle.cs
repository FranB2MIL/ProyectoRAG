namespace ProyectoRAG.Domain.Entities;

public class Vehicle
{
    public string Branch { get; set; } = string.Empty;
    public string Class { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Model { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string Plate { get; set; } = string.Empty;
    public int Kilometers { get; set; }
    public decimal SalePrice { get; set; }

    public string ToEmbeddingText()
    {
        var colorText = string.IsNullOrEmpty(Color) ? "" : $"Color: {Color}, ";
        return $"Vehicle at {Branch} branch: {Model}, class {Class}, year {Year}" +
               $"{colorText}, plate {Plate}, {Kilometers} km, sale price ${SalePrice:N0}.";
    }
}