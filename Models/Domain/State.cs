namespace GKVK_Api.Models.Domain
{
    public class State
    {
        [Key]
        public int StateId { get; set; }
        [Required]
        public string Name { get; set; } = null!;

        public ICollection<District> Districts { get; set; } = new List<District>();
    }
}
