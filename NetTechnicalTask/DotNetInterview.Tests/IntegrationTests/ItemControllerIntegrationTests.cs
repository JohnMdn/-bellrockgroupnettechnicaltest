using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using DotNetInterview.API.Dtos;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace DotNetInterview.Tests.IntegrationTests
{
    public class ItemControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ItemControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAllItems_ShouldReturnSeededItems()
        {
            // Act
            var response = await _client.GetAsync("/api/items");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var items = await response.Content.ReadFromJsonAsync<List<ItemDto>>();
            items.Should().NotBeNull().And.HaveCount(3);

            // Assert item properties based on seed data
            var item1 = items.SingleOrDefault(i => i.Reference == "A123");
            item1.Should().NotBeNull();
            item1.DiscountedPrice.Should().Be(31.5m); // 10% discount on 35
            item1.DiscountPercentage.Should().Be(10);

            var item2 = items.SingleOrDefault(i => i.Reference == "B456");
            item2.Should().NotBeNull();
            item2.DiscountedPrice.Should().Be(15); // No discount for item2
            item2.DiscountPercentage.Should().Be(0);

            var item3 = items.SingleOrDefault(i => i.Reference == "C789");
            item3.Should().NotBeNull();
            item3.DiscountedPrice.Should().Be(56m); // Assuming a 20% discount applies due to quantity of variations
            item3.DiscountPercentage.Should().Be(20); // Adjusted based on your discount logic
        }
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
