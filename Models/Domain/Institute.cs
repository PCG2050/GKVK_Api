namespace GKVK_Api.Models.Domain
{
    public class Institute
    {
        [Key]
        public int InstituteId { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; } = null!;

        //Optional Metadata
        public string? Address { get; set; }

        public string? ContactEmail { get; set; }
        //navigation
        public ICollection<OrgUnit> OrgUnits { get; set; } = new List<OrgUnit>();

    }
}
