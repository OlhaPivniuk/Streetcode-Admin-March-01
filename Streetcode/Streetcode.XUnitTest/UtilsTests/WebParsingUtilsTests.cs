using Streetcode.WebApi.Utils;
using Xunit;

namespace Streetcode.XUnitTest.Utils
{
    public class WebParsingUtilsTests
    {
        [Theory]
        [InlineData("пров. Велика", "Велика", "провулок")]
        [InlineData("проїзд Шевченка", "Шевченка", "проїзд")]
        [InlineData("вул. Гоголя", "Гоголя", "вулиця")]
        [InlineData("просп. Леніна", "Леніна", "проспект")]
        [InlineData("тупик Київський", "Київський", "тупик")]
        [InlineData("пл. Незалежності", "Незалежності", "площа")]
        [InlineData("майдан Шевченка", "Шевченка", "майдан")]
        [InlineData("узвіз Львівський", "Львівський", "узвіз")]
        [InlineData("дорога Мельниківська", "Мельниківська", "дорога")]
        [InlineData("парк Сосновий", "Сосновий", "парк")]
        [InlineData("жилий масив Індустріальний", "Індустріальний", "парк")]
        [InlineData("м-р Петропавлівська", "Петропавлівська", "мікрорайон")]
        [InlineData("алея Дубова", "Дубова", "алея")]
        [InlineData("хутір Сонячний", "Сонячний", "хутір")]
        [InlineData("кв-л Новий", "Новий", "квартал")]
        [InlineData("урочище Лісове", "Лісове", "урочище")]
        [InlineData("набережна Річкова", "Річкова", "набережна")]
        [InlineData("селище Загородне", "Загородне", "селище")]
        [InlineData("лінія Залізнична", "Залізнична", "лінія")]
        [InlineData("шлях Полівничий", "Полівничий", "шлях")]
        [InlineData("спуск Східний", "Східний", "спуск")]
        [InlineData("завулок Садовий", "Садовий", "завулок")]
        [InlineData("острів Тропічний", "Тропічний", "острів")]
        [InlineData("бульв. Шевченка", "Шевченка", "бульвар")]
        [InlineData("шосе Першотравневе", "Першотравневе", "шосе")]
        [InlineData("містечко Сонячне", "Сонячне", "містечко")]
        [InlineData("в’їзд Північний", "Північний", "в’їзд")]
        public void OptimizeStreetname_ValidStreet_ReturnsExpectedResult(string streetName, string expectedStreet, string expectedType)
        {
            // Arrange
            var expected = (expectedStreet, expectedType);

            // Act
            var result = WebParsingUtils.OptimizeStreetname(streetName);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void OptimizeStreetname_EmptyString_ReturnsEmpty()
        {
            // Arrange
            var streetName = "";

            // Act
            var result = WebParsingUtils.OptimizeStreetname(streetName);

            // Assert
            Assert.Equal((string.Empty, string.Empty), result);
        }

        [Fact]
        public void OptimizeStreetname_UnknownStreet_ReturnsEmpty()
        {
            // Arrange
            var streetName = "Some Unknown Street";

            // Act
            var result = WebParsingUtils.OptimizeStreetname(streetName);

            // Assert
            Assert.Equal((string.Empty, string.Empty), result);
        }
    }
}
