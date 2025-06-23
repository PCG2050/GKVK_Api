namespace GKVK_Api.Models.Domain
{
    /// <summary>Column metadata for every dynamic table.</summary>
    public class TableColumn
    {
        [Key] public int TableColumnId { get; set; }

        [Required] public string Name { get; set; } = null!;
        [Required] public ColumnDataType DataType { get; set; }

        /// <summary>1‑based position.</summary>
        public int Position { get; set; }

        // ── FK to TableDefinition ────────────────────────────────
        public int TableDefinitionId { get; set; }
        public TableDefinition TableDefinition { get; set; } = null!;
    }
    /// <summary>Uniform datatype list for dynamic columns.</summary>
    public enum ColumnDataType
    {
        String,
        Int,
        Decimal,
        Date,
        Boolean,
        Double
    }
}
