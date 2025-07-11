## Author: Philipp Andrew R. Redondo
## Date: 2021-07-01 22:00:00
## Description: A simple script to generate service, dto, and mapper classes for a given model.
## Modify to suit your needs.


import sys
import json
from argparse import ArgumentParser
from os import mkdir, listdir
from os.path import exists, dirname, abspath, join, isdir, isfile, basename

def capitalize(s:str):
    return s[0].upper() + s[1:]

def get_path_separator():
    if  sys.platform.startswith("win32") or sys.platform.startswith("win64"):
        return "\\"
    # unix?
    return "/"

__install_path__ = abspath(dirname(sys.executable)) if getattr(sys, 'frozen', False) else abspath(dirname(__file__))
PATH_CONFIG = join(__install_path__, "make.info.config")

KEY_API_PATH = "API_PATH"
KEY_APPLICATION_PATH = "APPLICATION_PATH"
KEY_DOMAIN_PATH = "DOMAIN_PATH"
KEY_INFRASTRUCTURE_PATH = "INFRASTRUCTURE_PATH"
# 
KEY_CONTROLLERS = "CONTROLLERS"
KEY_CONTROLLERS_GENERIC_NAME = "GENERIC_NAME"
KEY_CONTROLLERS_PATH = "PATH"
# 
KEY_DTO = "DTO"
KEY_DTO_PATH = "PATH"
KEY_DTO_LIST_PATH = "LIST_PATH"
# 
KEY_REPOSITORY = "REPOSITORY"
KEY_REPOSITORY_IPATH = "IPATH"
KEY_REPOSITORY_PATH = "PATH"
KEY_REPOSITORY_IGENERIC_NAME = "IGENERIC_NAME"
KEY_REPOSITORY_GENERIC_NAME = "GENERIC_NAME"
KEY_REPOSITORY_LIST_PATH = "LIST_PATH"
KEY_REPOSITORY_REPOSITORY_VARIABLE_NAME = "REPOSITORY_VARIABLE"
KEY_REPOSITORY_LIST_PATH = "LIST_PATH"
# 
KEY_SERVICE = "SERVICE"
KEY_SERVICE_IPATH = "IPATH"
KEY_SERVICE_PATH = "PATH"
KEY_SERVICE_IGENERIC_NAME = "IGENERIC_NAME"
KEY_SERVICE_GENERIC_NAME = "GENERIC_NAME"
KEY_SERVICE_SERVICE_VARIABLE_NAME = "SERVICE_VARIABLE"
KEY_SERVICE_LIST_PATH = "LIST_PATH"
# 
KEY_MAPPER = "MAPPER"
KEY_MAPPER_PATH = "PATH"
KEY_MAPPER_SERVICE_VARIABLE_NAME = "SERVICE_VARIABLE"
KEY_MAPPER_LIST_PATH = "LIST_PATH"
# 
KEY_DATA = "DATA"
KEY_DATA_PATH = "PATH"
# 
KEY_MODEL = "MODEL"
KEY_MODEL_PATH = "PATH"
KEY_MODEL_LIST = "LIST"

CONFIG = ({
    KEY_API_PATH: "API",
    KEY_APPLICATION_PATH: "APPLICATION",
    KEY_DOMAIN_PATH: "DOMAIN",
    KEY_INFRASTRUCTURE_PATH: "INFRASTRUCTURE",
    # 
    KEY_CONTROLLERS: {
        KEY_CONTROLLERS_GENERIC_NAME: "GenericController",
        KEY_CONTROLLERS_PATH: "API_PATH/Controllers"
    },
    # 
    KEY_DTO: {
        KEY_DTO_PATH: "APPLICATION_PATH/Dto",
        KEY_DTO_LIST_PATH: "APPLICATION_PATH/AppInjector.cs"
    },
    KEY_REPOSITORY: {
        KEY_REPOSITORY_IPATH: "APPLICATION_PATH/IRepository",
        KEY_REPOSITORY_PATH: "INFRASTRUCTURE_PATH/Repository",
        KEY_REPOSITORY_IGENERIC_NAME: "IGenericRepository",
        KEY_REPOSITORY_GENERIC_NAME: "GenericRepository",
        KEY_REPOSITORY_REPOSITORY_VARIABLE_NAME: "services",
        KEY_REPOSITORY_LIST_PATH: "INFRASTRUCTURE_PATH/InfraInjector.cs"
    },
    KEY_SERVICE: {
        KEY_SERVICE_IPATH: "APPLICATION_PATH/IService",
        KEY_SERVICE_PATH: "INFRASTRUCTURE_PATH/Service",
        KEY_SERVICE_IGENERIC_NAME: "IGenericService",
        KEY_SERVICE_GENERIC_NAME: "GenericService",
        KEY_SERVICE_SERVICE_VARIABLE_NAME: "services",
        KEY_SERVICE_LIST_PATH: "INFRASTRUCTURE_PATH/InfraInjector.cs"
    },
    KEY_MAPPER: {
        KEY_MAPPER_PATH: "APPLICATION_PATH/Mapper",
        KEY_MAPPER_SERVICE_VARIABLE_NAME: "services",
        KEY_MAPPER_LIST_PATH: "APPLICATION_PATH/AppInjector.cs"
    },
    KEY_DATA: {
        KEY_DATA_PATH: "INFRASTRUCTURE_PATH/Data"
    },
    KEY_MODEL: {
        KEY_MODEL_PATH: "DOMAIN_PATH/Model",
        KEY_MODEL_LIST: [
            "User"
        ]
    }
})

