using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using DataAnnotationsExtensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace MonteCarloSim.Models
{
    public enum OptionType { Call, Put } // the 2 option types

    public class Option
    {

        public int ID { get; set; } // Primary Key

        [Display(Name = "Contract Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = " Name Required")]
        public string ContractName { get; set; }

        [Display(Name = "Expiry Date")]
        [Required(ErrorMessage = "Expiry Date Required"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ExpiryDate { get; set; } // date the contract expires on

        [Display(Name = "Current Price")]
        [Required(ErrorMessage = "Enter Current Price"), Min(1, ErrorMessage = "Minimum Value is 1"), RegularExpression(@"^\d+.\d{0,2}$", ErrorMessage = "Price Can't Have More Than 2 Decimal Places")]
        public double CurrentPrice { get; set; } // current price of the company's stock

        [Display(Name = "Strike Price")]
        [Required(ErrorMessage = "Enter Strike Price"), Min(1, ErrorMessage = "Minimum Value is 1"), RegularExpression(@"^\d+.\d{0,2}$", ErrorMessage = "Price Can't Have More Than 2 Decimal Places")]
        public double StrikePrice { get; set; } // price sell \ buy option

        [Display(Name = "Implied Volatility")]
        [Required(ErrorMessage = "Enter Implied Volatility"), Min(0.00, ErrorMessage = "Positive Values Only"), RegularExpression(@"^\d+.\d{0,2}$", ErrorMessage = "IV Can't Have More Than 2 Decimal Places")]
        public double ImpliedVolatility { get; set; }

        [Display(Name = "Risk Free Rate")]
        [Required(ErrorMessage = "Enter Risk Free Rate"), Min(0.00, ErrorMessage = "Positive Values Only"), RegularExpression(@"^\d+.\d{0,2}$", ErrorMessage = "Risk Free Rate Can't Have More Than 2 Decimal Places")]
        public double RiskFreeRate { get; set; } // eg goverment bond ( us daily yield curve rate)

        [Display(Name = "Option Type")]
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
        private double Gaussian_Box_Muller()
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
        private double GenerateAssetPrice(double currentPrice, double volatility, double riskFree, double time)
        {
            return currentPrice * Math.Exp((riskFree - 0.5 * volatility * volatility) * time + volatility * Math.Sqrt(time) * Gaussian_Box_Muller());
        }


        // call payoff
        private double CallPayoff(double assetPrice, double strikePrice)
        {
            return Math.Max(assetPrice - strikePrice, 0.0);
        }

        // Put payoff
        private double PutPayoff(double assetPrice, double strikePrice)
        {
            return Math.Max(0.0, strikePrice - assetPrice);
        }


        // MC_call_option
        private double CallOption(double currentPrice, double strikePrice, double riskFree, double vol, double day)
        {
            vol = vol / 100;
            riskFree = riskFree / 100;
            int simulations = 1000000;
            double payoff = 0.0;
            double time = day / 365.0; // every calution seems to only use 365 days even for a leap year


            for (int i = 0; i < simulations; i++)
            {
                double assetPrice = GenerateAssetPrice(currentPrice, vol, riskFree, time);
                payoff += CallPayoff(assetPrice, strikePrice);
            }

            return Math.Round((payoff / simulations) * Math.Exp(-riskFree * time), 2); // Retrun output to 2 dp
        }

        // MC_put_option
        private double PutOption(double currentPrice, double strikePrice, double riskFree, double vol, int day)
        {
            vol = vol / 100;
            riskFree = riskFree / 100;
            int simulations = 1000000;
            double payoff = 0.0;
            double time = day / 365.0;

            for (int i = 0; i < simulations; i++)
            {
                double assetPrice = GenerateAssetPrice(currentPrice, vol, riskFree, time);
                payoff += PutPayoff(assetPrice, strikePrice);
            }

            return Math.Round((payoff / simulations) * Math.Exp(-riskFree * time), 2);
        }

        // public methods
        // The Create Controller will call these methods
        public void CreateCall(double currentPrice, double strikePrice, double riskFree, double vol, DateTime ExpiryDate)
        {
            double currPrice = currentPrice, spotPrice = strikePrice, riskFR = riskFree, iv = vol; // this allows to vary the inputs & Create with the original
            int differance = (ExpiryDate - DateTime.Now).Days;
            OptPrices = new List<OptionPrice>();

            for (int count = 0; count < 3; count++)
            {
                // loop through the days
                for (int day = 1; day < differance + 1; day++)
                {
                    OptionPrice optionPrices = new OptionPrice(); // each loop will create a new instance of OptionPrice class
                    optionPrices.Price = optionPrices.Price = CallOption(currPrice, spotPrice, riskFR, iv, day);
                    optionPrices.Day = DateTime.Now.AddDays(day);

                    if (count == 0)
                    {
                        optionPrices.Varation = "ORIGINAL";
                    }
                    else
                    {
                        optionPrices.Varation = "VARATION" + count + "    CURR: " + currPrice + "    SP: " + spotPrice + "  RFR: " + riskFR + " %    IV: " + iv + " %";
                    }
                    OptPrices.Add(optionPrices); // add to the list
                } // end the inner loop
                if (count % 2 == 0)
                {
                    currPrice += 3.21;
                    spotPrice -= 1.5;
                    iv += 5;
                    riskFR -= 0.01;

                    if(spotPrice < 0.00)
                    {
                        spotPrice = 5;
                    }
                    if (riskFR < 0.00)
                    {
                        riskFR = 5;
                    }

                }
                else
                {
                    currPrice -= 1;
                    spotPrice += 0.5;
                    riskFR += 0.02;
                    iv -= 5;
                    if (iv < 0.00) // IV can never be negative
                    {
                        iv = 0;
                    }
                }
            } // end outer loop varation
        } // end 

        // Put Create
        public void CreatePut(double currentPrice, double strikePrice, double riskFree, double vol, DateTime ExpiryDate)
        {
            double currPrice = currentPrice, spotPrice = strikePrice, riskFR = riskFree, iv = vol; // this allows to vary the inputs & Create with the original
            int differance = (ExpiryDate - DateTime.Now).Days;
            OptPrices = new List<OptionPrice>();

            for (int count = 0; count < 3; count++)
            {
                // loop through the days
                for (int day = 1; day < differance + 1; day++)
                {
                    OptionPrice optionPrices = new OptionPrice(); // each loop will create a new instance of OptionPrice class
                    optionPrices.Price = optionPrices.Price = PutOption(currPrice, spotPrice, riskFR, iv, day);
                    optionPrices.Day = DateTime.Now.AddDays(day);

                    if (count == 0)
                    {
                        optionPrices.Varation = "ORIGINAL";
                    }
                    else
                    {
                        optionPrices.Varation = "VARATION" + count + "    CURR: " + currPrice + "    SP: " + spotPrice + "  RFR: " + riskFR + " %    IV: " + iv + " %";
                    }
                    OptPrices.Add(optionPrices); // add to the list
                } // end the inner loop
                if (count % 2 == 0)
                {
                    currPrice += 1.75;
                    spotPrice -= 0.75;
                    iv += 12.25;
                    riskFR -= 0.01;
                    if(spotPrice < 0.00)
                    {
                        spotPrice = 5;
                    }
                    if (riskFR < 0.00)
                    {
                        riskFR = 1;
                    }
                }
                else
                {
                    currPrice -= 1;
                    spotPrice += 2.5;
                    riskFR += 0.02;
                    iv -= 7.25;
                    if (iv < 0.00) // IV can never be negative
                    {
                        iv = 0;
                    }
                    if (currPrice < 0.00) 
                    {
                        currPrice = 3;
                    }

                }
            } // end outer loop varation
        } // end 
    }
}