using System.Text.Json.Serialization;

namespace GrowFlow_Phoenix.DTOs
{
    public class LeviathanCustomerRequestDTO
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }       // Customer name
        [JsonPropertyName("Address")]
        public string Address { get; set; }    // Customer address (optional)
        [JsonPropertyName("employid")]
        public string Employeeid { get; set; }   // Employee ID from Phoenix
    }
}
