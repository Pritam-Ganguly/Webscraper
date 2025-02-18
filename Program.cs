using ClosedXML.Excel;
using HtmlAgilityPack;
using System.Web;
using static ReverseMarkdown.Config;

class Program
{
    public static async Task Main(string[] args)
    {
        string excelFilePath = @"../../../links.xlsx";
        var urlsAndPaths = ReadUrlAndPathsFromExcel(excelFilePath);

        string outputRoot = Path.Combine(Directory.GetCurrentDirectory(), @"../../../output/");
        Directory.CreateDirectory(outputRoot);

        await Parallel.ForEachAsync(urlsAndPaths, async (entry, i) =>
        {
            string folderPath = entry.folderPath;
            string url = entry.url;
            string? content = await ScrapeWebsiteContext(url);
            if (!string.IsNullOrEmpty(content))
            {
                string fileName = GenerateMarkdownFileName(folderPath);
                string? parentFolderPath = Path.GetDirectoryName(folderPath);
                string folderFullPath = Path.Combine(outputRoot, parentFolderPath ?? "/");
                Directory.CreateDirectory(folderFullPath);

                string filePath = Path.Combine(folderFullPath, fileName);
                await File.WriteAllTextAsync(filePath, content, i);
                await Console.Out.WriteLineAsync($"Markdown file saved to: {filePath}");
            }
            else
            {
                await Console.Out.WriteLineAsync($"Failed to scrape content from: {url}");
            }
        });

    }

    private static (string folderPath, string url)[] ReadUrlAndPathsFromExcel(string filePath)
    {
        var urlsAndPaths = new List<(string, string)>();
        using (var workbook = new XLWorkbook(filePath))
        {
            var worksheet = workbook.Worksheet(1);
            var rows = worksheet.RowsUsed();

            foreach (var row in rows)
            {
                var folderCell = row.Cell(2);
                var urlCell = row.Cell(1);

                string folderPath = folderCell.GetValue<string>();
                string url = urlCell.GetValue<string>();

                if(!string.IsNullOrEmpty(url) && !string.IsNullOrEmpty(folderPath))
                {
                    urlsAndPaths.Add((folderPath, url));
                }
            }
        }
        return urlsAndPaths.ToArray();
    }

    private static string GenerateMarkdownFileName(string folderPath)
    {
        string[] pathSegments = folderPath.Split('/');
        string lastSegment = pathSegments.Last();
        string fileName = lastSegment.Replace(Path.DirectorySeparatorChar, '_').Replace(':', '-');
        return fileName;
    }

    private static async Task<string?> ScrapeWebsiteContext(string url)
    {
        try
        {
            using(var httpClient = new HttpClient())
            {
                var response = await httpClient.GetStringAsync(url);
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response);

                var bodyContent = htmlDoc.DocumentNode.SelectSingleNode("//main");

                if(bodyContent != null)
                {
                    return ConvertHtmlToMarkdown(bodyContent, url);
                }
                else
                {
                    await Console.Out.WriteLineAsync("Body content not found");
                    return null;
                }
            }
        }
        catch (Exception ex)
        {
            await Console.Out.WriteLineAsync($"Error scraping {url}: {ex.Message}");
            return null;
        }
    }

    private static string ConvertHtmlToMarkdown(HtmlNode rootNode, string url)
    {
        var markdown = new System.Text.StringBuilder();
        ConvertNodesToMarkdown(rootNode, markdown, url);
        return HttpUtility.HtmlDecode(markdown.ToString().Trim());
    }

    static void ConvertNodesToMarkdown(HtmlNode node, System.Text.StringBuilder markdown, string url)
    {
        var footNotes = new Dictionary<string, string>();
        var config = new ReverseMarkdown.Config
        {
            GithubFlavored = true,
            SuppressDivNewlines = false,
            SmartHrefHandling = false,
            PassThroughTags = Array.Empty<string>(),
            UnknownTags = UnknownTagsOption.Bypass,
            CleanupUnnecessarySpaces = true,
        };
        var converter = new ReverseMarkdown.Converter(config);
        var aTags = node.SelectNodes("//a");
        if(aTags != null)
        {
            foreach(var aTag in aTags)
            {
                string href = aTag.GetAttributeValue("href", "");
                if (href.StartsWith("#"))
                {
                    aTag.SetAttributeValue("href", $"{url}/{href}");
                }
            }
        }
        string convertedText = converter.Convert(node.OuterHtml);
        var footNoteNodes = node.SelectNodes("//sup");

        if(footNoteNodes != null)
        {
            foreach(var footNoteNode in footNoteNodes)
            {
                string footNoteNumber = footNoteNode.InnerText;
                string footNoteLink = converter.Convert(footNoteNode.InnerHtml);
                convertedText = convertedText.Replace($"^{footNoteLink}^", $"[^{footNoteNumber}]");
                if (!footNotes.ContainsKey(footNoteNumber))
                {
                    footNotes[footNoteNumber] = $"[^{footNoteNumber}]: {footNoteLink}";
                }
            }
        }
        markdown.Append(convertedText);
        markdown.AppendLine("\n\n---\n");
        foreach(string footNote in footNotes.Values)
        {
            markdown.AppendLine(footNote);
        }
    }

}