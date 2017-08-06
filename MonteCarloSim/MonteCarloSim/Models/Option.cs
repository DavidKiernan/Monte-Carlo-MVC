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
        [Display(Name = "Contract")]
        public string ContractName { get; set; }

        // Display the date in this format
        [Display(Name = "Expiry Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ExpriryDate { get; set; } // date the contract expires on

        [Display(Name = "Current Price")]
        [DisplayFormat(DataFormatString = "{0:n2}",ApplyFormatInEditMode = true)]
        public double CurrentPrice { get; set; } // current price of the company's stock

        [Display(Name = "Strike Price")]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public double StrikePrice { get; set; } // price sell \ buy option

        [Display(Name = "Implied Volatility")]
        public double ImpliedVolatility { get; set; }

        [Display(Name = "Risk Free Rate")]
        public double RiskFreeRate { get; set; } // eg goverment bond ( us daily yield curve rate)

        [Display(Name = "Option Type")]
        public OptionType OptionType { get; set; }
        public List<double>  Prices { get; set; } // List for the prices 

        // Methods
        /*
        * Based on European vanilla option pricing with C++ via Monte Carlo methods
        * Available from https://www.quantstart.com/articles/European-vanilla-option-pricing-with-C-via-Monte-Carlo-methods
        * Author Michael Halls-Moore on February 2nd, 2013
        * Accessed on 15th July 2017
        */
        private double gaussian_box_muller()
        {
            double x = 0.0, y = 0.0;
            double euclid_sq = 0.0;
            Random rnd = new Random();

            // Continue generating two uniform random variables
            // until the square of their "euclidean distance" 
            // is less than unity
            // Generate a Gaussian random number via Box-Muller

            do
            {
                x = 2.0 * rnd.NextDouble() - 1;
                y = 2.0 * rnd.NextDouble() - 1;
                euclid_sq = (x * x) + (y * y);
            } while (euclid_sq >= 1);

            return (x * Math.Sqrt(-2 * Math.Log(euclid_sq) / euclid_sq));
        }

        // private methods as these will be used inside the public method & user has no need to access them
        // Asset Price
        private double generateAssetPrice(double currentPrice, double volatility, double riskFree, double time)
        {
            return currentPrice * Math.Exp((riskFree - 0.5 * volatility * volatility) * time + volatility * Math.Sqrt(time) * gaussian_box_muller());
        }

        // call payoff
        private double callPayoff(double assetPrice, double strikePrice)
        {
            return Math.Max(assetPrice - strikePrice, 0.0);
        }

        // Put payoff
        private double putPayoff(double assetPrice, double strikePrice)
        {
            return Math.Max(0.0, strikePrice - assetPrice);
        }

        // public methods used by the program
        // MC_call_option
        public double OptionPrice(double currentPrice, double strikePrice, double riskFree, double vol, double time)
        {
            double simulations = 1000000;
            double payoff = 0.0;

            for (int i = 0; i < simulations; i++)
            {
                double assetPrice = generateAssetPrice(currentPrice, vol, riskFree, time);
                payoff += callPayoff(assetPrice, strikePrice);
            }

            return (payoff / Convert.ToDouble(simulations)) * Math.Exp(-riskFree * time);
        }

        // MC_put_option
        public double putOption(double currentPrice, double strikePrice, double riskFree, double vol, double time)
        {
            double simulations = 1000000;
            double payoff = 0.0;

            for (int i = 0; i < simulations; i++)
            {
                double assetPrice = generateAssetPrice(currentPrice, vol, riskFree, time);
                payoff += putPayoff(assetPrice, strikePrice);
            }

            return (payoff / Convert.ToDouble(simulations)) * Math.Exp(-riskFree * time);
        }
    }
}