try:
    fobj = open(PATH_CONFIG, "r")
    CONFIG = {
        **CONFIG,
        **json.load(fobj)
    }
    fobj.close()
except Exception as e:
    print("make::error: failed to read config file.")
    exit(1)

def get_value_from_namespace(json_namespace:str):
    nested = json_namespace.split('.')[::-1]
    if  len(nested) == 0:
        return None
    
    current = CONFIG[nested.pop()]
    while len(nested) > 0:
        top = nested[-1]
        if  not isinstance(current, dict):
            return None
        
        if  not top in current:
            print("make::warning: namespace not found: {}".format(top))
            return None
        
        current = current[nested.pop()]

    return current

# Validate config
def check_type_namespace(json_namespace:str, t:type):
    nested = json_namespace.split('.')[::-1]
    if  len(nested) == 0:
        return False
    
    current = CONFIG[nested.pop()]
    while len(nested) > 0:
        top = nested[-1]
        if  not isinstance(current, dict):
            return False
        
        if  not top in current:
            print("make::warning: namespace not found: {}".format(top))
            return False
        
        current = current[nested.pop()]

    return isinstance(current, t)

def assert_type_namespace(json_namespace:str, t:type):
    if  not check_type_namespace(json_namespace, t):
        print("[make.info.config]make::error: invalid namespace type: {} (requires {})".format(json_namespace, t.__name__))
        exit(1)

def resolve_path(path:str):
    return path\
        .replace('ROOT_PATH', __install_path__)\
        .replace(KEY_API_PATH, join(__install_path__, CONFIG[KEY_API_PATH]))\
        .replace(KEY_APPLICATION_PATH, join(__install_path__, CONFIG[KEY_APPLICATION_PATH]))\
        .replace(KEY_DOMAIN_PATH, join(__install_path__, CONFIG[KEY_DOMAIN_PATH]))\
        .replace(KEY_INFRASTRUCTURE_PATH, join(__install_path__, CONFIG[KEY_INFRASTRUCTURE_PATH]))\
        .replace('/', get_path_separator())\
        .replace('\\', get_path_separator())

assert_type_namespace(KEY_CONTROLLERS, dict)
assert_type_namespace('CONTROLLERS.GENERIC_NAME', str)
assert_type_namespace('CONTROLLERS.PATH', str)
assert_type_namespace(KEY_DTO, dict)
assert_type_namespace('DTO.PATH', str)
assert_type_namespace('DTO.LIST_PATH', str)
assert_type_namespace(KEY_SERVICE, dict)
assert_type_namespace(KEY_REPOSITORY, dict)
assert_type_namespace('REPOSITORY.IPATH', str)
assert_type_namespace('REPOSITORY.PATH', str)
assert_type_namespace('REPOSITORY.IGENERIC_NAME', str)
assert_type_namespace('REPOSITORY.GENERIC_NAME', str)
assert_type_namespace('REPOSITORY.REPOSITORY_VARIABLE', str)
assert_type_namespace('REPOSITORY.LIST_PATH', str)
assert_type_namespace('SERVICE.IPATH', str)
assert_type_namespace('SERVICE.PATH', str)
assert_type_namespace('SERVICE.IGENERIC_NAME', str)
assert_type_namespace('SERVICE.GENERIC_NAME', str)
assert_type_namespace('SERVICE.SERVICE_VARIABLE', str)
assert_type_namespace('SERVICE.LIST_PATH', str)
assert_type_namespace(KEY_MAPPER, dict)
assert_type_namespace('MAPPER.PATH', str)
assert_type_namespace('MAPPER.SERVICE_VARIABLE', str)
assert_type_namespace('MAPPER.LIST_PATH', str)
assert_type_namespace(KEY_DATA, dict)
assert_type_namespace('DATA.PATH', str)
assert_type_namespace(KEY_MODEL, dict)
assert_type_namespace('MODEL.PATH', str)
assert_type_namespace('MODEL.LIST', list)

# API
PATH_API=resolve_path('API_PATH')
PATH_API_CONTROLLER=resolve_path(get_value_from_namespace('CONTROLLERS.PATH'))
# APP
PATH_APPLICATION=resolve_path('APPLICATION_PATH')
PATH_APPLICATION_DTO=resolve_path(get_value_from_namespace('DTO.PATH'))
PATH_APPLICATION_IREPOSITORY=resolve_path(get_value_from_namespace('REPOSITORY.IPATH'))
PATH_APPLICATION_ISERVICE=resolve_path(get_value_from_namespace('SERVICE.IPATH'))
PATH_APPLICATION_MAPPER=resolve_path(get_value_from_namespace('MAPPER.PATH'))
# DOM
PATH_DOMAIN=resolve_path('DOMAIN_PATH')
PATH_DOMAIN_MODEL=resolve_path(get_value_from_namespace('MODEL.PATH'))
# INF
PATH_INFRASTRUCTURE=resolve_path('INFRASTRUCTURE_PATH')
PATH_INFRASTRUCTURE_DATA=resolve_path(get_value_from_namespace('DATA.PATH'))
PATH_INFRASTRUCTURE_REPOSITORY=resolve_path(get_value_from_namespace('REPOSITORY.PATH'))
PATH_INFRASTRUCTURE_SERVICE=resolve_path(get_value_from_namespace('SERVICE.PATH'))

