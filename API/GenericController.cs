
using APPLICATION.IService;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API;

/// <summary>
/// Generic controller for all models.
/// </summary>
/// <typeparam name="TModel">The model type.</typeparam>
/// <typeparam name="IServiceProvider">The service provider type.</typeparam>
/// <typeparam name="ItemDto">The item dto type.</typeparam>
/// <typeparam name="GetDto">The get dto type.</typeparam>
public class GenericController <TModel, IServiceProvider, ItemDto, GetDto> : ControllerBase where IServiceProvider : IGenericService<TModel, ItemDto, GetDto>
{
  
    protected readonly IServiceProvider _repo;

    public GenericController(IServiceProvider repo)
    {
        _repo = repo;
    }

    /// <summary>
    /// Get all data.
    /// </summary>
    /// <returns>All data.</returns>
    protected async Task<ActionResult> GenericGetAll()
    {
        return Ok(await _repo.GetAllAsync());
    }

    /// <summary>
    /// Get paginated data.
    /// </summary>
    /// <param name="page">The page number.</param>
    /// <param name="rows">The number of rows per page.</param>
    /// <returns>A paginated data.</returns>
    protected async Task<ActionResult> GenericGetPaginated(int page, int rows)
    {
        return Ok(await _repo.Paginate(page, rows));
    }

    /// <summary>
    /// Get a chunk of data.
    /// </summary>
    /// <param name="page">The page number.</param>
    /// <param name="rows">The number of rows per page.</param>
    /// <returns>A chunk of data.</returns>
    protected async Task<ActionResult> GenericGetByChunk(int page, int rows)
    {
        return Ok(await _repo.GetByChunk(page, rows));
    }

    /// <summary>
    /// Get a single data.
    /// </summary>
    /// <param name="id">The id of the data.</param>
    /// <returns>A single data.</returns>
    protected async Task<ActionResult> GenericGet(int id)
    {
        return Ok(await _repo.GetAsync(id));
    }

    /// <summary>
    /// Create a new data.
    /// </summary>
    /// <param name="newItem">The new data.</param>
    /// <returns>The created data.</returns>
    protected async Task<ActionResult> GenericCreate(ItemDto newItem)
    {
        return Ok(await _repo.CreateAsync(newItem));
    }

    /// <summary>
    /// Create multiple data.
    /// </summary>
    /// <param name="newItems">The new data.</param>
    /// <returns>The created data.</returns>
    protected async Task<ActionResult> GenericCreateAll(List<ItemDto> newItems)
    {
        return Ok(await _repo.CreateAllAsync(newItems));
    }
    
    /// <summary>
    /// Update a data.
    /// </summary>
    /// <param name="id">The id of the data.</param>
    /// <param name="item">The updated data.</param>
    /// <returns>The updated data.</returns>
    protected async Task<ActionResult> GenericUpdate(int id, ItemDto item)
    {
        return Ok(await _repo.UpdateSync(id, item));
    }

    /// <summary>
    /// Delete a data.
    /// </summary>
    /// <param name="id">The id of the data.</param>
    /// <returns>The deleted data.</returns>
    protected async Task<ActionResult> GenericDelete(int id)
    {
        return Ok(await _repo.DeleteSync(id));
    }
}