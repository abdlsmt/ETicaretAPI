namespace ETicaretAPI.Repositories
{
    // <T> demek: "Bana hangi tabloyu verirsen onun için çalışırım" demektir.
    // where T : class => Sadece veritabanı tabloları (class) ile çalışırım kısıtlaması.
    public interface IGenericRepository<T> where T : class
    {
        // 👇 Parametre ekledik: "Yanında getirmemi istediğin tablo var mı?"
        // null gelirse sadece ana tabloyu geti rir.
        // Senkron: List<T> TumunuGetir(...)
        // Asenkron: Task<List<T>> TumunuGetirAsync(...)
        Task<List<T>> TumunuGetirAsync(string? includeTablo = null);

        Task<T?> IdIleGetirAsync(int id);

        Task EkleAsync(T entity); // void yerine Task

        Task GuncelleAsync(T entity);

        Task SilAsync(int id);
    }
}