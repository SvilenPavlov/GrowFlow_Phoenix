namespace GrowFlow_Phoenix.Services
{
    using GrowFlow_Phoenix.Data;
    using GrowFlow_Phoenix.DTOs;
    using System.Net.Http.Json;
    using System.Text;
    using System.Text.Json;

    public class LeviathanClient
    {
        private readonly HttpClient _http;
        private string _apiUser;
        private string _apiKey;

        public LeviathanClient(HttpClient http, IConfiguration configuration)
        {
            _http = http;
            _apiUser = configuration.GetValue<string>("LeviathanCreds:ApiUser");
            _apiKey = configuration.GetValue<string>("LeviathanCreds:ApiKey");
            http.BaseAddress = new Uri(configuration.GetValue<string>("LeviathanApi:BaseURL"));
        }

        public async Task<string> CreateEmployeeAsync(Employee employee)
        {
            var leviathanDto = new LeviathanCustomerRequestDTO
            {
                Name = employee.FirstName + " " + employee.LastName,
                Address = "Test Address LV",
                Employeeid = employee.Id.ToString()
            };
            var dict = AddCredentials(_apiKey, _apiUser, leviathanDto);

            var jsonString = JsonSerializer.Serialize(dict);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var leviathanEndpoint = "/customer/";
            var response = await _http.PostAsync(leviathanEndpoint,content);
            var responseString = await response.Content.ReadAsStringAsync(); // HTML response

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                throw new InvalidOperationException("Business rule violation from Leviathan");

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException("Leviathan failure");
            // Leviathan does not return ID cleanly — document this in README
            
            return string.Empty;
        }

        private Dictionary<string,object?> AddCredentials(string apiKey, string apiUser, LeviathanCustomerRequestDTO payload)
        {
            // Use reflection or dynamic type to inject properties
            var dict = payload.GetType().GetProperties().ToDictionary(
                prop => prop.Name,
                prop => prop.GetValue(payload)
            );

            dict["ApiUser"] = _apiUser;
            dict["ApiKey"] = _apiKey;

            return dict;
        }

        public async Task<List<LeviathanCustomerResponseDTO>> GetEmployeesAsync()
        {
            try
            {
                var url = $"/employee/?ApiUser={_apiUser}&ApiKey={_apiKey}";
                var response = await _http.GetFromJsonAsync<List<LeviathanCustomerResponseDTO>>(url);
                return response;
            }
            catch (Exception)
            {
                throw;
            }
           
            //return response ?? new();
        }
    }
}
