namespace APPLICATION.Dto.Response;


public record PaginationResponseDto<T>
{
    public IList<T> Data {get; set;} = [];

    public PaginationMetaDto? PaginationMeta {get; set;}
}

public record PaginationMetaDto
{
    public int Page {get; set;}
    public int Rows {get; set;}
    public int TotalPages {get; set;}
}