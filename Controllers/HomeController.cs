using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using simple_app.Models;
using System.Text;  // Add this for Encoding
using System.Net.Http; // Required for HttpClient and StringContent
using System.Text.Json; // For JSON serialization

namespace simple_app.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AppDbContext _context;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
        _context = new AppDbContext();
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpGet]
    public IActionResult AddPerson()
    {
        var people = _context.People?.ToList(); // List of people
        return View(people); // Pass the list to the view
    }

    [HttpPost]
    public async Task<IActionResult> AddPerson(Person person)
    {
        if (ModelState.IsValid)
        {
            // Add person to the database
            _context.People?.Add(person);
            await _context.SaveChangesAsync();

            // Optionally, use ViewBag to show a message
            ViewBag.Message = $"Person {person.Name} added successfully!";

            // Now, send the POST request to http://localhost:8080/
            using (var client = new HttpClient())
            {
                // Create the data to send as JSON
                var data = new
                {
                    id = person.Id // Use an anonymous object for JSON serialization
                };

                // Serialize the object to JSON using System.Text.Json
                var json = JsonSerializer.Serialize(data);

                // Create StringContent with the JSON data and set the media type to application/json
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                try
                {
                    var response = await client.PostAsync("http://simple-server:8080/person", content);

                    if (response.IsSuccessStatusCode)
                    {
                        // Optionally, handle the success
                        Console.WriteLine("POST request successful!");
                    }
                    else
                    {
                        // Log failure to console
                        Console.WriteLine($"POST request failed with status code: {response.StatusCode}-{response.ReasonPhrase}");
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception (error) to the console
                    Console.WriteLine($"Error occurred during POST request: {ex.Message}");
                }
            }
        }

        // Fetch the updated list of people and return the view
        var people = _context.People?.ToList();
        return View(people);
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
