namespace GKVK_Api.Models.Domain
{
    public class TableDataRow
    {
        [Key] public int TableDataRowId { get; set; }

        public int TableDefinitionId { get; set; }
        public TableDefinition TableDefinition { get; set; } = null!;

        public int OrgUnitId { get; set; }
        public OrgUnit OrgUnit { get; set; } = null!;

        public int TrainerId { get; set; }
        public User Trainer { get; set; } = null!;

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public ICollection<TableDataCell> Cells { get; set; } = new List<TableDataCell>();
    }

}
