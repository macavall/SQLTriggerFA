# SQLTriggerFA

## Example of SQL Trigger Function App and example showing scaling performance on Consumption App Service Plan

The SQL Trigger for the Function App for this Function App looks like this

The following are a common, _but by no means exhaustive_, set of scenarios for Azure Functions.

| If you want to... | then... |
| --- | --- |
| **Build a web API** | Implement an endpoint for your web applications using the [HTTP trigger](./functions-bindings-http-webhook.md) |
| **Process file uploads** | Run code when a file is uploaded or changed in [blob storage](./functions-bindings-storage-blob.md) |
| **Build a serverless workflow** | Create an event-driven workflow from a series of functions using [durable functions](./durable/durable-functions-overview.md) |
| **Respond to database changes** | Run custom logic when a document is created or updated in [Azure Cosmos DB](./functions-bindings-cosmosdb-v2.md) |
| **Run scheduled tasks** | Execute code on [pre-defined timed intervals](./functions-bindings-timer.md) |
| **Create reliable message queue systems** | Process message queues using [Queue Storage](./functions-bindings-storage-queue.md), [Service Bus](./functions-bindings-service-bus.md), or [Event Hubs](./functions-bindings-event-hubs.md) |
| **Analyze IoT data streams** | Collect and process [data from IoT devices](./functions-bindings-event-iot.md) |
| **Process data in real time** | Use [Functions and SignalR](./functions-bindings-signalr-service.md) to respond to data in the moment |

These scenarios allow you to build event-driven systems using modern architectural patterns.

As you build your functions, you have the following options and resources available:

- **Use your preferred language**: Write functions in [C#, Java, JavaScript, PowerShell, or Python](./supported-languages.md), or use a [custom handler](./functions-custom-handlers.md) to use virtually any other language.

- **Automate deployment**: From a tools-based approach to using external pipelines, there's a [myriad of deployment options](./functions-deployment-technologies.md) available.

- **Troubleshoot a function**: Use [monitoring tools](./functions-monitoring.md) and [testing strategies](./functions-test-a-function.md) to gain insights into your apps.

- **Flexible pricing options**: With the [Consumption](./pricing.md) plan, you only pay while your functions are running, while the [Premium](./pricing.md) and [App Service](./pricing.md) plans offer features for specialized needs.
