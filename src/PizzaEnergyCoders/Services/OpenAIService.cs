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
                ""model"": ""gpt-4o-mini"",
                ""messages"": [
                    {{ ""role"": ""system"", ""content"": ""you are a content editor in sitecore"" }},
                    {{ ""role"": ""user"", ""content"": ""Give me the corresponding sitecore field types foreach string separated with pipes. Respond only the field type separated by pipes. Having the folowing strings separated with pipes: {data}"" }}
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