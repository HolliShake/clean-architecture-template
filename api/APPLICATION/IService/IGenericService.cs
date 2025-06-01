using System.Numerics;
using APPLICATION.Dto.Response;

namespace APPLICATION.IService;

public interface IGenericService<TModel, TSetter,TGetter>
{
    public Task<List<TGetter>> GetAllAsync();
    public Task<PaginationResponseDto<TGetter>> Paginate(int page, int rows, Func<IQueryable<TModel>, IQueryable<TModel>>? query = null);
    public Task<List<TGetter>> GetByChunk(int page, int rows);
    public Task<TGetter?> GetAsync(long id);
    public Task<TGetter?> GetAsync(string id);
    public Task<TGetter?> CreateAsync(TSetter newItem);
    public Task<List<TGetter>> CreateAllAsync(IList<TSetter> newItems);
    public Task<TGetter?> UpdateSync(long id, TSetter updatedItem);
    public Task<TGetter?> UpdateSync(string id, TSetter updatedItem);
    public Task<bool> DeleteSync(long id);
    public Task<bool> DeleteSync(string id);
}

