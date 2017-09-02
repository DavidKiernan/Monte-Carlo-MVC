using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DataAnnotationsExtensions;

namespace MonteCarloSim.Models
{
    public enum OptionType { Call, Put } // the 2 option types

    public class Option
    {

        public int ID { get; set; } // Primary Key

        [Display(Name = "Contract Name")]           // No empty boxes & Allow all chars expect white spaces
        [Required(AllowEmptyStrings = false, ErrorMessage = " Name Required"), RegularExpression(@"^\S*$", ErrorMessage = "No white space allowed")]
        public string ContractName { get; set; }

        [Display(Name = "Expiry Date")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Expiry Date Required"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ExpiryDate { get; set; } // date the contract expires on

        [Display(Name = "Current Price"), DisplayFormat(DataFormatString = "{0:N2}")] //display to 2 DP
        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter Current Price") ,Min(1, ErrorMessage = "Minimum Value is 1"), RegularExpression(@"\d+(\.\d{0,2})?", ErrorMessage = "Ensure Format is 00.00 No white space allowed")]
        public decimal CurrentPrice { get; set; } // current price of the company's stock

        [Display(Name = "Strike Price"), DisplayFormat(DataFormatString = "{0:N2}")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter Strike Price"), Min(1, ErrorMessage = "Minimum Value is 1"), RegularExpression(@"\d+(\.\d{0,2})?", ErrorMessage = "Ensure Format is 00.00 No white space allowed")]
        public decimal StrikePrice { get; set; } // price sell \ buy option

        [Display(Name = "Implied Volatility"), DisplayFormat(DataFormatString = "{0:N2}")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter Implied Volatility"), Min(0.00, ErrorMessage = "Positive Values Only"), RegularExpression(@"\d+(\.\d{0,2})?", ErrorMessage = "Ensure Format is 00.00 No white space allowed")]
        public decimal ImpliedVolatility { get; set; }

        [Display(Name = "Risk Free Rate"), DisplayFormat(DataFormatString = "{0:N2}")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter Risk Free Rate"), Min(0.00, ErrorMessage = "Positive Values Only"), RegularExpression(@"\d+(\.\d{0,2})?", ErrorMessage = "Ensure Format is 00.00 No White Spaces Allowed ")]
        public decimal RiskFreeRate { get; set; } // eg goverment bond ( us daily yield curve rate)

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

        // Empty constructor for testing purposes
        public Option()
        {
        }
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

        // Should be private methods as these will be used inside the public method & user has no need to access them
        // Changed to public for the unit tests
        // Asset Price
        public decimal GenerateAssetPrice(decimal currentPrice, decimal volatility, decimal riskFree, double time)
        {
            return currentPrice * (decimal)Math.Exp(((double)riskFree - 0.5 * (double)volatility * (double)volatility) * time + (double)volatility * Math.Sqrt(time) * Gaussian_Box_Muller());
        }


        // call payoff
        public decimal CallPayoff(decimal assetPrice, decimal strikePrice)
        {
            const decimal zero = 0.00M;
            return Math.Max((assetPrice - strikePrice), zero); //return the max value of either the assest - strike or 0.00
        }

        // Put payoff
        public decimal PutPayoff(decimal assetPrice, decimal strikePrice)
        {
            const decimal zero = 0.00M;
            return Math.Max(zero, (strikePrice - assetPrice));
        }


        // MC_call_option
        public decimal CallOption(decimal currentPrice, decimal strikePrice, decimal riskFree, decimal vol, int day)
        {
            vol = vol / 100.00M;
            riskFree = riskFree / 100.00M;
            int simulations = 1000000;
            decimal payoff = 0.00M;
            double time = day / 365.0; // every calution seems to only use 365 days even for a leap year


            for (int i = 0; i < simulations; i++)
            {
                decimal assetPrice = GenerateAssetPrice(currentPrice, vol, riskFree, time);
                payoff += CallPayoff(assetPrice, strikePrice);
            }
                                                        // Discount factor
            return Math.Round((payoff / simulations) * (decimal)Math.Exp((double)-riskFree * time), 2); // Retrun output to 2 dp
        }

        // MC_put_option
        public decimal PutOption(decimal currentPrice, decimal strikePrice, decimal riskFree, decimal vol, int day)
        {
            vol = vol / 100.00M;
            riskFree = riskFree / 100.00M;
            int simulations = 1000000;
            decimal payoff = 0.00M;
            double time = day / 365.0;

            for (int i = 0; i < simulations; i++)
            {
                decimal assetPrice = GenerateAssetPrice(currentPrice, vol, riskFree, time);
                payoff += PutPayoff(assetPrice, strikePrice);
            }

            return Math.Round((payoff / simulations) * (decimal)Math.Exp((double)-riskFree * time), 2);
        }

        // public methods
        // The Create Controller will call these methods
        public void CreateCall(decimal currentPrice, decimal strikePrice, decimal riskFree, decimal vol, DateTime ExpiryDate)
        {
            decimal currPrice = currentPrice, spotPrice = strikePrice, riskFR = riskFree, iv = vol; // this allows to vary the inputs & Create with the original
            int differance = (ExpiryDate - DateTime.Now).Days;
            OptPrices = new List<OptionPrice>();

            for (int count = 0; count < 3; count++)
            {
                // loop through the days
                for (int day = 1; day < differance + 1; day++)
                {
                    OptionPrice optionPrices = new OptionPrice(); // each loop will create a new instance of OptionPrice class
                    optionPrices.Price = CallOption(currPrice, spotPrice, riskFR, iv, day);
                    optionPrices.Day = DateTime.Now.AddDays(day);

                    if (count == 0) // 1st run through
                    {
                        optionPrices.Varation = "ORIGINAL";
                    }
                    else
                    {
                        optionPrices.Varation = "VARATION" + count + "    CURR: " + currPrice + "    SP: " + spotPrice + "  RFR: " + riskFR + " %    IV: " + iv + " %";
                    }
                    OptPrices.Add(optionPrices); // add to the list
                } // end the inner loop

                if (count % 2 == 0)  // even
                {
                    currPrice += 3.21M;
                    spotPrice -= 1.5M;
                    iv += 5;
                    riskFR -= 0.01M;

                    if (spotPrice <= 0.00M) // check that the strike price above 0.00 
                    {
                        spotPrice = 5.00M;  // set to 5 if it is not
                    }
                    if (riskFR < 0.00M)
                    {
                        riskFR = 1.00M;
                    }

                }
                else
                {
                    currPrice -= 1.00M;
                    spotPrice += 0.50M;
                    riskFR += 0.02M;
                    iv -= 5;
                    if (iv < 0.00M) // IV can never be negative but can be zero
                    {
                        iv = 0;
                    }

                    if (currPrice <= 0.00M) // set current price to 3 if 0 or less
                    {
                        currPrice = 3.00M;
                    }
                }
            } // end outer loop varation
        } // end 

        // Put Create
        public void CreatePut(decimal currentPrice, decimal strikePrice, decimal riskFree, decimal vol, DateTime ExpiryDate)
        {
            decimal currPrice = currentPrice, spotPrice = strikePrice, riskFR = riskFree, iv = vol; // this allows to vary the inputs & Create with the original
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
                    currPrice += 1.75M;
                    spotPrice -= 0.75M;
                    iv += 12.25M;
                    riskFR -= 0.01M;
                    if (spotPrice <= 0.00M)
                    {
                        spotPrice = 5;
                    }
                    if (riskFR < 0.00M)
                    {
                        riskFR = 1.00M;
                    }
                }
                else
                {
                    currPrice -= 1.00M;
                    spotPrice += 2.50M;
                    riskFR += 0.02M;
                    iv -= 7.25M;
                    if (iv < 0.00M) // IV can never be negative
                    {
                        iv = 0;
                    }
                    if (currPrice <= 0.00M)
                    {
                        currPrice = 3.00M;
                    }

                }
            } // end outer loop varation
        } // end 
    }
}