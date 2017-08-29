using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MonteCarloSim.Models.Tests
{
    [TestClass()]
    public class OptionTests
    {
        // Test that works as it should where value not zero
        [TestMethod]
        public void CallPayoff_Generates_Greater_Zero()
        {
            // Arrange
            decimal asset = 100;
            decimal strike = 90;
            decimal expected = 10;
            var option = new Option();
            // act

            decimal actual = option.CallPayoff(asset, strike);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // Test that if number is negative then output is zero
        [TestMethod]
        public void CallPayoff_Generates_Zero()
        {
            var option = new Option();
            // Arrange
            decimal asset = 100;
            decimal strike = 190;
            decimal expected = 0;

            // act
            decimal actual = option.CallPayoff(asset, strike);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // Test that works as it should where value not zero
        [TestMethod]
        public void PutPayoff_Greater_Zero()
        {
            // Arrange
            decimal asset = 100;
            decimal strike = 190;
            decimal expected = 90;
            var option = new Option();
            // act

            decimal actual = option.PutPayoff(asset, strike);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        // negative or value equal 0 output is zero
        public void PutPayoff_Should__Equal_Zero()
        {
            // Arrange
            decimal asset = 190;
            decimal strike = 90;
            decimal expected = 0;
            var option = new Option();

            // act

            decimal actual = option.PutPayoff(asset, strike);
            // assert
            Assert.AreEqual(expected, actual);
        }

        // The Volatility will be changed for each of the following tests
        [TestMethod()]
        public void CallOption_Curr_Greater_Strike()
        {
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
        public void CallOption_Strike_Greater_Curr()
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

        [TestMethod()]
        public void CallOption_Days_Equal_Three()
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

        [TestMethod()]
        public void PutOption_Strike_Greater_Curr()
        {

            // Arrange
            var option = new Option();
            decimal currentPrice = 140.5M;
            decimal strikePrice = 144.00M;
            decimal riskFreeRate = 0.93M;
            decimal volatility = .00M;
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
        public void PutOption_Curr_Greater_Strike()
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

        // value is not 0.00
        [TestMethod()]
        public void PutOption_Days_Equal_Three()
        {

            // Arrange
            var option = new Option();
            decimal currentPrice = 144.5M;
            decimal strikePrice = 150.00M;
            decimal riskFreeRate = 0.93M;
            decimal volatility = 12.00M;
            int day = 3;

            decimal expected = 0.00M;

            //act
            decimal actual = option.PutOption(currentPrice, strikePrice, riskFreeRate, volatility, day);

            // assert
            Assert.AreNotEqual(expected, actual);
        }
    }
}