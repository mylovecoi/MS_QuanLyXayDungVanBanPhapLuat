using Microsoft.AspNetCore.Mvc;

namespace DanhMucService.Controllers;

[ApiController]
[Route("health")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            service = "DanhMucService",
            status = "OK",
            timestamp = DateTime.UtcNow
        });
    }
}

