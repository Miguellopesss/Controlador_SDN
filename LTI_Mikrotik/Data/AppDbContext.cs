using Microsoft.EntityFrameworkCore;

namespace LTI_Mikrotik.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Device> Devices { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=D:\\Documentos\\Universidade\\LTI\\LTI-TL1\\LTI_Mikrotik\\Data\\Mikrotik.mdf;Integrated Security=True;Connect Timeout=30");
        }
    }
}
