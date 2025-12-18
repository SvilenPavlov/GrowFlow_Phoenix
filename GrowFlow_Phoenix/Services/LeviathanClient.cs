namespace GrowFlow_Phoenix.Services
{
    using AutoMapper;
    using GrowFlow_Phoenix.Data;
    using GrowFlow_Phoenix.DTOs;
    using GrowFlow_Phoenix.DTOs.Leviathan.Employee;
    using System;
    using System.Net;
    using System.Net.Http.Json;
    using System.Text;
    using System.Text.Json;

    public class LeviathanClient
    {
        private readonly HttpClient _http;
        private string _apiUser;
        private string _apiKey;
        private readonly IMapper _mapper;
        public const string _employeeEndpoint = "/employee/";
        public const string _customerEndpoint = "/customer/";

        public LeviathanClient(HttpClient http, IConfiguration configuration, IMapper mapper)
        {
            _http = http;
            _mapper = mapper;
            _apiUser = configuration.GetValue<string>("LeviathanCreds:ApiUser");
            _apiKey = configuration.GetValue<string>("LeviathanCreds:ApiKey");
            http.BaseAddress = new Uri(configuration.GetValue<string>("LeviathanApi:BaseURL"));
        }

        public async Task<string> CreateEmployeeAsync(Employee employee)
        {
            var filePath = Path.Combine("TestResponses", "CreateEmployee.json"); // testing


            var leviathanDto = _mapper.Map<EmployeeCreateDTO>(employee);

            var request = new LeviathanApiRequest<EmployeeCreateDTO>(leviathanDto, _apiUser, _apiKey);
            var jsonString = JsonSerializer.Serialize(request);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            //var localResponse = await File.ReadAllTextAsync(filePath); //testing
            var response = RecreateFromLoggedResponse(filePath); //testing
            //var response = await _http.PostAsync(_employeeEndpoint, content); // API CALL
            //LogResponse(filePath, response);


            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                throw new InvalidOperationException("Business rule violation from Leviathan");

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException("Leviathan failure");
            // Leviathan does not return ID cleanly — document this in README

            return string.Empty;
        }

        private HttpResponseMessage RecreateFromLoggedResponse(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Logged response file not found.", filePath);

            string json = File.ReadAllText(filePath);
            var doc = System.Text.Json.JsonDocument.Parse(json);
            var root = doc.RootElement;

            // Recreate response
            var response = new HttpResponseMessage
            {
                StatusCode = (HttpStatusCode)root.GetProperty("StatusCode").GetInt32(),
                ReasonPhrase = root.GetProperty("ReasonPhrase").GetString() ?? string.Empty,
                Version = Version.Parse(root.GetProperty("Version").GetString() ?? "1.1"),
                Content = new StringContent(root.GetProperty("Body").GetString() ?? "")
            };

            // Re-add content headers
            foreach (var header in root.GetProperty("ContentHeaders").EnumerateObject())
            {
                foreach (var value in header.Value.EnumerateArray())
                {
                    response.Content.Headers.TryAddWithoutValidation(header.Name, value.GetString());
                }
            }

            // Re-add general headers
            foreach (var header in root.GetProperty("Headers").EnumerateObject())
            {
                foreach (var value in header.Value.EnumerateArray())
                {
                    response.Headers.TryAddWithoutValidation(header.Name, value.GetString());
                }
            }

            return response;
        }

        private async void LogResponse(string filePath, HttpResponseMessage response)
        {
            // Ensure directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? "TestResponses");

            // Capture metadata
            var statusCode = response.StatusCode;
            var reasonPhrase = response.ReasonPhrase;
            var version = response.Version;
            var headers = response.Headers;
            var contentHeaders = response.Content.Headers;
            var body = await response.Content.ReadAsStringAsync();

            // Build a simple JSON object for storage
            var logObject = new
            {
                StatusCode = (int)statusCode,
                ReasonPhrase = reasonPhrase,
                Version = version.ToString(),
                Headers = headers.ToDictionary(h => h.Key, h => h.Value),
                ContentHeaders = contentHeaders.ToDictionary(h => h.Key, h => h.Value),
                Body = body
            };

            string json = System.Text.Json.JsonSerializer.Serialize(logObject, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(filePath, json);
        }

        public async Task<List<LeviathanCustomerResponseDTO>> GetEmployeesAsync()
        {
            try
            {
                var url = $"/employee/?ApiUser={_apiUser}&ApiKey={_apiKey}";
                var jsonString = await _http.GetStringAsync(url);
                var rawList = System.Text.Json.JsonSerializer.Deserialize<List<Dictionary<string, object>>>(jsonString);

                var result = new List<LeviathanCustomerResponseDTO>();

                foreach (var item in rawList)
                {
                    var dto = new LeviathanCustomerResponseDTO
                    {
                        //This mapping is incorrect as I was trying to parse Leviathan customer entries to Phoenix employee ones when I disovered Leviathan has its own employee endpoint. Will think about how to address this.
                        //Name =
                        //Address =
                        //LeviathanEmployeeId
                        //LeviathanId =
                    };

                    result.Add(dto);
                }

                return result;
            }
            catch (Exception)
            {
                throw;
            }

            //return response ?? new();
        }
    }
}
