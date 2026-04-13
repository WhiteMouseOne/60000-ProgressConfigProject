using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace Progress.Repository
{
    public class ProgressDbContextFactory : IDesignTimeDbContextFactory<ProgressDbContext>
    {
        public ProgressDbContext CreateDbContext(string[] args)
        {
            var conn = Environment.GetEnvironmentVariable("PROGRESS_MYSQL")
                       ?? "Server=127.0.0.1;Port=3306;Database=ProgressMES;Uid=root;Pwd=123456;CharSet=utf8mb4;";
            var options = new DbContextOptionsBuilder<ProgressDbContext>()
                .UseMySql(conn, new MySqlServerVersion(new Version(8, 0, 36)))
                .Options;
            return new ProgressDbContext(options);
        }
    }
}
