using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading;

namespace SQLTriggerFA
{
    public class httpScaleTrigger
    {
        private readonly HttpClient _client;
        private readonly IMyService _myService;
        private readonly int _semaphoreCount = Convert.ToInt32(Environment.GetEnvironmentVariable("semaphoreCount"));
        private readonly int _loopCount = Convert.ToInt32(Environment.GetEnvironmentVariable("loopCount"));
        private readonly int _sqlLoopCount = Convert.ToInt32(Environment.GetEnvironmentVariable("sqlLoopCount"));
        private readonly string _url = Environment.GetEnvironmentVariable("url");
        private readonly bool _sendSQL = Convert.ToBoolean(Environment.GetEnvironmentVariable("sendSQL"));

        public httpScaleTrigger(IHttpClientFactory httpClientFactory, IMyService myService)
        {
            this._client = httpClientFactory.CreateClient();
            this._myService = myService;
        }

        [Disable("httpScaleTrigger")]
        [FunctionName("httpScaleTrigger")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log,
            [Sql("dbo.ToDo", ConnectionStringSetting = "SqlConnectionString")] IAsyncCollector<ToDoItem> toDoItems)
        {
            await SendRequests();

            if (_sendSQL)
            {
                await SendSQLRequests(toDoItems);
            }

            var client = new HttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, new Uri("http://perfcount56wdfa.azurewebsites.net/"));

            #region default code
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
            #endregion
        }

        public async Task SendRequests()
        {
            var semaphore = new SemaphoreSlim(_semaphoreCount);

            var tasks = new List<Task>();
            for (int i = 0; i < _loopCount; i++)
            {
                await semaphore.WaitAsync();

                var request = new HttpRequestMessage(HttpMethod.Get, _url);

                tasks.Add(_client.SendAsync(request).ContinueWith((t) => semaphore.Release()));
            }
            await Task.WhenAll(tasks);
        }

        public async Task SendSQLRequests(IAsyncCollector<ToDoItem> toDoItems)
        {
            // List of ToDoItems to be inserted into the ToDo table
            List<ToDoItem> toDoItemsList = new List<ToDoItem>();
            
            // Populate the toDoItemsList
            for (int i = 0; i < _sqlLoopCount; i++)
            {
                // Add a new ToDoItem to the list
                toDoItemsList.Add(new ToDoItem
                {
                    Id = Guid.NewGuid(),
                    order = _myService.GetCount(),
                    title = "New ToDoItem",
                    url = "https://www.microsoft.com",
                    completed = false
                });

                _myService.AddOneToCount();
            }

            // Add new ToDo Class to the SQL Table
            foreach (ToDoItem toDoItem in toDoItemsList)
            {
                await toDoItems.AddAsync(toDoItem);
            }

            // Flushing the accumulated items in the toDoItems
            await toDoItems.FlushAsync();
        }
    }
}
