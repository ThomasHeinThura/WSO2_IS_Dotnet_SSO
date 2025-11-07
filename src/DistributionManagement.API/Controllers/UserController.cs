using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DistributionManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "yks_admin")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;

    public UserController(ILogger<UserController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetAllUsers()
    {
        var users = new[]
        {
            new { Username = "yks", Email = "yks@example.com", Role = "yks_admin" },
            new { Username = "yks1", Email = "yks1@example.com", Role = "yks_test" },
            new { Username = "bimdevops", Email = "bimdevops@example.com", Role = "yks_user" }
        };
        return Ok(users);
    }
}
