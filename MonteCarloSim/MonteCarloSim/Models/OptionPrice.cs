using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MonteCarloSim.Models
{
    public class OptionPrice
    {
        public int OptionPriceID { get; set; } // pk
        public int OptionID { get; set; } // FK 

        public double Price { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime Day { get; set; }

        public string Varation { get; set; }
        public virtual Option Option { get; set; }
    }
}

/*
 * OptionID property is FK & the nav property is Option.
 * Price entity is associated with 1 Option so property can only hold a single option
 * Entity Framework interprets a property as a foreign key property if it's named <navigation property name><primary key property name>
 * OptionID for Option nav property since the Option entity's PK is ID
 */
