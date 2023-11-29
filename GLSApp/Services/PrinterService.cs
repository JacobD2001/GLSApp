using GLSApp.Interfaces;
using GLSApp.Models;
using GLSApp.Repositories;
using iTextSharp.text;
using iTextSharp.text.pdf;
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
        /// <exception cref="Exception">Thrown when the printer returns a status code other than OK.</exce
        public async Task PrintLabelsAsync(List<byte[]> pdfBytesList)
        {
            if (pdfBytesList.Count > 10)
                throw new InvalidOperationException("Too many labels. Maximum allowed is 10.");

            var client = new RestClient(printerUrl);
            var request = new RestRequest(Method.POST);

            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", pdfBytesList, ParameterType.RequestBody);

            var response = await client.ExecuteAsync(request); 

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Error sending labels to printer. Status code: {response.StatusCode}");
            }
        }


        /// <summary>
        /// <summary>
        /// Generates a PDF document from a consignment.
        /// </summary>
        /// <param name="consignment">The consignment.</param>
        /// <returns>The byte array representing the generated PDF.</returns>
        public async Task<byte[]> GeneratePdfFromConsign(Consign consignment)
        {
            await using (MemoryStream ms = new MemoryStream())
            {
                iTextSharp.text.Document document = new iTextSharp.text.Document();
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                document.Open();

                document.Add(new Paragraph($"Id: {consignment.Id}"));
                document.Add(new Paragraph($"RName1: {consignment.RName1}"));
                document.Add(new Paragraph($"RName2: {consignment.RName2}"));
                document.Add(new Paragraph($"RName3: {consignment.RName3}"));
                document.Add(new Paragraph($"RCountry: {consignment.RCountry}"));
                document.Add(new Paragraph($"RZipcode: {consignment.RZipcode}"));
                document.Add(new Paragraph($"RCity: {consignment.RCity}"));
                document.Add(new Paragraph($"RStreet: {consignment.RStreet}"));
                document.Add(new Paragraph($"RPhone: {consignment.RPhone}"));
                document.Add(new Paragraph($"RContact: {consignment.RContact}"));
                document.Add(new Paragraph($"References: {consignment.References}"));
                document.Add(new Paragraph($"Notes: {consignment.Notes}"));
                document.Add(new Paragraph($"Quantity: {consignment.Quantity}"));
                document.Add(new Paragraph($"Weight: {consignment.Weight}"));
                document.Add(new Paragraph($"Date: {consignment.Date}"));
                document.Add(new Paragraph($"Pfc: {consignment.Pfc}"));
             
                document.Close();
                writer.Close();

                return ms.ToArray();
            }
        }





    }
}
