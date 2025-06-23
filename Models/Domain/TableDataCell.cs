namespace GKVK_Api.Models.Domain
{
    public class TableDataCell
    {
        [Key] 
        public int TableDataCellId { get; set; }

        public int TableDataRowId { get; set; }
        public TableDataRow TableDataRow { get; set; } = null!;

        public int TableColumnId { get; set; }
        public TableColumn TableColumn { get; set; } = null!;

        [Required]
        public string Value { get; set; } = string.Empty;
    }
}
