namespace BillingWeb.Models;

public class PrepareBillViewModel
{
    public string CustomerName { get; set; }
        = string.Empty;

    public string CustomerPhone { get; set; }
        = string.Empty;

    public List<BillingItemViewModel> Items
    {
        get;
        set;
    } = new();
}