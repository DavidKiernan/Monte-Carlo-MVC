using System;
using System.ComponentModel.DataAnnotations;

namespace MonteCarloSim.Models
{
    public class OptionPrice
    {
        public int OptionPriceID { get; set; } // pk uses the classname Patterns

        public int OptionID { get; set; } // FK Entity Framework interprets a property as a foreign key property if it's named <navigation property name><primary key property name>


        [DisplayFormat(DataFormatString = "{0:N2}")] // display as 2 DP so 9.5 will display as 9.50
        public decimal Price { get; set; }

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")] // display in format of 12-JUL-2017
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
