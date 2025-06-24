using APPLICATION.Dto.Response;

namespace APPLICATION.IRepository;

public interface IGenericRepository<TModel, TSetter,TGetter>
{
    /// <summary>
    /// Apply filters and relations to the query
    /// </summary>
    /// <param name="filters">The filters to apply to the query, in the format of "field[operator]=value|field[operator]=value|..."</param>
    /// <param name="relations">The relations to apply to the query, separated by '|'</param>
    /// <returns>The query with the filters and relations applied</returns>
    public IQueryable<TModel> Query(string filters = "", string relations = "");
    
    /// <summary>
    /// Get all items from the database
    /// </summary>
    /// <returns>List of items</returns>
    public Task<List<TGetter>> GetAllAsync();

    /// <summary>
    /// Paginate the query
    /// </summary>
    /// <param name="page">The page number (zero-based index)</param>
    /// <param name="rows">The number of rows to retrieve per page</param>
    /// <param name="query">The query to paginate</param>
    /// <returns>Paginated items</returns>
    public Task<PaginationResponseDto<TGetter>> Paginate(int page, int rows, Func<IQueryable<TModel>, IQueryable<TModel>>? query = null);
    
    /// <summary>
    /// Get items by chunk for efficient batch processing
    /// </summary>
    /// <param name="page">The page number (zero-based index)</param>
    /// <param name="rows">The number of rows to retrieve per chunk</param>
    /// <returns>A list of items of type TGetter for the specified chunk</returns>
    public Task<List<TGetter>> GetByChunk(int page, int rows);

    /// <summary>
    /// Get an item by its id
    /// </summary>
    /// <param name="id">The id of the item</param>
    /// <returns>The item of type TGetter</returns>
    public Task<TGetter?> GetAsync(long id);

    /// <summary>
    /// Get an item by its id
    /// </summary>
    /// <param name="id">The id of the item</param>
    /// <returns>The item of type TGetter</returns>
    public Task<TGetter?> GetAsync(string id);

    /// <summary>
    /// Create a new item
    /// </summary>
    /// <param name="newItem">The item to create</param>
    /// <returns>The created item of type TGetter</returns>
    public Task<TGetter?> CreateAsync(TSetter newItem);

    /// <summary>
    /// Create multiple items
    /// </summary>
    /// <param name="newItems">The items to create</param>
    /// <returns>The created items of type TGetter</returns>
    public Task<List<TGetter>> CreateAllAsync(List<TSetter> newItems);

    /// <summary>
    /// Update an item by its id
    /// </summary>
    /// <param name="id">The id of the item</param>
    /// <param name="updatedItem">The item to update</param>
    /// <returns>The updated item of type TGetter</returns>
    public Task<TGetter?> UpdateAsync(long id, TSetter updatedItem);

    /// <summary>
    /// Update an item by its id
    /// </summary>
    /// <param name="id">The id of the item</param>
    /// <param name="updatedItem">The item to update</param>
    /// <returns>The updated item of type TGetter</returns>
    public Task<TGetter?> UpdateAsync(string id, TSetter updatedItem);

    /// <summary>
    /// Delete an item by its id
    /// </summary>
    /// <param name="id">The id of the item</param>
    /// <returns>True if the item was deleted, false otherwise</returns>
    public Task<bool> DeleteAsync(long id);

    /// <summary>
    /// Delete an item by its id
    /// </summary>
    /// <param name="id">The id of the item</param>
    /// <returns>True if the item was deleted, false otherwise</returns>
    public Task<bool> DeleteAsync(string id);

    /// <summary>
    /// Save the changes to the database
    /// </summary>
    /// <returns>True if the changes were saved, false otherwise</returns>
    public Task<bool> Save();
}
