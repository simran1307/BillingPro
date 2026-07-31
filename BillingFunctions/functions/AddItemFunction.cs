using System.Net;
using BillingFunctions.Models;
using BillingFunctions.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Newtonsoft.Json;

namespace BillingFunctions;

public class AddItemFunction
{
    private readonly SqlService _sqlService;

    public AddItemFunction(
        SqlService sqlService)
    {
        _sqlService = sqlService;
    }

    [Function("AddItem")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "post")]
        HttpRequestData req)
    {
        string requestBody =
            await new StreamReader(req.Body)
            .ReadToEndAsync();

        Item? item =
            JsonConvert.DeserializeObject<Item>(
                requestBody);

        if (item == null)
        {
            var badResponse =
                req.CreateResponse(
                    HttpStatusCode.BadRequest);

            await badResponse.WriteStringAsync(
                "Invalid Request");

            return badResponse;
        }

        await _sqlService.AddItemAsync(
            item.ItemName,
            item.Price);

        var response =
            req.CreateResponse(
                HttpStatusCode.OK);

        await response.WriteStringAsync(
            "Item Added Successfully");

        return response;
    }
}