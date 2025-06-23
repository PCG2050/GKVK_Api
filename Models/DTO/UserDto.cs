namespace GKVK_Api.Models.DTO
{
    public class UserDto
    {
        public int UserId {  get; set; }
        public string FullName { get; set; }        
        public string Email { get; set; } //for login
        public string PhoneNumber { get; set; }        
        public string Role { get; set; }  //Admin or Trainer
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
    }
}
