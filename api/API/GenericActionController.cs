using APPLICATION.IService;
using Microsoft.AspNetCore.Mvc;
using API.Attributes;

namespace API;

[ApiController]
[Route("Api/[controller]")]
public class
    GenericActionController<TModel, IServiceProvider, ItemDto, GetDto> : GenericController<TModel, IServiceProvider,
    ItemDto, GetDto> where IServiceProvider : IGenericService<TModel, ItemDto, GetDto>
{
    public GenericActionController(IServiceProvider repo):base(repo)
    {
    }
    
    /****************** ACTION ROUTES ******************/
    /// <summary>
    /// Get all data.
    /// </summary>
    /// <remarks>(From GenericController)</remarks>
    /// <returns>Array[TItem]</returns>
    // [Casl("Admin:read" )]
    [HttpGet("all")]
    public async Task<ActionResult> GetAllAction()
    {
        return await GenericGetAll();
    }

    /// <summary>
    /// Get paginated data.
    /// </summary>
    /// <param name="page">The page number.</param>
    /// <param name="rows">The number of rows per page.</param>
    /// <returns>A paginated list of data.</returns>
    [Casl("Admin:read")]
    [HttpGet("paginate")]
    public async Task<ActionResult> GetPaginatedAction([FromQuery] int page=1, [FromQuery] int rows=10)
    {
        return await GenericGetPaginated(page, rows);
    }
    
    /// <summary>
    /// Get a chunk of data.
    /// </summary>
    /// <param name="page">The page number.</param>
    /// <param name="rows">The number of rows per page.</param>
    /// <returns>A chunk of data.</returns>
    [Casl("Auth:read", "Admin:read")]
    [HttpGet("chunk/{page:int}/{rows:int}")]
    public async Task<ActionResult> GetByChunk(int page, int rows)
    {
        return await GenericGetByChunk(page, rows);
    }
    
    /// <summary>
    /// Get specific data by id.
    /// </summary>
    /// <remarks>(From GenericController)</remarks>
    /// <returns>Array[T]></returns>
    [Casl("Auth:read", "Admin:read")]
    [HttpGet("{id:long}")]
    public async Task<ActionResult> GetAction(long id)
    {
        return await GenericGet(id);
    }
    
    /// <summary>
    /// Creates new item.
    /// </summary>
    /// <remarks>(From GenericController)</remarks>
    /// <returns>TItem</returns>
    // [Casl("Auth:read", "Admin:read")]
    [HttpPost("create")]
    public async Task<ActionResult> CreateAction(ItemDto item)
    {
        return await GenericCreate(item);
    }
    
    /// <summary>
    /// Creates multiple item.
    /// </summary>
    /// <remarks>(From GenericController)</remarks>
    /// <returns>Array[TItem]</returns>
    [Casl("Auth:read", "Admin:read")]
    [HttpPost("insert")]
    public async Task<ActionResult> CreateAllAction(List<ItemDto> items)
    {
        return await GenericCreateAll(items);
    }
    
    /// <summary>
    /// Updates single property of an item.
    /// </summary>
    /// <remarks>(From GenericController)</remarks>
    /// <returns>TItem</returns>
    [Casl("Auth:read", "Admin:read")]
    [HttpPatch("patch/{id:int}")]
    public async Task<ActionResult> PatchAction(int id, ItemDto item)
    {
        return await GenericUpdate(id, item);
    }
    
    /// <summary>
    /// Updates multiple property of an item.
    /// </summary>
    /// <remarks>(From GenericController)</remarks>
    /// <returns>TItem</returns>
    [Casl("Auth:read", "Admin:read")]
    [HttpPut("update/{id:long}")]
    public async Task<ActionResult> UpdateAction(long id, ItemDto item)
    {
        return await GenericUpdate(id, item);
    }
    
    /// <summary>
    /// Deletes an item.
    /// </summary>
    /// <remarks>(From GenericController)</remarks>
    /// <returns>Null</returns>
    [Casl("Auth:read", "Admin:read")]
    [HttpDelete("delete/{id:long}")]
    public async Task<ActionResult> DeleteAction(long id)
    {
        return await GenericDelete(id);
    }
}