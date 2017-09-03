namespace MonteCarloSim.Migrations
{
    using MonteCarloSim.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<MonteCarloSim.DAL.OptionContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(MonteCarloSim.DAL.OptionContext context)
        {
            var options = new List<Option>
            {
                new Option{ContractName = "IBM112", CurrentPrice = 145.13M, ExpiryDate = DateTime.Parse("2017-Aug-04"), StrikePrice = 135.00M , ImpliedVolatility = 0.00M, RiskFreeRate = 1.00M , OptionType = OptionType.Call},
                new Option{ContractName = "IBM170811P00165000", CurrentPrice = 145.30M, ExpiryDate = DateTime.Parse("2017-Aug-11"), StrikePrice = 165.00M , ImpliedVolatility = 92.58M, RiskFreeRate = 1.00M , OptionType = OptionType.Put}
            };
            options.ForEach(o => context.Options.AddOrUpdate(opt => opt.ContractName, o)); // Assumes ContractNames are unqiue. Will get error if duplicate contract name
            context.SaveChanges();

            var optionPrices = new List<OptionPrice>
            {
                new OptionPrice
                {
                    // Can use Id here as its set when savechanges for options collection. EF auto gets PK when inserts entity into DB & updates the ID property
                    OptionID = options.Single(o => o.ContractName == "IBM112").ID,
                    Varation = "ORIGINAL", Day = DateTime.Parse("2017-Aug-2"), Price = 0.10M
                },
                new OptionPrice
                {
                    OptionID = options.Single(o => o.ContractName == "IBM112").ID,
                    Varation = "ORIGINAL", Day = DateTime.Parse("2017-Aug-3"), Price = 0.50M
                },
                new OptionPrice
                {
                    OptionID = options.Single(o => o.ContractName == "IBM170811P00165000").ID,
                    Varation = "ORIGINAL", Day = DateTime.Parse("2017-Aug-8") , Price = 19.19M
                },
                new OptionPrice
                {
                    OptionID = options.Single(o => o.ContractName == "IBM170811P00165000").ID,
                    Varation = "ORIGINAL", Day = DateTime.Parse("2017-Aug-9") , Price = 20.18M
                }
            };

            foreach (OptionPrice op in optionPrices)
            {
                var optionPriceInDB = context.OptionPrices.Where(opt => opt.OptionID == op.OptionID).SingleOrDefault();
                if(optionPriceInDB == null)
                {
                    context.OptionPrices.Add(op);
                }
            } // end foreach
            context.SaveChanges();
        }
    }
}
