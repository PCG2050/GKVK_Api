namespace GKVK_Api.Models.Domain
{
    public class TrainerUnitAssignment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User Trainer { get; set; }
        public int UnitId { get; set; }
        public Unit Unit { get; set; }
        // Additional properties can be added here if needed
        // For example, you might want to track the date of assignment or status
    }
}
