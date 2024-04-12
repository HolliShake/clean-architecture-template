namespace APPLICATION.IService;

public interface IGenericService<TModel>
{
    public Task<ICollection<TModel>> GetAllAsync();
    public Task<ICollection<TModel>> GetByChunk(int max);
    public Task<TModel?> GetAsync(int id);
    public Task<bool> CreateAsync(TModel newItem);
    public Task<bool> CreateAllAsync(IList<TModel> newItems);
    public Task<bool> UpdateSync(TModel updatedItem);
    public Task<bool> DeleteSync(TModel oldItem);
}

