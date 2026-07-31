namespace BillingFunctions.Models;

public class Item
{
    public int ItemId { get; set; }

    public string ItemName { get; set; } = string.Empty;

    public decimal Price { get; set; }
}