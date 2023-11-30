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
using GLSApp.Models.CommunicationModels;

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
                    LoginResponse responseData = JsonConvert.DeserializeObject<LoginResponse>(response.Content);
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
                        consign.Id,
                        consign.RName1,
                        consign.RName2,
                        consign.RName3,
                        consign.RCountry,
                        consign.RZipcode,
                        consign.RCity,
                        consign.RStreet,
                        consign.RPhone,
                        consign.RContact,
                        consign.References,
                        consign.Notes,
                        consign.Quantity,
                        consign.Weight,
                        consign.Date,
                        consign.Pfc,
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
                    Consign responseData = JsonConvert.DeserializeObject<Consign>(response.Content);
                    consignmentId = responseData.Id;
                    _consignRepository.Save();
                    return responseData.Id;
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
        /// <param name="mode">The label mode.</param>
        /// <returns>A list of label files as strings.</returns>
        public async Task<List<string>> GetLabelsAsync(string session, LabelMode mode)
        {
            try
            {
                var client = new RestClient(ApiUrl);

                //get consignments from database
                List<Consign> consignments = await _consignRepository.GetConsignmentsAsync();

                // Fetch labels for each consignment
                foreach (var consignment in consignments)
                {
                    var request = new RestRequest("adePreparingBox_GetConsignLabelsExt", Method.POST);

                    var requestData = new
                    {
                        Session = session,
                        id = consignment.Id,
                        mode = mode.ToString()
                    };

                    request.AddParameter("application/json", JsonConvert.SerializeObject(requestData), ParameterType.RequestBody);

                    var response = await client.ExecuteAsync(request);

                    if (response.IsSuccessful)
                    {
                        LabelsArrayResponse responseData = JsonConvert.DeserializeObject<LabelsArrayResponse>(response.Content);
                        var labelsArray = responseData?.Labels;

                        // Process labelsArray and add label files to the consignment
                        if (labelsArray != null)
                        {
                            consignment.Labels.AddRange(labelsArray);
                            _consignRepository.Save();
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Error fetching labels for consignment {consignment.Id}: {response.ErrorMessage}");
                    }
                }


                // Return list of labels from all consignments
                return _consignRepository.GetAllLabelsAsync().Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return null;
            }
        }


    }

}

