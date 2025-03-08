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
            //Sets the promt to call Open AI
            var jsonBody = $@"{{
                ""model"": ""gpt-4o"",
                ""messages"": [
                    {{ ""role"": ""system"", ""content"": ""you are a content editor in sitecore"" }},
                    {{ ""role"": ""user"", ""content"": ""Transform the given word by replacing it with its corresponding Sitecore field type. Allowed Sitecore field types: Date, Datetime, Number, Single-Line Text, Rich Text. Classification Rules: If the word contains only numbers, classify it as Number.If the word matches a date format like dd/MM/yyyy or yyyy-MM-dd, classify it as Date.If the word matches a datetime format (including time), classify it as Datetime.If the word has more than 50 characters or has HTML tags, classify it as Rich Text.Respond only with the replaced field type. Use this word:: {data.Replace("\n", "").Replace("\r", "")}"" }}
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