# 
PATH_MAPPER_LIST_PATH=resolve_path(get_value_from_namespace('MAPPER.LIST_PATH'))
PATH_REPOSITORY_LIST_PATH=resolve_path(get_value_from_namespace('REPOSITORY.LIST_PATH'))
PATH_SERVICE_LIST_PATH=resolve_path(get_value_from_namespace('SERVICE.LIST_PATH'))

VALID_SEARCH_PATHS = [
    PATH_API,
    PATH_API_CONTROLLER,
    PATH_APPLICATION,
    PATH_APPLICATION_DTO,
    PATH_APPLICATION_IREPOSITORY,
    PATH_APPLICATION_ISERVICE,
    PATH_APPLICATION_MAPPER,
    PATH_DOMAIN,
    PATH_DOMAIN_MODEL,
    PATH_INFRASTRUCTURE,
    PATH_INFRASTRUCTURE_DATA,
    PATH_INFRASTRUCTURE_SERVICE,
    # 
    PATH_MAPPER_LIST_PATH,
    PATH_REPOSITORY_LIST_PATH,
    PATH_SERVICE_LIST_PATH
]

print("make::info: checking paths...")
for path in VALID_SEARCH_PATHS:
    if  not exists(path):
        print("make::error: path not found: {}".format(path))
        exit(1)
print("make::info: paths are valid.")

print("""
MAKE INFO:
    - CONFIG PATH: {}
    - API PATH: {}
        - CONTROLLER PATH: {}
    - APPLICATION PATH: {}
        - DTO PATH: {}
        - IREPOSITORY PATH: {}
        - ISERVICE PATH: {}
        - MAPPER PATH: {}
    - DOMAIN PATH: {}
        - MODEL PATH: {}
    - INFRASTRUCTURE PATH: {}
        - DATA PATH: {}
        - REPOSITORY PATH: {}
        - SERVICE PATH: {}
      
    +---------------------------------+
    |        OTHER INFORMATION        |
    +---------------------------------+
      
    [1]. MAPPER  LIST PATH: {}
        Description: This file is used to inject all mappers into the application.
            example:
                services.AddAutoMapper(
                    typeof(UserMapper)
                    // typeof(/*MapperProfile*/)
                );
    [2]. SERVICE LIST PATH: {}
        Description: This file is used to inject all services into the application.
            example:
                services.AddScoped<IUserService, UserService>();
                // services.AddScoped</*IService*/, /*Service*/>();
""".format(
    PATH_CONFIG,
    PATH_API,
    PATH_API_CONTROLLER,
    PATH_APPLICATION,
    PATH_APPLICATION_DTO,
    PATH_APPLICATION_IREPOSITORY,
    PATH_APPLICATION_ISERVICE,
    PATH_APPLICATION_MAPPER,
    PATH_DOMAIN,
    PATH_DOMAIN_MODEL,
    PATH_INFRASTRUCTURE,
    PATH_INFRASTRUCTURE_DATA,
    PATH_INFRASTRUCTURE_REPOSITORY,
    PATH_INFRASTRUCTURE_SERVICE,
    # OTHER
    PATH_MAPPER_LIST_PATH,
    PATH_REPOSITORY_LIST_PATH,
    PATH_SERVICE_LIST_PATH
))

