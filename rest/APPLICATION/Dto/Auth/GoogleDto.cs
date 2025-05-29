using System.ComponentModel.DataAnnotations;

namespace APPLICATION.Dto.Auth;

public class GoogleDto
{
    [Required]
    public string GToken { get; set; }
}