using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace DT.Orders.API.Controllers;

[ApiController]
[Route("api/participant")]
public class ParticipantController : ControllerBase
{
    [HttpPost("prepare")]
    public IActionResult Prepare()
    {
        Log.Information("Prepare request received.");
        return Ok(new { Status = "Prepared" });
    }

    [HttpPost("commit")]
    public IActionResult Commit()
    {
        Log.Information("Commit request received.");
        return Ok(new { Status = "Committed" });
    }

    [HttpPost("rollback")]
    public IActionResult Rollback()
    {
        Log.Information("Rollback request received.");
        return Ok(new { Status = "RolledBack" });
    }
}