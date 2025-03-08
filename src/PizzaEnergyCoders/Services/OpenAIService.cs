using Newtonsoft.Json;
using PizzaEnergyCoders.Models;
using Sitecore.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PizzaEnergyCoders.Services
{
    public class OpenAIService
    {
        public OpenAIService()
        {
            
        }

        public async Task<ChatCompletionResponse> GetChatCompletionAsync(string data, bool checkSensitiveData)
        {
            string apiKey = Settings.GetSetting("OPENAI_APIKEY");

            var url = Settings.GetSetting("OPENAI_URL");

            var openAIPromptUser = "";

            if (checkSensitiveData)
            {
                openAIPromptUser = Settings.GetSetting("OpenAIPromptUserCheckSensitive");
            }
            else
            {
                openAIPromptUser = Settings.GetSetting("OpenAIPromptUser");
            }

            var jsonBody = $@"{{
                ""model"": ""gpt-4o"",
                ""messages"": [
                    {{ ""role"": ""system"", ""content"": ""{Settings.GetSetting("OpenAIPromptSystem")}"" }},
                    {{ ""role"": ""user"", ""content"": ""{openAIPromptUser} {data.Replace("\n","").Replace("\r", "")}"" }}
                ]
            }}";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var aiResponse = await response.Content.ReadAsStringAsync();
                    ChatCompletionResponse responsex = JsonConvert.DeserializeObject<ChatCompletionResponse>(aiResponse);
                    return responsex;
                }
                else
                {
                    return new ChatCompletionResponse(){ StatusCode = response.StatusCode.ToString()};
                }
            }
        }
    }
}