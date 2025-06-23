namespace GKVK_Api.Models.DTO
{
    public class AuthResponseDto
    {
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string Role { get; set; }
        public int UserId { get; set; }
    }
}