CONTROLLER_TEMPLATE = """
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using {dto-namespace}.{controller-name};
using {dto-namespace}.Response;
using {iservice-namespace};
using {model-namespace};

namespace {controller-namespace};

/// <summary>
/// Controller for the {controller-name} model.
/// </summary>
[ApiController]
[Route("[controller]")]
public class {controller-name}Controller : {generic-name}<{controller-name}, I{controller-name}Service, {controller-name}Dto, Get{controller-name}Dto>
{
    /// <summary>
    /// Constructor for the {controller-name}Controller.
    /// </summary>
    /// <param name="repo"></param>
    public {controller-name}Controller(I{controller-name}Service repo):base(repo)
    {
    }

    /****************** ACTION ROUTES ******************/
    
    /// <summary>
    /// Get all data.
    /// </summary>
    /// <returns>A list of all {controller-name}s</returns>
    /// <response code="200">When all {controller-name}s are successfully retrieved</response>
    /// <response code="401">When the user is not authenticated</response>
    /// <response code="403">When the user is not authorized</response>
    /// <response code="500">When an unexpected error occurs during retrieval</response>
    [HttpGet("all", Name = "getAll{controller-name}s")]
    [SwaggerOperation(OperationId = "getAll{controller-name}s")]
    [ProducesResponseType<List<Get{controller-name}Dto>>(StatusCodes.Status200OK)]
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
    /// <response code="200">When the paginated {controller-name}s are successfully retrieved</response>
    /// <response code="401">When the user is not authenticated</response>
    /// <response code="403">When the user is not authorized</response>
    /// <response code="500">When an unexpected error occurs during retrieval</response>
    [HttpGet("paginate")]
    [SwaggerOperation(OperationId = "getPaginated{controller-name}s")]
    [ProducesResponseType<PaginationResponseDto<Get{controller-name}Dto>>(StatusCodes.Status200OK)]
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
    /// <returns>A chunked collection of {controller-name}s</returns>
    /// <response code="200">When the {controller-name}s are successfully retrieved</response>
    /// <response code="401">When the user is not authenticated</response>
    /// <response code="403">When the user is not authorized</response>
    /// <response code="500">When an unexpected error occurs during retrieval</response>
    [HttpGet("chunk/{page:int}/{rows:int}")]
    [SwaggerOperation(OperationId = "get{controller-name}sByChunk")]
    [ProducesResponseType<List<Get{controller-name}Dto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetByChunkAction(int page, int rows)
    {
        return await GenericGetByChunk(page, rows);
    }
    
    /// <summary>
    /// Get specific data ({controller-name}) by id.
    /// </summary>
    /// <param name="id">The ID of the {controller-name} to retrieve</param>
    /// <returns>The {controller-name} with the specified ID</returns>
    /// <response code="200">When the {controller-name} is successfully retrieved</response>
    /// <response code="404">When the {controller-name} with specified ID is not found</response>
    /// <response code="500">When an unexpected error occurs during retrieval</response>
    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "get{controller-name}ById")]
    [ProducesResponseType<Get{controller-name}Dto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetAction(long id)
    {
        return await GenericGet(id);
    }
    
    /// <summary>
    /// Creates new {controller-name} entry.
    /// </summary>
    /// <param name="item">The {controller-name} data to create</param>
    /// <returns>The created {controller-name}</returns>
    /// <response code="200">When the {controller-name} is successfully created</response>
    /// <response code="400">When the provided data is invalid</response>
    /// <response code="401">When the user is not authenticated</response>
    /// <response code="403">When the user is not authorized</response>
    /// <response code="500">When an unexpected error occurs during creation</response>
    [HttpPost("create")]
    [SwaggerOperation(OperationId = "create{controller-name}")]
    [ProducesResponseType<Get{controller-name}Dto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> CreateAction({controller-name}Dto item)
    {
        return await GenericCreate(item);
    }
    
    /// <summary>
    /// Creates multiple instance of {controller-name}.
    /// </summary>
    /// <param name="items">List of {controller-name} data to create</param>
    /// <returns>List of created {controller-name}s</returns>
    /// <response code="200">When the {controller-name}s are successfully created</response>
    /// <response code="400">When the provided data is invalid</response>
    /// <response code="401">When the user is not authenticated</response>
    /// <response code="403">When the user is not authorized</response>
    /// <response code="500">When an unexpected error occurs during creation</response>
    [HttpPost("insert")]
    [SwaggerOperation(OperationId = "createAll{controller-name}s")]
    [ProducesResponseType<List<Get{controller-name}Dto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> CreateAllAction(List<{controller-name}Dto> items)
    {
        return await GenericCreateAll(items);
    }
    
    /// <summary>
    /// Updates single property of {controller-name}.
    /// </summary>
    /// <param name="id">The ID of the {controller-name} to patch</param>
    /// <param name="item">The {controller-name} property to update</param>
    /// <returns>The patched {controller-name}</returns>
    /// <response code="200">When the {controller-name} property is successfully updated</response>
    /// <response code="400">When the provided data is invalid</response>
    /// <response code="401">When the user is not authenticated</response>
    /// <response code="403">When the user is not authorized</response>
    /// <response code="404">When the {controller-name} with specified ID is not found</response>
    /// <response code="500">When an unexpected error occurs during update</response>
    [HttpPatch("patch/{id:long}")]
    [SwaggerOperation(OperationId = "patch{controller-name}")]
    [ProducesResponseType<Get{controller-name}Dto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> PatchAction(long id, {controller-name}Dto item)
    {
        return await GenericUpdate(id, item);
    }
    
    /// <summary>
    /// Updates multiple property of {controller-name}.
    /// </summary>
    /// <param name="id">The ID of the {controller-name} to update</param>
    /// <param name="item">The {controller-name} data to update</param>
    /// <returns>The updated {controller-name}</returns>
    /// <response code="200">When the {controller-name} is successfully updated</response>
    /// <response code="400">When the provided data is invalid</response>
    /// <response code="401">When the user is not authenticated</response>
    /// <response code="403">When the user is not authorized</response>
    /// <response code="404">When the {controller-name} with specified ID is not found</response>
    /// <response code="500">When an unexpected error occurs during update</response>
    [HttpPut("update/{id:long}")]
    [SwaggerOperation(OperationId = "update{controller-name}")]
    [ProducesResponseType<Get{controller-name}Dto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> UpdateAction(long id, {controller-name}Dto item)
    {
        return await GenericUpdate(id, item);
    }
    
    /// <summary>
    /// Deletes single {controller-name} entry.
    /// </summary>
    /// <param name="id">The ID of the {controller-name} to delete</param>
    /// <returns>204 No Content response on successful deletion</returns>
    /// <response code="204">When the {controller-name} is successfully deleted</response>
    /// <response code="401">When the user is not authenticated</response>
    /// <response code="403">When the user is not authorized</response>
    /// <response code="404">When the {controller-name} with specified ID is not found</response>
    /// <response code="500">When an unexpected error occurs during deletion</response>
    [HttpDelete("delete/{id:long}")]
    [SwaggerOperation(OperationId = "delete{controller-name}")]
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
"""

IREPOSITORY_TEMPLATE = """
using {dto-namespace}.{irepository-name};
using {model-namespace};

namespace {irepository-namespace};

public interface I{irepository-name}Repository : {igeneric-name}<{repository-name}, {repository-name}Dto, Get{repository-name}Dto>
{
}
"""

REPOSITORY_TEMPLATE = """
using AutoMapper;
using {dto-namespace}.{repository-name};
using {irepository-namespace};
using {model-namespace};
using {data-namespace};

namespace {repository-namespace};

public class {repository-name}Repository:{generic-name}<{repository-name}, {repository-name}Dto, Get{repository-name}Dto>, I{repository-name}Repository
{
    public {repository-name}Repository(AppDbContext context, IMapper mapper):base(context, mapper)
    {
    }
}

"""

