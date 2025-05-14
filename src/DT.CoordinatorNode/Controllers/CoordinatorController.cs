using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace CoordinatorNode.Controllers;

public class CoordinatorController : Controller
{
    private readonly HttpClient _httpClient = new HttpClient();
    private readonly List<string> _participantsAddresses = new()
    {
        "http://localhost:5017/",
        "http://localhost:5018/",
        "http://localhost:5019/",
    };

    [HttpPost("start-transaction")]
    public async Task<IActionResult> StartTransaction()
    {
        Log.Information("Starting 2PC transaction.");
        
        var prepareResults = await Task.WhenAll(
            _participantsAddresses.Select(url => _httpClient.PostAsync($"{url}/prepare", null))
        );

        if (prepareResults.All(r => r.IsSuccessStatusCode))
        {
            await Task.WhenAll(
                _participantsAddresses.Select(url => _httpClient.PostAsync($"{url}/commit", null))
            );
            Log.Information("Transaction committed.");
            return Ok("Transaction committed.");
        }
        else
        {

            await Task.WhenAll(
                _participantsAddresses.Select(url => _httpClient.PostAsync($"{url}/rollback", null))
            );
            Log.Error("Transaction rolled back.");
            return BadRequest("Transaction rolled back.");
        }
    }
    
}