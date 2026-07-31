namespace BillingFunctions.Models;

public class BillResponse
{
    public string CustomerName { get; set; } = string.Empty;

    public string CustomerPhone { get; set; } = string.Empty;

    public List<BillResponseItem> Items { get; set; } = new();

    public decimal GrandTotal { get; set; }
    public string GrandTotalInWords{ get; set;} = string.Empty;
    
}
