using System.Data.Entity;
using System.Data.Entity.SqlServer;

namespace MonteCarloSim.DAL
{
    public class OptionConfiguration : DbConfiguration
    {
        // EF ato runs code it finds in a class that drives from DbConfiguration
        // otherwise have to do in the Web.config
        public OptionConfiguration()
        {
            SetExecutionStrategy("System.Data.SqlClient", () => new SqlAzureExecutionStrategy());
        }
    }
}