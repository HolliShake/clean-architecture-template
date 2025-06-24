using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using APPLICATION.Dto.RoleAction;
using APPLICATION.Dto.Response;
using APPLICATION.IService;
using DOMAIN.Model;

namespace API.Controllers;


/// <summary>
/// Controller for the RoleAction model.
/// </summary>
[ApiController]
[Route("[controller]")]
public class RoleActionController : GenericController<RoleAction, IRoleActionService, RoleActionDto, GetRoleActionDto>
{
    /// <summary>
    /// Constructor for the RoleActionController.
    /// </summary>
    /// <param name="repo"></param>
    public RoleActionController(IRoleActionService repo):base(repo)
    {
    }

    /****************** ACTION ROUTES ******************/
    
    /// <summary>
    /// Get all data.
    /// </summary>
    /// <returns>A list of all RoleActions</returns>
    /// <response code="200">When all RoleActions are successfully retrieved</response>
    /// <response code="401">When the user is not authenticated</response>
    /// <response code="403">When the user is not authorized</response>
    /// <response code="500">When an unexpected error occurs during retrieval</response>
    [HttpGet("all", Name = "getAllRoleActions")]
    [SwaggerOperation(OperationId = "getAllRoleActions")]
    [ProducesResponseType<List<GetRoleActionDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
    /// <response code="200">When the paginated RoleActions are successfully retrieved</response>
    /// <response code="401">When the user is not authenticated</response>
    /// <response code="403">When the user is not authorized</response>
    /// <response code="500">When an unexpected error occurs during retrieval</response>
    [HttpGet("paginate")]
    [SwaggerOperation(OperationId = "getPaginatedRoleActions")]
    [ProducesResponseType<PaginationResponseDto<GetRoleActionDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetPaginatedAction([FromQuery] int page=1, [FromQuery] int rows=10)
    {
        return await GenericGetPaginated(page, rows);
    }

    /// <summary>
    /// Get data in chunks based on page number and rows per page.
    /// </summary>
    /// <param name="page">The page number to retrieve</param>
    /// <param name="rows">The number of rows per page</param>
    /// <returns>A chunked collection of RoleActions</returns>
    /// <response code="200">When the RoleActions are successfully retrieved</response>
    /// <response code="401">When the user is not authenticated</response>
    /// <response code="403">When the user is not authorized</response>
    /// <response code="500">When an unexpected error occurs during retrieval</response>
    [HttpGet("chunk/{page:int}/{rows:int}")]
    [SwaggerOperation(OperationId = "getRoleActionsByChunk")]
    [ProducesResponseType<List<GetRoleActionDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetByChunkAction(int page, int rows)
    {
        return await GenericGetByChunk(page, rows);
    }
    
    /// <summary>
    /// Get specific data (RoleAction) by id.
    /// </summary>
    /// <param name="id">The ID of the RoleAction to retrieve</param>
    /// <returns>The RoleAction with the specified ID</returns>
    /// <response code="200">When the RoleAction is successfully retrieved</response>
    /// <response code="404">When the RoleAction with specified ID is not found</response>
    /// <response code="500">When an unexpected error occurs during retrieval</response>
    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "getRoleActionById")]
    [ProducesResponseType<GetRoleActionDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetAction(long id)
    {
        return await GenericGet(id);
    }
    
    /// <summary>
    /// Creates new RoleAction entry.
    /// </summary>
    /// <param name="item">The RoleAction data to create</param>
    /// <returns>The created RoleAction</returns>
    /// <response code="200">When the RoleAction is successfully created</response>
    /// <response code="400">When the provided data is invalid</response>
    /// <response code="401">When the user is not authenticated</response>
    /// <response code="403">When the user is not authorized</response>
    /// <response code="500">When an unexpected error occurs during creation</response>
    [HttpPost("create")]
    [SwaggerOperation(OperationId = "createRoleAction")]
    [ProducesResponseType<GetRoleActionDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> CreateAction(RoleActionDto item)
    {
        return await GenericCreate(item);
    }
    
    /// <summary>
    /// Creates multiple instance of RoleAction.
    /// </summary>
    /// <param name="items">List of RoleAction data to create</param>
    /// <returns>List of created RoleActions</returns>
    /// <response code="200">When the RoleActions are successfully created</response>
    /// <response code="400">When the provided data is invalid</response>
    /// <response code="401">When the user is not authenticated</response>
    /// <response code="403">When the user is not authorized</response>
    /// <response code="500">When an unexpected error occurs during creation</response>
    [HttpPost("insert")]
    [SwaggerOperation(OperationId = "createAllRoleActions")]
    [ProducesResponseType<List<GetRoleActionDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> CreateAllAction(List<RoleActionDto> items)
    {
        return await GenericCreateAll(items);
    }
    
    /// <summary>
    /// Updates single property of RoleAction.
    /// </summary>
    /// <param name="id">The ID of the RoleAction to patch</param>
    /// <param name="item">The RoleAction property to update</param>
    /// <returns>The patched RoleAction</returns>
    /// <response code="200">When the RoleAction property is successfully updated</response>
    /// <response code="400">When the provided data is invalid</response>
    /// <response code="401">When the user is not authenticated</response>
    /// <response code="403">When the user is not authorized</response>
    /// <response code="404">When the RoleAction with specified ID is not found</response>
    /// <response code="500">When an unexpected error occurs during update</response>
    [HttpPatch("patch/{id:long}")]
    [SwaggerOperation(OperationId = "patchRoleAction")]
    [ProducesResponseType<GetRoleActionDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> PatchAction(long id, RoleActionDto item)
    {
        return await GenericUpdate(id, item);
    }
    
    /// <summary>
    /// Updates multiple property of RoleAction.
    /// </summary>
    /// <param name="id">The ID of the RoleAction to update</param>
    /// <param name="item">The RoleAction data to update</param>
    /// <returns>The updated RoleAction</returns>
    /// <response code="200">When the RoleAction is successfully updated</response>
    /// <response code="400">When the provided data is invalid</response>
    /// <response code="401">When the user is not authenticated</response>
    /// <response code="403">When the user is not authorized</response>
    /// <response code="404">When the RoleAction with specified ID is not found</response>
    /// <response code="500">When an unexpected error occurs during update</response>
    [HttpPut("update/{id:long}")]
    [SwaggerOperation(OperationId = "updateRoleAction")]
    [ProducesResponseType<GetRoleActionDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> UpdateAction(long id, RoleActionDto item)
    {
        return await GenericUpdate(id, item);
    }
    
    /// <summary>
    /// Deletes single RoleAction entry.
    /// </summary>
    /// <param name="id">The ID of the RoleAction to delete</param>
    /// <returns>204 No Content response on successful deletion</returns>
    /// <response code="204">When the RoleAction is successfully deleted</response>
    /// <response code="401">When the user is not authenticated</response>
    /// <response code="403">When the user is not authorized</response>
    /// <response code="404">When the RoleAction with specified ID is not found</response>
    /// <response code="500">When an unexpected error occurs during deletion</response>
    [HttpDelete("delete/{id:long}")]
    [SwaggerOperation(OperationId = "deleteRoleAction")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteAction(long id)
    {
        return await GenericDelete(id);
    }
}
