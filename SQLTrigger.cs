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
        [FunctionName("ToDoTrigger")]
        public static async Task Run(
            [SqlTrigger("[dbo].[ToDo]", ConnectionStringSetting = "SqlConnectionString")]
            IReadOnlyList<SqlChange<ToDoItem>> changes,
            ILogger logger,
            [Sql("dbo.ToDo", ConnectionStringSetting = "SqlConnectionString")] IAsyncCollector<ToDoItem> toDoItems)
        {
            foreach (SqlChange<ToDoItem> change in changes)
            {
                ToDoItem toDoItem = change.Item;
                logger.LogInformation($"Change operation: {change.Operation}");
                logger.LogInformation($"Id: {toDoItem.Id}, Title: {toDoItem.title}, Url: {toDoItem.url}, Completed: {toDoItem.completed}");
            }

            // List of ToDoItems to be inserted into the ToDo table
            List<ToDoItem> toDoItemsList = new List<ToDoItem>();

            for (int i = 0; i < 10; i++)
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

            await toDoItems.FlushAsync();
        }
    }
}