ISERVICE_TEMPLATE = """
using {dto-namespace}.{service-name};
using {model-namespace};

namespace {iservice-namespace};

public interface I{service-name}Service:{igeneric-name}<{service-name}, {service-name}Dto, Get{service-name}Dto>
{
}
"""

SERVICE_TEMPLATE = """
using {dto-namespace}.{service-name};
using {iservice-namespace};
using {model-namespace};
using {irepository-namespace};

namespace {service-namespace};

public class {service-name}Service:{generic-name}<I{service-name}Repository, {service-name}, {service-name}Dto, Get{service-name}Dto>, I{service-name}Service
{
    public {service-name}Service(I{service-name}Repository repository):base(repository)
    {
    }
}
"""

DTO_TEMPLATE = """\
using System.ComponentModel.DataAnnotations;

namespace {dto-namespace};

{dto-class}
"""

MAPPER_TEMPLATE="""\
using {dto-namespace}.{mapper-name};
using {model-namespace};
using AutoMapper;

namespace {mapper-namespace};

public class {mapper-name}Mapper : Profile
{
    public {mapper-name}Mapper()
    {
        CreateMap<{mapper-name}Dto, {mapper-name}>();
        CreateMap<{mapper-name}, Get{mapper-name}Dto>();
    }
}
"""

def get_model_class(_modelName):
    modelName = capitalize(_modelName)
    if  not (modelName + '.cs') in listdir(PATH_DOMAIN_MODEL):
        print("get_model_class::error: model class {}.cs not found.".format(modelName))
        exit(1)

    CLASS_PATH = join(PATH_DOMAIN_MODEL, modelName + '.cs')
    content = ""
    try:
        fobj = open(CLASS_PATH, "r")
        content = fobj.read()
        fobj.close()
    except Exception as e:
        print("get_model_class::error: failed to read model class.")
        exit(1)
    
    ## First class inside the file is the model class
    index_of_public = content.find("public class")

    if  index_of_public == -1:
        print("get_model_class::error: public class not found.")
        exit(1)

    index = index_of_public
    class_content = ""

    bracket_stack = []

    while index < len(content):
        char = content[index]
        class_content += char

        if  char == "{":
            bracket_stack.append("{")
        elif char == "}":
            bracket_stack.pop()

            if  len(bracket_stack) == 0:
                break

        index += 1
    
    return class_content

def to_new_class(class_name, new_class_name):
    class_content = get_model_class(class_name)
    index = class_content.find("{")
    if  index == -1:
        print("to_new_class::error: public class not found.")
        exit(1)

    return "public class " + new_class_name + "\n" + class_content[index:]

def get_repository_list():
    content = ""
    try:
        fobj = open(PATH_REPOSITORY_LIST_PATH, "r", encoding="utf-8")
        content = fobj.read()
        fobj.close()
    except Exception as e:
        print("get_repository_list::error: failed to read repository list.")
        exit(1)
    
    SEARCH_KEY="#region REPOSITORIES"
    SEACRH_END="#endregion"

    index_start = content.find(SEARCH_KEY)
    index_ended = content.find(SEACRH_END, max(index_start, 0))

    if  index_start == -1 or index_ended == -1:
        print("get_repository_list::error: REPOSITORIES region not found.")
        exit(1)

    index = 0
    new_content = ""

    tab_index = 0

    while (index < len(content)):
        if  index >= index_start and index <= index_ended:
            copy = index
            while (copy < index_ended):
                new_content += content[copy]
                copy += 1
            
            new_content += ("\t{new-repository}\n")
            new_content += ("\t" * tab_index)

            index = index_ended
            while (index < (index_ended + len(SEACRH_END))):
                new_content += content[index]
                index += 1
        else:
            if  content[index] == "{":
                tab_index += 1

            new_content += content[index]
            index += 1

    return new_content

def get_services_list():
    content = ""
    try:
        fobj = open(PATH_SERVICE_LIST_PATH, "r", encoding="utf-8")
        content = fobj.read()
        fobj.close()
    except Exception as e:
        print("get_services_list::error: failed to read services list.")
        exit(1)
    
    SEARCH_KEY="#region SERVICES"
    SEACRH_END="#endregion"

    index_start = content.find(SEARCH_KEY)
    index_ended = content.find(SEACRH_END, max(index_start, 0))

    if  index_start == -1 or index_ended == -1:
        print("get_services_list::error: SERVICES region not found.")
        exit(1)

    index = 0
    new_content = ""

    tab_index = 0

    while (index < len(content)):
        if  index >= index_start and index <= index_ended:
            copy = index
            while (copy < index_ended):
                new_content += content[copy]
                copy += 1
            
            new_content += ("\t{new-service}\n")
            new_content += ("\t" * tab_index)

            index = index_ended
            while (index < (index_ended + len(SEACRH_END))):
                new_content += content[index]
                index += 1
        else:
            if  content[index] == "{":
                tab_index += 1

            new_content += content[index]
            index += 1

    return new_content

