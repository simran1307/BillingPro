using System.Net;
using BillingFunctions.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Newtonsoft.Json;

namespace BillingFunctions;

public class GetItemsFunction
{
    private readonly SqlService _sqlService;
    private readonly JwtService _jwtService;

    public GetItemsFunction(
        SqlService sqlService,
        JwtService jwtService)
    {
        _sqlService = sqlService;
        _jwtService = jwtService;
    }

    [Function("GetItems")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get")]
        HttpRequestData req)
    {
        string authHeader =
            req.Headers.TryGetValues(
                "Authorization",
                out var values)
                    ? values.FirstOrDefault() ?? ""
                    : "";

        if (!authHeader.StartsWith("Bearer "))
        {
            var unauthorized =
                req.CreateResponse(
                    HttpStatusCode.Unauthorized);

            await unauthorized.WriteStringAsync(
                "Missing Token");

            return unauthorized;
        }

        var token =
            authHeader.Replace(
                "Bearer ",
                "");

        if (!_jwtService.ValidateToken(token))
        {
Console.WriteLine("JWT VALIDATION FAILED");

            var unauthorized =
                req.CreateResponse(
                    HttpStatusCode.Unauthorized);

            await unauthorized.WriteStringAsync(
                "Invalid Token");

            return unauthorized;
        }
Console.WriteLine("JWT VALIDATION SUCCESS");


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