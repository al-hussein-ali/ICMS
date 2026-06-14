using ICMS.Domain.Entites.Common;
using ICMS.Domain.Entites.Identity;
using ICMS.Domain.Entites.Clinical;
using ICMS.Domain.Entites.Maternal;
using ICMS.Domain.Entites.Visits;
using ICMS.Domain.Entites.Audit;
using ICMS.Domain.Entites.Geography;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICMS.Domain.Entites.Identity;

public class RefreshToken
{
    public Guid TokenId { get; set; }

    public string Token {  get; set; }

    [NotMapped]
    public string? RawToken { get; set; }

    public int UserId { get; set; } 

    public DateTime CreatedAt { get; set; }
    public DateTime ExpirationDate {  get; set; }

    public bool IsRevoked { get; set; }

    public User? User { get; init; } 
}
