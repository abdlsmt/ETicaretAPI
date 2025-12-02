using ETicaretAPI.Models.DTOs;
using ETicaretAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.Controllers; // File-Scoped Namespace


[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UrunController(IUrunService _service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var urunler = await _service.TumunuGetirAsync();
        return Ok(urunler);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Post([FromBody] UrunEkleDTO dto)
    {
        await _service.EkleAsync(dto);
        return Ok("Ürün eklendi.");
    }
}