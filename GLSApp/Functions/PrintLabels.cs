using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using GLSApp.Services;
using Newtonsoft.Json;
using System;
using GLSApp.Interfaces;
using GLSApp.Models;

public class PrintLabels
{
    private readonly IPrinterService _printerService;
    public PrintLabels(IPrinterService printerService)
    {
        _printerService = printerService;
    }

    /// <summary>
    /// Azure Function triggered by an HTTP POST request to print labels.
    /// </summary>
    /// <param name="req">The HTTP request.</param>
    /// <param name="log">The logger instance.</param>
    /// <returns>An IActionResult representing the result of the operation.</returns>
    [FunctionName("PrintLabels")]
    public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger log)
    {
        try
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            List<Consign> consignments = JsonConvert.DeserializeObject<List<Consign>>(requestBody); //TO DO : TO Powinna byæ lista intów gdzie ka¿dy int to id przesy³ki 

            if (consignments != null && consignments.Count > 0)
            {
                List<byte[]> pdfBytesList = new List<byte[]>();

                foreach (Consign consignment in consignments)
                {
                    byte[] pdfBytes = await _printerService.GeneratePdfFromConsign(consignment);
                    pdfBytesList.Add(pdfBytes);
                }

                await _printerService.PrintLabelsAsync(pdfBytesList);

                return new OkResult();
            }
            else
            {
                log.LogError("No consignments found in the request.");
                return new BadRequestResult();
            }
        }
        catch (Exception ex)
        {
            log.LogError($"Error in PrintLabelsFunction: {ex.Message}");
            return new StatusCodeResult(500);
        }

    }

    //do funkcji drukarki wysy³am zapytanie z jsonem w formacie: { "package_id": [label1, label2, ...] } - done
    //zamieniam jsona na listê pdfów(binarn¹) => pdfBytesList = [pdf1, pdf2, ...]
    //wysy³am listê pdfów do serwisu drukarki jako argument funkcji PrintLabelsAsync(pdfBytesList) => Ona drukuje
    //w funkcji drukarki zwracam ok result lub nie ok result


}
