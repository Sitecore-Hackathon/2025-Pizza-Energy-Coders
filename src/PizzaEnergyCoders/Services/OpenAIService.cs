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

        public async Task<ChatCompletionResponse> GetChatCompletionAsync(string data)
        {
            string apiKey = Settings.GetSetting("OPENAI_APIKEY");

            var url = "https://api.openai.com/v1/chat/completions";

            var jsonBody = $@"{{
                ""model"": ""gpt-4o"",
                ""messages"": [
                    {{ ""role"": ""system"", ""content"": ""you are a content editor in sitecore"" }},
                    {{ ""role"": ""user"", ""content"": ""Transform the given word by replacing it with its corresponding Sitecore field type. Allowed Sitecore field types: Date, Datetime, Number, Single-Line Text, Rich Text. Classification Rules: If the word contains only numbers, classify it as Number.If the word matches a date format like dd/MM/yyyy or yyyy-MM-dd, classify it as Date.If the word matches a datetime format (including time), classify it as Datetime.If the word has more than 50 characters or has HTML tags, classify it as Rich Text.Respond only with the replaced field type. Use this word:: {data.Replace("\n","").Replace("\r", "")}"" }}
                ]
            }}";

            using (var client = new HttpClient())
            {
                // Configura el encabezado de autenticación
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                // Crea el contenido de la solicitud
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                // Realiza la solicitud POST
                var response = await client.PostAsync(url, content);

                // Si la respuesta es exitosa, devuelve el contenido como string
                if (response.IsSuccessStatusCode)
                {
                    var aiResponse = await response.Content.ReadAsStringAsync();
                    ChatCompletionResponse responsex = JsonConvert.DeserializeObject<ChatCompletionResponse>(aiResponse);
                    return responsex;
                }
                else
                {
                    // Si hay un error, devuelve el mensaje de error
                    return new ChatCompletionResponse(){ StatusCode = response.StatusCode.ToString()};
                }
            }
        }
    }
}