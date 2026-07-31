using System.Net;
using BillingFunctions.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Newtonsoft.Json;

namespace BillingFunctions;

public class GetItemsFunction
{
    private readonly SqlService _sqlService;

    public GetItemsFunction(
        SqlService sqlService)
    {
        _sqlService = sqlService;
    }

    [Function("GetItems")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get")]
        HttpRequestData req)
    {
        var items =
            await _sqlService.GetItemsAsync();

        var response =
            req.CreateResponse(
                HttpStatusCode.OK);

        await response.WriteStringAsync(
            JsonConvert.SerializeObject(items));

        return response;
    }
}