namespace BillingWeb.Models;

public class GenerateBillRequestViewModel
{
    public string CustomerName { get; set; }
        = string.Empty;

    public string CustomerPhone { get; set; }
        = string.Empty;

    public List<GenerateBillItemViewModel> Items
    {
        get;
        set;
    } = new();
}