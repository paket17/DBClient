using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using System.Reflection.Metadata;
using System.Net;
using System.Xml.Linq;

namespace DBClient
{
    static class DBOperations
    {
        static DBOperations()
        {
            var builberConnection = new ConfigurationBuilder();
            builberConnection.SetBasePath(Directory.GetCurrentDirectory());
            builberConnection.AddJsonFile("appsettings.json");
            var config = builberConnection.Build();
            Host = config.GetConnectionString("Host");
            Port = config.GetConnectionString("Port");
            Uri = $"https://{Host}:{Port}/api/documents";
            //Bypass the certificate
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            HttpClient = new HttpClient(clientHandler);
        }

        static string Host { get; }
        static string Port { get; }
        public static string Uri { get; }


        static HttpClient HttpClient { get; }

        public static async Task TestConnection()
        {
            var response = await HttpClient.GetAsync(Uri);
            if (response.IsSuccessStatusCode)
                Console.WriteLine($"Connected to {Host}:{Port}");
        }
        public static async Task Print()
        {
            List<Document>? documents = await HttpClient.GetFromJsonAsync<List<Document>>(Uri);
            Console.WriteLine("List of documents:");
            if (documents != null)
            {
                Console.WriteLine(String.Format("| {0,-4} | {1,-6} | {2,-20} | {3,-19} | {4,-19} |",
                        "Id",
                        "Number",
                        "Name",
                        "DateAdd",
                        "DateUpdate"));
                Console.WriteLine(new string('-', 84));
                foreach (var document in documents)
                {
                    Console.WriteLine(String.Format("| {0,-4} | {1,-6} | {2,-20} | {3,-19} | {4,-19} |",
                        document.Id,
                        document.Number,
                        document.Name,
                        document.DateAdd,
                        document.DateUpdate));
                }
            }
        }
        public static async Task PrintId(int id)
        {
            var document = Get(id).Result;
            
            if (document != null)
            {
                Console.WriteLine("Printing a document:");
                Console.WriteLine(String.Format("| {0,-4} | {1,-6} | {2,-20} | {3,-19} | {4,-19} |",
                        "Id",
                        "Number",
                        "Name",
                        "DateAdd",
                        "DateUpdate"));
                Console.WriteLine(new string('-', 84));
                Console.WriteLine(String.Format("| {0,-4} | {1,-6} | {2,-20} | {3,-19} | {4,-19} |",
                        document.Id,
                        document.Number,
                        document.Name,
                        document.DateAdd,
                        document.DateUpdate));
            }
        }

        public static async Task<Document?> Get(int id)
        {
            using var response = await HttpClient.GetAsync($"{Uri}/{id}");
            if (response.StatusCode == HttpStatusCode.NotFound)
                Console.WriteLine("Document not found");
            else if (response.StatusCode != System.Net.HttpStatusCode.OK)
                Console.WriteLine("An error has occurred " + response.StatusCode);
            else
            {
                return await response.Content.ReadFromJsonAsync<Document>();
            }
            return null;
        }

        public static async Task Add(Document document)
        {
            using var response = await HttpClient.PostAsJsonAsync(Uri, document);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                Console.WriteLine("An error has occurred " + response.StatusCode);
            else
                Console.WriteLine($"\nThe object \"{document.Name}\" is added to the database.\n");
        }

        public static async Task Edit(int id, Document document)
        {
            var oldDocument = Get(id);
            document.Id = id;
            if (oldDocument.Result != null)
            {
                document.DateAdd = oldDocument.Result.DateAdd;
                using var response = await HttpClient.PutAsJsonAsync(Uri, document);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    Console.WriteLine("An error has occurred " + response.StatusCode);
                else
                    Console.WriteLine($"\nThe object \"{document.Name}\" has been changed in the database.\n");
            }
            
        }

        public static async Task Delete(int id)
        {
            var document = Get(id);
            if (document != null)
            {
                using var response = await HttpClient.DeleteAsync($"{Uri}/{id}");
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    Console.WriteLine("An error has occurred " + response.StatusCode);
                else
                    Console.WriteLine($"\nThe object \"{document.Result.Name}\" was deleted from the database.\n");
            }

        }
    }
}
