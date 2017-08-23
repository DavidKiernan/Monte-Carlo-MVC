using System;
using System.Collections.Generic;

using MonteCarloSim.Models;

namespace MonteCarloSim.DAL
{
    public class OptionInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<OptionContext>
    {
        protected override void Seed(OptionContext context)
        {
            var options = new List<Option>
            {
                new Option{ContractName = "IBM112", CurrentPrice = 145.13M, ExpiryDate = DateTime.Parse("2017-Aug-04"), StrikePrice = 135.00M , ImpliedVolatility = 0.00M, RiskFreeRate = 1.00M , OptionType = OptionType.Call},
                new Option{ContractName = "IBM170811P00165000", CurrentPrice = 145.30M, ExpiryDate = DateTime.Parse("2017-Aug-11"), StrikePrice = 165.00M , ImpliedVolatility = 92.58M, RiskFreeRate = 1.00M , OptionType = OptionType.Put}
            };

            options.ForEach(o => context.Options.Add(o));
            context.SaveChanges();

            var optionPrices = new List<OptionPrice>
            {
                new OptionPrice{OptionID=1, Varation = "ORIGINAL", Day = DateTime.Parse("2017-Aug-2"), Price = 0.10M},
                new OptionPrice{OptionID=1, Varation = "ORIGINAL", Day = DateTime.Parse("2017-Aug-3"), Price = 0.50M},
                new OptionPrice{OptionID=2, Varation = "ORIGINAL", Day = DateTime.Parse("2017-Aug-8") , Price = 19.19M},
                new OptionPrice{OptionID=2, Varation = "ORIGINAL", Day = DateTime.Parse("2017-Aug-9") , Price = 20.18M}
            };
            optionPrices.ForEach(o => context.OptionPrices.Add(o));
            context.SaveChanges();
        }
    }
}