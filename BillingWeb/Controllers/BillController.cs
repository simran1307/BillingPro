using BillingWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace BillingWeb.Controllers;

public class BillController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _baseUrl;

   public BillController(
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration)
{
    _httpClientFactory = httpClientFactory;

    _baseUrl =
        configuration["ApiSettings:BaseUrl"]
        ?? string.Empty;
}
    public async Task<IActionResult> PrepareBill()
    {
        if(string.IsNullOrEmpty(
    HttpContext.Session.GetString(
        "JwtToken")))
{
    return RedirectToAction(
        "Login",
        "Account");
}

        var client =
    _httpClientFactory.CreateClient();

var token =
    HttpContext.Session.GetString("JwtToken");

  
client.DefaultRequestHeaders.Authorization =
    new System.Net.Http.Headers.AuthenticationHeaderValue(
        "Bearer",
        token);

        string json =
           await client.GetStringAsync(
    _baseUrl + "GetItems");

        var items =
            JsonConvert.DeserializeObject<
                List<BillingItemViewModel>>
                (json);

        var model =
            new PrepareBillViewModel
            {
                Items = items ?? new()
            };

        return View(model);
    }


    [HttpPost]
public async Task<IActionResult> PrepareBill(
    PrepareBillViewModel model)
{
    if(string.IsNullOrEmpty(
    HttpContext.Session.GetString(
        "JwtToken")))
{
    return RedirectToAction(
        "Login",
        "Account");
}

    var client =
        _httpClientFactory.CreateClient();

        var token =
    HttpContext.Session.GetString("JwtToken");

   

client.DefaultRequestHeaders.Authorization =
    new System.Net.Http.Headers.AuthenticationHeaderValue(
        "Bearer",
        token);

    var request =
        new GenerateBillRequestViewModel
        {
            CustomerName = model.CustomerName,
            CustomerPhone = model.CustomerPhone
        };

    foreach(var item in model.Items)
    {
        if(item.Quantity > 0)
        {
            request.Items.Add(
                new GenerateBillItemViewModel
                {
                    ItemId = item.ItemId,
                    Quantity = item.Quantity
                });
        }
    }


    var json =
        JsonConvert.SerializeObject(request);

    var content =
        new StringContent(
            json,
            Encoding.UTF8,
            "application/json");

    var response =
        await client.PostAsync(
    _baseUrl + "GenerateBill",
    content);

    string result =
        await response.Content
            .ReadAsStringAsync();
            
    var bill =    JsonConvert.DeserializeObject<BillResultViewModel>(result);
             ViewBag.Bill = bill;
    // ViewBag.BillResult = result;

    return View(model);
}

[HttpGet]
public IActionResult AddItem()
{
    if (string.IsNullOrEmpty(
        HttpContext.Session.GetString("JwtToken")))
    {
        return RedirectToAction(
            "Login",
            "Account");
    }

    return View();
}

[HttpPost]
public async Task<IActionResult> AddItem(
    AddItemViewModel model)
{
    if (string.IsNullOrEmpty(
        HttpContext.Session.GetString("JwtToken")))
    {
        return RedirectToAction(
            "Login",
            "Account");
    }

    var client =
        _httpClientFactory.CreateClient();

        var token =
    HttpContext.Session.GetString("JwtToken");



client.DefaultRequestHeaders.Authorization =
    new System.Net.Http.Headers.AuthenticationHeaderValue(
        "Bearer",
        token);




    var json =
        JsonConvert.SerializeObject(model);

    var content =
        new StringContent(
            json,
            Encoding.UTF8,
            "application/json");

    var response =
       await client.PostAsync(
    _baseUrl + "AddItem",
    content);

    if (response.IsSuccessStatusCode)
    {
        ViewBag.Message =
            "Item Added Successfully";
             return RedirectToAction("PrepareBill");
    }

    return View();
}
}