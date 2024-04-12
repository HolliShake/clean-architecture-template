
using APPLICATION.IService;
using AutoMapper;
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
}