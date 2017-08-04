using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using MonteCarloSim.Models;

namespace MonteCarloSim.DAL
{
    public class OptionContext : DbContext
    {
        public OptionContext() : base("OptionContext") // connection string add to web.config
        { }

        public DbSet<Option> Options { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}