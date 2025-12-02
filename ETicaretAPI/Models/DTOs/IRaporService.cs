using ETicaretAPI.Models.DTOs;

namespace ETicaretAPI.Services;

public interface IRaporService
{
    Task<DashboardRaporDTO> GenelRaporGetirAsync();
}