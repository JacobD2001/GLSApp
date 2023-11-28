using GLSApp.Interfaces;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GLSApp.Services
{
    public class PrinterService : IPrinterService
    {
        const string printerUrl = "https://moja-drukarka.pl/print";

        /// <summary>
        /// Prints a list of labels asynchronously.
        /// </summary>
        /// <param name="labels">The list of labels to print.</param>
        /// <exception cref="InvalidOperationException">Thrown when more than 10 labels are provided.</exception>
        /// <exception cref="Exception">Thrown when the printer returns a status code other than OK.</exce
        public async Task PrintLabelsAsync(List<string> labels)
        {
            if (labels.Count > 10)
                throw new InvalidOperationException("Too many labels. Maximum allowed is 10.");

            string jsonBody = JsonConvert.SerializeObject(new { package_id = labels });

            var client = new RestClient(printerUrl);
            var request = new RestRequest(Method.POST);

            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", jsonBody, ParameterType.RequestBody);

            var response = await client.ExecuteAsync(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Error sending labels to printer. Status code: {response.StatusCode}");
            }
        }
    }
}
