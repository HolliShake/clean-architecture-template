namespace APPLICATION.Dto.Response;


public class PaginationResponseDto<T>
{
    public IList<T> Data {get; set;} = new List<T>();

    public PaginationMetaDto? PaginationMeta {get; set;}
}

public class PaginationMetaDto
{
    public int Page {get; set;}
    public int Rows {get; set;}
    public int TotalPages {get; set;}
}