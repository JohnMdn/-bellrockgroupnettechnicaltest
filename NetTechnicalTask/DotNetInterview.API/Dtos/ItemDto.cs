using System;
using System.Collections.Generic;

namespace DotNetInterview.API.Dtos
{
    public class ItemDto
    {
        public Guid Id { get; set; }
        public string Reference { get; set; }
        public string Name { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal DiscountedPrice { get; set; }
        public decimal DiscountPercentage { get; set; }  
        public string Status { get; set; }
        public List<VariationDto> Variations { get; set; } = new();
    }
}
