namespace GKVK_Api.Models.Domain
{
    /// <summary>Recursive org‑unit tree + template pointer.</summary>
    public class OrgUnit
    {
        [Key] public int OrgUnitId { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; } = null!;

        //Link to its parent institute
        public int InstituteId{ get; set; }

        public Institute Institute { get; set; } = null!;

        public int? ParentId { get; set; }
        public OrgUnit? Parent { get; set; }

        public ICollection<OrgUnit> Children { get; set; } = new List<OrgUnit>();

        // NEW: geographic linkage  ───────────────────────────
        public int? DistrictId { get; set; }  // nullable for top‑level templates
        public District? District { get; set; } 

        // template linking (for KVK clones, etc.)
        public int? TemplateSourceId { get; set; }
        public OrgUnit? TemplateSource { get; set; }

        // ── Navigation collections ────────────────────────────────
        public ICollection<TableDefinition> TableDefinitions { get; set; } = new List<TableDefinition>();
        public ICollection<TableDataRow> DataRows { get; set; } = new List<TableDataRow>();
        public ICollection<TrainerUnitAssignment> TrainerUnitAssignments { get; set; } = new List<TrainerUnitAssignment>();

    }


}