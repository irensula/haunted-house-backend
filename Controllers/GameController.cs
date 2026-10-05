using Microsoft.AspNetCore.Mvc;

namespace GameBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    [HttpGet]
    public IActionResult GetGame()
    {
        return Ok(new
        {
            name = "Haunted House",
            version = "1.0.0",
            status = "online"
        });
    }
}