def get_automapper_list():
    content = ""
    try:
        fobj = open(PATH_MAPPER_LIST_PATH, "r", encoding="utf-8")
        content = fobj.read()
        fobj.close()
    except Exception as e:
        print("get_services_list::error: failed to read automapper list.")
        exit(1)
    
    SEARCH_KEY="#region AUTOMAPPER"
    SEACRH_END="#endregion"

    index_start = content.find(SEARCH_KEY)
    index_ended = content.find(SEACRH_END, max(index_start, 0))

    if  index_start == -1 or index_ended == -1:
        print("get_services_list::error: AUTOMAPPER region not found.")
        exit(1)

    index = 0
    new_content = ""

    tab_index = 0

    while (index < len(content)):
        if  index >= index_start and index <= index_ended:
            copy = index
            while (copy < index_ended):
                new_content += content[copy]
                copy += 1
            
            new_content += ("\t{new-mapper}\n")
            new_content += ("\t" * tab_index)

            index = index_ended
            while (index < (index_ended + len(SEACRH_END))):
                new_content += content[index]
                index += 1
        else:
            if  content[index] == "{":
                tab_index += 1

            new_content += content[index]
            index += 1

    return new_content

###############################################
def get_name_space_from_root(path:str):
    return path.replace(__install_path__, "").replace(get_path_separator(), ".")[1:]
###############################################

def make_controller(_controllerName):
    controllerName = capitalize(_controllerName)

    try:

        if  not exists(join(PATH_API_CONTROLLER, f"{controllerName}Controller.cs")):
            with open(join(PATH_API_CONTROLLER, f"{controllerName}Controller.cs"), "w") as f:
                f.write(CONTROLLER_TEMPLATE
                    .replace("{dto-namespace}", get_name_space_from_root(PATH_APPLICATION_DTO))
                    .replace("{iservice-namespace}", get_name_space_from_root(PATH_APPLICATION_ISERVICE))
                    .replace("{model-namespace}", get_name_space_from_root(PATH_DOMAIN_MODEL))
                    .replace("{controller-namespace}", get_name_space_from_root(PATH_API_CONTROLLER))
                    .replace("{controller-name}", controllerName)
                    .replace("{generic-name}", get_value_from_namespace('CONTROLLERS.GENERIC_NAME')))
                f.close()
        else:
            print("make_controller::warning: controller already exists (skipped).")

    except Exception as e:
        print("make_controller::error: failed to create controller.")
        exit(1)

def get_irepository_path(_repositoryName):
    repositoryName = capitalize(_repositoryName)
    return join(PATH_APPLICATION_IREPOSITORY, f"I{repositoryName}Repository.cs")

def get_repository_path(_repositoryName):
    repositoryName = capitalize(_repositoryName)
    return join(PATH_INFRASTRUCTURE_REPOSITORY, f"{repositoryName}Repository.cs")

def get_iservice_path(_serviceName):
    serviceName = capitalize(_serviceName)
    return join(PATH_APPLICATION_ISERVICE, f"I{serviceName}Service.cs")

def get_service_path(_serviceName):
    serviceName = capitalize(_serviceName)
    return join(PATH_INFRASTRUCTURE_SERVICE, f"{serviceName}Service.cs")

def make_repository(_repositoryName):
    repositoryName = capitalize(_repositoryName)

    irepository_exists = False

    try:
        if  not (get_value_from_namespace('REPOSITORY.IGENERIC_NAME') + '.cs') in listdir(PATH_APPLICATION_IREPOSITORY):
            print("make_repository::error: {}.cs not found at {}.".format(get_value_from_namespace('REPOSITORY.IGENERIC_NAME'), PATH_APPLICATION_IREPOSITORY))
            exit(1)

        if  not exists(get_irepository_path(_repositoryName)):
            with open(get_irepository_path(_repositoryName), "w") as f:
                f.write(IREPOSITORY_TEMPLATE
                    .replace("{dto-namespace}", get_name_space_from_root(PATH_APPLICATION_DTO))
                    .replace("{irepository-namespace}", get_name_space_from_root(PATH_APPLICATION_IREPOSITORY))
                    .replace("{irepository-name}", repositoryName)
                    .replace("{igeneric-name}", get_value_from_namespace('REPOSITORY.IGENERIC_NAME'))
                    .replace("{repository-name}", repositoryName)
                    .replace("{model-namespace}", get_name_space_from_root(PATH_DOMAIN_MODEL)))
                f.close()
        
        else:
            irepository_exists = True
            print("make_repository::warning: repository interface already exists (skipped).")
    except Exception as e:
        print("make_repository::error: failed to create repository interface {}.".format(get_irepository_path(_repositoryName)))
        exit(1)

    repository_exists = False

    try:
        if  not (get_value_from_namespace('REPOSITORY.GENERIC_NAME') + '.cs') in listdir(PATH_INFRASTRUCTURE_REPOSITORY):
            print("make_repository::error: {}.cs not found at {}.".format(get_value_from_namespace('REPOSITORY.GENERIC_NAME'), PATH_INFRASTRUCTURE_REPOSITORY))
            exit(1)
        
        if  not exists(get_repository_path(_repositoryName)):
            with open(get_repository_path(_repositoryName), "w") as f:
                f.write(REPOSITORY_TEMPLATE
                    .replace("{dto-namespace}", get_name_space_from_root(PATH_APPLICATION_DTO))
                    .replace("{irepository-namespace}", get_name_space_from_root(PATH_APPLICATION_IREPOSITORY))
                    .replace("{repository-namespace}", get_name_space_from_root(PATH_INFRASTRUCTURE_REPOSITORY))
                    .replace("{generic-name}", get_value_from_namespace('REPOSITORY.GENERIC_NAME'))
                    .replace("{repository-name}", repositoryName)
                    .replace("{data-namespace}", get_name_space_from_root(PATH_INFRASTRUCTURE_DATA))
                    .replace("{model-namespace}", get_name_space_from_root(PATH_DOMAIN_MODEL)))
                f.close()
        else:
            repository_exists = True
            print("make_repository::warning: repository implementation already exists (skipped).")
    except Exception as e:
        print("make_repository::error: failed to create repository implementation {}.".format(get_repository_path(_repositoryName)))
        exit(1)

    if  repository_exists and irepository_exists:
        print("make_repository::info: repository already exists (repository list not updated, skipped).")
        return
    
    BAK = PATH_REPOSITORY_LIST_PATH + ".bak"
    try:
        fobj0 = open(PATH_REPOSITORY_LIST_PATH, "r", encoding="utf-8")
        fobj1 = open(BAK, "w", encoding="utf-8")
        content = fobj0.read()
        fobj1.write(content)
        fobj0.close()
        fobj1.close()
    except Exception as e:
        print("make_repository::error: failed to backup repository list.")
        exit(1)

    # Save
    LINE = get_value_from_namespace('REPOSITORY.REPOSITORY_VARIABLE') + ".AddScoped<I{}Repository, {}Repository>(); /* added by make.py */".format(repositoryName, repositoryName)
    NEW_REPOSITORY_LIST = get_repository_list().replace("{new-repository}", LINE)

    try:
        fobj = open(PATH_REPOSITORY_LIST_PATH, "w", encoding="utf-8")
        fobj.write(NEW_REPOSITORY_LIST)
        fobj.close()
    except Exception as e:
        print("make_repository::error: failed to write repository list.")
        exit(1)

