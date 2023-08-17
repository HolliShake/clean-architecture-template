using APPLICATION.IService;

namespace INFRASTRUCTURE.Service;

public class GenericService <TModel> : IGenericService<TModel> where TModel : class
{
}