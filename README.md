# SQLTriggerFA

Example of SQL Trigger Function App and example showing scaling performance on Consumption App Service Plan

The SQL Trigger for the Function App for this Function App looks like this

```json
{
    "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "SqlConnectionString": "Server=tcp:myDatabaseServer.database.windows.net,1433;Initial Catalog=myDatabase;Persist Security Info=False;User ID=casecountuser;Password={password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  }
}
```

```csharp
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

```

These scenarios allow you to build event-driven systems using modern architectural patterns.

As you build your functions, you have the following options and resources available:

- **Use your preferred language**: Write functions in [C#, Java, JavaScript, PowerShell, or Python](./supported-languages.md), or use a [custom handler](./functions-custom-handlers.md) to use virtually any other language.

- **Automate deployment**: From a tools-based approach to using external pipelines, there's a [myriad of deployment options](./functions-deployment-technologies.md) available.

- **Troubleshoot a function**: Use [monitoring tools](./functions-monitoring.md) and [testing strategies](./functions-test-a-function.md) to gain insights into your apps.

- **Flexible pricing options**: With the [Consumption](./pricing.md) plan, you only pay while your functions are running, while the [Premium](./pricing.md) and [App Service](./pricing.md) plans offer features for specialized needs.
