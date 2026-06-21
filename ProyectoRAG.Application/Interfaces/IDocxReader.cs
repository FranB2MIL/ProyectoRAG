using ProyectoRAG.Domain.Entities;

namespace ProyectoRAG.Application.Interfaces;

public interface IDocxReader
{
    IEnumerable<LoreEntry> ReadLoreEntries(Stream fileStream);
}