def make_service(_serviceName):
    serviceName = capitalize(_serviceName)

    iservice_exists = False

    try:
        if  not (get_value_from_namespace('SERVICE.IGENERIC_NAME') + '.cs') in listdir(PATH_APPLICATION_ISERVICE):
            print("make_service:IService::error: {}.cs not found at {}.".format(get_value_from_namespace('SERVICE.IGENERIC_NAME'), PATH_APPLICATION_ISERVICE))
            exit(1)

        if  not exists(get_iservice_path(_serviceName)):
            with open(get_iservice_path(_serviceName), "w") as f:
                f.write(ISERVICE_TEMPLATE
                    .replace("{dto-namespace}", get_name_space_from_root(PATH_APPLICATION_DTO))
                    .replace("{igeneric-name}", get_value_from_namespace('SERVICE.IGENERIC_NAME'))
                    .replace("{iservice-namespace}", get_name_space_from_root(PATH_APPLICATION_ISERVICE))
                    .replace("{model-namespace}", get_name_space_from_root(PATH_DOMAIN_MODEL))
                    .replace("{service-name}", serviceName))
                f.close()
        else:
            iservice_exists = True
            print("make_service:IService::warning: service interface already exists (skipped).")
    except Exception as e:
        print("make_service:IService::error: failed to create service interface {}.".format(get_iservice_path(_serviceName)))
        exit(1)

    service_exists = False

    try:
        if  not (get_value_from_namespace('SERVICE.GENERIC_NAME') + '.cs') in listdir(PATH_INFRASTRUCTURE_SERVICE):
            print("make_service:Service::error: {}.cs not found at {}.".format(get_value_from_namespace('SERVICE.GENERIC_NAME'), PATH_INFRASTRUCTURE_SERVICE))
            exit(1)

        if  not exists(get_service_path(_serviceName)):
            with open(get_service_path(_serviceName), "w") as f:
                f.write(SERVICE_TEMPLATE
                    .replace("{dto-namespace}", get_name_space_from_root(PATH_APPLICATION_DTO))
                    .replace("{generic-name}", get_value_from_namespace('SERVICE.GENERIC_NAME'))
                    .replace("{iservice-namespace}", get_name_space_from_root(PATH_APPLICATION_ISERVICE))
                    .replace("{irepository-namespace}", get_name_space_from_root(PATH_APPLICATION_IREPOSITORY))
                    .replace("{model-namespace}", get_name_space_from_root(PATH_DOMAIN_MODEL))
                    .replace("{data-namespace}", get_name_space_from_root(PATH_INFRASTRUCTURE_DATA))
                    .replace("{service-namespace}", get_name_space_from_root(PATH_INFRASTRUCTURE_SERVICE))
                    .replace("{service-name}", serviceName))
                f.close()
        else:
            service_exists = True
            print("make_service:Service::warning: service implementation already exists (skipped).")
    except Exception as e:
        print("make_service:Service::error: failed to create service implementation {}.".format(get_service_path(_serviceName)))
        exit(1)

    if  service_exists and iservice_exists:
        print("make_service::info: service already exists (services list not updated, skipped).")
        return

    BAK = PATH_SERVICE_LIST_PATH + ".bak"
    try:
        fobj0 = open(PATH_SERVICE_LIST_PATH, "r", encoding="utf-8")
        fobj1 = open(BAK, "w", encoding="utf-8")
        content = fobj0.read()
        fobj1.write(content)
        fobj0.close()
        fobj1.close()
    except Exception as e:
        print("make_service::error: failed to backup service list.")
        exit(1)

    # Save
    LINE = get_value_from_namespace('SERVICE.SERVICE_VARIABLE') + ".AddScoped<I{}Service, {}Service>(); /* added by make.py */".format(serviceName, serviceName)
    NEW_SERVICE_LIST = get_services_list().replace("{new-service}", LINE)

    try:
        fobj = open(PATH_SERVICE_LIST_PATH, "w", encoding="utf-8")
        fobj.write(NEW_SERVICE_LIST)
        fobj.close()
    except Exception as e:
        print("make_service::error: failed to write service list.")
        exit(1)

