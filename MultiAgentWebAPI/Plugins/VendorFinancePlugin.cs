﻿using System.ComponentModel; 
using MultiAgentWebAPI.Entities;
using Microsoft.SemanticKernel;
using MultiAgentWebAPI.Utilities;

namespace MultiAgentWebAPI.Plugins
{
    public class VendorFinancePlugin()
    {
        [KernelFunction("get_vendor_finance_details")]
        [Description("Gets vendor financials details for the Project given ProjectID.")]
        public IEnumerable<VendorFinancials> GetVendorFinanceDetails(string ProjectID)
        {
             try
            {
                Console.WriteLine($"Get vendor finance details details from CSV for: {ProjectID}.");

                var csvFilePath = "data/VendorFinancials.csv";

                var financials = CSVHelper.ReadRecords<VendorFinancials>(csvFilePath, s => s.ProjectID.Equals(ProjectID, StringComparison.OrdinalIgnoreCase));

                if (financials != null)
                {
                    return financials;
                }
                else
                {
                    Console.WriteLine("No schedule found in CSV.");
                    return Enumerable.Empty<VendorFinancials>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An exception occurred while getting schedule details: {ex.Message}");
            }
        }
    }
}
