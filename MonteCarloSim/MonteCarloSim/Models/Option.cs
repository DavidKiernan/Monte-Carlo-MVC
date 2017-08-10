using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MonteCarloSim.Models
{
    public enum OptionType { Call, Put } // the 2 option types

    public class Option
    {
        public int ID { get; set; } // Primary Key
        public string ContractName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ExpriryDate { get; set; } // date the contract expires on
        public double CurrentPrice { get; set; } // current price of the company's stock
        public double StrikePrice { get; set; } // price sell \ buy option
        public double ImpliedVolatility { get; set; }
        public double RiskFreeRate { get; set; } // eg goverment bond ( us daily yield curve rate)
        public OptionType OptionType { get; set; }

        public virtual ICollection<OptionPrice> OptPrices { get; set; }
        /* hold the entities related to the Option Price
         * so if a given Option has 2 related OptionPrice rows
         * row containing Option PK in thier OptionID FKC
         * then Option entity's OptPrices navigation Property will 
         * contain these OptionPrice entities
         * the virtual takes advantage of lazy loading
         * done in icollection as it will hold multiple entities
         */

    }
}