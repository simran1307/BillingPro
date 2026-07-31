using System.Net;
using BillingFunctions.Models;
using BillingFunctions.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Newtonsoft.Json;
using BillingFunctions.Helpers;

namespace BillingFunctions;

public class GenerateBillFunction
{
    private readonly SqlService _sqlService;

    public GenerateBillFunction(
        SqlService sqlService)
    {
        _sqlService = sqlService;
    }

    [Function("GenerateBill")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "post")]
        HttpRequestData req)
    {
        string body =
            await new StreamReader(req.Body)
            .ReadToEndAsync();

        BillRequest? request =
            JsonConvert.DeserializeObject<BillRequest>(
                body);

        if (request == null)
        {
            return req.CreateResponse(
                HttpStatusCode.BadRequest);
        }

        BillResponse response =
            new BillResponse
            {
                CustomerName =
                    request.CustomerName,

                CustomerPhone =
                    request.CustomerPhone
            };

        var billItemsToSave =
            new List<(int ItemId,
                      int Quantity,
                      decimal LineTotal)>();

        foreach (var billItem in request.Items)
        {
            Item? item =
                await _sqlService
                    .GetItemByIdAsync(
                        billItem.ItemId);

            if (item == null)
                continue;

            decimal total =
                item.Price *
                billItem.Quantity;

            billItemsToSave.Add(
            (
                billItem.ItemId,
                billItem.Quantity,
                total
            ));

            response.Items.Add(
                new BillResponseItem
                {
                    ItemName = item.ItemName,
                    Price = item.Price,
                    Quantity = billItem.Quantity,
                    Total = total
                });

            response.GrandTotal += total;
        }

        int billId =
            await _sqlService.SaveBillAsync(
                request.CustomerName,
                request.CustomerPhone,
                response.GrandTotal);

    response.GrandTotalInWords =
    NumberToWordsHelper.Convert(
        (int)response.GrandTotal)
    + " Rupees Only";


        foreach (var itemToSave in billItemsToSave)
        {
            await _sqlService.SaveBillItemAsync(
                billId,
                itemToSave.ItemId,
                itemToSave.Quantity,
                itemToSave.LineTotal);
        }

        var httpResponse =
            req.CreateResponse(
                HttpStatusCode.OK);

        await httpResponse.WriteStringAsync(
            JsonConvert.SerializeObject(
                response));

        return httpResponse;
    }
}