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
            //if (URL.Contains("https://docs.google.com/spreadsheets"))
            //{
            return await ReadGoogleSheets(URL);
            //}
            //else
            //{
            //    if (URL.Contains("https://docs.google.com/document"))
            //    {
            //    }
            //}
        }
        static async Task<UserCredential> GetGoogleCredentials(string[] scopes)
        {
            using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                return await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore("token.json", true));
            }
        }
        //static async Task ReadGoogleDocs()
        //{
        //    try
        //    {
        //        string[] Scopes = { DocsService.Scope.DocumentsReadonly };
        //        var credential = await GetGoogleCredentials(Scopes);

        //        var service = new DocsService(new BaseClientService.Initializer()
        //        {
        //            HttpClientInitializer = credential,
        //            ApplicationName = "Google Docs API C#",
        //        });

        //        Console.WriteLine("Enter the Google Docs document ID:");
        //        string documentId = Console.ReadLine();

        //        Document doc = service.Documents.Get(documentId).Execute();

        //        Console.WriteLine("\nDocument Title: " + doc.Title);
        //        Console.WriteLine("Document Content:");

        //        foreach (var element in doc.Body.Content)
        //        {
        //            // Read normal text
        //            if (element.Paragraph != null)
        //            {
        //                foreach (var text in element.Paragraph.Elements)
        //                {
        //                    if (text.TextRun != null)
        //                    {
        //                        Console.Write(text.TextRun.Content);
        //                    }
        //                }
        //                Console.WriteLine();
        //            }

        //            // Read tables
        //            if (element.Table != null)
        //            {
        //                Console.WriteLine("\n--- Table found ---");
        //                int rowIndex = 0;
        //                foreach (var row in element.Table.TableRows)
        //                {
        //                    Console.Write($"Row {rowIndex + 1}: ");
        //                    foreach (var cell in row.TableCells)
        //                    {
        //                        foreach (var cellElement in cell.Content)
        //                        {
        //                            if (cellElement.Paragraph != null)
        //                            {
        //                                foreach (var text in cellElement.Paragraph.Elements)
        //                                {
        //                                    if (text.TextRun != null)
        //                                    {
        //                                        Console.Write(text.TextRun.Content + " | ");
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                    Console.WriteLine();
        //                    rowIndex++;
        //                }
        //            }
        //        }
        //    }
        //    catch (Google.GoogleApiException ex)
        //    {
        //        Console.WriteLine("Google Docs API Error:");
        //        Console.WriteLine($"Message: {ex.Message}");
        //        Console.WriteLine($"Error Code: {ex.HttpStatusCode}");
        //        Console.WriteLine($"Details: {ex.Error}");
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("General error:");
        //        Console.WriteLine(ex.Message);
        //    }
        //}
        static async Task<List<DocumentModel>> ReadGoogleSheets(string URL)
        {
            string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
            var credential = await GetGoogleCredentials(Scopes);

            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Google Sheets API C#",
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