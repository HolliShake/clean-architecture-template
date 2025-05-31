
using Microsoft.AspNetCore.Mvc;
using APPLICATION.Dto.Role;
using APPLICATION.IService;
using DOMAIN.Model;

namespace API.Controllers;

[ApiController]
[Route("Api/[controller]")]
public class RoleController : GenericController<Role, IRoleService, RoleDto, GetRoleDto>
{
    public RoleController(IRoleService repo):base(repo)
    {
    }

    /****************** ACTION ROUTES ******************/
    /// <summary>
    /// Get all data.
    /// </summary>
    /// <returns>Array[Role]</returns>
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
    /// <returns>A paginated data.</returns>
    [HttpGet("paginate")]
    public async Task<ActionResult> GetPaginatedAction([FromQuery] int page=1, [FromQuery] int rows=10)
    {
        return await GenericGetPaginated(page, rows);
    }

    /// <summary>
    /// Get 1st to n (where n := size(parameter)) data.
    /// </summary>
    /// <returns>Array[Role]</returns>
    [HttpGet("chunk/{page:int}/{rows:int}")]
    public async Task<ActionResult> GetByChunk(int page, int rows)
    {
        return await GenericGetByChunk(page, rows);
    }
    
    /// <summary>
    /// Get specific data (Role) by id.
    /// </summary>
    /// <returns>Array[Role]></returns>
    [HttpGet("{id:int}")]
    public async Task<ActionResult> GetAction(int id)
    {
        return await GenericGet(id);
    }
    
    /// <summary>
    /// Creates new Role entry.
    /// </summary>
    /// <returns>Role</returns>
    [HttpPost("create")]
    public async Task<ActionResult> CreateAction(RoleDto item)
    {
        return await GenericCreate(item);
    }
    
    /// <summary>
    /// Creates multiple instance of Role.
    /// </summary>
    /// <returns>Array[Role]</returns>
    [HttpPost("insert")]
    public async Task<ActionResult> CreateAllAction(List<RoleDto> items)
    {
        return await GenericCreateAll(items);
    }
    
    /// <summary>
    /// Updates single property of Role.
    /// </summary>
    /// <remarks>(From GenericController)</remarks>
    /// <returns>Role</returns>
    [HttpPatch("patch/{id:int}")]
    public async Task<ActionResult> PatchAction(int id, RoleDto item)
    {
        return await GenericUpdate(id, item);
    }
    
    /// <summary>
    /// Updates multiple property of Role.
    /// </summary>
    /// <returns>Role</returns>
    [HttpPut("update/{id:int}")]
    public async Task<ActionResult> UpdateAction(int id, RoleDto item)
    {
        return await GenericUpdate(id, item);
    }
    
    /// <summary>
    /// Deletes single Role entry.
    /// </summary>
    /// <returns>Null</returns>
    [HttpDelete("delete/{id:int}")]
    public async Task<ActionResult> DeleteAction(int id)
    {
        return await GenericDelete(id);
    }
}
