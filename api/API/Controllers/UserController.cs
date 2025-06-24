
using APPLICATION.Dto.Response;
using APPLICATION.Dto.User;
using APPLICATION.IService;
using DOMAIN.Model;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers;

/// <summary>
/// Controller for the User model.
/// </summary>
[ApiController]
[Route("Api/[controller]")]
public class UserController : GenericController<User, IUserService, UserDto, GetUserDto>
{
    /// <summary>
    /// Constructor for the UserController.
    /// </summary>
    /// <param name="repo"></param>
    public UserController(IUserService repo):base(repo)
    {
    }

    /****************** ACTION ROUTES ******************/

    /// <summary>
    /// Get all data.
    /// </summary>
    /// <returns>A list of all users</returns>
    /// <response code="200">When all users are successfully retrieved</response>
    /// <response code="401">When the user is not authenticated</response>
    /// <response code="403">When the user is not authorized</response>
    /// <response code="500">When an unexpected error occurs during retrieval</response>
    [HttpGet("all", Name = "getAllUsers")]
    [SwaggerOperation(OperationId = "getAllUsers")]
    [ProducesResponseType<List<GetUserDto>>(StatusCodes.Status200OK)]
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
    /// <response code="200">When the paginated users are successfully retrieved</response>
    /// <response code="401">When the user is not authenticated</response>
    /// <response code="403">When the user is not authorized</response>
    /// <response code="500">When an unexpected error occurs during retrieval</response>
    [HttpGet("paginate")]
    [SwaggerOperation(OperationId = "getPaginatedUsers")]
    [ProducesResponseType<PaginationResponseDto<GetUserDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetPaginatedAction([FromQuery] int page=1, [FromQuery] int rows=10)
    {
        return await GenericGetPaginated(page, rows);
    }

    /// <summary>
    /// Get data by chunk.
    /// </summary>
    /// <param name="page">The page number.</param>
    /// <param name="rows">The number of rows per page.</param>
    /// <returns>A chunk of data.</returns>
    /// <response code="200">When the chunk of users are successfully retrieved</response>
    /// <response code="401">When the user is not authenticated</response>
    /// <response code="403">When the user is not authorized</response>
    /// <response code="500">When an unexpected error occurs during retrieval</response>
    [HttpGet("chunk/{page:int}/{rows:int}")]
    [SwaggerOperation(OperationId = "getUsersByChunk")]
    [ProducesResponseType<List<GetUserDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetByChunkAction(int page, int rows)
    {
        return await GenericGetByChunk(page, rows);
    }

    /// <summary>
    /// Get data by id.
    /// </summary>
    /// <param name="id">The id of the user.</param>
    /// <returns>A user.</returns>
    /// <response code="200">When the user is successfully retrieved</response>
    /// <response code="401">When the user is not authenticated</response>
    /// <response code="403">When the user is not authorized</response>
    /// <response code="500">When an unexpected error occurs during retrieval</response>
    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "getUserById")]
    [ProducesResponseType<GetUserDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetAction(long id)
    {
        return await GenericGet(id);
    }

    /// <summary>
    /// Create a new user.
    /// </summary>
    /// <param name="item">The user to create.</param>
    /// <returns>The created user.</returns>
    /// <response code="200">When the user is successfully created</response>
    /// <response code="400">When the user is not valid</response>
    /// <response code="401">When the user is not authenticated</response>
    /// <response code="403">When the user is not authorized</response>
    /// <response code="500">When an unexpected error occurs during creation</response>
    [HttpPost("create")]
    [SwaggerOperation(OperationId = "createUser")]
    [ProducesResponseType<GetUserDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> CreateAction(UserDto item)
    {
        return await GenericCreate(item);
    }

    /// <summary>
    /// Create multiple users.
    /// </summary>
    /// <param name="items">The users to create.</param>
    /// <returns>The created users.</returns>
    /// <response code="200">When the users are successfully created</response>
    /// <response code="400">When the users are not valid</response>
    /// <response code="401">When the users are not authenticated</response>
    /// <response code="403">When the users are not authorized</response>
    /// <response code="500">When an unexpected error occurs during creation</response>
    [HttpPost("insert")]
    [SwaggerOperation(OperationId = "createAllUsers")]
    [ProducesResponseType<List<GetUserDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> CreateAllAction(List<UserDto> items)
    {
        return await GenericCreateAll(items);
    }

    /// <summary>
    /// Updates single property of User.
    /// </summary>
    /// <param name="id">The ID of the user to patch</param>
    /// <param name="item">The user property to update</param>
    /// <returns>The patched user</returns>
    /// <response code="200">When the user property is successfully updated</response>
    /// <response code="400">When the provided data is invalid</response>
    /// <response code="401">When the user is not authenticated</response>
    /// <response code="403">When the user is not authorized</response>
    /// <response code="404">When the user with specified ID is not found</response>
    /// <response code="500">When an unexpected error occurs during update</response>
    [HttpPatch("patch/{id:long}")]
    [SwaggerOperation(OperationId = "patchUser")]
    [ProducesResponseType<GetUserDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> PatchAction(long id, UserDto item)
    {
        return await GenericUpdate(id, item);
    }

    /// <summary>
    /// Updates multiple properties of User.
    /// </summary>
    /// <param name="id">The ID of the user to update</param>
    /// <param name="item">The user properties to update</param>
    /// <returns>The updated user</returns>
    /// <response code="200">When the user properties are successfully updated</response>
    /// <response code="400">When the provided data is invalid</response>
    /// <response code="401">When the user is not authenticated</response>
    /// <response code="403">When the user is not authorized</response>
    /// <response code="404">When the user with specified ID is not found</response>
    /// <response code="500">When an unexpected error occurs during update</response>
    [HttpPut("update/{id}")]
    [SwaggerOperation(OperationId = "updateUser")]
    [ProducesResponseType<GetUserDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> UpdateAction(string id, UserDto item)
    {
        return await GenericUpdate(id, item);
    }

    /// <summary>
    /// Delete a user.
    /// </summary>
    /// <param name="id">The ID of the user to delete</param>
    /// <returns>The deleted user</returns>
    /// <response code="200">When the user is successfully deleted</response>
    /// <response code="401">When the user is not authenticated</response>
    /// <response code="403">When the user is not authorized</response>
    /// <response code="404">When the user with specified ID is not found</response>
    /// <response code="500">When an unexpected error occurs during deletion</response>
    [HttpDelete("delete/{id}")]
    [SwaggerOperation(OperationId = "deleteUser")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteAction(string id)
    {
        return await GenericDelete(id);
    }
}