using Azure.Data.Tables;
using System;

class Program
{
    static void Main(string[] args)
    {
        var tableClient = new TableClient(
            "DefaultEndpointsProtocol=https;AccountName=yourstorage;AccountKey=yourkey;EndpointSuffix=core.windows.net",
            "UserTable");

        var entities = tableClient.Query<TableEntity>(filter: $"PartitionKey eq 'Users'");

        foreach (var entity in entities)
        {
            Console.WriteLine($"UserName: {entity["UserName"]}, Password: {entity["Password"]}");
        }
    }
}