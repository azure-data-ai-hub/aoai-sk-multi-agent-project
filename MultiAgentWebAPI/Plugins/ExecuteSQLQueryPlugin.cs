using Microsoft.Data.SqlClient;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace MultiAgentWebAPI.Plugins
{
    public class ExecuteSQLQueryPlugin
    {
        /// <summary>
        /// Executes the given SQL query, using a connection string from appsettings.json.
        /// </summary>
        [KernelFunction("execute_sql_query")]
        [Description("Execute SQL Query by connecting to the backend SQL Server database")]
        public async Task<List<Dictionary<string, object>>> ExecutePromptAsync(Kernel kernel, string sqlQuery)
        {
            // Retrieve the connection string from appsettings.json, e.g., "DefaultConnection"
            string connectionString = (string) kernel.Data["sqlConnectionString"]!;

            List<Dictionary<string, object>> results = new List<Dictionary<string, object>>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[reader.GetName(i)] = reader.GetValue(i);
                            }
                            results.Add(row);
                        }
                    }
                }
            }
            return results;
        }
    }
}
