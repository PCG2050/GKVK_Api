//using System.Runtime.InteropServices;

//namespace GKVK_Api.Repositories
//{
//    public interface IAdminRepository
//    {
//        // User Management
//        Task<User> RegisterUserAsync(User user);
//        Task<User> GetUserbyEmailAsync(string email);
//        Task<User> GetUserbyUserIdAsync(int userId);
//        Task<bool> DeleteUserbyEmailAsync(string email);
//        Task<User> CreateAdminAsync(User user);
//        Task<bool> DeleteAdminAsync(int userId);

//        // Trainer-Unit Assignment
//        Task<bool> AssignTrainerToUnitAsync(int trainerId, int unitId);
//        Task<bool> RemoveTrainerFromUnitAsync(int trainerId, int unitId);
//        Task<List<OrgUnit>> GetTrainerUnitsAsync(int trainerId);

//        // Unit Hierarchy
//        Task<List<OrgUnit>> GetAllUnitsWithSubUnitsAsync();
//    }
//    public class AdminRepository : IAdminRepository
//    {
//        private readonly ApplicationDbContext _context;

//        public AdminRepository(ApplicationDbContext context)
//        {
//            _context = context;
//        }
//        public async Task<User> RegisterUserAsync (User user)
//        {
//            _context.Users.Add(user);
//            await _context.SaveChangesAsync();
//            return user;
//        }

//        public async Task<User> GetUserbyEmailAsync (string email)
//        {
//            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
//        }

//        public async Task<User> GetUserbyUserIdAsync(int userId)
//        {
//            return await _context.Users.FindAsync(userId);
//        }

//        public async Task<bool> DeleteUserbyEmailAsync(string email)
//        {
//            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
//            if (user == null) return false;

//            _context.Users.Remove(user);
//            await _context.SaveChangesAsync();
//            return true;
//        }

//        public async Task<User> CreateAdminAsync(User user)
//        {
//            user.Role = "Admin";
//            _context.Users.Add(user);
//            await _context.SaveChangesAsync();  
//            return user;
//        }

//        public async Task<bool> DeleteAdminAsync(int userId)
//        {
//            var admin = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId && u.Role == "Admin");
//            if(admin == null) return false;

//            _context.Users.Remove(admin);
//            await _context.SaveChangesAsync();
//            return true;
//        }

//        public async Task<bool> AssignTrainerToUnitAsync(int trainerid, int unitid)
//        {
//            var exists = await _context.TrainerUnitAssignments
//                .AnyAsync(t => t.FK_UserId == trainerid && t.FK_UnitId == unitid);
//            if (exists) return false;
//            _context.TrainerUnitAssignments.Add(new TrainerUnitAssignment
//            {
//                FK_UserId = trainerid,
//                FK_UnitId = unitid
//            });
//            await _context.SaveChangesAsync();
//            return true;
//        }
//        public async Task<bool> RemoveTrainerFromUnitAsync(int trainerId, int unitId)
//        {
//            var assignment = await _context.TrainerUnitAssignments
//                .FirstOrDefaultAsync(t => t.FK_UserId == trainerId && t.FK_UnitId == unitId);

//            if (assignment == null) return false;

//            _context.TrainerUnitAssignments.Remove(assignment);
//            await _context.SaveChangesAsync();
//            return true;
//        }
//        public async Task<List<OrgUnit>> GetTrainerUnitsAsync(int trainerId)
//        {
//            return await _context.TrainerUnitAssignments
//                .Where(t => t.FK_UserId == trainerId)
//                .Select(t => t.Unit)
//                .Include(u => u.SubUnits)
//                .ToListAsync();
//        }

//        public async Task<List<OrgUnit>> GetAllUnitsWithSubUnitsAsync()
//        {
//            return await _context.Units
//                .Where(u => u.FK_ParentUnitId == null)
//                .Include(u => u.SubUnits)
//                .ToListAsync();
//        }

//    }
//}
