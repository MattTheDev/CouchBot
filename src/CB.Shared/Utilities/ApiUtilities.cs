using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace CB.Shared.Utilities;

public static class ApiUtilities
{
    public static async Task<T> ApiHelper<T>(HttpClient client, 
        string url, 
        List<(string name, string value)> headers = null)
    {
        try
        {
            if (headers?.Count > 0)
            {
                foreach (var (name, value) in headers)
                {
                    if (name.Equals("Content-Type", StringComparison.InvariantCultureIgnoreCase))
                    {
                        client.DefaultRequestHeaders.Accept
                            .Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    }
                    else if (name.Equals("User-Agent", StringComparison.InvariantCultureIgnoreCase))
                    {
                        client.DefaultRequestHeaders.TryAddWithoutValidation(name, value);
                    }
                    else
                    {
                        client.DefaultRequestHeaders.Add(name, value);
                    }
                }
            }

            var response = await client
                .GetAsync(url)
                .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                return default;
            }

            var responseText = await response
                .Content
                .ReadAsStringAsync()
                .ConfigureAwait(false);

            return JsonConvert.DeserializeObject<T>(responseText);
        }
        catch (Exception)
        {
            // Error on the return. Return null, and handle it up the call stack.
            return default;
        }
    }

    public static async Task<T> ApiHelper<T>(string url, 
        List<(string name, string value)> headers = null)
    {
        try
        {
            using var client = new HttpClient();

            if (headers?.Count > 0)
            {
                foreach (var (name, value) in headers)
                {
                    if (name.Equals("Content-Type", StringComparison.InvariantCultureIgnoreCase))
                    {
                        client.DefaultRequestHeaders.Accept
                            .Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    }
                    else if (name.Equals("User-Agent", StringComparison.InvariantCultureIgnoreCase))
                    {
                        client.DefaultRequestHeaders.TryAddWithoutValidation(name, value);
                    }
                    else
                    {
                        client.DefaultRequestHeaders.Add(name, value);
                    }
                }
            }

            var response = await client
                .GetAsync(url)
                .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                return default;
            }

            var responseText = await response
                .Content
                .ReadAsStringAsync()
                .ConfigureAwait(false);

            return JsonConvert.DeserializeObject<T>(responseText);
        }
        catch (Exception)
        {
            // Error on the return. Return null, and handle it up the call stack.
            return default;
        }
    }

    public static async Task<T> PostApiHelper<T>(string url, 
        List<(string name, string value)> headers = null, 
        string payloadString = null)
    {
        try
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Accept
            .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (headers?.Count > 0)
            {
                foreach (var (name, value) in headers)
                {
                    client.DefaultRequestHeaders.Add(name, value);
                }
            }

            StringContent content = null;
            if (!string.IsNullOrWhiteSpace(payloadString))
            {
                content = new StringContent(payloadString, Encoding.UTF8, "application/json");
            }

            var response = await client
                .PostAsync(url, content)
                .ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                return default;
            }

            var responseText = await response
                .Content
                .ReadAsStringAsync()
                .ConfigureAwait(false);
            return JsonConvert.DeserializeObject<T>(responseText);
        }
        catch (Exception)
        {
            // Error on the return. Return null, and handle it up the call stack.
            return default;
        }
    }

    public static async Task<T> GetAsync<T>(string url,
    CancellationToken cancellationToken,
    List<(string name, string value)> headers = null)
    {
        try
        {
            using var client = new HttpClient();

            if (headers?.Count > 0)
            {
                foreach (var (name, value) in headers)
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation(name, value);
                }
            }

            var response = await client
                .GetAsync(url, cancellationToken)
                .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Matt needs to do better. Tell him this.");
                return default;
            }

            var responseText = await response
                .Content
                .ReadAsStringAsync(cancellationToken)
                .ConfigureAwait(false);

            return JsonConvert.DeserializeObject<T>(responseText);
        }
        catch (Exception)
        {
            Console.WriteLine("Matt needs to do better. Tell him this.");
            return default;
        }
    }

    public static async Task<T> PostAsync<T>(string url,
        CancellationToken cancellationToken,
        List<(string name, string value)> headers = null,
        string payloadString = null)
    {
        try
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (headers?.Count > 0)
            {
                foreach (var (name, value) in headers)
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation(name, value);
                }
            }

            StringContent content = null;
            if (!string.IsNullOrWhiteSpace(payloadString))
            {
                content = new StringContent(payloadString, Encoding.UTF8, "application/json");
            }

            var response = await client
                .PostAsync(url, content, cancellationToken)
                .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Matt needs to do better. Tell him this.");
                return default;
            }

            var responseText = await response
                .Content
                .ReadAsStringAsync(cancellationToken)
                .ConfigureAwait(false);

            return JsonConvert.DeserializeObject<T>(responseText);
        }
        catch (Exception)
        {
            Console.WriteLine("Matt needs to do better. Tell him this.");
            return default;
        }
    }
}