using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using MonteCarloSim.Models;

namespace MonteCarloSim.DAL
{
    public class OptionContext : DbContext
    {
        public OptionContext() : base("OptionContext") // pass to web config
        { }

        public DbSet<Option> Options { get; set; }
        public DbSet<OptionPrice> OptionPrices { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>(); // names table Option & Not Options
        }
    }
}