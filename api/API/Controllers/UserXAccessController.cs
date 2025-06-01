
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using APPLICATION.Dto.UserXAccess;
using APPLICATION.Dto.Response;
using APPLICATION.IService;
using DOMAIN.Model;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class UserXAccessController : GenericController<UserXAccess, IUserXAccessService, UserXAccessDto, GetUserXAccessDto>
{
    public UserXAccessController(IUserXAccessService repo):base(repo)
    {
    }

    /****************** ACTION ROUTES ******************/
    
    /// <summary>
    /// Get all data.
    /// </summary>
    /// <returns>A list of all UserXAccesss</returns>
    /// <response code="200">When all UserXAccesss are successfully retrieved</response>
    /// <response code="401">When the user is not authenticated</response>
    /// <response code="403">When the user is not authorized</response>
    /// <response code="500">When an unexpected error occurs during retrieval</response>
    [HttpGet("all", Name = "getAllUserXAccesss")]
    [SwaggerOperation(OperationId = "getAllUserXAccesss")]
    [ProducesResponseType<List<GetUserXAccessDto>>(StatusCodes.Status200OK)]
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
    /// <response code="200">When the paginated UserXAccesss are successfully retrieved</response>
    /// <response code="401">When the user is not authenticated</response>
    /// <response code="403">When the user is not authorized</response>
    /// <response code="500">When an unexpected error occurs during retrieval</response>
    [HttpGet("paginate")]
    [SwaggerOperation(OperationId = "getPaginatedUserXAccesss")]
    [ProducesResponseType<PaginationResponseDto<GetUserXAccessDto>>(StatusCodes.Status200OK)]
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
    /// <returns>A chunked collection of UserXAccesss</returns>
    /// <response code="200">When the UserXAccesss are successfully retrieved</response>
    /// <response code="401">When the user is not authenticated</response>
    /// <response code="403">When the user is not authorized</response>
    /// <response code="500">When an unexpected error occurs during retrieval</response>
    [HttpGet("chunk/{page:int}/{rows:int}")]
    [SwaggerOperation(OperationId = "getUserXAccesssByChunk")]
    [ProducesResponseType<List<GetUserXAccessDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetByChunkAction(int page, int rows)
    {
        return await GenericGetByChunk(page, rows);
    }
    
    /// <summary>
    /// Get specific data (UserXAccess) by id.
    /// </summary>
    /// <param name="id">The ID of the UserXAccess to retrieve</param>
    /// <returns>The UserXAccess with the specified ID</returns>
    /// <response code="200">When the UserXAccess is successfully retrieved</response>
    /// <response code="404">When the UserXAccess with specified ID is not found</response>
    /// <response code="500">When an unexpected error occurs during retrieval</response>
    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "getUserXAccessById")]
    [ProducesResponseType<GetUserXAccessDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetAction(long id)
    {
        return await GenericGet(id);
    }
    
    /// <summary>
    /// Creates new UserXAccess entry.
    /// </summary>
    /// <param name="item">The UserXAccess data to create</param>
    /// <returns>The created UserXAccess</returns>
    /// <response code="200">When the UserXAccess is successfully created</response>
    /// <response code="400">When the provided data is invalid</response>
    /// <response code="401">When the user is not authenticated</response>
    /// <response code="403">When the user is not authorized</response>
    /// <response code="500">When an unexpected error occurs during creation</response>
    [HttpPost("create")]
    [SwaggerOperation(OperationId = "createUserXAccess")]
    [ProducesResponseType<GetUserXAccessDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> CreateAction(UserXAccessDto item)
    {
        return await GenericCreate(item);
    }
    
    /// <summary>
    /// Creates multiple instance of UserXAccess.
    /// </summary>
    /// <param name="items">List of UserXAccess data to create</param>
    /// <returns>List of created UserXAccesss</returns>
    /// <response code="200">When the UserXAccesss are successfully created</response>
    /// <response code="400">When the provided data is invalid</response>
    /// <response code="401">When the user is not authenticated</response>
    /// <response code="403">When the user is not authorized</response>
    /// <response code="500">When an unexpected error occurs during creation</response>
    [HttpPost("insert")]
    [SwaggerOperation(OperationId = "createAllUserXAccesss")]
    [ProducesResponseType<List<GetUserXAccessDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> CreateAllAction(List<UserXAccessDto> items)
    {
        return await GenericCreateAll(items);
    }
    
    /// <summary>
    /// Updates single property of UserXAccess.
    /// </summary>
    /// <param name="id">The ID of the UserXAccess to patch</param>
    /// <param name="item">The UserXAccess property to update</param>
    /// <returns>The patched UserXAccess</returns>
    /// <response code="200">When the UserXAccess property is successfully updated</response>
    /// <response code="400">When the provided data is invalid</response>
    /// <response code="401">When the user is not authenticated</response>
    /// <response code="403">When the user is not authorized</response>
    /// <response code="404">When the UserXAccess with specified ID is not found</response>
    /// <response code="500">When an unexpected error occurs during update</response>
    [HttpPatch("patch/{id:long}")]
    [SwaggerOperation(OperationId = "patchUserXAccess")]
    [ProducesResponseType<GetUserXAccessDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> PatchAction(long id, UserXAccessDto item)
    {
        return await GenericUpdate(id, item);
    }
    
    /// <summary>
    /// Updates multiple property of UserXAccess.
    /// </summary>
    /// <param name="id">The ID of the UserXAccess to update</param>
    /// <param name="item">The UserXAccess data to update</param>
    /// <returns>The updated UserXAccess</returns>
    /// <response code="200">When the UserXAccess is successfully updated</response>
    /// <response code="400">When the provided data is invalid</response>
    /// <response code="401">When the user is not authenticated</response>
    /// <response code="403">When the user is not authorized</response>
    /// <response code="404">When the UserXAccess with specified ID is not found</response>
    /// <response code="500">When an unexpected error occurs during update</response>
    [HttpPut("update/{id:long}")]
    [SwaggerOperation(OperationId = "updateUserXAccess")]
    [ProducesResponseType<GetUserXAccessDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> UpdateAction(long id, UserXAccessDto item)
    {
        return await GenericUpdate(id, item);
    }
    
    /// <summary>
    /// Deletes single UserXAccess entry.
    /// </summary>
    /// <param name="id">The ID of the UserXAccess to delete</param>
    /// <returns>204 No Content response on successful deletion</returns>
    /// <response code="204">When the UserXAccess is successfully deleted</response>
    /// <response code="401">When the user is not authenticated</response>
    /// <response code="403">When the user is not authorized</response>
    /// <response code="404">When the UserXAccess with specified ID is not found</response>
    /// <response code="500">When an unexpected error occurs during deletion</response>
    [HttpDelete("delete/{id:long}")]
    [SwaggerOperation(OperationId = "deleteUserXAccess")]
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
