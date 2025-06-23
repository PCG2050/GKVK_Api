namespace GKVK_Api.Models.Domain
{
    public class District
    {
        [Key]
        public int DistrictId { get; set; }
        [Required]
        public string Name { get; set; } = null!;

        //FK to state
        public int StateId { get; set; }
        public State State { get; set; } = null!;

        //Reverse nav to the orgunits that are live in the district
        public ICollection<OrgUnit> OrgUnits { get; set; } = new List<OrgUnit>();
    }
}
