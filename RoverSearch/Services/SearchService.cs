using RoverSearch.Models;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace RoverSearch.Services;

public class SearchService
{
    private string path = @".\Data\";

    public SearchService()
    {

    }

    /// <summary>
    /// Naive search implementation
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    public SearchResults Search(string query)
    {
        var sw = new Stopwatch();
        sw.Start();

        var results = new List<Result>();

        if (string.IsNullOrWhiteSpace(query))
        {
            return new SearchResults
            {
                Query = query,
                Results = results,
                Duration = sw.Elapsed
            };
        }

        var words = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        foreach (string file in Directory.GetFiles(path))
        {
            var text = File.ReadAllText(file);

            // Check if any of the words appear (case-insensitive)
            if (words.Any(word => text.Contains(word, StringComparison.OrdinalIgnoreCase)))
            {
                var filename = Path.GetFileName(file);

                // Extract title, season, episode (optional)
                string title = Regex.Match(text, @"^title:\s*(.+)", RegexOptions.IgnoreCase | RegexOptions.Multiline).Groups[1].Value.Trim();
                int.TryParse(Regex.Match(text, @"^season:\s*(\d+)", RegexOptions.IgnoreCase | RegexOptions.Multiline).Groups[1].Value, out int season);
                int.TryParse(Regex.Match(text, @"^episode:\s*(\d+)", RegexOptions.IgnoreCase | RegexOptions.Multiline).Groups[1].Value, out int episode);

                results.Add(new Result
                {
                    Filename = filename,
                    Title = string.IsNullOrEmpty(title) ? Path.GetFileNameWithoutExtension(filename) : title,
                    Season = season,
                    Episode = episode,
                    Description = text
                });
            }
        }

        sw.Stop();

        return new SearchResults
        {
            Query = query,
            Results = results,
            Duration = sw.Elapsed
        };
    }
}
