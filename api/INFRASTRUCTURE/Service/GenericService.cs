using APPLICATION.IService;
using APPLICATION.Dto.Response;
using APPLICATION.IRepository;

namespace INFRASTRUCTURE.Service;

public class GenericService <IRepository, TModel, TSetter, TGetter> : IGenericService<TModel, TSetter, TGetter> where IRepository : IGenericRepository<TModel, TSetter, TGetter> where TModel : class where TSetter : class where TGetter : class
{
    protected readonly IRepository _repository;

    public GenericService(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<TGetter>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<PaginationResponseDto<TGetter>> Paginate(int page, int rows, Func<IQueryable<TModel>, IQueryable<TModel>>? query = null)
    {
        return await _repository.Paginate(page, rows, query);
    }
    
    public async Task<List<TGetter>> GetByChunk(int page, int rows)
    {
        return await _repository.GetByChunk(page, rows);
    }

    public async Task<TGetter?> GetAsync(long id)
    {
        return await _repository.GetAsync(id);
    }

    public async Task<TGetter?> GetAsync(string id)
    {
        return await _repository.GetAsync(id);
    }

    public async Task<TGetter?> CreateAsync(TSetter newItem)
    {
        return await _repository.CreateAsync(newItem);
    }

    public async Task<List<TGetter>> CreateAllAsync(List<TSetter> newItems)
    {
        return await _repository.CreateAllAsync(newItems);
    }

    public async Task<TGetter?> UpdateAsync(long id, TSetter updatedItem)
    {
        return await _repository.UpdateAsync(id, updatedItem);
    }

    public async Task<TGetter?> UpdateAsync(string id, TSetter updatedItem)
    {
        return await _repository.UpdateAsync(id, updatedItem);
    }

    public async Task<bool> DeleteAsync(long id)
    {
        return await _repository.DeleteAsync(id);
    }

    public async Task<bool> DeleteAsync(string id)
    {
       return await _repository.DeleteAsync(id);
    }
}