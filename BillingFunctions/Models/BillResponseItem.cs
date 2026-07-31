namespace BillingFunctions.Models;

public class BillResponseItem
{
    public string ItemName { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public decimal Total { get; set; }
}