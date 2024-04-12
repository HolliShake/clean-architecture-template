using APPLICATION.IService;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API;

public class GenericController <TModel, IServiceProvider, ItemDto, GetDto> : ControllerBase where IServiceProvider : IGenericService<TModel>
{
    protected readonly IMapper _mapper;
    protected readonly IServiceProvider _repo;

    public GenericController(IMapper mapper, IServiceProvider repo)
    {
        _mapper = mapper;
        _repo = repo;
    }

    protected async Task<ActionResult> GenericGetAll()
    {
        var result = /**/
            await _repo.GetAllAsync();

        return Ok(_mapper.Map<ICollection<GetDto>>(result));
    }
    
    protected async Task<ActionResult> GenericGetByChunk(int sizeMax)
    {
        var result = /**/
            await _repo.GetByChunk(sizeMax);

        return Ok(_mapper.Map<ICollection<GetDto>>(result));
    }

    protected async Task<ActionResult> GenericGet(int id)
    {
        var result = /**/
            await _repo.GetAsync(id);

        return (result != null) 
            ? Ok(_mapper.Map<GetDto>(result))
            : NotFound();
    }

    protected async Task<ActionResult> GenericCreate<ItemDto>(ItemDto newItem)
    {
        var model = _mapper.Map<TModel>(newItem);

        var result = /**/
            await _repo.CreateAsync(model);

        return (result)
            ? Ok(_mapper.Map<GetDto>(model))
            : BadRequest("Something went wrong!");
    }

    protected async Task<ActionResult> GenericCreateAll<ItemDto>(List<ItemDto> newItems)
    {
        var model = _mapper.Map<IList<TModel>>(newItems);

        var result = /**/
            await _repo.CreateAllAsync(model);

        return (result)
            ? Ok(_mapper.Map<ICollection<GetDto>>(model))
            : BadRequest("Something went wrong!");
    }
    
    protected async Task<ActionResult> GenericUpdate<ItemDto>(int id, ItemDto item)
    {
        var record = await _repo.GetAsync(id);

        if (record == null)
        {
            return NotFound();
        }

        var model = _mapper.Map(item, record);

        var result = /**/
            await _repo.UpdateSync(model);

        return (result)
            ? Ok(_mapper.Map<GetDto>(model))
            : BadRequest("Something went wrong!");
    }

    protected async Task<ActionResult> GenericDelete(int id)
    {
        var record = await _repo.GetAsync(id);

        if (record == null)
        {
            return NotFound();
        }

        var result = /**/
            await _repo.DeleteSync(record);

        return (result)
            ? NoContent()
            : BadRequest("Something went wrong!");
    }
    
    /****************** ACTION ROUTES ******************/
    /// <summary>
    /// Get all data.
    /// </summary>
    /// <remarks>(From GenericController)</remarks>
    /// <returns>Array[TItem]</returns>
    [Authorize]
    [HttpGet("/[controller]/all")]
    public async Task<ActionResult> GetAllAction()
    {
        return await GenericGetAll();
    }
    
    /// <summary>
    /// Get 1st to n (where n := size(parameter)) data.
    /// </summary>
    /// <remarks>(From GenericController)</remarks>
    /// <returns>Array[Item]</returns>
    [Authorize]
    [HttpGet("/[controller]/chunk/{size:int}")]
    public async Task<ActionResult> GetByChunk(int size)
    {
        return await GenericGetByChunk(size);
    }
    
    /// <summary>
    /// Get specific data by id.
    /// </summary>
    /// <remarks>(From GenericController)</remarks>
    /// <returns>Array[T]></returns>
    [Authorize]
    [HttpGet("/[controller]/{id:int}")]
    public async Task<ActionResult> GetAction(int id)
    {
        return await GenericGet(id);
    }
    
    /// <summary>
    /// Creates new item.
    /// </summary>
    /// <remarks>(From GenericController)</remarks>
    /// <returns>TItem</returns>
    [Authorize]
    [HttpPost($"/[controller]/create")]
    public async Task<ActionResult> CreateAction(ItemDto item)
    {
        return await GenericCreate(item);
    }
    
    /// <summary>
    /// Creates multiple item.
    /// </summary>
    /// <remarks>(From GenericController)</remarks>
    /// <returns>Array[TItem]</returns>
    [Authorize]
    [HttpPost("/[controller]/insert")]
    public async Task<ActionResult> CreateAllAction(List<ItemDto> items)
    {
        return await GenericCreateAll(items);
    }
    
    /// <summary>
    /// Updates single property of an item.
    /// </summary>
    /// <remarks>(From GenericController)</remarks>
    /// <returns>TItem</returns>
    [Authorize]
    [HttpPatch("/[controller]/patch/{id:int}")]
    public async Task<ActionResult> PatchAction(int id, ItemDto item)
    {
        return await GenericUpdate(id, item);
    }
    
    /// <summary>
    /// Updates multiple property of an item.
    /// </summary>
    /// <remarks>(From GenericController)</remarks>
    /// <returns>TItem</returns>
    [Authorize]
    [HttpPut("/[controller]/update/{id:int}")]
    public async Task<ActionResult> UpdateAction(int id, ItemDto item)
    {
        return await GenericUpdate(id, item);
    }
    
    /// <summary>
    /// Deletes an item.
    /// </summary>
    /// <remarks>(From GenericController)</remarks>
    /// <returns>Null</returns>
    [Authorize]
    [HttpDelete("/[controller]/delete/{id:int}")]
    public async Task<ActionResult> UpdateUser(int id)
    {
        return await GenericDelete(id);
    }
}