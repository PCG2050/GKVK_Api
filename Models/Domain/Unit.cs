namespace GKVK_Api.Models.Domain
{
    public class Unit
    {
        public int Id { get; set; }
        public string  Name { get; set; }
        public int? FK_ParentUnitId { get; set; }
        public Unit ParentUnit { get; set; }

        public ICollection<Unit> SubUnits { get; set; }
        public ICollection<TrainingSession> TrainingSessions { get; set; }

    }
}
