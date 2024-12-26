using CsvHelper;
using System.Globalization;

namespace MultiAgentWebAPI.Utilities
{
    public static class CSVHelper
    {
        // Function to read all records from the CSV file
        public static IEnumerable<T> ReadRecords<T>(string csvFilePath, Func<T, bool> predicate)
        {
            using (var reader = new StreamReader(csvFilePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                return csv.GetRecords<T>().Where(predicate).Take(5).ToList();
            }
        }
    }
}
