
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
        public string _employeeEndpoint;

        public LeviathanClient(HttpClient http, IConfiguration configuration, IMapper mapper)
        {
            _http = http;
            _mapper = mapper;
            _http.BaseAddress = new Uri(configuration.GetValue<string>("LeviathanApi:BaseURL"));
            _employeeEndpoint = configuration.GetValue<string>("LeviathanApi:Endpoints:EmployeeEndpoint");
            _mapper = mapper;
        }

        public async Task<Guid> CreateEmployeeAsync(Employee employee)
        {

            var requestDto = _mapper.Map<EmployeeCreateDTO>(employee);

            var request = new PayloadRequest<EmployeeCreateDTO>(requestDto);
            var jsonRequest = JsonSerializer.Serialize(request);
            var httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var filePath = Path.Combine("TestResponses", "CreateEmployee.json"); // For teesting only.
            //HttpResponseMessage response = RecreateFromLoggedResponse(filePath); // 
            //HttpResponseMessage response = await RecreateResponseFromText(filePath); //For testing: recreates HttpResponseMessage from locally stored Leviathan API response.
            var response = await _http.PostAsync(_employeeEndpoint, httpContent); // Live API call. Uncomment to test live API after saving file once.
            //LogResponse(filePath, response); // For testing: logs Leviathan API response locally for to avoid bloating its data. When testing, uncomment this and above row only once to save file, then use Live API Call.
            await SaveHttpResponseToFile(response, filePath); 
            var responseJson = await response.Content.ReadAsStringAsync();
            var responseDto = JsonSerializer.Deserialize<EmployeeResponseDTO>(responseJson);

            if (response.StatusCode == HttpStatusCode.BadRequest)
                throw new InvalidOperationException("Business rule violation from Leviathan");

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException("Leviathan failure");
            // Leviathan does not return ID cleanly — document this in README

            return responseDto.LeviathanId;
        }
        public async Task<List<EmployeeResponseDTO>> GetEmployeesAsync(CancellationToken stopToken)
        {
            var result = new List<EmployeeResponseDTO>();
            try
            {
                var request = new BaseRequest();
                //This debauchery is necessary due to Leviathan requiring authentication in query parameters instead of headers 
                var url = QueryHelpers.AddQueryString(_employeeEndpoint,
                    new Dictionary<string, string?>
                    {
                        [request.User.Key] = request.User.Value,
                        [request.Key.Key] = request.Key.Value
                    });

                var uri = new Uri(url, UriKind.Relative);
                var filePath = Path.Combine("TestResponses", "GetEmployees.json");
                HttpResponseMessage response = await RecreateResponseFromText(filePath);  //For testing: recreates HttpResponseMessage from locally stored Leviathan API response.
                //var response = await _http.GetAsync(uri); // Live API Call
                var jsonString = await response.Content.ReadAsStringAsync();
                //await SaveHttpResponseToFile(response, filePath);  // For testing: logs Leviathan API response locally for to avoid bloating its data. When testing, uncomment this and above row only once to save file, then use Live API Call.
                result = JsonSerializer.Deserialize<List<EmployeeResponseDTO>>(jsonString);
            }
            catch (Exception)
            {
                //TODO: Leviathan does not populate the list
            }
            return result;
        }

        private HttpResponseMessage RecreateFromLoggedResponse(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Logged response file not found.", filePath);

            string json = File.ReadAllText(filePath);
            var doc = JsonDocument.Parse(json);
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

            string json = JsonSerializer.Serialize(logObject, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(filePath, json);
        }

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
    }
}
