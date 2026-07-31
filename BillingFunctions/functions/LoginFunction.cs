using System.Net;
using System.Text;
using BillingFunctions.Models;
using BillingFunctions.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Newtonsoft.Json;

namespace BillingFunctions;

public class LoginFunction
{
    private readonly SqlService _sqlService;
    private readonly KeyVaultService _keyVaultService;
    private readonly JwtService _jwtService;

    public LoginFunction(
        SqlService sqlService,
        KeyVaultService keyVaultService,
        JwtService jwtService)
    {
        _sqlService = sqlService;
        _keyVaultService = keyVaultService;
        _jwtService = jwtService;
    }

    [Function("Login")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")]
        HttpRequestData req)
    {
        string requestBody =
            await new StreamReader(req.Body).ReadToEndAsync();

        LoginRequest? request =
            JsonConvert.DeserializeObject<LoginRequest>(
                requestBody);

        if (request == null)
        {
            var badResponse =
                req.CreateResponse(HttpStatusCode.BadRequest);

            await badResponse.WriteStringAsync(
                "Invalid Request");

            return badResponse;
        }

        string? secretName =
            await _sqlService.GetSecretNameAsync(
                request.UserName);

        if (string.IsNullOrEmpty(secretName))
        {
            var unauthorized =
                req.CreateResponse(
                    HttpStatusCode.Unauthorized);

            await unauthorized.WriteStringAsync(
                "Invalid Username");

            return unauthorized;
        }

        string storedPassword =
            await _keyVaultService.GetSecretAsync(
                secretName);

        if (storedPassword != request.Password)
        {
            var unauthorized =
                req.CreateResponse(
                    HttpStatusCode.Unauthorized);

            await unauthorized.WriteStringAsync(
                "Invalid Password");

            return unauthorized;
        }

        string token =
            _jwtService.GenerateToken(
                request.UserName);

        var response =
            req.CreateResponse(HttpStatusCode.OK);

        await response.WriteStringAsync(token);

        return response;
    }
}