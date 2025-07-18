using AutoMapper;
using APPLICATION.Dto.Response;
using INFRASTRUCTURE.Data;
using INFRASTRUCTURE.ExceptionHandler;
using INFRASTRUCTURE.Util;
using Microsoft.EntityFrameworkCore;
using APPLICATION.IRepository;

namespace INFRASTRUCTURE.Repository;

public class GenericRepository<TModel, TSetter, TGetter> : IGenericRepository<TModel, TSetter, TGetter> where TModel : class where TSetter : class where TGetter : class
{
    protected readonly AppDbContext _dbContext;
    protected readonly DbSet<TModel> _dbModel;
    protected readonly IMapper _mapper;

    public GenericRepository(AppDbContext context, IMapper mapper)
    {
        _dbContext = context;
        _dbModel = _dbContext.Set<TModel>();
        _mapper = mapper;
    }
    public IQueryable<TModel> Query(string filters = "", string relations = "")
    {
        return QueryBuilder<TModel>.Apply(_dbModel.AsQueryable(), filters, relations);
    }

    public async Task<List<TGetter>> GetAllAsync()
    {
        return _mapper.Map<List<TGetter>>(await _dbModel.ToListAsync());
    }

    public async Task<PaginationResponseDto<TGetter>> Paginate(int page, int rows, Func<IQueryable<TModel>, IQueryable<TModel>>? query = null)
    {
        // Get raw query
        var rawQuery = _dbModel.AsQueryable();

        if (query != null)
        {
            rawQuery = query(rawQuery);
        }

        // Calculate total rows
        var totalRows = await rawQuery.CountAsync();

        // Calculate total pages
        var totalPages = (int)Math.Ceiling(totalRows / (double)rows);

        // Paginate
        var items = await rawQuery
            .Skip((page - 1) * rows)
            .Take(rows)
            .ToListAsync();

        var result = _mapper.Map<List<TGetter>>(items);

        return new PaginationResponseDto<TGetter>
        {
            Data = result,
            PaginationMeta = new PaginationMetaDto
            {
                Page = page,
                Rows = rows,
                TotalPages = totalPages
            }
        };
    }
    
    public async Task<List<TGetter>> GetByChunk(int page, int rows)
    {
        return _mapper.Map<List<TGetter>>(await _dbModel.Skip(page * rows).Take(rows).ToListAsync());
    }

    public async Task<TGetter?> GetAsync(long id)
    {
        var item = await _dbModel.FindAsync(id) ?? throw new Error404Exception("Item not found");
        return _mapper.Map<TGetter>(item);
    }

    public async Task<TGetter?> GetAsync(string id)
    {
        var item = await _dbModel.FindAsync(id) ?? throw new Error404Exception("Item not found");
        return _mapper.Map<TGetter>(item);
    }

    public async Task<TGetter?> CreateAsync(TSetter newItem)
    {
        var item = _mapper.Map<TModel>(newItem);

        await _dbModel.AddAsync(item);
        if (!await Save())
        {
            throw new Error400Exception("Failed to create item");
        }
        return _mapper.Map<TGetter>(item);
    }

    public async Task<List<TGetter>> CreateAllAsync(List<TSetter> newItems)
    {
        var items = _mapper.Map<List<TModel>>(newItems);

        await _dbModel.AddRangeAsync(items);

        if (!await Save())
        {
            throw new Error400Exception("Failed to create items");
        }

        return _mapper.Map<List<TGetter>>(items);
    }

    public async Task<TGetter?> UpdateAsync(long id, TSetter updatedItem)
    {
        var item = await _dbModel.FindAsync(id) ?? throw new Error404Exception("Item not found");

        _mapper.Map(updatedItem, item);

        _dbModel.Update(item);

        if (!await Save())
        {
            throw new Error400Exception("Failed to update item");
        }
        return _mapper.Map<TGetter>(item);
    }

    public async Task<TGetter?> UpdateAsync(string id, TSetter updatedItem)
    {
        var item = await _dbModel.FindAsync(id) ?? throw new Error404Exception("Item not found");

        _mapper.Map(updatedItem, item);

        _dbModel.Update(item);

        if (!await Save())
        {
            throw new Error400Exception("Failed to update item");
        }

        return _mapper.Map<TGetter>(item);
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var item = await _dbModel.FindAsync(id) ?? throw new Error404Exception("Item not found");
        
        _dbModel.Remove(item);

        if (!await Save())
        {
            throw new Error400Exception("Failed to delete item");
        }

        return true;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var item = await _dbModel.FindAsync(id) ?? throw new Error404Exception("Item not found");
        
        _dbModel.Remove(item);

        if (!await Save())
        {
            throw new Error400Exception("Failed to delete item");
        }

        return true;
    }

    public async Task<bool> Save()
    {
        return (await _dbContext.SaveChangesAsync()) > 0;
    }
}