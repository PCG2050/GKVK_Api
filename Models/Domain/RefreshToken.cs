namespace GKVK_Api.Models.Domain
{
    public class RefreshToken
    {
        [Key]
        public int RefreshTokenId { get; set; }
        
        public int FK_UserId { get; set; }
        [ForeignKey(nameof(FK_UserId))]
        [Newtonsoft.Json.JsonIgnore]
        public User User { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }
        public bool IsRevoked { get; set; } = false;
    }
}
