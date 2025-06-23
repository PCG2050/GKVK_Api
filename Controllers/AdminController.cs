//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//namespace GKVK_Api.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class AdminController : ControllerBase
//    {
//        private readonly ApplicationDbContext _context;
//        private readonly IMapper _mapper;

//        public AdminController(ApplicationDbContext context, IMapper mapper)
//        {
//            _context = context;
//            _mapper = mapper;
//        }

//        // 1. Register Trainer
//        [HttpPost("register-trainer")]
//        public async Task<IActionResult> RegisterTrainer([FromBody] RegisterDto dto)
//        {
//            // Validate, hash password, save user
//            // Assign role as Trainer
//        }

//        // 2. Assign Trainer to Unit/SubUnit
//        [HttpPost("assign-trainer")]
//        public async Task<IActionResult> AssignTrainer([FromBody] AssignTrainerToUnitDto dto)
//        {
//            // Add TrainerUnitAssignment logic
//        }

//        // 3. Create Unit
//        [HttpPost("create-unit")]
//        public async Task<IActionResult> CreateUnit([FromBody] CreateUnitDto dto)
//        {
//            // Save Unit to DB
//        }

//        // 4. Create SubUnit
//        [HttpPost("create-subunit")]
//        public async Task<IActionResult> CreateSubUnit([FromBody] CreateSubUnitDto dto)
//        {
//            // Save SubUnit to DB
//        }

//        // 5. Get Full Unit Tree
//        [HttpGet("unit-tree")]
//        public async Task<IActionResult> GetUnitTree()
//        {
//            // Return UnitTreeDto
//        }

//        // 6. Delete User
//        [HttpDelete("delete-user/{userId}")]
//        public async Task<IActionResult> DeleteUser(int userId)
//        {
//            // Find user, delete
//        }
//    }

//}
