using Google.Apis.Auth.OAuth2;
using Google.Apis.Docs.v1;
using Google.Apis.Docs.v1.Data;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using PizzaEnergyCoders.Models;
using Sitecore.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PizzaEnergyCoders.Services
{
    public static class ReadFileGoogle
    {
        /// <summary>
        /// Method to call child methods depending on the type of the string
        /// </summary>
        /// <param name="URL">Shared Google Doc or Google Spreadsheet</param>
        /// <returns>Return a List of DocumentModel</returns>
        public static async Task<List<DocumentModel>> Import(string URL)
        {
            //Sets file id from URL
            string fileId = ExtractFileId(URL);

            if (string.IsNullOrEmpty(fileId))
            {
                throw new Exception("Invalid Google Drive URL.");
            }

            string fileType = await GetGoogleDriveFileType(fileId);

            if (fileType == "application/vnd.google-apps.document")
            {
                return await ReadGoogleDocs(URL);
            }
            else if (fileType == "application/vnd.google-apps.spreadsheet")
            {
                return await ReadGoogleSheets(URL);
            }
            else
            {
                throw new Exception("Unsupported file type.");
            }
        }
        /// <summary>
        /// Gets the type of the google doc from the URL
        /// </summary>
        /// <param name="fileId">Field id from the URL</param>
        /// <returns>Returns google type</returns>
        static async Task<string> GetGoogleDriveFileType(string fileId)
        {
            string[] scopes = { DriveService.Scope.DriveMetadataReadonly };
            string credentialsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\Creds", "credentials.json");

            var credential = GoogleCredential.FromFile(credentialsPath).CreateScoped(scopes);
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "SCHackaton2025",
            });

            try
            {
                var request = service.Files.Get(fileId);
                request.Fields = "mimeType";
                var file = await request.ExecuteAsync();
                return file.MimeType;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving file type: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Method to extract the doc Id from the URL
        /// </summary>
        /// <param name="url">Shared Google Doc or Google Spreadsheet</param>
        /// <returns></returns>
        static string ExtractFileId(string url)
        {
            var match = Regex.Match(url, @"/d/([a-zA-Z0-9-_]+)");
            return match.Success ? match.Groups[1].Value : string.Empty;
        }
        /// <summary>
        /// Method to read Google Doc
        /// </summary>
        /// <param name="URL">Shared Google Doc or Google Spreadsheet</param>
        /// <returns>Return a List of DocumentModel</returns>
        static async Task<List<DocumentModel>> ReadGoogleDocs(string URL)
        {
            string[] scopes = { DocsService.Scope.DocumentsReadonly };
            string credentialsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\Creds", "credentials.json");

            var credential = GoogleCredential.FromFile(credentialsPath).CreateScoped(scopes);
            var service = new DocsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "SCHackaton2025",
            });

            string documentId = ExtractFileId(URL);
            var document = new List<DocumentModel>();

            try
            {
                Google.Apis.Docs.v1.Data.Document doc = service.Documents.Get(documentId).Execute();

                var docModel = new DocumentModel()
                {
                    TemplateName = doc.Title,
                    KeyValues = new Dictionary<string, string>()
                };

                bool firstRowProcessed = false;

                foreach (var element in doc.Body.Content)
                {
                    if (element.Table != null)
                    {
                        foreach (var row in element.Table.TableRows)
                        {
                            if (row.TableCells.Count >= 2)
                            {

                                string header = ExtractTextFromCell(row.TableCells[0]).Replace(":", "").Trim();


                                string content = ExtractTextFromCell(row.TableCells[1]).Trim();

                                if (!string.IsNullOrEmpty(header) && !string.IsNullOrEmpty(content))
                                {
                                    if (!firstRowProcessed)
                                    {
                                        docModel.Title = ExtractFirstValueBeforeComma(content);
                                        firstRowProcessed = true;
                                    }

                                    docModel.KeyValues[header] = content;
                                }
                            }
                        }
                    }
                }

                document.Add(docModel);
            }
            catch (Google.GoogleApiException ex)
            {
                Console.WriteLine($"Google Docs API Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error: {ex.Message}");
            }

            return document;
        }
        /// <summary>
        /// Method to read Google Spreadsheet
        /// </summary>
        /// <param name="URL">Shared Google Doc or Google Spreadsheet</param>
        /// <returns>Return a List of DocumentModel</returns>
        static async Task<List<DocumentModel>> ReadGoogleSheets(string URL)
        {
            string[] scopes = { SheetsService.Scope.SpreadsheetsReadonly };
            string credentialsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\Creds", "credentials.json");

            var credential = GoogleCredential.FromFile(credentialsPath).CreateScoped(scopes);
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "SCHackaton2025",
            });

            string spreadsheetId = ExtractFileId(URL);
            string range = Settings.GetSetting("SpreadsheetRange");
            var document = new List<DocumentModel>();

            try
            {
                SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(spreadsheetId, range);
                ValueRange response = request.Execute();
                IList<IList<object>> values = response.Values;

                if (values != null && values.Count > 0)
                {
                    var firstRow = values.First()
                        .Where(cell => cell != null && !string.IsNullOrWhiteSpace(cell.ToString()))
                        .Select(cell => cell.ToString())
                        .ToList();

                    foreach (var row in values.Skip(1))
                    {
                        if (row.Any(cell => cell == null || string.IsNullOrWhiteSpace(cell.ToString())))
                            break;
                        List<string> rowData = row.Select(cell => cell.ToString()).ToList();

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
                Console.WriteLine($"Google Sheets API Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error: {ex.Message}");
            }

            return document;
        }
        /// <summary>
        /// Method to Extract Text from a Cell
        /// </summary>
        /// <param name="cell">Table cell</param>
        /// <returns>Cell text</returns>
        static string ExtractTextFromCell(TableCell cell)
        {
            StringBuilder cellText = new StringBuilder();

            foreach (var cellElement in cell.Content)
            {
                if (cellElement.Paragraph != null)
                {
                    foreach (var text in cellElement.Paragraph.Elements)
                    {
                        if (text.TextRun != null)
                        {
                            cellText.Append(text.TextRun.Content);
                        }
                    }
                }
            }

            return cellText.ToString();
        }

        /// <summary>
        /// Method to Extract First Value before Comma
        /// </summary>
        /// <param name="content">Content to be analized</param>
        /// <returns>Returns a trimed text</returns>
        static string ExtractFirstValueBeforeComma(string content)
        {
            int commaIndex = content.IndexOf(",");
            return commaIndex != -1 ? content.Substring(0, commaIndex).Trim() : content.Trim();
        }
    }
}
