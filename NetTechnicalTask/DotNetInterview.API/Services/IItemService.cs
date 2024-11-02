using DotNetInterview.API.Domain;
using DotNetInterview.API.Dtos;

namespace DotNetInterview.API.Services
{
    public interface IItemService
    {
        Task<IEnumerable<ItemDto>> GetAllItemsAsync();
        Task<ItemDto> GetItemByIdAsync(Guid id);
        Task AddItemAsync(ItemDto itemDto);
        Task UpdateItemAsync(Guid id, ItemDto itemDto);
        Task DeleteItemAsync(Guid id);
        decimal CalculateDiscountedPrice(Item item);
    }
}
