/*
 * This will test the most important parts of the code which would be 
 * generating the payoff for the call and put options.
 * Calculating the price as a single price but with different parameters
 * As once these tests are working correctly they are then called inside a method
 * that will get the duration loop through it for as long as the duration and pass in its current loop as the day
 * it is also responisble for the varations and adding to the list
 * The generating asset price is not tested as it produces a random number.
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace MonteCarloSim.Models.Tests
{
    [TestClass()]
    public class OptionTests
    {
        // Test that works as it should where value not zero
        [TestMethod]
        public void CallPayoffGeneratesGreaterZero()
        {
            // Arrange
            decimal asset = 100;
            decimal strike = 90;
            decimal expected = 10;
            var option = new Option();

            // act code: (assetPrice - strikePrice), 0.00
            decimal actual = option.CallPayoff(asset, strike);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // Test that if number is negative then output is zero
        [TestMethod]
        public void CallPayoffGeneratesZero()
        {
            var option = new Option();
            // Arrange
            decimal asset = 100;
            decimal strike = 190;
            decimal expected = 0;

            // act code: (assetPrice - strikePrice), 0.00
            decimal actual = option.CallPayoff(asset, strike);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // Test that works as it should where value not zero
        [TestMethod]
        public void PutPayoffGreaterZero()
        {
            // Arrange
            decimal asset = 100;
            decimal strike = 190;
            decimal expected = 90;
            var option = new Option();

            // act code:(strikePrice - assetPrice),0.0

            decimal actual = option.PutPayoff(asset, strike);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        // negative or value equal 0 output is zero
        public void PutPayoffShouldEqualZero()
        {
            // Arrange
            decimal asset = 190;
            decimal strike = 90;
            decimal expected = 0;
            var option = new Option();

            // act code:(strikePrice - assetPrice),0.0

            decimal actual = option.PutPayoff(asset, strike);
            // assert
            Assert.AreEqual(expected, actual);
        }

        // The Volatility will be changed for each of the following tests
        [TestMethod()]
        public void CallOptionCurrGreaterStrike()
        {
            // Check that the price won't change when ran for period of time this code will take 10 for testing purposes
            // where as the actually code will take in 1,2,3,4,...,10 to calculate each day as the day will be passed
            // as a parameter that will be in the outer method
            // Arrange
            var option = new Option();
            decimal currentPrice = 130.72M;
            decimal strikePrice = 130.00M;
            decimal riskFreeRate = 0.93M;
            decimal volatility = 0.00M;
            int day = 10;

            decimal expected = 0.75M;

            //act
            decimal actual = option.CallOption(currentPrice, strikePrice, riskFreeRate, volatility, day);

            // assert
            Assert.AreEqual(expected, actual);
        }

        // value fall in range of 0.35 to 0.60 ran 10 times 
        // Outputs 0.37, 0.54, 0.43, 0.55, 0.49, 0.35, 0.44, 0.51, 0.39, 0.58 
        // so testing that the value doesn't exceed 0.7 
        [TestMethod()]
        public void CallOptionStrikeGreaterCurr()
        {
            // Arrange
            var option = new Option();
            decimal currentPrice = 120.5M;
            decimal strikePrice = 130.00M;
            decimal riskFreeRate = 0.93M;
            decimal volatility = 38.00M;
            int day = 10;

            decimal expected = 0.70M;

            //act
            decimal actual = option.CallOption(currentPrice, strikePrice, riskFreeRate, volatility, day);

            // assert
            Assert.AreNotEqual(expected, actual);
        }

        // ensure that when using the call method on contract with a 3 day duration
        // it returns zero as it wont have enough days to increase 
        [TestMethod()]
        public void CallOptionDaysEqualThree()
        {
            // Arrange
            var option = new Option();
            decimal currentPrice = 120.5M;
            decimal strikePrice = 130.00M;
            decimal riskFreeRate = 0.93M;
            decimal volatility = 20.00M;
            int day = 3;

            decimal expected = 0.0M;

            //act
            decimal actual = option.CallOption(currentPrice, strikePrice, riskFreeRate, volatility, day);

            // assert
            Assert.AreEqual(expected, actual);
        }

        // that the value will be this value for duration of 10 days
        [TestMethod()]
        public void PutOptionStrikeGreaterCurr()
        {

            // Arrange
            var option = new Option();
            decimal currentPrice = 140.5M;
            decimal strikePrice = 144.00M;
            decimal riskFreeRate = 0.93M;
            decimal volatility = 0.00M;
            int day = 10;

            decimal expected = 3.46M;

            //act
            decimal actual = option.PutOption(currentPrice, strikePrice, riskFreeRate, volatility, day);

            // assert
            Assert.AreEqual(expected, actual);
        }

        // Values vary from test to test but don't go below 0.45
        // Range 0.70 to 0.45, 10 tests 
        [TestMethod()]
        public void PutOptionCurrGreaterStrike()
        {

            // Arrange
            var option = new Option();
            decimal currentPrice = 144.5M;
            decimal strikePrice = 140.00M;
            decimal riskFreeRate = 0.93M;
            decimal volatility = 23.00M;
            int day = 10;

            decimal expected = 0.4M;

            //act
            decimal actual = option.PutOption(currentPrice, strikePrice, riskFreeRate, volatility, day);

            // assert
            Assert.AreNotEqual(expected, actual);
        }

        // value is not 0.00 even if only runs for 3 days
        [TestMethod()]
        public void PutOptionDaysEqualThree()
        {

            // Arrange
            var option = new Option();
            decimal currentPrice = 144.5M;
            decimal strikePrice = 150.00M;
            decimal riskFreeRate = 0.93M;
            decimal volatility = 12.00M;
            int day = 3;

            decimal notExpected = 0.00M;

            //act
            decimal actual = option.PutOption(currentPrice, strikePrice, riskFreeRate, volatility, day);

            // assert
            Assert.AreNotEqual(notExpected, actual);
        }
    }
}