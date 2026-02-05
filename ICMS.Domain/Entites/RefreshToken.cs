using ICMS.Domain.Entites;
using System.ComponentModel.DataAnnotations;

namespace ICMS.Domain.Entites;

public class RefreshToken
{
    public Guid TokenId { get; set; }

    public string Token {  get; set; }
    public int UserId { get; set; } 

    public DateTime CreatedAt { get; set; }
    public DateTime ExpirationDate {  get; set; }

    public bool IsRevoked { get; set; }

    public User? User { get; init; } 
}
