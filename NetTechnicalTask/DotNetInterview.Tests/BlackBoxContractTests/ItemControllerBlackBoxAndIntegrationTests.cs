using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using DotNetInterview.API.Dtos;
using DotNetInterview.API.Utility;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
//using Microsoft.VisualStudio.TestPlatform.TestHost;
using Xunit;

namespace DotNetInterview.Tests.BlackBoxContractTests
{
    public class ItemControllerBlackBoxAndIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ItemControllerBlackBoxAndIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAllItems_ShouldReturn200AndCorrectJsonStructure()
        {
            // Act
            var response = await _client.GetAsync("/api/items");

            // Assert Status Code
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Assert Content Type
            response.Content.Headers.ContentType.ToString().Should().Be("application/json; charset=utf-8");

            // Assert JSON Structure
            var items = await response.Content.ReadFromJsonAsync<List<ItemDto>>();
            items.Should().NotBeNull().And.HaveCountGreaterThan(0);

            foreach (var item in items)
            {
                Assert.IsType<Guid>(item.Id); // Check that Id is of type Guid
                Assert.False(Guid.Empty.Equals(item.Id)); // Ensure ID is not the default empty GUID

                Assert.IsType<string>(item.Reference); // Check that Reference is a string
                Assert.False(string.IsNullOrWhiteSpace(item.Reference)); // Ensure Reference is not null or empty

                Assert.IsType<string>(item.Name); // Check that Name is a string
                Assert.False(string.IsNullOrWhiteSpace(item.Name)); // Ensure Name is not null or empty

                Assert.IsType<decimal>(item.OriginalPrice); // Check that OriginalPrice is of type decimal
                Assert.True(item.OriginalPrice > 0); // Assert that OriginalPrice is positive

                Assert.IsType<decimal>(item.DiscountedPrice); // Check that DiscountedPrice is of type decimal
                Assert.True(item.DiscountedPrice >= 0); // Assert that DiscountedPrice is valid

                Assert.IsType<decimal>(item.DiscountPercentage); // Check that DiscountPercentage is of type decimal
                Assert.True(item.DiscountPercentage >= 0 && item.DiscountPercentage <= 100); // Assert it's within a reasonable range

                Assert.IsType<string>(item.Status); // Check that Status is a string
                Assert.False(string.IsNullOrWhiteSpace(item.Status)); // Ensure Status is not null or empty

                Assert.IsAssignableFrom<List<VariationDto>>(item.Variations); // Check that Variations is of type List<VariationDto>
                Assert.NotNull(item.Variations); // Ensure Variations is not null

                foreach (var variation in item.Variations)
                {
                    Assert.IsType<string>(variation.Size); // Check that Size is a string
                    Assert.False(string.IsNullOrWhiteSpace(variation.Size)); // Ensure Size is not null or empty

                    Assert.IsType<int>(variation.Quantity); // Check that Quantity is of type int
                }
            }
        }

        [Fact]
        public async Task GetItemById_ShouldReturn200AndCorrectJsonStructure()
        {
            // Arrange
            var testItemId = DefaultGuid.Default;
            var response = await _client.GetAsync($"/api/items/{testItemId}");

            // Assert Status Code
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Assert Content Type
            response.Content.Headers.ContentType.ToString().Should().Be("application/json; charset=utf-8");

            // Assert JSON Structure
            var item = await response.Content.ReadFromJsonAsync<ItemDto>();
            Assert.NotNull(item);

            Assert.IsType<Guid>(item.Id); // Check that Id is of type Guid
            Assert.False(Guid.Empty.Equals(item.Id)); // Ensure ID is not the default empty GUID

            Assert.IsType<string>(item.Reference); // Check that Reference is a string
            Assert.False(string.IsNullOrWhiteSpace(item.Reference)); // Ensure Reference is not null or empty

            Assert.IsType<string>(item.Name); // Check that Name is a string
            Assert.False(string.IsNullOrWhiteSpace(item.Name)); // Ensure Name is not null or empty

            Assert.IsType<decimal>(item.OriginalPrice); // Check that OriginalPrice is of type decimal
            Assert.True(item.OriginalPrice > 0); // Assert that OriginalPrice is positive

            Assert.IsType<decimal>(item.DiscountedPrice); // Check that DiscountedPrice is of type decimal
            Assert.True(item.DiscountedPrice >= 0); // Assert that DiscountedPrice is valid

            Assert.IsType<decimal>(item.DiscountPercentage); // Check that DiscountPercentage is of type decimal
            Assert.True(item.DiscountPercentage >= 0 && item.DiscountPercentage <= 100); // Assert it's within a reasonable range

            Assert.IsType<string>(item.Status); // Check that Status is a string
            Assert.False(string.IsNullOrWhiteSpace(item.Status)); // Ensure Status is not null or empty

            Assert.IsAssignableFrom<List<VariationDto>>(item.Variations); // Check that Variations is of type List<VariationDto>
            Assert.NotNull(item.Variations); // Ensure Variations is not null

            foreach (var variation in item.Variations)
            {
                Assert.IsType<string>(variation.Size); // Check that Size is a string
                Assert.False(string.IsNullOrWhiteSpace(variation.Size)); // Ensure Size is not null or empty

                Assert.IsType<int>(variation.Quantity); // Check that Quantity is of type int
            }
        }

