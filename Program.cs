using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Docs.v1;
using Google.Apis.Docs.v1.Data;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Select the data source:");
        Console.WriteLine("1. Google Docs");
        Console.WriteLine("2. Google Sheets");
        string option = Console.ReadLine();

        if (option == "1")
        {
            await ReadGoogleDocs();
        }
        else if (option == "2")
        {
            await ReadGoogleSheets();
        }
        else
        {
            Console.WriteLine("Invalid option.");
        }
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

    static async Task ReadGoogleDocs()
    {
        try
        {
            string[] Scopes = { DocsService.Scope.DocumentsReadonly };
            var credential = await GetGoogleCredentials(Scopes);

            var service = new DocsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Google Docs API C#",
            });

            Console.WriteLine("Enter the Google Docs document ID:");
            string documentId = Console.ReadLine();

            Document doc = service.Documents.Get(documentId).Execute();

            Console.WriteLine("\nDocument Title: " + doc.Title);
            Console.WriteLine("Document Content:");

            foreach (var element in doc.Body.Content)
            {
                // Read normal text
                if (element.Paragraph != null)
                {
                    foreach (var text in element.Paragraph.Elements)
                    {
                        if (text.TextRun != null)
                        {
                            Console.Write(text.TextRun.Content);
                        }
                    }
                    Console.WriteLine();
                }

                // Read tables
                if (element.Table != null)
                {
                    Console.WriteLine("\n--- Table found ---");
                    int rowIndex = 0;
                    foreach (var row in element.Table.TableRows)
                    {
                        Console.Write($"Row {rowIndex + 1}: ");
                        foreach (var cell in row.TableCells)
                        {
                            foreach (var cellElement in cell.Content)
                            {
                                if (cellElement.Paragraph != null)
                                {
                                    foreach (var text in cellElement.Paragraph.Elements)
                                    {
                                        if (text.TextRun != null)
                                        {
                                            Console.Write(text.TextRun.Content + " | ");
                                        }
                                    }
                                }
                            }
                        }
                        Console.WriteLine();
                        rowIndex++;
                    }
                }
            }
        }
        catch (Google.GoogleApiException ex)
        {
            Console.WriteLine("Google Docs API Error:");
            Console.WriteLine($"Message: {ex.Message}");
            Console.WriteLine($"Error Code: {ex.HttpStatusCode}");
            Console.WriteLine($"Details: {ex.Error}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("General error:");
            Console.WriteLine(ex.Message);
        }
    }

    static async Task ReadGoogleSheets()
    {
        string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        var credential = await GetGoogleCredentials(Scopes);

        var service = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "Google Sheets API C#",
        });

        Console.WriteLine("Enter the Google Sheets spreadsheet ID:");
        string spreadsheetId = Console.ReadLine();

        Console.WriteLine("Enter the cell range (Example: Sheet1!A1:C10):");
        string range = Console.ReadLine();

        try
        {
            SpreadsheetsResource.ValuesResource.GetRequest request =
                service.Spreadsheets.Values.Get(spreadsheetId, range);

            ValueRange response = request.Execute();
            var values = response.Values;

            if (values != null && values.Count > 0)
            {
                Console.WriteLine("\nGoogle Sheets Data:");
                foreach (var row in values)
                {
                    Console.WriteLine(string.Join(" | ", row));
                }
            }
            else
            {
                Console.WriteLine("No data found in the sheet.");
            }
        }
        catch (Google.GoogleApiException ex)
        {
            Console.WriteLine($"Google API Error: {ex.Message}");
            Console.WriteLine($"Error Code: {ex.HttpStatusCode}");
            Console.WriteLine($"Details: {ex.Error.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"General error: {ex.Message}");
        }
    }
}
