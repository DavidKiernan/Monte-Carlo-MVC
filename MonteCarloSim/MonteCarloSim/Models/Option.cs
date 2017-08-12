﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

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

        /* hold the entities related to the Option Price
         * so if a given Option has 2 related OptionPrice rows
         * row containing Option PK in thier OptionID FKC
         * then Option entity's OptPrices navigation Property will 
         * contain these OptionPrice entities
         * the virtual takes advantage of lazy loading
         * done in icollection as it will hold multiple entities
         */
        public virtual ICollection<OptionPrice> OptPrices { get; set; }

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
        public double callOption(double currentPrice, double strikePrice, double riskFree, double vol, double day)
        {
            int simulations = 1000000;
            double payoff = 0.0;
            double time = day / 365.0;
            vol = vol / 100;
            riskFree = riskFree / 100;

            for (int i = 0; i < simulations; i++)
            {
                double assetPrice = generateAssetPrice(currentPrice, vol, riskFree, time);
                payoff += callPayoff(assetPrice, strikePrice);
            }

            return Math.Round((payoff / simulations) * Math.Exp(-riskFree * time), 2); // Retrun output to 2 dp
        }

        // MC_put_option
        public double putOption(double currentPrice, double strikePrice, double riskFree, double vol, double day)
        {
            int simulations = 1000000;
            double payoff = 0.0;
            double time = day / 365.0; // divide by year Make leap Year? 
            vol = vol / 100;
            riskFree = riskFree / 100;
            for (int i = 0; i < simulations; i++)
            {
                double assetPrice = generateAssetPrice(currentPrice, vol, riskFree, time);
                payoff += putPayoff(assetPrice, strikePrice);
            }

            return Math.Round((payoff / simulations) * Math.Exp(-riskFree * time), 2);
        }
    }
}