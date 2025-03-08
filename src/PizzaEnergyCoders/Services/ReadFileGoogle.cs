using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using PizzaEnergyCoders.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace PizzaEnergyCoders.Services
{
    public static class ReadFileGoogle
    {
        public static async Task<List<DocumentModel>> Import(string URL)
        {
            return await ReadGoogleSheets(URL);
        }

        static async Task<List<DocumentModel>> ReadGoogleSheets(string URL)
        {
            string[] scopes = { SheetsService.Scope.SpreadsheetsReadonly };

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string credentialsPath = Path.Combine(baseDirectory, "App_Data\\Creds", "credentials.json");
            var credential = GoogleCredential.FromFile(credentialsPath).CreateScoped(scopes);

            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "SCHackaton2025",
            });

            string spreadsheetId = ExtractSheetId(URL);
            //this will iterate up to 100 entries
            string range = "Sheet1!A1:Z101";
            var document = new List<DocumentModel>();
            try
            {
                SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(spreadsheetId, range);
                ValueRange response = request.Execute();
                IList<IList<object>> values = response.Values;

                if (values != null && values.Count > 0)
                {
                    var firstRow = values.First().Where(cell => cell != null && !string.IsNullOrWhiteSpace(cell.ToString()))
                                          .Select(cell => cell.ToString())
                                          .ToList();

                    var secondRow = values[1]
                    .Take(firstRow.Count) // Tomar solo la cantidad de columnas que hay en headers
                    .Select(cell => cell?.ToString() ?? string.Empty) // Manejar valores nulos
                    .ToList();

                    string secondRowStr = string.Join("|", secondRow);

                    //get AI datatypes
                    OpenAIService openAIService = new OpenAIService();
                    var data = await openAIService.GetChatCompletionAsync(secondRowStr);
                    var dataTypes = data.Choices[0].Message.Content.Split('|'); //"Single-Line Text|Number|Date|Multi-Line Text"

                    foreach (var row in values.Skip(1))
                    {
                        if (row.Any(cell => cell == null || string.IsNullOrWhiteSpace(cell.ToString())))
                            break;
                        List<string> rowData = row.Select(cell => cell.ToString())
                                          .ToList();
                        var obj = new DocumentModel()
                        {
                            TemplateName = rowData[0],
                            Title = rowData[1],
                            KeyValues = new Dictionary<string, string>()
                        };
                        for (int i = 2; i < firstRow.Count; i++)
                        {
                            obj.KeyValues.Add(firstRow[i].ToString(), rowData[i].ToString());
                        }
                        document.Add(obj);
                    }
                }
            }
            catch (Google.GoogleApiException ex)
            {
            }
            catch (Exception ex)
            {
            }
            return document;

        }
        static string ExtractSheetId(string url)
        {
            var match = Regex.Match(url, @"/d/([a-zA-Z0-9-_]+)");
            return match.Success ? match.Groups[1].Value : string.Empty;
        }
    }
}