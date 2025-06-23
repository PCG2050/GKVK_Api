namespace GKVK_Api.Models.Domain
{
    public class TrainerUnitAssignment
    {
        [Key] 
        public int TrainerUnitAssignmentId { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;            // MUST have Role = Trainer

        public int OrgUnitId { get; set; }
        public OrgUnit OrgUnit { get; set; } = null!;

        public DateTime AssignedOn { get; set; } = DateTime.UtcNow;
        public DateTime? UnassignedOn { get; set; }
    }
}
