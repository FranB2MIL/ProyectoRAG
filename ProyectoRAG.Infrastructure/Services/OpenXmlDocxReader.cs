using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using ProyectoRAG.Application.Interfaces;
using ProyectoRAG.Domain.Entities;

namespace ProyectoRAG.Infrastructure.Services;

public class OpenXmlDocxReader : IDocxReader
{
    public IEnumerable<LoreEntry> ReadLoreEntries(Stream fileStream)
{
    var entries = new List<LoreEntry>();
    string currentSection = "Untitled Section";

    using var wordDoc = WordprocessingDocument.Open(fileStream, false);
    var body = wordDoc.MainDocumentPart?.Document?.Body;
    if (body == null) return entries;

    foreach (var paragraph in body.Elements<Paragraph>())
    {
        var styleId = paragraph.ParagraphProperties?.ParagraphStyleId?.Val?.Value;
        var text = paragraph.InnerText.Trim();

        if (string.IsNullOrWhiteSpace(text))
            continue;

        if (styleId != null && styleId.StartsWith("Heading"))
        {
            currentSection = text;
            continue;
        }

        var firstRun = paragraph.Elements<Run>().FirstOrDefault();
        bool startsItalic = firstRun?.RunProperties?.Italic != null;

        string subTopic = "";
        string content = text;

        if (startsItalic)
        {
            var colonIndex = text.IndexOf(':');
            if (colonIndex > 0 && colonIndex < 100)
            {
                subTopic = text[..colonIndex].Trim();
                content = text[(colonIndex + 1)..].Trim();
            }
        }

        entries.Add(new LoreEntry
        {
            Section = currentSection,
            SubTopic = subTopic,
            Content = content
        });
    }

    return entries;
}
}