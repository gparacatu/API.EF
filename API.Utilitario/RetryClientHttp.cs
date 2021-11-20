using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AppPromotora.Api.Utilitario
{
    public class RetryClientHttp 
    {
        private const int HTTP_TIME_OUT = 20000;
        private readonly HttpClient _client;

        public RetryClientHttp(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient();
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.Timeout = TimeSpan.FromSeconds(HTTP_TIME_OUT);
        }

        /// <summary>
        /// Construtor com Authorization e APIKEY
        /// </summary>
        /// <param name="httpClientFactory"></param>
        /// <param name="authorizationToken">'Authorization'</param>
        /// <param name="apiKey">'APIKEY'</param>
        public RetryClientHttp(IHttpClientFactory httpClientFactory, string authorizationToken, string apiKey)
        {
            _client = httpClientFactory.CreateClient();
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.Timeout = TimeSpan.FromSeconds(HTTP_TIME_OUT);

            _client.DefaultRequestHeaders.Add("APIKEY", apiKey);
            _client.DefaultRequestHeaders.Add("Authorization", authorizationToken);
        }

        public async Task<HttpResponseMessage> Post(string url, string postData)
        {
            using (var content = new StringContent(postData, Encoding.UTF8, "application/json"))
                return await _client.PostAsync($"{new Uri(url)}", content);
        }

        public async Task<T> PostFromJson<T>(string url, string postData)
        {
            using (var content = new StringContent(postData, Encoding.UTF8, "application/json"))
            {
                using (var response = _client.PostAsync($"{new Uri(url)}", content).Result)
                { 
                    response.EnsureSuccessStatusCode();
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<T>(jsonString);
                    }
                }
            }
            return default(T);
        }

        public async  Task<HttpResponseMessage> Get(string url)
        {
            return await _client.GetAsync($"{new Uri(url)}");
        }

        public async Task<HttpResponseMessage> Put( string url, string putData)
        {
            using (var content = new StringContent(putData, Encoding.UTF8, "application/json"))
                return await _client.PutAsync(url, content);
        }

        //public class DateTimeConverterUsingDateTimeParseAsFallback : JsonConverter<DateTime>
        //{
        //    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        //    {
        //        Debug.Assert(typeToConvert == typeof(DateTime));

        //        if (!reader.TryGetDateTime(out DateTime value))
        //        {
        //            value = DateTime.Parse(reader.GetString());
        //        }

        //        return value;
        //    }

        //    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        //    {
        //        writer.WriteStringValue(value.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ssZ"));
        //    }
        //}
    }
}
