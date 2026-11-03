using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace GameBackend.Controller;

[ApiController]
[Route("api/jwt-test")]
[Authorize]
public class JwtTestController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            Message = "JWT authentication successful",
            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            Username = User.Identity?.Name
        });
    }
}