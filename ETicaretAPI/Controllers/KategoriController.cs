using ETicaretAPI.Models.DTOs;
using ETicaretAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.Controllers;


[Route("api/[controller]")]
[ApiController]
[Authorize]
public class KategoriController(IKategoriService _service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var kategoriler = await _service.TumunuGetirAsync();
        return Ok(kategoriler);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Post([FromBody] KategoriEkleDTO dto)
    {
        await _service.EkleAsync(dto);
        return Ok("Kategori eklendi.");
    }
}