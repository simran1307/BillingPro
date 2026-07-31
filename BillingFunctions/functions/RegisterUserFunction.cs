using System.Net;
using BillingFunctions.Models;
using BillingFunctions.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Newtonsoft.Json;

namespace BillingFunctions;

public class RegisterUserFunction
{
    private readonly SqlService _sqlService;
    private readonly KeyVaultService _keyVaultService;

    public RegisterUserFunction(
        SqlService sqlService,
        KeyVaultService keyVaultService)
    {
        _sqlService = sqlService;
        _keyVaultService = keyVaultService;
    }

    [Function("RegisterUser")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")]
        HttpRequestData req)
    {
        string requestBody =
            await new StreamReader(req.Body)
                .ReadToEndAsync();

        RegisterUserRequest? request =
            JsonConvert.DeserializeObject<RegisterUserRequest>(
                requestBody);

        if (request == null)
        {
            var badResponse =
                req.CreateResponse(HttpStatusCode.BadRequest);

            await badResponse.WriteStringAsync(
                "Invalid request");

            return badResponse;
        }

        string secretName =
            $"{request.UserName}-password";

        await _keyVaultService.SavePasswordAsync(
            secretName,
            request.Password);

        await _sqlService.AddUserAsync(
            request.UserName,
            secretName);

        var response =
            req.CreateResponse(HttpStatusCode.OK);

        await response.WriteStringAsync(
            "User registered successfully");

        return response;
    }
   
}
