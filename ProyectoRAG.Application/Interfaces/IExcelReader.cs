using ProyectoRAG.Domain.Entities;
using System.IO;
namespace ProyectoRAG.Application.Interfaces;

public interface IExcelReader
{
    IEnumerable<Vehicle> ReadVehicles(Stream fileStream);
}