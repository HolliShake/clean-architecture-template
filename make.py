##
##
##
##

import sys
import json
from os.path import exists, dirname, abspath, join
from os import mkdir, listdir

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
KEY_SERVICE_LIST_PATH = "LIST_PATH"
# 
KEY_MAPPER = "MAPPER"
KEY_MAPPER_PATH = "PATH"
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
        KEY_SERVICE_LIST_PATH: "INFRASTRUCTURE_PATH/InfraInjector.cs"
    },
    KEY_MAPPER: {
        KEY_MAPPER_PATH: "APPLICATION_PATH/Mapper",
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
assert_type_namespace('SERVICE.LIST_PATH', str)
assert_type_namespace(KEY_MAPPER, dict)
assert_type_namespace('MAPPER.PATH', str)
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
    if not exists(path):
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
    [2]. SERVICE LIST PATH: {}
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
    modelName = _modelName.capitalize()
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

###############################################
def get_name_space_from_root(path:str):
    return path.replace(__install_path__, "").replace(get_path_separator(), ".")[1:]
###############################################

def get_iservice_path(_serviceName):
    serviceName = _serviceName.capitalize()
    return join(PATH_APPLICATION_ISERVICE, f"I{serviceName}Service.cs")

def get_service_path(_serviceName):
    serviceName = _serviceName.capitalize()
    return join(PATH_INFRASTRUCTURE_SERVICE, f"{serviceName}Service.cs")

def make_service(_serviceName):
    serviceName = _serviceName.capitalize()
    try:
        if  not (get_value_from_namespace('SERVICE.IGENERIC_NAME') + '.cs') in listdir(PATH_APPLICATION_ISERVICE):
            print("make_service:IService::error: {}.cs not found at {}.".format(get_value_from_namespace('SERVICE.IGENERIC_NAME'), PATH_APPLICATION_ISERVICE))
            exit(1)

        if  not exists(get_iservice_path(_serviceName)):
            with open(get_iservice_path(_serviceName), "w") as f:
                f.write(ISERVICE_TEMPLATE
                    .replace("{model-namespace}", get_name_space_from_root(PATH_DOMAIN_MODEL))
                    .replace("{service-name}", serviceName))
                f.close()
        else:
            print("make_service:IService::warning: service interface already exists (not created).")
    except Exception as e:
        print("make_service:IService::error: failed to create service interface {}.".format(get_iservice_path(_serviceName)))
        exit(1)

    try:
        if  not (get_value_from_namespace('SERVICE.GENERIC_NAME') + '.cs') in listdir(PATH_INFRASTRUCTURE_SERVICE):
            print("make_service:Service::error: {}.cs not found at {}.".format(get_value_from_namespace('SERVICE.GENERIC_NAME'), PATH_INFRASTRUCTURE_SERVICE))
            exit(1)

        if  not exists(get_service_path(_serviceName)):
            with open(get_service_path(_serviceName), "w") as f:
                f.write(SERVICE_TEMPLATE
                    .replace("{iservice-namespace}", get_name_space_from_root(PATH_APPLICATION_ISERVICE))
                    .replace("{model-namespace}", get_name_space_from_root(PATH_DOMAIN_MODEL))
                    .replace("{data-namespace}", get_name_space_from_root(PATH_INFRASTRUCTURE_DATA))
                    .replace("{service-namespace}", get_name_space_from_root(PATH_INFRASTRUCTURE_SERVICE))
                    .replace("{service-name}", serviceName))
                f.close()
        else:
            print("make_service:Service::warning: service implementation already exists (not created).")
    except Exception as e:
        print("make_service:Service::error: failed to create service implementation {}.".format(get_service_path(_serviceName)))
        exit(1)

def get_dto_path(_dtoName):
    return join(PATH_APPLICATION_DTO, _dtoName.capitalize())

def make_dto(_dtoName):
    dtoName = _dtoName.capitalize()
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
                print(f"make_dto::warning: {file}.cs already exists (not created).")
    except Exception as e:
        print("make_dto::error: failed to create dto.")
        exit(1)

def make_mapper(_serviceName):
    serviceName = _serviceName.capitalize()

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
            print("make_mapper::warning: mapper already exists (not created).")
    except Exception as e:
        print("make_mapper::error: failed to create mapper.")
        exit(1)

if  __name__ == "__main__":
    model = "Test"
    print("make::info:[step 1 of 3]: running make service...")
    make_service(model)
    print("make::info:[step 2 of 3]: running make dto...")
    make_dto(model)
    print("make::info:[step 3 of 3]: running make mapper...")
    make_mapper(model)
    print("make::info: done.")
    if not model in CONFIG["MODEL"]["LIST"]: CONFIG["MODEL"]["LIST"].append(model)
    try:
        json.dump(CONFIG, open(PATH_CONFIG, "w"), indent=4)
    except Exception as e:
        print("make::error: failed to write config file.")
        exit(1)
