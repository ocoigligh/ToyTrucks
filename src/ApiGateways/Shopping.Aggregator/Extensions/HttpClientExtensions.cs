using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Shopping.Aggregator.Extensions
{
    public static class HttpClientExtensions
    {
        public async static Task<T> ReadContentAs<T>(this HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
                throw new ApplicationException($"Error calling the Api: {response.ReasonPhrase}");

            var dataAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonSerializer.Deserialize<T>(dataAsString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        // public static Task<T> PostAsJsonAsync()
        // {

        // }
    }
}