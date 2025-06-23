namespace GKVK_Api.Models.Domain
{
    /// <summary>Per‑unit table (“1. OFT”, “Training Programmes Organized”, …).</summary>
    public class TableDefinition
    {
        [Key] public int TableDefinitionId { get; set; }

        [Required] public string Title { get; set; } = null!;
        public int DisplayOrder { get; set; }

        // ── FK to OrgUnit ─────────────────────────────────────────
        public int OrgUnitId { get; set; }
        public OrgUnit OrgUnit { get; set; } = null!;

        // ── Navigation ───────────────────────────────────────────
        public ICollection<TableColumn> Columns { get; set; } = new List<TableColumn>();
        public ICollection<TableDataRow> DataRows { get; set; } = new List<TableDataRow>();

    }
}
