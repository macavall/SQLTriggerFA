using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.Sql;
using System.Collections.Generic;

namespace SQLTriggerFA
{
    public static class SQLTrigger
    {
        [Disable]
        [FunctionName("ToDoTrigger")]
        public static async Task Run(
            [SqlTrigger("[dbo].[ToDo]", ConnectionStringSetting = "SqlConnectionString")]
            IReadOnlyList<SqlChange<ToDoItem>> changes,
            ILogger logger,
            [Sql("dbo.ToDo", ConnectionStringSetting = "SqlConnectionString")] IAsyncCollector<ToDoItem> toDoItems)
        {
            int loopCount = Convert.ToInt32(Environment.GetEnvironmentVariable("loopCount"));

            // Loop through the SQL Change Records from the SQL Trigger
            foreach (SqlChange<ToDoItem> change in changes)
            {
                ToDoItem toDoItem = change.Item;
                logger.LogInformation($"Change operation: {change.Operation}");
                logger.LogInformation($"Id: {toDoItem.Id}, Title: {toDoItem.title}, Url: {toDoItem.url}, Completed: {toDoItem.completed}");
            }

            // List of ToDoItems to be inserted into the ToDo table
            List<ToDoItem> toDoItemsList = new List<ToDoItem>();

            // Populate the toDoItemsList
            for (int i = 0; i < loopCount; i++)
            {
                // Add a new ToDoItem to the list
                toDoItemsList.Add(new ToDoItem
                {
                    Id = Guid.NewGuid(),
                    title = "New ToDoItem",
                    url = "https://www.microsoft.com",
                    completed = false
                });
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
