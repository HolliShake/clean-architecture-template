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
PATH_APPLICATION_ISERVICE=resolve_path(get_value_from_namespace('SERVICE.IPATH'))
PATH_APPLICATION_MAPPER=resolve_path(get_value_from_namespace('MAPPER.PATH'))
# DOM
PATH_DOMAIN=resolve_path('DOMAIN_PATH')
PATH_DOMAIN_MODEL=resolve_path(get_value_from_namespace('MODEL.PATH'))
# INF
PATH_INFRASTRUCTURE=resolve_path('INFRASTRUCTURE_PATH')
PATH_INFRASTRUCTURE_DATA=resolve_path(get_value_from_namespace('DATA.PATH'))
PATH_INFRASTRUCTURE_SERVICE=resolve_path(get_value_from_namespace('SERVICE.PATH'))

# 
PATH_MAPPER_LIST_PATH=resolve_path(get_value_from_namespace('MAPPER.LIST_PATH'))
PATH_SERVICE_LIST_PATH=resolve_path(get_value_from_namespace('SERVICE.LIST_PATH'))

VALID_SEARCH_PATHS = [
    PATH_API,
    PATH_API_CONTROLLER,
    PATH_APPLICATION,
    PATH_APPLICATION_DTO,
    PATH_APPLICATION_ISERVICE,
    PATH_APPLICATION_MAPPER,
    PATH_DOMAIN,
    PATH_DOMAIN_MODEL,
    PATH_INFRASTRUCTURE,
    PATH_INFRASTRUCTURE_DATA,
    PATH_INFRASTRUCTURE_SERVICE,
    # 
    PATH_MAPPER_LIST_PATH,
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
        - ISERVICE PATH: {}
        - MAPPER PATH: {}
    - DOMAIN PATH: {}
        - MODEL PATH: {}
    - INFRASTRUCTURE PATH: {}
        - DATA PATH: {}
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
    PATH_APPLICATION_ISERVICE,
    PATH_APPLICATION_MAPPER,
    PATH_DOMAIN,
    PATH_DOMAIN_MODEL,
    PATH_INFRASTRUCTURE,
    PATH_INFRASTRUCTURE_DATA,
    PATH_INFRASTRUCTURE_SERVICE,
    # OTHER
    PATH_MAPPER_LIST_PATH,
    PATH_SERVICE_LIST_PATH
))

CONTROLLER_TEMPLATE = """
using {dto-namespace}.{controller-name};
using {iservice-namespace};
using {model-namespace};
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace {controller-namespace};

[ApiController]
[Route("[controller]")]
public class {controller-name}Controller : {generic-name}<{controller-name}, I{controller-name}Service, {controller-name}Dto, Get{controller-name}Dto>
{
    public {controller-name}Controller(IMapper mapper, I{controller-name}Service repo):base(mapper, repo)
    {
    }

    /****************** ACTION ROUTES ******************/
    /// <summary>
    /// Get all data.
    /// </summary>
    /// <returns>Array[{controller-name}]</returns>
    [HttpGet("/Api/[controller]/all")]
    public async Task<ActionResult> GetAllAction()
    {
        return await GenericGetAll();
    }
    
    /// <summary>
    /// Get 1st to n (where n := size(parameter)) data.
    /// </summary>
    /// <returns>Array[{controller-name}]</returns>
    [HttpGet("/Api/[controller]/chunk/{size:int}")]
    public async Task<ActionResult> GetByChunk(int size)
    {
        return await GenericGetByChunk(size);
    }
    
    /// <summary>
    /// Get specific data ({controller-name}) by id.
    /// </summary>
    /// <returns>Array[{controller-name}]></returns>
    [HttpGet("/Api/[controller]/{id:int}")]
    public async Task<ActionResult> GetAction(int id)
    {
        return await GenericGet(id);
    }
    
    /// <summary>
    /// Creates new {controller-name} entry.
    /// </summary>
    /// <returns>{controller-name}</returns>
    [HttpPost("/Api/[controller]/create")]
    public async Task<ActionResult> CreateAction({controller-name}Dto item)
    {
        return await GenericCreate(item);
    }
    
    /// <summary>
    /// Creates multiple instance of {controller-name}.
    /// </summary>
    /// <returns>Array[{controller-name}]</returns>
    [HttpPost("/Api/[controller]/insert")]
    public async Task<ActionResult> CreateAllAction(List<{controller-name}Dto> items)
    {
        return await GenericCreateAll(items);
    }
    
    /// <summary>
    /// Updates single property of {controller-name}.
    /// </summary>
    /// <remarks>(From GenericController)</remarks>
    /// <returns>{controller-name}</returns>
    [HttpPatch("/Api/[controller]/patch/{id:int}")]
    public async Task<ActionResult> PatchAction(int id, {controller-name}Dto item)
    {
        return await GenericUpdate(id, item);
    }
    
    /// <summary>
    /// Updates multiple property of {controller-name}.
    /// </summary>
    /// <returns>{controller-name}</returns>
    [HttpPut("/Api/[controller]/update/{id:int}")]
    public async Task<ActionResult> UpdateAction(int id, {controller-name}Dto item)
    {
        return await GenericUpdate(id, item);
    }
    
    /// <summary>
    /// Deletes single {controller-name} entry.
    /// </summary>
    /// <returns>Null</returns>
    [HttpDelete("/Api/[controller]/delete/{id:int}")]
    public async Task<ActionResult> DeleteAction(int id)
    {
        return await GenericDelete(id);
    }
}
"""

ISERVICE_TEMPLATE = """
using {model-namespace};

namespace {iservice-namespace};
public interface I{service-name}Service:{igeneric-name}<{service-name}>
{
}
"""

SERVICE_TEMPLATE = """
using {iservice-namespace};
using {model-namespace};
using {data-namespace};

namespace {service-namespace};
public class {service-name}Service:{generic-name}<{service-name}>, I{service-name}Service
{
    public {service-name}Service(AppDbContext context):base(context)
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
using AutoMapper;
using {dto-namespace}.{mapper-name};
using {model-namespace};

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
    index_ended = content.find(SEACRH_END)

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
    index_ended = content.find(SEACRH_END)

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

def get_iservice_path(_serviceName):
    serviceName = capitalize(_serviceName)
    return join(PATH_APPLICATION_ISERVICE, f"I{serviceName}Service.cs")

def get_service_path(_serviceName):
    serviceName = capitalize(_serviceName)
    return join(PATH_INFRASTRUCTURE_SERVICE, f"{serviceName}Service.cs")

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
                    .replace("{generic-name}", get_value_from_namespace('SERVICE.GENERIC_NAME'))
                    .replace("{iservice-namespace}", get_name_space_from_root(PATH_APPLICATION_ISERVICE))
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
    print("make::info:[step 1 of 4]: running make service...")
    make_controller(model)
    print("make::info:[step 2 of 4]: running make service...")
    make_service(model)
    print("make::info:[step 3 of 4]: running make dto...")
    make_dto(model)
    print("make::info:[step 4 of 4]: running make mapper...")
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
