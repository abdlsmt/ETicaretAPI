# --------------------------------------------------------
# 1. AŞAMA: İNŞAAT (BUILD)
# --------------------------------------------------------
# Microsoft'un SDK (Geliştirme Kiti) imajını indiriyoruz.
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

# Önce sadece proje dosyasını kopyalayıp kütüphaneleri indiriyoruz (Cache için)
COPY ["ETicaretAPI.csproj", "./"]
RUN dotnet restore "ETicaretAPI.csproj"

# Şimdi kalan tüm kodları kopyalıyoruz
COPY . .

# Projeyi derleyip yayınlıyoruz (Publish)
RUN dotnet publish "ETicaretAPI.csproj" -c Release -o /app/publish

# --------------------------------------------------------
# 2. AŞAMA: ÇALIŞTIRMA (RUNTIME)
# --------------------------------------------------------
# Sadece çalıştırma dosyalarını içeren hafif imajı indiriyoruz.
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS final
WORKDIR /app

# İnşaat aşamasından (build) çıkan dosyaları buraya alıyoruz
COPY --from=build /app/publish .

# Port ayarı (Docker içi port)
EXPOSE 8080

# Uygulamayı başlat
ENTRYPOINT ["dotnet", "ETicaretAPI.dll"]