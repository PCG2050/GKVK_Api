namespace GKVK_Api.Models.Domain
{
    public class User
    {
        [Key] public int UserId { get; set; }

        [Required, MaxLength(150)]
        public string FullName { get; set; } = null!;

        [Required, EmailAddress]
        public string Email { get; set; } = null!;         // login

        [Phone] public string? PhoneNumber { get; set; }

        [Required] public string PasswordHash { get; set; } = null!;

        [Required, EnumDataType(typeof(UserRole))]
        public UserRole Role { get; set; }

        // ── Navigation collections ────────────────────────────────
            // ── Navigation collections ───────────────────────────────
    public ICollection<TrainerUnitAssignment> TrainerUnitAssignments { get; set; } = new List<TrainerUnitAssignment>();
    public ICollection<TableDataRow>CreatedRows           { get; set; } = new List<TableDataRow>();  // ⬅️ NEW
    public ICollection<RefreshToken> RefreshTokens         { get; set; } = new List<RefreshToken>();

    public DateTime CreatedAt     { get; set; } = DateTime.UtcNow;
    public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;
    }
    public enum UserRole
    {
        Admin,
        Trainer
    }
}
