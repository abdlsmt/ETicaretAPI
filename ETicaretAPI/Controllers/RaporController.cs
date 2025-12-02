using ETicaretAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RaporController(IRaporService _service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var rapor = await _service.GenelRaporGetirAsync();
        return Ok(rapor);
    }
}