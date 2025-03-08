using Newtonsoft.Json;
using PizzaEnergyCoders.Models;
using Sitecore.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PizzaEnergyCoders.Services
{
    public class OpenAIService
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public OpenAIService()
        {

        }
        const string url = "https://api.openai.com/v1/chat/completions";
        /// <summary>
        /// Method to call Open AI
        /// </summary>
        /// <param name="data">Receives the data to be analyzed </param>
        /// <returns>This method returns a string with the types for each field</returns>
        public async Task<ChatCompletionResponse> GetChatCompletionAsync(string data)
        {
            //Reads the API Key from the setting config. See the documentation to UPDATE
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
                // Sets authorization
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                // Creates a request body
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                // POST the resquest
                var response = await client.PostAsync(url, content);

                //If there is a success response, it returns a string
                if (response.IsSuccessStatusCode)
                {
                    var aiResponse = await response.Content.ReadAsStringAsync();
                    ChatCompletionResponse responsex = JsonConvert.DeserializeObject<ChatCompletionResponse>(aiResponse);
                    return responsex;
                }
                else
                {
                    // If there is an error, it returns a message error.
                    return new ChatCompletionResponse() { StatusCode = response.StatusCode.ToString() };
                }
            }
        }
    }
}