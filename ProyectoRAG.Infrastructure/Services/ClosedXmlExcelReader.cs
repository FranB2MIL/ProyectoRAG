using ClosedXML.Excel;
using ProyectoRAG.Application.Interfaces;
using ProyectoRAG.Domain.Entities;

namespace ProyectoRAG.Infrastructure.Services;

public class ClosedXmlExcelReader : IExcelReader
{
    public IEnumerable<Vehicle> ReadVehicles(Stream fileStream)
    {
        using var workbook = new XLWorkbook(fileStream);
        var worksheet = workbook.Worksheet(1);

        var vehicles = new List<Vehicle>();

        var rows = worksheet.RowsUsed().Skip(1); // Skip header row
        foreach (var row in rows)
        {
            var vehicle = new Vehicle
            {
                Branch = row.Cell(1).GetString(),
                Class = row.Cell(2).GetString(),
                Year = row.Cell(3).GetValue<int>(),
                Model = row.Cell(4).GetString(),
                Color = row.Cell(5).GetString(),
                Plate = row.Cell(6).GetString(),
                Kilometers = row.Cell(7).GetValue<int>(),
                SalePrice = row.Cell(8).GetValue<decimal>()
            };
            vehicles.Add(vehicle);
        }
        return vehicles;
    }
}