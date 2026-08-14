using BillingWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace BillingWeb.Controllers;

public class AccountController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _baseUrl;


    public AccountController(
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration)
{
    _httpClientFactory = httpClientFactory;

    _baseUrl =
        configuration["ApiSettings:BaseUrl"]
        ?? string.Empty;
}

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(
        RegisterViewModel model)
    {
        var client =
            _httpClientFactory.CreateClient();

        var json =
            JsonConvert.SerializeObject(model);

        var content =
            new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

        var response =
            await client.PostAsync(
             _baseUrl + "RegisterUser", content);


        
        if (response.IsSuccessStatusCode)
{
    TempData["SuccessMessage"] =
        "User Registered Successfully";

    return RedirectToAction("Login");
}

        return View();
    }

    [HttpGet]
public IActionResult Login()
{
    return View();
}

[HttpPost]
public async Task<IActionResult> Login(
    LoginViewModel model)
{
    var client =
        _httpClientFactory.CreateClient();

    var json =
        JsonConvert.SerializeObject(model);

    var content =
        new StringContent(
            json,
            Encoding.UTF8,
            "application/json");

    var response =
        await client.PostAsync(
    _baseUrl + "Login",
        content);

    if (!response.IsSuccessStatusCode)
    {
        ViewBag.Message =
    "User not found or password is incorrect.";


        return View();
    }

    var token =
        await response.Content
            .ReadAsStringAsync();



    HttpContext.Session.SetString(
        "JwtToken",
        token);

    HttpContext.Session.SetString(
        "UserName",
        model.UserName);

    return RedirectToAction(
        "PrepareBill",
        "Bill");
}

public IActionResult Logout()
{
    HttpContext.Session.Clear();

    return RedirectToAction(
        "Login");
}

}