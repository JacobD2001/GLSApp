using GLSApp.Interfaces;
using GLSApp.Models;
using GLSApp.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using static GLSApp.Data.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

public class GetLabels
{
    private readonly IConsignRepository _consignRepository;
    private readonly IGlsApiServiceInterface _glsApiService;

    public GetLabels(IConsignRepository consignRepository, IGlsApiServiceInterface glsApiService)
    {
        _consignRepository = consignRepository;
        _glsApiService = glsApiService;
    }

    /// <summary>
    /// Azure Function triggered every 10 minutes to get labels.
    /// </summary>
    /// <param name="myTimer">The timer information.</param>
    /// <param name="log">The logger instance.</param>
    /// <returns>A list of labels as strings.</returns>
    [FunctionName("GetLabels")]
    public async Task<List<string>> RunAsync([TimerTrigger("0 */10 * * * *")] TimerInfo myTimer, ILogger log, LabelMode label)
    {
        try
        {
            string session = await _glsApiService.LoginAsync();

            if (session != null)
            {
                log.LogInformation("User logged in.");

                List<Consign> consignments = await _consignRepository.GetConsignmentsAsync();

                // Prepare boxes and get consignment IDs
                List<int?> preparedConsignmentIds = new List<int?>();

                foreach (var consignData in consignments)
                {
                    int? consignmentId = await _glsApiService.PrepareBoxAsync(session, consignData);

                    if (consignmentId.HasValue)
                    {
                        log.LogInformation($"Parcel prepared. Parcel id: {consignmentId}");
                        preparedConsignmentIds.Add(consignmentId);
                    }
                    else
                    {
                        log.LogError("Error when trying to prepare parcel.");
                    }
                }

                // Fetch labels for all prepared consignments
                List<string> labelsForPrinting = await _glsApiService.GetLabelsAsync(session, label); 

                if (labelsForPrinting != null && labelsForPrinting.Any())
                {
                    log.LogInformation($"Fetched {labelsForPrinting.Count} labels for printing.");

                    return labelsForPrinting;
                }
                else
                {
                    log.LogWarning("No labels found for printing.");
                    return new List<string>(); 
                }
            }
            else
            {
                log.LogError("Error when logging in the user");
                return null;
            }
        }
        catch (Exception ex)
        {
            log.LogError($"Error: {ex.Message}");
            return null;
        }
    }


}
