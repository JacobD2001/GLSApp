using GLSApp.Interfaces;
using GLSApp.Models;
using GLSApp.Repositories;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace GLSApp.Services
{
    public class PrinterService : IPrinterService
    {
        const string printerUrl = "https://moja-drukarka.pl/print";
        private readonly IConsignRepository _consignRepository;

        public PrinterService(IConsignRepository consignRepository)
        {
               _consignRepository = consignRepository;
        }

        /// <summary>
        /// Sends labels to printer.
        /// </summary>
        /// <param name="labels">The list of labels as byte pdf to print.</param>
        /// <exception cref="InvalidOperationException">Thrown when more than 10 labels are provided.</exception>
        /// <exception cref="Exception">Thrown when the printer returns a status code other than OK.</exception>
        /// <exception cref="Exception">Other exceptions.</exception>
        public async Task PrintLabelsAsync(List<byte[]> pdfBytesList)
        {
            if (pdfBytesList.Count > 10)
                throw new InvalidOperationException("Too many labels. Maximum allowed is 10.");

            try
            {
                var client = new RestClient(printerUrl);
                var request = new RestRequest(Method.POST);

                request.AddHeader("Content-Type", "application/pdf");

                foreach (var pdfBytes in pdfBytesList)
                {
                    request.AddFile("pdfFile", pdfBytes, "label.pdf", "application/pdf");
                }

                var response = await client.ExecuteAsync(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception($"Error sending labels to printer. Status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error sending labels to printer: {ex.Message}");
            }
        }
    }
}
