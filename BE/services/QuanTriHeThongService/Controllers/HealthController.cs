using Microsoft.AspNetCore.Mvc;

namespace QuanTriHeThongService.Controllers;

[ApiController]
[Route("api/health")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            service = "QuanTriHeThongService",
            status = "OK",
            timestamp = DateTime.UtcNow
        });
    }
}

