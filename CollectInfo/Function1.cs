using CollectInfo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Messaging.ServiceBus;
using Azure.Data.Tables;

namespace CollectInfo
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;

        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;
        }

        [Function("Function1")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            _logger.LogInformation("Processing user info...");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var user = JsonConvert.DeserializeObject<User>(requestBody);

           
            var serviceBusClient = new ServiceBusClient(
                Environment.GetEnvironmentVariable("ServiceBusConnection"));

            var sender = serviceBusClient.CreateSender("userqueue");

            var message = new ServiceBusMessage(JsonConvert.SerializeObject(user));

            await sender.SendMessageAsync(message);

            var tableClient = new TableClient(
                Environment.GetEnvironmentVariable("AzureWebJobsStorage"),
                "UserTable");

            await tableClient.CreateIfNotExistsAsync();

            var entity = new TableEntity("Users", Guid.NewGuid().ToString())
            {
                { "UserName", user.UserName },
                { "Password", user.Password }
            };

            await tableClient.AddEntityAsync(entity);

            _logger.LogInformation($"User {user.UserName} stored and queued dddddd.");
            Console.WriteLine("Hi");

            return new OkObjectResult($"User {user.UserName} stored successfully.");
        }
    }
}