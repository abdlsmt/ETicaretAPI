using ETicaretAPI.Models.DTOs;
using ETicaretAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.Controllers;


[Route("api/[controller]")]
[ApiController]
[Authorize]
public class SiparisController(ISiparisService _service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var siparisler = await _service.TumunuGetirAsync();
        return Ok(siparisler);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] SiparisOlusturDTO dto)
    {
        // Try-Catch sildik, ExceptionMiddleware halledecek.
        await _service.SiparisOlusturAsync(dto);
        return Ok("Sipariş başarıyla oluşturuldu.");
    }
}