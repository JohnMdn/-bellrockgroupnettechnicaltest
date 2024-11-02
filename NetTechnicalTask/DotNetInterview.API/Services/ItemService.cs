using DotNetInterview.API.Domain;
using DotNetInterview.API.Dtos;
using DotNetInterview.API.Enums;
using DotNetInterview.API.Repositories;
using DotNetInterview.API.Utility;

namespace DotNetInterview.API.Services
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository _itemRepository;

        public ItemService(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }

        public async Task<IEnumerable<ItemDto>> GetAllItemsAsync()
        {
            var items = await _itemRepository.GetAllItemsAsync();
            return items.Select(MapToItemDto);
        }

        public async Task<ItemDto> GetItemByIdAsync(Guid id)
        {
            var item = await _itemRepository.GetItemByIdAsync(id);
            return item == null ? null : MapToItemDto(item);
        }

        public async Task AddItemAsync(ItemDto itemDto)
        {
            var item = MapToItem(itemDto);
            await _itemRepository.AddItemAsync(item);
        }

        public async Task UpdateItemAsync(Guid id, ItemDto itemDto)
        {
            var item = await _itemRepository.GetItemByIdAsync(id);
            if (item == null) return;

            item.Reference = itemDto.Reference;
            item.Name = itemDto.Name;
            item.Price = itemDto.OriginalPrice;
            item.Variations = itemDto.Variations.Select(v => new Variation { Size = v.Size, Quantity = v.Quantity }).ToList();

            await _itemRepository.UpdateItemAsync(item);
        }

        public async Task DeleteItemAsync(Guid id) => await _itemRepository.DeleteItemAsync(id);

        private ItemDto MapToItemDto(Item item)
        {
            decimal originalPrice = item.Price;
            decimal discountedPrice = CalculateDiscountedPrice(item);
            decimal discountPercentage = originalPrice > 0
                ? (1 - (discountedPrice / originalPrice)) * 100
                : 0;

            return new ItemDto
            {
                Id = item.Id,
                Reference = item.Reference,
                Name = item.Name,
                OriginalPrice = originalPrice,
                DiscountedPrice = discountedPrice,
                DiscountPercentage = discountPercentage,
                Status = DetermineItemStatus(item).ToString(), 
                Variations = item.Variations.Select(v => new VariationDto { Size = v.Size, Quantity = v.Quantity }).ToList()
            };
        }

        private static Item MapToItem(ItemDto itemDto)
        {
            // Parse the status safely using Enum.TryParse
            ItemStatus status;
            if (!Enum.TryParse(itemDto.Status, true, out status))
            {
                status = ItemStatus.Unknown;
            }

            return new Item
            {
                Id = itemDto.Id,
                Reference = itemDto.Reference,
                Name = itemDto.Name,
                Price = itemDto.OriginalPrice,
                Status = status, 
                Variations = itemDto.Variations.Select(v => new Variation { Size = v.Size, Quantity = v.Quantity }).ToList()
            };
        }

        public decimal CalculateDiscountedPrice(Item item)
        {
            decimal discountPrice = item.Price;
            decimal highestDiscount = 1.0m; // Start with no discount (100%)

            // Calculate total quantity across all variations
            var totalQuantity = item.Variations.Sum(v => v.Quantity);

            // Check for quantity-based discounts
            if (totalQuantity > 10)
            {
                highestDiscount = Math.Min(highestDiscount, 0.8m); // 20% discount
            }
            else if (totalQuantity > 5)
            {
                highestDiscount = Math.Min(highestDiscount, 0.9m); // 10% discount
            }

            // Check for Monday discount
            var now = MockedDateTimeScope.Now;
            if (now.DayOfWeek == DayOfWeek.Monday && now.Hour >= 12 && now.Hour < 17)
            {
                highestDiscount = Math.Min(highestDiscount, 0.5m); // 50% discount
            }

            // Apply the highest discount
            discountPrice *= highestDiscount;

            return discountPrice;
        }
        private static ItemStatus DetermineItemStatus(Item item)
        {
            // Total quantity across all variations
            int totalQuantity = item.Variations.Sum(v => v.Quantity);

            if (totalQuantity == 0)
                return ItemStatus.OutOfStock;
            if (totalQuantity < 5)
                return ItemStatus.LowStock;

            return ItemStatus.InStock;
        }

    }
}
