namespace RoverSearch.Models;

public class Result
{
    public string Filename { get; set; }
    public string Title { get; set; }
    public int Season { get; set; }
    public int Episode { get; set; }
    public string Description { get; set; } // for full script
}
