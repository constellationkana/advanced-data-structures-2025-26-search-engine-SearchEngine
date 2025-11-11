using Microsoft.AspNetCore.Mvc;
using RoverSearch.Models;
using System.Diagnostics;
using RoverSearch.Services;
using System.IO;
using System.Text.RegularExpressions;

namespace RoverSearch.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly SearchService _search;

    public HomeController(ILogger<HomeController> logger, SearchService search)
    {
        _logger = logger;
        _search = search;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Search(string query)
    {
        var results = _search.Search(query);
        return View(results);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    // ✅ NEW: View episode details
    [HttpGet]
    public IActionResult Details(string filename, string query)
    {
        if (string.IsNullOrEmpty(filename))
            return RedirectToAction("Index");

        string path = Path.Combine(Directory.GetCurrentDirectory(), "Data", filename);

        if (!System.IO.File.Exists(path))
            return NotFound();

        string content = System.IO.File.ReadAllText(path);

        // ✅ Multi-word highlighting
        if (!string.IsNullOrEmpty(query))
        {
            // Split query into words (ignore extra spaces)
            var words = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            foreach (var word in words.Distinct())
            {
                content = Regex.Replace(
                    content,
                    Regex.Escape(word),
                    match => $"<mark>{match.Value}</mark>",
                    RegexOptions.IgnoreCase
                );
            }
        }

        // Extract episode info
        string title = Regex.Match(content, @"^title:\s*(.+)", RegexOptions.IgnoreCase | RegexOptions.Multiline).Groups[1].Value.Trim();
        int.TryParse(Regex.Match(content, @"^season:\s*(\d+)", RegexOptions.IgnoreCase | RegexOptions.Multiline).Groups[1].Value, out int season);
        int.TryParse(Regex.Match(content, @"^episode:\s*(\d+)", RegexOptions.IgnoreCase | RegexOptions.Multiline).Groups[1].Value, out int episode);

        var model = new Result
        {
            Filename = filename,
            Title = string.IsNullOrEmpty(title) ? Path.GetFileNameWithoutExtension(filename) : title,
            Season = season,
            Episode = episode,
            Description = content
        };

        ViewData["Query"] = query;

        return View(model);
    }
    

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}