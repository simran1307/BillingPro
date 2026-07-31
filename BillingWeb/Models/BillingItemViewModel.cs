namespace BillingWeb.Models;

public class BillingItemViewModel
{
    public int ItemId { get; set; }

    public string ItemName { get; set; }
        = string.Empty;

    public decimal Price { get; set; }

    public int Quantity { get; set; }
}