        [Fact]
        public async Task AddItem_ShouldReturn201AndCorrectJsonStructure()
        {
            // Arrange
            var newItem = new ItemDto
            {
                Id = Guid.NewGuid(),
                Reference = "ABC123",
                Name = "Test Item",
                OriginalPrice = 100m,
                DiscountedPrice = 0m,
                DiscountPercentage = 0m,
                Status = "InStock",
                Variations = new List<VariationDto>
            {
                new VariationDto { Size = "M", Quantity = 10 }
            }
            };

            var content = JsonContent.Create(newItem);

            // Act
            var response = await _client.PostAsync("/api/items", content);

            // Assert Status Code
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            // Assert Content Type
            response.Content.Headers.ContentType.ToString().Should().Be("application/json; charset=utf-8");

            // Assert JSON Structure
            var createdItem = await response.Content.ReadFromJsonAsync<ItemDto>();
            Assert.NotNull(createdItem);

            Assert.IsType<Guid>(createdItem.Id); // Check that Id is of type Guid
            Assert.False(Guid.Empty.Equals(createdItem.Id)); // Ensure ID is not the default empty GUID

            Assert.IsType<string>(createdItem.Reference); // Check that Reference is a string
            Assert.False(string.IsNullOrWhiteSpace(createdItem.Reference)); // Ensure Reference is not null or empty

            Assert.IsType<string>(createdItem.Name); // Check that Name is a string
            Assert.False(string.IsNullOrWhiteSpace(createdItem.Name)); // Ensure Name is not null or empty

            Assert.IsType<decimal>(createdItem.OriginalPrice); // Check that OriginalPrice is of type decimal
            Assert.True(createdItem.OriginalPrice > 0); // Assert that OriginalPrice is positive

            Assert.IsType<decimal>(createdItem.DiscountedPrice); // Check that DiscountedPrice is of type decimal
            Assert.True(createdItem.DiscountedPrice >= 0); // Assert that DiscountedPrice is valid

            Assert.IsType<decimal>(createdItem.DiscountPercentage); // Check that DiscountPercentage is of type decimal
            Assert.True(createdItem.DiscountPercentage >= 0 && createdItem.DiscountPercentage <= 100); // Assert it's within a reasonable range

            Assert.IsType<string>(createdItem.Status); // Check that Status is a string
            Assert.False(string.IsNullOrWhiteSpace(createdItem.Status)); // Ensure Status is not null or empty

            Assert.IsAssignableFrom<List<VariationDto>>(createdItem.Variations); // Check that Variations is of type List<VariationDto>
            Assert.NotNull(createdItem.Variations); // Ensure Variations is not null

            foreach (var variation in createdItem.Variations)
            {
                Assert.IsType<string>(variation.Size); // Check that Size is a string
                Assert.False(string.IsNullOrWhiteSpace(variation.Size)); // Ensure Size is not null or empty

                Assert.IsType<int>(variation.Quantity); // Check that Quantity is of type int
            }

            // Tear down
            // Act
            response = await _client.DeleteAsync($"/api/items/{newItem.Id}");
        }

        [Fact]
        public async Task UpdateItem_ShouldReturn204()
        {
            // Arrange
            var testItemId = Guid.NewGuid(); // Replace with an actual test item ID if necessary
            var updatedItem = new ItemDto
            {
                Id = testItemId,
                Reference = "ABC1234",
                Name = "Updated Test Item",
                OriginalPrice = 150m,
                DiscountedPrice = 0m,
                DiscountPercentage = 0m,
                Status = "InStock",
                Variations = new List<VariationDto>
            {
                new VariationDto { Size = "L", Quantity = 5 }
            }
            };

            var content = JsonContent.Create(updatedItem);

            // Act
            var response = await _client.PutAsync($"/api/items/{testItemId}", content);

            // Assert Status Code
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

           
        }

        [Fact]
        public async Task DeleteItem_ShouldReturn204()
        {
            // Arrange
            var testItemId = Guid.NewGuid(); // Replace with an actual test item ID if necessary

            // Act
            var response = await _client.DeleteAsync($"/api/items/{testItemId}");

            // Assert Status Code
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        // Integration tests

      
        [Fact]
        public async Task GetItemById_ShouldReturnSpecificItemWithCorrectDiscount()
        {
            // Arrange
            var response = await _client.GetAsync("/api/items");
            var items = await response.Content.ReadFromJsonAsync<List<ItemDto>>();
            var seedItem = items.Single(i => i.Reference == "B456");

            // Act
            var result = await _client.GetAsync($"/api/items/{seedItem.Id}");
            result.StatusCode.Should().Be(HttpStatusCode.OK);

            var item = await result.Content.ReadFromJsonAsync<ItemDto>();
            item.Should().NotBeNull();
            item.Id.Should().Be(seedItem.Id);
            item.DiscountedPrice.Should().Be(15); // No discount for item2
            item.DiscountPercentage.Should().Be(0);
        }

        [Fact]
        public async Task UpdateItem_WithModifiedQuantity_ShouldApplyCorrectDiscount()
        {
            // Arrange
            var response = await _client.GetAsync("/api/items");
            var items = await response.Content.ReadFromJsonAsync<List<ItemDto>>();
            var itemToUpdate = items.Single(i => i.Reference == "A123");

            // Modify the quantity to exceed the threshold for the discount
            itemToUpdate.Variations[0].Quantity = 8; // Setting to 8 to trigger 20% discount

            // Act
            var updateResponse = await _client.PutAsJsonAsync($"/api/items/{itemToUpdate.Id}", itemToUpdate);
            updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Retrieve the updated item by ID
            var getResponse = await _client.GetAsync($"/api/items/{itemToUpdate.Id}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK); // Ensure we received a successful response

            var updatedItem = await getResponse.Content.ReadFromJsonAsync<ItemDto>();

            // Assert updated discount
            updatedItem.Should().NotBeNull(); // Ensure that the updated item is not null
            updatedItem.DiscountedPrice.Should().Be(28.0m); // Expecting a 20% discount on the original price of 35
            updatedItem.DiscountPercentage.Should().Be(20); // Verify that the discount percentage is applied correctly
        }
    }
}
