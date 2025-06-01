using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using APPLICATION.Dto.Role;
using APPLICATION.Dto.Response;
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
    /// <returns>A list of all roles</returns>
    /// <response code="200">When all roles are successfully retrieved</response>
    /// <response code="401">When the user is not authenticated</response>
    /// <response code="403">When the user is not authorized</response>
    /// <response code="500">When an unexpected error occurs during retrieval</response>
    [HttpGet("all", Name = "getAllRoles")]
    [SwaggerOperation(OperationId = "getAllRoles")]
    [ProducesResponseType<List<GetRoleDto>>(StatusCodes.Status200OK)]
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
    /// <response code="200">When the paginated roles are successfully retrieved</response>
    /// <response code="401">When the user is not authenticated</response>
    /// <response code="403">When the user is not authorized</response>
    /// <response code="500">When an unexpected error occurs during retrieval</response>
    [HttpGet("paginate")]
    [SwaggerOperation(OperationId = "getPaginatedRoles")]
    [ProducesResponseType<PaginationResponseDto<GetRoleDto>>(StatusCodes.Status200OK)]
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
    /// <returns>A chunked collection of roles</returns>
    /// <response code="200">When the roles are successfully retrieved</response>
    /// <response code="401">When the user is not authenticated</response>
    /// <response code="403">When the user is not authorized</response>
    /// <response code="500">When an unexpected error occurs during retrieval</response>
    [HttpGet("chunk/{page:int}/{rows:int}")]
    [SwaggerOperation(OperationId = "getRolesByChunk")]
    [ProducesResponseType<List<GetRoleDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetByChunkAction(int page, int rows)
    {
        return await GenericGetByChunk(page, rows);
    }
    
    /// <summary>
    /// Get specific data (Role) by id.
    /// </summary>
    /// <param name="id">The ID of the role to retrieve</param>
    /// <returns>The role with the specified ID</returns>
    /// <response code="200">When the role is successfully retrieved</response>
    /// <response code="404">When the role with specified ID is not found</response>
    /// <response code="500">When an unexpected error occurs during retrieval</response>
    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "getRoleById")]
    [ProducesResponseType<GetRoleDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetAction(long id)
    {
        return await GenericGet(id);
    }
    
    /// <summary>
    /// Creates new Role entry.
    /// </summary>
    /// <param name="item">The role data to create</param>
    /// <returns>The created role</returns>
    /// <response code="200">When the role is successfully created</response>
    /// <response code="400">When the provided data is invalid</response>
    /// <response code="401">When the user is not authenticated</response>
    /// <response code="403">When the user is not authorized</response>
    /// <response code="500">When an unexpected error occurs during creation</response>
    [HttpPost("create")]
    [SwaggerOperation(OperationId = "createRole")]
    [ProducesResponseType<GetRoleDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> CreateAction(RoleDto item)
    {
        return await GenericCreate(item);
    }
    
    /// <summary>
    /// Creates multiple instance of Role.
    /// </summary>
    /// <param name="items">List of role data to create</param>
    /// <returns>List of created roles</returns>
    /// <response code="200">When the roles are successfully created</response>
    /// <response code="400">When the provided data is invalid</response>
    /// <response code="401">When the user is not authenticated</response>
    /// <response code="403">When the user is not authorized</response>
    /// <response code="500">When an unexpected error occurs during creation</response>
    [HttpPost("insert")]
    [SwaggerOperation(OperationId = "createAllRoles")]
    [ProducesResponseType<List<GetRoleDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> CreateAllAction(List<RoleDto> items)
    {
        return await GenericCreateAll(items);
    }
    
    /// <summary>
    /// Updates single property of Role.
    /// </summary>
    /// <param name="id">The ID of the role to patch</param>
    /// <param name="item">The role property to update</param>
    /// <returns>The patched role</returns>
    /// <response code="200">When the role property is successfully updated</response>
    /// <response code="400">When the provided data is invalid</response>
    /// <response code="401">When the user is not authenticated</response>
    /// <response code="403">When the user is not authorized</response>
    /// <response code="404">When the role with specified ID is not found</response>
    /// <response code="500">When an unexpected error occurs during update</response>
    [HttpPatch("patch/{id:long}")]
    [SwaggerOperation(OperationId = "patchRole")]
    [ProducesResponseType<GetRoleDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> PatchAction(long id, RoleDto item)
    {
        return await GenericUpdate(id, item);
    }
    
    /// <summary>
    /// Updates multiple property of Role.
    /// </summary>
    /// <param name="id">The ID of the role to update</param>
    /// <param name="item">The role data to update</param>
    /// <returns>The updated role</returns>
    /// <response code="200">When the role is successfully updated</response>
    /// <response code="400">When the provided data is invalid</response>
    /// <response code="401">When the user is not authenticated</response>
    /// <response code="403">When the user is not authorized</response>
    /// <response code="404">When the role with specified ID is not found</response>
    /// <response code="500">When an unexpected error occurs during update</response>
    [HttpPut("update/{id:long}")]
    [SwaggerOperation(OperationId = "updateRole")]
    [ProducesResponseType<GetRoleDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> UpdateAction(long id, RoleDto item)
    {
        return await GenericUpdate(id, item);
    }
    
    /// <summary>
    /// Deletes single Role entry.
    /// </summary>
    /// <param name="id">The ID of the role to delete</param>
    /// <returns>204 No Content response on successful deletion</returns>
    /// <response code="204">When the role is successfully deleted</response>
    /// <response code="401">When the user is not authenticated</response>
    /// <response code="403">When the user is not authorized</response>
    /// <response code="404">When the role with specified ID is not found</response>
    /// <response code="500">When an unexpected error occurs during deletion</response>
    [HttpDelete("delete/{id:long}")]
    [SwaggerOperation(OperationId = "deleteRole")]
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
