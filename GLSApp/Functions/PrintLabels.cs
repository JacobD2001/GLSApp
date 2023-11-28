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
            // Read the request body to get the list of labels
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            List<string> labels = JsonConvert.DeserializeObject<List<string>>(requestBody);

            // Print labels using the PrinterService
            await _printerService.PrintLabelsAsync(labels);

            return new OkResult();
        }
        catch (Exception ex)
        {
            log.LogError($"Error in PrintLabelsFunction: {ex.Message}");
            return new StatusCodeResult(500);
        }
    }
}
