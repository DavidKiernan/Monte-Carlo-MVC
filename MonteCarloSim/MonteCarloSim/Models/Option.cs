using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MonteCarloSim.Models
{
    public enum OptionType { Call, Put } // the 2 option types

    public class Option
    {
        public int ID { get; set; } // Primary Key
        public string ContractName { get; set; }
        public DateTime ExpriryDate { get; set; } // date the contract expires on
        public double CurrentPrice { get; set; } // current price of the company's stock
        public double StrikePrice { get; set; } // price sell \ buy option
        public double ImpliedVolatility { get; set; }
        public double RiskFreeRate { get; set; } // eg goverment bond ( us daily yield curve rate)
        public OptionType OptionType { get; set; }

        public List<double>  Prices { get; set; }
    }
}