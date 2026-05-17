using FluentAssertions;
using TechMove.Web.Services;

namespace TechMove.Tests
{
    public class CurrencyServiceTests
    {
        private readonly CurrencyService _currencyService;

        public CurrencyServiceTests()
        {
            // We use null here because ConvertUsdToZar doesn't need HttpClient or config
            _currencyService = new CurrencyService(null!, null!);
        }

        [Fact]
        public void ConvertUsdToZar_CorrectCalculation_ReturnsExpectedResult()
        {
            // Arrange
            decimal usdAmount = 100;
            decimal rate = 18.50m;

            // Act
            var result = _currencyService.ConvertUsdToZar(usdAmount, rate);

            // Assert
            result.Should().Be(1850.00m);
        }

        [Fact]
        public void ConvertUsdToZar_ZeroAmount_ReturnsZero()
        {
            // Arrange
            decimal usdAmount = 0;
            decimal rate = 18.50m;

            // Act
            var result = _currencyService.ConvertUsdToZar(usdAmount, rate);

            // Assert
            result.Should().Be(0);
        }

        [Fact]
        public void ConvertUsdToZar_ZeroRate_ReturnsZero()
        {
            // Arrange
            decimal usdAmount = 100;
            decimal rate = 0;

            // Act
            var result = _currencyService.ConvertUsdToZar(usdAmount, rate);

            // Assert
            result.Should().Be(0);
        }

        [Fact]
        public void ConvertUsdToZar_RoundsToTwoDecimalPlaces()
        {
            // Arrange
            decimal usdAmount = 100;
            decimal rate = 18.556m;

            // Act
            var result = _currencyService.ConvertUsdToZar(usdAmount, rate);

            // Assert
            result.Should().Be(1855.60m);
        }

        [Fact]
        public void ConvertUsdToZar_LargeAmount_CalculatesCorrectly()
        {
            // Arrange
            decimal usdAmount = 10000;
            decimal rate = 19.25m;

            // Act
            var result = _currencyService.ConvertUsdToZar(usdAmount, rate);

            // Assert
            result.Should().Be(192500.00m);
        }
    }
}