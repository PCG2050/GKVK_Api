//using GKVK_Api.Interfaces;

//namespace GKVK_Api.Services
//{
//    public class TrainingSessionService : ITrainingSessionService
//    {
//        private readonly ApplicationDbContext _context;
//        private readonly ILogger<TrainingSessionService> _logger;

//        public TrainingSessionService(ApplicationDbContext context, ILogger<TrainingSessionService> logger)
//        {
//            _context = context;
//            _logger = logger;
//        }

//        public async Task<bool> UploadTrainingSessionAsync(int trainerId, TrainerUploadSessionDto dto)
//        {
//            // ✅ Validation for Unit or SubUnit (only one must be set)
//            if ((dto.FK_UnitId == null && dto.FK_SubUnitId == null) ||
//                (dto.FK_UnitId != null && dto.FK_SubUnitId != null))
//            {
//                throw new ArgumentException("You must assign either a Unit or a SubUnit, not both.");
//            }

//            // ✅ Check if the trainer is assigned
//            var isAssigned = await _context.TrainerUnitAssignments.AnyAsync(a =>
//                a.FK_UserId == trainerId &&
//                ((dto.FK_UnitId != null && a.FK_UnitId == dto.FK_UnitId) ||
//                 (dto.FK_SubUnitId != null && a.FK_SubUnitId == dto.FK_SubUnitId)));

//            if (!isAssigned)
//                throw new UnauthorizedAccessException("Trainer is not assigned to this unit or subunit.");

//            // ✅ Create TrainingSession
//            var session = new TrainingSession
//            {
//                FK_UnitId = dto.FK_UnitId ?? 0,
//                FK_SubUnitId = dto.FK_SubUnitId ?? 0,
//                Date = dto.Date,
//                JsonData = dto.JsonData,
//                FK_CreatedByUserId = trainerId
//            };

//            _context.TrainingSessions.Add(session);
//            await _context.SaveChangesAsync();

//            return true;
//        }
//    }

//}
