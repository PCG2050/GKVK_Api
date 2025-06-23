//namespace GKVK_Api.Services
//{
//    public class UnitService
//    {
//        private readonly ApplicationDbContext _dbContext;

//        public UnitService(ApplicationDbContext dbContext)
//        {
//            _dbContext = dbContext;
//        }

//        //public async Task DeleteUnitAndSubUnits(int unitId)
//        //{
//        //    var unitToDelete = await _dbContext.Units
//        //        .Include(u => u.SubUnitList)
//        //            .ThenInclude(s => s.ChildSubUnits) // for deeper levels, repeat ThenInclude or use recursive logic
//        //        .FirstOrDefaultAsync(u => u.Id == unitId);

//        //    if (unitToDelete == null)
//        //        throw new Exception("Unit not found.");

//        //    if (unitToDelete.SubUnitList?.Any() == true)
//        //        _dbContext.SubUnits.RemoveRange(unitToDelete.SubUnitList);

//        //    _dbContext.Units.Remove(unitToDelete);
//        //    await _dbContext.SaveChangesAsync();
//        //}
//        public async Task DeleteUnitAndAllAssociatedSubUnits(int unitId)
//        {
//            var unitToDelete = await _dbContext.Units
//                .Include(u => u.SubUnitList)
//                .FirstOrDefaultAsync(u => u.Id == unitId);

//            if (unitToDelete == null) return;

//            var allSubUnitsToDelete = new List<SubUnit>();

//            async Task CollectAllSubUnits(List<SubUnit> subUnits)
//            {
//                if (subUnits == null) return;

//                foreach (var subUnit in subUnits)
//                {
//                    allSubUnitsToDelete.Add(subUnit);

//                    // Explicitly load children to go deeper
//                    await _dbContext.Entry(subUnit)
//                        .Collection(s => s.ChildSubUnits)
//                        .LoadAsync();

//                    await CollectAllSubUnits(subUnit.ChildSubUnits?.ToList() ?? new List<SubUnit>());
//                }
//            }

//            await CollectAllSubUnits(unitToDelete.SubUnitList.ToList());

//            _dbContext.SubUnits.RemoveRange(allSubUnitsToDelete);
//            _dbContext.Units.Remove(unitToDelete);

//            await _dbContext.SaveChangesAsync();
//        }


//        public async Task DeleteSubUnitAndAllChildren(int subUnitId)
//        {
//            var subUnit = await _dbContext.SubUnits
//                .Include(s => s.ChildSubUnits)
//                .FirstOrDefaultAsync(s => s.Id == subUnitId);

//            if (subUnit == null) return;

//            var allToDelete = new List<SubUnit>();

//            async Task CollectChildren(SubUnit parent)
//            {
//                allToDelete.Add(parent);
//                await _dbContext.Entry(parent).Collection(s => s.ChildSubUnits).LoadAsync();

//                foreach (var child in parent.ChildSubUnits)
//                {
//                    await CollectChildren(child);
//                }
//            }

//            await CollectChildren(subUnit);

//            _dbContext.SubUnits.RemoveRange(allToDelete);
//            await _dbContext.SaveChangesAsync();
//        }
//    }

//}
