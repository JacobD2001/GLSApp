using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using GLSApp.Interfaces;
using GLSApp.Models;
using GLSApp.Repositories;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GLSApp.Data.Enums;

namespace GLSApp.Services
{
    public class GlsApiService : IGlsApiServiceInterface
    {
        private const string ApiUrl = "https://xxx.xxx.xxx/ade_webapi2.php?wsdl"; //API GLS URL - no access
        private readonly IConsignRepository _consignRepository;
        public GlsApiService(IConsignRepository consignRepository)
        {
            _consignRepository = consignRepository;
        }

        /// <summary>
        /// Logs in to the GLS API and returns the session ID.
        /// </summary>
        /// <returns>The session ID as a string.</returns>
        public async Task<string> LoginAsync()
        {
            try
            {
                //get user login and user password from Azure Key Vault
                var keyVaultUri = new Uri("https://GLSKeyVault1.vault.azure.net/");
                var secretClient = new SecretClient(keyVaultUri, new DefaultAzureCredential());

                var usernameSecret = await secretClient.GetSecretAsync("ApiUsername");
                var passwordSecret = await secretClient.GetSecretAsync("ApiPassword");

                // adeLogin 
                var client = new RestClient(ApiUrl);
                var request = new RestRequest("adeLogin", Method.POST);

                var loginData = new
                {
                    user_name = usernameSecret.Value.Value, 
                    user_password = passwordSecret.Value.Value
                };

                request.AddParameter("application/json", JsonConvert.SerializeObject(loginData), ParameterType.RequestBody); //json format -> object to json -> send in body

                var response = await client.ExecuteAsync(request);

                if (response.IsSuccessful)
                {
                    dynamic responseData = JsonConvert.DeserializeObject(response.Content);
                    return responseData.session;
                }
                else
                {
                    Console.WriteLine($"Error: {response.ErrorMessage}");
                    return null;
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Exception: {ex.Message}");
                return null;
            }

        }

        /// <summary>
        /// Prepares a box(consignment) and returns the consignment ID.
        /// </summary>
        /// <param name="session">The session ID.</param>
        /// <param name="consignRepository">An instance of IConsignRepository.</param>
        /// <param name="consign">The consignment details.</param>
        /// <returns>The consignment ID as an integer.</returns>
        public async Task<int?> PrepareBoxAsync(string session, Consign consign)
        {
            try
            {
                var client = new RestClient(ApiUrl);
                var request = new RestRequest("adePreparingBox_Insert", Method.POST);

                //data to be sent in api request
                var consignData = new
                {
                    Session = session,
                    consign_prep_data = new
                    {
                        Id = consign.Id,
                        RName1 = consign.RName1,
                        RName2 = consign.RName2,
                        RName3 = consign.RName3,
                        RCountry = consign.RCountry,
                        RZipcode = consign.RZipcode,
                        RCity = consign.RCity,
                        RStreet = consign.RStreet,
                        RPhone = consign.RPhone,
                        RContact = consign.RContact,
                        References = consign.References,
                        Notes = consign.Notes,
                        Quantity = consign.Quantity,
                        Weight = consign.Weight,
                        Date = consign.Date,
                        Pfc = consign.Pfc,
                    }
                };

                // Save consign_prep_data to the database and get id of the consignment
                int? consignmentId = await _consignRepository.AddAsync(consign);

                if (!consignmentId.HasValue)
                {
                    Console.WriteLine("Error saving consign to the database.");
                    return null;
                }

                

                request.AddParameter("application/json", JsonConvert.SerializeObject(consignData), ParameterType.RequestBody);

                var response = await client.ExecuteAsync(request);

                if (response.IsSuccessful)
                {
                    dynamic responseData = JsonConvert.DeserializeObject(response.Content);
                    responseData.id = consignmentId.Value; //TO DO: test it
                    _consignRepository.Save(); 
                    return responseData.id;
                }
                else
                {
                    Console.WriteLine($"Error: {response.ErrorMessage}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Gets the labels for the consignments and returns a list of label files.
        /// </summary>
        /// <param name="session">The session ID.</param>
        /// <param name="consignRepository">An instance of IConsignRepository.</param>
        /// <param name="mode">The label mode.</param>
        /// <returns>A list of label files as strings.</returns>
        public async Task<List<string>> GetLabelsAsync(string session, LabelMode mode)
        {
            try
            {
                var client = new RestClient(ApiUrl);
                var request = new RestRequest("adePreparingBox_GetConsignLabelsExt", Method.POST);

                // Get consignment IDs from the database
                List<Consign> consignments = await _consignRepository.GetConsignmentsAsync();
                List<int> consignmentIds = consignments.Select(c => c.Id).ToList();

                // Fetch labels for each consignment
                List<string> labelFiles = new List<string>();
                foreach (var consignmentId in consignmentIds)
                {
                    var requestData = new
                    {
                        Session = session,
                        id = consignmentId,
                        mode = mode.ToString()
                    };

                    request.AddParameter("application/json", JsonConvert.SerializeObject(requestData), ParameterType.RequestBody);

                    var response = await client.ExecuteAsync(request);

                    if (response.IsSuccessful)
                    {
                        dynamic responseData = JsonConvert.DeserializeObject(response.Content);
                        var labelsArray = responseData.LabelsArray;

                        // Process labelsArray and add label files to the list
                        foreach (var labelData in labelsArray)
                        {
                            string labelFile = labelData.file;
                            labelFiles.Add(labelFile);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Error fetching labels for consignment {consignmentId}: {response.ErrorMessage}");
                    }
                }

                return labelFiles;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return null;
            }
        }
    }

}

