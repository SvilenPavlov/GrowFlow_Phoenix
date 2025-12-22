
namespace GrowFlow_Phoenix.Infrastructure.Leviathan.Services
{
    using AutoMapper;
    using GrowFlow_Phoenix.DTOs;
    using GrowFlow_Phoenix.DTOs.Leviathan.Employee;
    using GrowFlow_Phoenix.Infrastructure.Leviathan.Requests;
    using GrowFlow_Phoenix.Models.Phoenix;
    using Microsoft.AspNetCore.WebUtilities;
    using System;
    using System.Net;
    using System.Net.Http.Json;
    using System.Text;
    using System.Text.Json;

    public class LeviathanClient
    {
        private readonly HttpClient _http;
        private readonly IMapper _mapper;
        private string _employeeEndpoint;
        private string _testFilePathCreate = Path.Combine("TestResponses", "CreateEmployee.json");
        private string _testFilePathGet = Path.Combine("TestResponses", "GetEmployees.json");
        private bool _isTesting;

        public LeviathanClient(HttpClient http, IConfiguration configuration, IMapper mapper)
        {
            _http = http;
            _mapper = mapper;
            _http.BaseAddress = new Uri(configuration.GetValue<string>("LeviathanApi:BaseURL"));
            _employeeEndpoint = configuration.GetValue<string>("LeviathanApi:Endpoints:EmployeeEndpoint");
            _isTesting = configuration.GetValue<bool>("LeviathanApi:Settings:UseLocalResponse");
        }

        public async Task<Guid> CreateEmployeeAsync(Employee employee)
        {
            var requestDto = _mapper.Map<EmployeeCreateDTO>(employee);

            var request = new PayloadRequest<EmployeeCreateDTO>(requestDto);
            var jsonRequest = JsonSerializer.Serialize(request);
            var httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            Uri uri = new Uri(_employeeEndpoint, UriKind.Relative);
            
            var response = await AquireResponse(_isTesting, _testFilePathCreate, uri, httpContent);
            
            var responseJson = await response.Content.ReadAsStringAsync();
            var responseDto = JsonSerializer.Deserialize<EmployeeResponseDTO>(responseJson);

            if (response.StatusCode == HttpStatusCode.BadRequest)
                throw new InvalidOperationException("Business rule violation from Leviathan");

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException("Leviathan failure");
            // TODO: Leviathan does not return ID cleanly — document this in README

            return responseDto.LeviathanId;
        }
        public async Task<List<EmployeeResponseDTO>> GetEmployeesAsync(CancellationToken stopToken)
        {
            var result = new List<EmployeeResponseDTO>();
            try
            {
                var request = new BaseRequest();
                // Necessary uglyness due to Leviathan requiring authentication in query parameters instead of headers 
                var url = QueryHelpers.AddQueryString(_employeeEndpoint,
                    new Dictionary<string, string?>
                    {
                        [request.User.Key] = request.User.Value,
                        [request.Key.Key] = request.Key.Value
                    });

                var uri = new Uri(url, UriKind.Relative);
                var response = await AquireResponse(_isTesting, _testFilePathGet, uri);

                var jsonString = await response.Content.ReadAsStringAsync();
                result = JsonSerializer.Deserialize<List<EmployeeResponseDTO>>(jsonString);
            }
            catch (Exception)
            {
                throw new HttpRequestException("Leviathan failure");
            }
            return result;
        }


        // Helper method to save a live Leviathan API response from a text file for simpler testing
        // To replicate a Leviathan-side data change, manually modify the saved text file for a given entity
        public async Task SaveHttpResponseToFile(HttpResponseMessage response, string filePath)
        {
            using var writer = new StreamWriter(filePath, false);

            // Status code
            await writer.WriteLineAsync($"StatusCode: {response.StatusCode}");
            await writer.WriteLineAsync("--- Headers ---");

            // Headers
            foreach (var header in response.Headers)
            {
                await writer.WriteLineAsync($"{header.Key}: {string.Join(", ", header.Value)}");
            }

            await writer.WriteLineAsync("--- Content ---");

            // Body
            var content = await response.Content.ReadAsStringAsync();
            await writer.WriteLineAsync(content);
        }
        // Helper method to recreate a live Leviathan API response from a text file for simpler testing
        public async Task<HttpResponseMessage> RecreateResponseFromText(string filePath)
        {
            var lines = await File.ReadAllLinesAsync(filePath);
            var statusLine = lines[0].Replace("StatusCode: ", "").Trim();
            var statusCode = Enum.Parse<HttpStatusCode>(statusLine);

            var response = new HttpResponseMessage(statusCode);
            int i = 2; // Skip "StatusCode" + "--- Headers ---"

            // Read headers until "--- Content ---"
            while (i < lines.Length && lines[i] != "--- Content ---")
            {
                var headerLine = lines[i];
                var parts = headerLine.Split(':', 2);
                if (parts.Length == 2)
                    response.Headers.TryAddWithoutValidation(parts[0].Trim(), parts[1].Trim());
                i++;
            }

            // Skip "--- Content ---"
            i++;
            var content = string.Join(Environment.NewLine, lines.Skip(i));
            response.Content = new StringContent(content);

            return response;
        }

        // Helper methods to acquire HttpResponseMessage either from testing file or live API call
        
        public async Task<HttpResponseMessage> AquireResponse(bool testing, string testingFilePath, Uri uri)
        {
            return await AquireResponse(testing, testingFilePath, uri, null);
        }
        public async Task<HttpResponseMessage> AquireResponse(bool testing, string filePath, Uri uri, StringContent? httpstring)
        {
            var response = new HttpResponseMessage();
            if (testing) // Recreates HttpResponseMessage from locally stored Leviathan API response.
            {
                response = await RecreateResponseFromText(filePath);
            }
            else // Live API Call
            {
                if (httpstring != null)
                {
                    response = await _http.PostAsync(uri, httpstring);
                }
                else
                {
                    response = await _http.GetAsync(uri);
                }
                if (!File.Exists(filePath)) // Initially saves live API response text as text file, which can manually be modified to replicate a Leviathan-side data change for testing purposes.
                {
                    await SaveHttpResponseToFile(response, filePath);
                }
            }
            return response;
        }
    }
}
