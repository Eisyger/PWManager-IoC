using Microsoft.AspNetCore.Mvc;

namespace PWManager.Controller;

[Route("api/[controller]")]
[ApiController]
public class TestController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new {Message = "Test Testosteron"});
    }
}