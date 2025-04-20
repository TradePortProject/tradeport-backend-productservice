using Xunit;
using ProductManagement.Utilities;
using System;

namespace ProductManagement.Tests.Utilities
{
    public class EnumHelperTests
    {
        [Fact]
        public void GetDescription_FromEnumValue_ReturnsCorrectDescription()
        {
            // Act
            var description = EnumHelper.GetDescription(Category.Furniture);

            // Assert
            Assert.Equal("Furniture", description);
        }

        [Fact]
        public void GetDescription_FromIntValue_ReturnsCorrectDescription()
        {
            // Act
            var description = EnumHelper.GetDescription<Category>(2); // Furniture

            // Assert
            Assert.Equal("Furniture", description);
        }

        [Fact]
        public void GetDescription_ReturnsEnumName_IfNoDescriptionAttribute()
        {
            // Arrange: create a dummy enum with no [Description]
            var result = EnumHelper.GetDescription(DayOfWeek.Monday);

            // Assert
            Assert.Equal("Monday", result); // falls back to enum name
        }

        [Fact]
        public void GetEnumFromDescription_ReturnsCorrectEnumValue()
        {
            // Act
            var result = EnumHelper.GetEnumFromDescription<Category>("Home & Garden");

            // Assert
            Assert.Equal((int)Category.HomeAndGarden, result);
        }

        [Fact]
        public void GetEnumFromDescription_Throws_WhenDescriptionNotFound()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() =>
            {
                EnumHelper.GetEnumFromDescription<Category>("NonExistent");
            });

            Assert.Contains("No matching enum value", ex.Message);
        }

        [Fact]
        public void GetDescription_ReturnsNull_WhenFieldIsNull()
        {
            var unknown = (Category)999;

            var description = EnumHelper.GetDescription(unknown);

            Assert.Null(description); // ✅ covers the branch
        }


    }
}
