namespace GKVK_Api.Models.Domain
{
    public class RefreshTokenData
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiryTime { get; set; }             
       
    }
}
