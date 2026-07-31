namespace BillingFunctions.Models;

public class BillRequest
{
    public string CustomerName { get; set; } = string.Empty;

    public string CustomerPhone { get; set; } = string.Empty;

    public List<BillRequestItem> Items { get; set; } = new();
}
