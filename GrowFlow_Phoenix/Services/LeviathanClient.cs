namespace GrowFlow_Phoenix.Services
{
    using GrowFlow_Phoenix.Data;
    using System.Net.Http.Json;

    public class LeviathanClient
    {
        private readonly HttpClient _http;
        private const string ApiUser = "CHALLENGEUSER";
        private const string ApiKey = "CHALLENGEKEY";

        public LeviathanClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<int> CreateEmployeeAsync(Employee employee)
        {
            var payload = new
            {
                firstName = employee.FirstName,
                lastName = employee.LastName,
                telephone = employee.Telephone,
                ApiUser,
                ApiKey
            };

            var response = await _http.PostAsJsonAsync("/employee/", payload);

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                throw new InvalidOperationException("Business rule violation from Leviathan");

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException("Leviathan failure");

            // Leviathan does not return ID cleanly — document this in README
            return 0;
        }

        public async Task<List<Employee>> GetEmployeesAsync()
        {
            var url = $"/employee/?ApiUser={ApiUser}&ApiKey={ApiKey}";
            var response = await _http.GetFromJsonAsync<List<Employee>>(url);
            return response ?? new();
        }
    }
}
