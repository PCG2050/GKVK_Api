namespace GKVK_Api
{
    public class ApplicationDbContext :DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        //public DbSet<Models.User> Users { get; set; }
        //public DbSet<Models.Role> Roles { get; set; }
        //public DbSet<Models.UserRole> UserRoles { get; set; }
        //public DbSet<Models.Product> Products { get; set; }
        //public DbSet<Models.Order> Orders { get; set; }
        //public DbSet<Models.OrderItem> OrderItems { get; set; }
        //public DbSet<Models.CartItem> CartItems { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            //modelBuilder.Entity<Models.User>().ToTable("Users");
            //modelBuilder.Entity<Models.Role>().ToTable("Roles");
            //modelBuilder.Entity<Models.UserRole>().ToTable("UserRoles");
            //modelBuilder.Entity<Models.Product>().ToTable("Products");
            //modelBuilder.Entity<Models.Order>().ToTable("Orders");
            //modelBuilder.Entity<Models.OrderItem>().ToTable("OrderItems");
            //modelBuilder.Entity<Models.CartItem>().ToTable("CartItems");
        }
    }

}
