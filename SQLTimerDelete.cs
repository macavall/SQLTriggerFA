using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace SQLTriggerFA
{
    public class SQLTimerDelete
    {
        [Disable("SQLTimerDelete")]
        [FunctionName("SQLTimerDelete")]
        public void Run([TimerTrigger("*/1 * * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            // create sql client

            using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SqlConnectionString")))
            {
                connection.Open();
                // Do work here; connection closed on following line.

                // delete row
                using (SqlCommand command = new SqlCommand("DELETE FROM dbo.ToDo WHERE completed = 0", connection))
                {
                    command.ExecuteNonQuery();
                }
            }

            // create sql connection
            // create sql command
            // execute sql command


        }
    }
}
