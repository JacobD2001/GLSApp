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
    private readonly IConsignRepository _consignRepository;
    public PrintLabels(IPrinterService printerService, IConsignRepository consignRepository)
    {
        _printerService = printerService;
        _consignRepository = consignRepository;
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
            var request = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(requestBody);

            if (request.ContainsKey("package_id"))
            {
                List<string> base64EncodedPdfs = request["package_id"];

                if (base64EncodedPdfs != null && base64EncodedPdfs.Count > 0)
                {
                    List<byte[]> pdfBytesList = new List<byte[]>();

                    foreach (string base64EncodedPdf in base64EncodedPdfs)
                    {
                        // Decode the base64 string to bytes
                        byte[] pdfBytes = Convert.FromBase64String(base64EncodedPdf);
                        pdfBytesList.Add(pdfBytes);
                    }

                    await _printerService.PrintLabelsAsync(pdfBytesList);

                    return new OkResult();
                }
                else
                {
                    log.LogError("No base64-encoded PDFs provided in the request.");
                    return new BadRequestResult();
                }
            }
            else
            {
                log.LogError("No labels provided in the request.");
                return new BadRequestResult();
            }
        }
        catch (Exception ex)
        {
            log.LogError($"Error in PrintLabelsFunction: {ex.Message}");
            return new StatusCodeResult(500);
        }
    }


}
