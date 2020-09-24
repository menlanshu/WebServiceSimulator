//using Microsoft.EntityFrameworkCore;
//using WS_Simulator.Models;

//namespace WS_Simulator.DataAccess
//{
//    public class SQLiteContext : DbContext
//    {
//        private const string ConnString = "DataSource=TestRepositories.db";
//        public DbSet<TestRepository> TestRespositories { get; set; }
//        public DbSet<Node> NodeList { get; set; }

//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        {
//            optionsBuilder.UseSqlite(ConnString);
//        }

//        protected override void OnModelCreating(ModelBuilder modelBuilder)
//        {
//            modelBuilder.Entity<TestRepository>()
//                .HasMany(t => t.TestNodeList)
//                .WithOne(n => n.Repository)
//                .HasForeignKey(t => t.RepositoryId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);

//            modelBuilder.Entity<Node>()
//                .HasOne(n => n.MotherNode)
//                .WithMany()
//                .HasForeignKey(n => n.MotherNodeId).IsRequired(false);

//        }
//    }
//}
