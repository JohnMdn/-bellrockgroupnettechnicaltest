using DotNetInterview.API.Dtos;
using DotNetInterview.API.Services;
using DotNetInterview.API.Repositories;
using Moq;
using Xunit;
using FluentAssertions;
using DotNetInterview.API.Domain;
using DotNetInterview.API.Utility;


namespace DotNetInterview.Tests.UnitTests
{
    public class ItemServiceTests
    {
        private readonly Mock<IItemRepository> _itemRepositoryMock;
        private readonly ItemService _itemService;

        public ItemServiceTests()
        {
            _itemRepositoryMock = new Mock<IItemRepository>();
            _itemService = new ItemService(_itemRepositoryMock.Object);
        }

        [Fact]
        public async Task GetAllItemsAsync_ShouldReturnMappedItems()
        {
            // Arrange
            var items = new List<Item>
        {
            new Item { Id = Guid.NewGuid(), Reference = "Ref1", Name = "Item1", Price = 100 },
            new Item { Id = Guid.NewGuid(), Reference = "Ref2", Name = "Item2", Price = 200 }
        };
            _itemRepositoryMock.Setup(repo => repo.GetAllItemsAsync()).ReturnsAsync(items);

            // Act
            var result = await _itemService.GetAllItemsAsync();

            // Assert
            result.Should().HaveCount(2);
            result.First().Name.Should().Be("Item1");
            result.Last().OriginalPrice.Should().Be(200);
        }

        [Fact]
        public async Task GetItemByIdAsync_ShouldReturnCorrectItem()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var item = new Item { Id = itemId, Reference = "Ref1", Name = "Item1", Price = 100 };
            _itemRepositoryMock.Setup(repo => repo.GetItemByIdAsync(itemId)).ReturnsAsync(item);

            // Act
            var result = await _itemService.GetItemByIdAsync(itemId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(itemId);
            result.OriginalPrice.Should().Be(100);
        }

        [Fact]
        public async Task AddItemAsync_ShouldCallRepositoryToAddItem()
        {
            // Arrange
            var itemDto = new ItemDto { Id = Guid.NewGuid(), Reference = "Ref1", Name = "Item1", OriginalPrice = 100 };

            // Act
            await _itemService.AddItemAsync(itemDto);

            // Assert
            _itemRepositoryMock.Verify(repo => repo.AddItemAsync(It.IsAny<Item>()), Times.Once);
        }

        [Fact]
        public async Task UpdateItemAsync_ShouldUpdateRepositoryWhenItemExists()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var existingItem = new Item { Id = itemId, Reference = "Ref1", Name = "Item1", Price = 100 };
            _itemRepositoryMock.Setup(repo => repo.GetItemByIdAsync(itemId)).ReturnsAsync(existingItem);

            var updatedItemDto = new ItemDto { Id = itemId, Reference = "Ref1", Name = "UpdatedItem", OriginalPrice = 150 };

            // Act
            await _itemService.UpdateItemAsync(itemId, updatedItemDto);

            // Assert
            _itemRepositoryMock.Verify(repo => repo.UpdateItemAsync(It.Is<Item>(i => i.Name == "UpdatedItem")), Times.Once);
        }

        [Fact]
        public async Task DeleteItemAsync_ShouldCallRepositoryToDeleteItem()
        {
            // Arrange
            var itemId = Guid.NewGuid();

            // Act
            await _itemService.DeleteItemAsync(itemId);

            // Assert
            _itemRepositoryMock.Verify(repo => repo.DeleteItemAsync(itemId), Times.Once);
        }

        [Fact]
        public void CalculateDiscountedPrice_ShouldApply10PercentDiscount_WhenQuantityIsGreaterThan5()
        {
            // Arrange
            var item = new Item
            {
                Price = 100m,
                Variations = new List<Variation>
            {
                new Variation { Size = "M", Quantity = 6 }
            }
            };

            // Act
            var discountedPrice = _itemService.CalculateDiscountedPrice(item);

            // Assert
            discountedPrice.Should().Be(90m); // 10% discount on 100
        }

        [Fact]
        public void CalculateDiscountedPrice_ShouldApply20PercentDiscount_WhenQuantityIsGreaterThan10()
        {
            // Arrange
            var item = new Item
            {
                Price = 100m,
                Variations = new List<Variation>
            {
                new Variation { Size = "M", Quantity = 11 }
            }
            };

            // Act
            var discountedPrice = _itemService.CalculateDiscountedPrice(item);

            // Assert
            discountedPrice.Should().Be(80m); // 20% discount on 100
        }

        [Fact]
        public void CalculateDiscountedPrice_ShouldApply50PercentDiscount_WhenMondayBetween12And5()
        {
            // Arrange
            var item = new Item
            {
                Price = 100m,
                Variations = new List<Variation>
            {
                new Variation { Size = "M", Quantity = 1 }
            }
            };

            // Mock DateTime to Monday, 1 PM
            var mockDateTime = new DateTime(2024, 1, 1, 13, 0, 0); // Monday, 1 PM

            using (new MockedDateTimeScope(mockDateTime))
            {
                // Act
                var discountedPrice = _itemService.CalculateDiscountedPrice(item);

                // Assert
                discountedPrice.Should().Be(50m); // 50% discount on 100
            }
        }

        [Fact]
        public void CalculateDiscountedPrice_ShouldApplyOnlyHighestDiscount_WhenMultipleDiscountsApply()
        {
            // Arrange
            var item = new Item
            {
                Price = 100m,
                Variations = new List<Variation>
            {
                new Variation { Size = "M", Quantity = 15 }
            }
            };

            // Mock DateTime to Monday, 1 PM
            var mockDateTime = new DateTime(2024, 1, 1, 13, 0, 0); // Monday, 1 PM

            using (new MockedDateTimeScope(mockDateTime))
            {
                // Act
                var discountedPrice = _itemService.CalculateDiscountedPrice(item);

                // Assert
                discountedPrice.Should().Be(50m); // Only 50% discount should apply, not 20%
            }
        }
    }
}
