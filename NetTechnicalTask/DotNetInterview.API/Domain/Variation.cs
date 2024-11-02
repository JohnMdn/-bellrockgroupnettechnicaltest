namespace DotNetInterview.API.Domain;

public record Variation
{
    public Guid Id { get; set; }
    public Guid ItemId { get; set; }
    public required string Size { get; set; }
    public int Quantity { get; set; }
}
