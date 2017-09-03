using System.Data.Entity;
using System.Data.Entity.SqlServer;

namespace MonteCarloSim.DAL
{
    public class OptionConfiguration : DbConfiguration
    {
        public OptionConfiguration()
        {
            SetExecutionStrategy("System.Data.SqlClient", () => new SqlAzureExecutionStrategy());
        }
    }
}