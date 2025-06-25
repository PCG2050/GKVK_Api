namespace GKVK_Api.Models.Domain
{
    public class Institute
    {
        [Key]
        public int InstituteId { get; set; }

        [Required, MaxLength(150)]
        public string InstituteName { get; set; } 

        public string InstituteLogo { get; set; }

        //Optional Metadata
        public string? Address { get; set; }

        public string? ContactEmail { get; set; }
        //navigation
        public ICollection<OrgUnit> OrgUnits { get; set; } = new List<OrgUnit>();

    }
}
