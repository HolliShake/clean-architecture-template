using APPLICATION.IService;
using INFRASTRUCTURE.Data;
using Microsoft.EntityFrameworkCore;

namespace INFRASTRUCTURE.Service;

public class GenericService <TModel> : IGenericService<TModel> where TModel : class
{
    private readonly AppDbContext _dbContext;
    private readonly DbSet<TModel> _dbModel;

    public GenericService(AppDbContext context)
    {
        _dbContext = context;
        _dbModel = _dbContext.Set<TModel>();
    }

    public async Task<ICollection<TModel>> GetAllAsync()
    {
        return await _dbModel.ToListAsync();
    }

    public async Task<TModel?> GetAsync(int id)
    {
        return await _dbModel.FindAsync(id);
    }

    public async Task<bool> CreateAsync(TModel newItem)
    {
        await _dbModel.AddAsync(newItem);
        return await Save();
    }

    public async Task<bool> CreateAllAsync(IList<TModel> newItems)
    {
        await _dbModel.AddRangeAsync(newItems);
        return await Save();
    }

    public async Task<bool> UpdateSync(TModel updatedItem)
    {
        _dbModel.Update(updatedItem);
        return await Save();
    }

    public async Task<bool> DeleteSync(TModel oldItem)
    {
        _dbModel.Remove(oldItem);
        return await Save();
    }

    private async Task<bool> Save()
    {
        return ((await _dbContext.SaveChangesAsync()) > 0);
    }
}