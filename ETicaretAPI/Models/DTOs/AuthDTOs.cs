namespace ETicaretAPI.Models.DTOs;

// Kayıt için gerekli veriler
public record RegisterDTO(string Email, string Password, string Ad, string Soyad);

// Giriş için gerekli veriler
public record LoginDTO(string Email, string Password);