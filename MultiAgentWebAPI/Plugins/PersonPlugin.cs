using System.ComponentModel;
using MultiAgentWebAPI.Entities;
using Microsoft.SemanticKernel;
using MultiAgentWebAPI.Utilities;

namespace MultiAgentWebAPI.Plugins
{
    public class PersonPlugin
    {
        [KernelFunction("get_person_details")]
        [Description("Gets Person details for the given BusinessEntityID.")]
        public IEnumerable<Person> GetPersonDetails(int BusinessEntityID)
        {
            try
            {
                Console.WriteLine($"Get person details from CSV for: {BusinessEntityID}.");

                var csvFilePath = "data/Person.csv";

                var persons = CSVHelper.ReadRecords<Person>(csvFilePath, p => p.BusinessEntityID == BusinessEntityID);

                if (persons != null && persons.Any())
                {
                    return persons;
                }
                else
                {
                    Console.WriteLine("No person data found in CSV.");
                    return Enumerable.Empty<Person>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An exception occurred while getting person details: {ex.Message}");
            }
        }
    }
}