using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using MonteCarloSim.Models;

namespace MonteCarloSim.DAL
{
    public class OptionInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<OptionContext>
    {
        protected override void Seed(OptionContext context)
        {
            var options = new List<Option>
            {
                new Option{ContractName = "IBM112", CurrentPrice = 145.13, ExpriryDate = DateTime.Parse("2017-Aug-04"), StrikePrice = 135.00 , ImpliedVolatility = 0.00, RiskFreeRate = 1.00 , OptionType = OptionType.Call, Prices = new List<double> {0.1, 0.5} },
                new Option{ContractName = "FB170811C00170000", CurrentPrice = 169.86, ExpriryDate = DateTime.Parse("2017-Aug-11"), StrikePrice = 170.00 , ImpliedVolatility = 0.20, RiskFreeRate = 1.00 , OptionType = OptionType.Call, Prices = new List<double>{0.1, 0.5,0.45, 0.6, 1.0, 0.93, 1.4, 1.75, 2.0} },
                new Option{ContractName = "GOOGL170804P00935000", CurrentPrice = 946.56, ExpriryDate = DateTime.Parse("2017-Aug-04"), StrikePrice = 935.00, ImpliedVolatility = 3.13, RiskFreeRate = 1.00 , OptionType = OptionType.Put, Prices = new List<double>{8.1, 7.5, 9.6, 7.5} },
                new Option{ContractName = "IBM170811P00165000", CurrentPrice = 145.30, ExpriryDate = DateTime.Parse("2017-Aug-11"), StrikePrice = 165.00 , ImpliedVolatility = 92.58, RiskFreeRate = 1.00 , OptionType = OptionType.Put, Prices = new List<double>{19.19,19.99,19.82,20.01,21.82,22.15,20.08,20.56,23.71,20.95,23.85} } // last 2 are last bid & asking
            };

            options.ForEach(o => context.Options.Add(o));
            context.SaveChanges();
        }
    }
}