def get_dto_path(_dtoName):
    return join(PATH_APPLICATION_DTO, capitalize(_dtoName))

def make_dto(_dtoName):
    dtoName = capitalize(_dtoName)
    GETTER = f"Get{dtoName}Dto"
    SETTER = f"{dtoName}Dto"
    REQUIRED_FILES = [GETTER, SETTER]

    DTO_MODEL_FOLDER = get_dto_path(_dtoName)

    if  not exists(DTO_MODEL_FOLDER):
        try:
            print(f"make_dto::info: creating folder {DTO_MODEL_FOLDER}.")
            mkdir(DTO_MODEL_FOLDER)
        except Exception as e:
            print(f"make_dto::error: failed to create folder {DTO_MODEL_FOLDER}.")
            exit(1)

    try:
        for file in REQUIRED_FILES:
            if  not exists(join(DTO_MODEL_FOLDER, file) + '.cs'):
                with open(join(DTO_MODEL_FOLDER, file) + '.cs', "w") as f:
                    f.write(DTO_TEMPLATE
                        .replace("{dto-namespace}", get_name_space_from_root(DTO_MODEL_FOLDER))
                        .replace("{dto-class}", to_new_class(_dtoName, file)))
                    f.close()
            else:
                print(f"make_dto::warning: {file}.cs already exists (skipped).")
    except Exception as e:
        print("make_dto::error: failed to create dto.")
        exit(1)

def make_mapper(_serviceName):
    serviceName = capitalize(_serviceName)

    mapper_exists = False

    try:
        if  not exists(join(PATH_APPLICATION_MAPPER, f"{serviceName}Mapper.cs")):
            with open(join(PATH_APPLICATION_MAPPER, f"{serviceName}Mapper.cs"), "w") as f:
                f.write(MAPPER_TEMPLATE
                    .replace("{dto-namespace}", get_name_space_from_root(PATH_APPLICATION_DTO))
                    .replace("{model-namespace}", get_name_space_from_root(PATH_DOMAIN_MODEL))
                    .replace("{mapper-namespace}", get_name_space_from_root(PATH_APPLICATION_MAPPER))
                    .replace("{mapper-name}", serviceName))
                f.close()
        else:
            mapper_exists = True
            print(f"make_mapper::warning: mapper {serviceName}Mapper.cs already exists (skipped).")
    except Exception as e:
        print("make_mapper::error: failed to create mapper.")
        exit(1)

    if  mapper_exists:
        print("make_mapper::info: mapper already exists (mapper list not updated, skipped).")
        return
    
    BAK = PATH_MAPPER_LIST_PATH + ".bak"
    try:
        fobj0 = open(PATH_MAPPER_LIST_PATH, "r", encoding="utf-8")
        fobj1 = open(BAK, "w", encoding="utf-8")
        content = fobj0.read()
        fobj1.write(content)
        fobj0.close()
        fobj1.close()
    except Exception as e:
        print("make_service::error: failed to backup mapper list.")
        exit(1)

    # Save
    LINE = get_value_from_namespace('MAPPER.SERVICE_VARIABLE') + ".AddAutoMapper(typeof({}Mapper)); /* added by make.py */".format(serviceName)
    NEW_MAPPER_LIST = get_automapper_list().replace("{new-mapper}", LINE)

    try:
        fobj = open(PATH_MAPPER_LIST_PATH, "w", encoding="utf-8")
        fobj.write(NEW_MAPPER_LIST)
        fobj.close()
    except Exception as e:
        print("make_service::error: failed to write mapper list.")
        exit(1)


argparse = ArgumentParser(description="A simple script to generate controller, service, dto, and mapper classes for a given model.")
argparse.add_argument("-m", '--model', metavar='\b', type=str, help="Generate controller, service, dto, and mapper.")
argparse.add_argument("-p", '--patch', action='store_true', help="Update model list inside config file.")

args = argparse.parse_args()

if  args.model:
    model = args.model
    # Check if model exists
    if  not ((capitalize(str(model)) + '.cs') in listdir(PATH_DOMAIN_MODEL)):
        print("make::error: model {} not found at {}.".format(capitalize(str(model)) + '.cs', PATH_DOMAIN_MODEL))
        exit(1)
    print("make::info:[step 1 of 5]: running make service...")
    make_controller(model)
    print("make::info:[step 2 of 5]: running make repository...")
    make_repository(model)
    print("make::info:[step 3 of 5]: running make service...")
    make_service(model)
    print("make::info:[step 4 of 5]: running make dto...")
    make_dto(model)
    print("make::info:[step 5 of 5]: running make mapper...")
    make_mapper(model)
    print("make::info: done.")
    if not model in CONFIG["MODEL"]["LIST"]: CONFIG["MODEL"]["LIST"].append(model)
    try:
        json.dump(CONFIG, open(PATH_CONFIG, "w"), indent=4)
    except Exception as e:
        print("make::error: failed to write config file.")
        exit(1)

elif args.patch:
    models = list(map(lambda cs_file: basename(str(cs_file)).replace('.cs', ''), filter(lambda f: isfile(join(PATH_DOMAIN_MODEL, str(f))) and str(f).endswith('.cs'), listdir(PATH_DOMAIN_MODEL))))
    CONFIG["MODEL"]["LIST"] = models
    try:
        json.dump(CONFIG, open(PATH_CONFIG, "w"), indent=4)
        print("make::info: done.")
    except Exception as e:
        print("make::error: failed to write config file.")
        exit(1)
else:
    argparse.print_help()
