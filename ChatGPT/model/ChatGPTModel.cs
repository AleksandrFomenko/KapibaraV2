using System.Net.Http;
using System.Text;
using Newtonsoft.Json; 

namespace ChatGPT.model;



internal class ChatGptModel
{
    private readonly string _host = "http://185.240.51.72";
    // всем пофиг на этот ключ, так что пусть будет тут
    private readonly string _apiKey = "qweqweqweqweqweqwe";
    private readonly HttpClient _httpClient;
    
    public ChatGptModel()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(_host)
        };
        _httpClient.DefaultRequestHeaders.Add("Authorization", _apiKey);
        //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
    }
    
    public async Task<string> SendPromptAsync(string prompt)
    {
        var requestPayload = new
        {
            prompt = prompt
        };

        var json = JsonConvert.SerializeObject(requestPayload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync("/api/openai", content); 
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(responseContent);
            return apiResponse?.Response ?? "Нет ответа от сервера.";
        }
        catch (HttpRequestException ex)
        {
            return $"Ошибка при запросе: {ex.Message}";
        }
        catch (Exception ex)
        {
            return $"Произошла ошибка: {ex.Message}";
        }
    }
}


public class ApiResponse
{
    [JsonProperty("response")]
    public string Response { get; set; }
}