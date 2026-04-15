using Microsoft.EntityFrameworkCore;
using Progress.Model.Entitys;

namespace Progress.Repository
{
    public class ProgressDbContext : DbContext
    {
        public ProgressDbContext(DbContextOptions<ProgressDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>().ToTable("Users");
            modelBuilder.Entity<Role>().ToTable("Roles");
            modelBuilder.Entity<UserRole>().ToTable("UserRoles");
            modelBuilder.Entity<Menu>().ToTable("Menus");
            modelBuilder.Entity<MenuRole>().ToTable("MenuRoles");

            modelBuilder.Entity<UserRole>(e =>
            {
                e.HasOne(ur => ur.User).WithMany().HasForeignKey(ur => ur.UserId);
                e.HasOne(ur => ur.Role).WithMany().HasForeignKey(ur => ur.RoleId);
            });
            modelBuilder.Entity<MenuRole>(e =>
            {
                e.HasOne(mr => mr.Menu).WithMany().HasForeignKey(mr => mr.MenuId);
                e.HasOne(mr => mr.Role).WithMany().HasForeignKey(mr => mr.RoleId);
            });

            modelBuilder.Entity<WorkpieceOrderLine>(e =>
            {
                e.HasOne(x => x.Supplier).WithMany().HasForeignKey(x => x.SupplierId);
                e.HasOne(x => x.CraftRecipe).WithMany().HasForeignKey(x => x.CraftRecipeId);
            });

            //数据库索引
            modelBuilder.Entity<Supplier>().HasIndex(s => s.SupplierNumber).IsUnique();
            modelBuilder.Entity<Craft>().HasIndex(c => c.Code).IsUnique();
            modelBuilder.Entity<CraftRecipe>().HasIndex(r => r.Code).IsUnique();
            modelBuilder.Entity<AlertNotificationLog>()
                .HasIndex(x => new { x.OrderLineId, x.AlertDay }).IsUnique();
        }

        public DbSet<Menu>? Menus { get; set; }
        public DbSet<Users>? Users { get; set; }
        public DbSet<Role>? Roles { get; set; }
        public DbSet<UserRole>? UserRoles { get; set; }
        public DbSet<MenuRole>? MenuRoles { get; set; }
        public DbSet<Supplier>? Suppliers { get; set; }
        public DbSet<Craft>? Crafts { get; set; }
        public DbSet<CraftRecipe>? CraftRecipes { get; set; }
        public DbSet<CraftRecipeStep>? CraftRecipeSteps { get; set; }
        public DbSet<WorkpieceOrderLine>? WorkpieceOrderLines { get; set; }
        public DbSet<RepairRecord>? RepairRecords { get; set; }
        public DbSet<AlertSetting>? AlertSettings { get; set; }
        public DbSet<AlertNotificationLog>? AlertNotificationLogs { get; set; }
    }
}
