using ETicaretAPI.Data;
using Microsoft.EntityFrameworkCore; // Bu kütüphane şart!

namespace ETicaretAPI.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ETicaretContext _context;
        internal DbSet<T> _dbSet;

        public GenericRepository(ETicaretContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        // 👇 ASENKRON LİSTELEME
        public async Task<List<T>> TumunuGetirAsync(string? includeTablo = null)
        {
            var sorgu = _dbSet.AsQueryable();

            if (!string.IsNullOrEmpty(includeTablo))
            {
                sorgu = sorgu.Include(includeTablo);
            }

            return await sorgu.ToListAsync(); // ToList yerine ToListAsync
        }

        // 👇 ASENKRON TEK KAYIT GETİRME
        public async Task<T?> IdIleGetirAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        // 👇 ASENKRON EKLEME
        public async Task EkleAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        // 👇 ASENKRON GÜNCELLEME
        public async Task GuncelleAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        // 👇 ASENKRON SİLME
        public async Task SilAsync(int id)
        {
            var kayit = await _dbSet.FindAsync(id);
            if (kayit != null)
            {
                _dbSet.Remove(kayit);
                await _context.SaveChangesAsync();
            }
        }
    }
}