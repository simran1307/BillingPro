namespace BillingWeb.Models;

public class BillResultViewModel
{
    public string CustomerName { get; set; } = string.Empty;

    public string CustomerPhone { get; set; } = string.Empty;

    public decimal GrandTotal { get; set; }

    public string GrandTotalInWords { get; set; }
        = string.Empty;

    public List<BillResultItemViewModel> Items
    {
        get;
        set;
    } = new();
}