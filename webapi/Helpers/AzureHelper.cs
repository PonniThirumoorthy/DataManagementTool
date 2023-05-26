using Azure;
using Azure.Storage.Files.Shares;
using Microsoft.Data.SqlClient;
using System.Net.NetworkInformation;
using Microsoft.Extensions.Configuration;
using webapi.Models;
using System.Data;
using System.Reflection;
using System.Diagnostics;

namespace webapi
{
    public class AzureHelper
    {
        private readonly IConfiguration _configuration;
        public AzureHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public bool SQLBulkInsertion(List<Details> details)
        {
            DataTable dt = new DataTable();
            try
            {
               
                dt = ToDataTable(details);
                string connstr = _configuration.GetConnectionString("Azure-DB");
                using (SqlConnection connection = new SqlConnection(connstr))
                {
                    connection.Open();

                    Stopwatch sq = Stopwatch.StartNew();
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                    {
                        bulkCopy.BatchSize = 500; // Adjust batch size as needed
                        bulkCopy.DestinationTableName = "Details";

                        // Set up the column mappings by name.
                        SqlBulkCopyColumnMapping mapseqno =
                            new SqlBulkCopyColumnMapping("seqno", "seqno");
                        bulkCopy.ColumnMappings.Add(mapseqno);

                        SqlBulkCopyColumnMapping mapFName =
                            new SqlBulkCopyColumnMapping("FirstName", "FirstName");
                        bulkCopy.ColumnMappings.Add(mapFName);

                        SqlBulkCopyColumnMapping mapLastName =
                            new SqlBulkCopyColumnMapping("LastName", "LastName");
                        bulkCopy.ColumnMappings.Add(mapLastName);
                        
                        SqlBulkCopyColumnMapping mapPhone =
                            new SqlBulkCopyColumnMapping("Phone", "Phone");
                        bulkCopy.ColumnMappings.Add(mapPhone);

                        SqlBulkCopyColumnMapping mapEmail =
                            new SqlBulkCopyColumnMapping("Email", "Email");
                        bulkCopy.ColumnMappings.Add(mapEmail);



                        bulkCopy.WriteToServer(dt);
                        sq.Stop();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool SqlBulkInsertold(List<Details> details)
        {
            try
            {
                if (details == null)
                {
                    return false;
                }
                else
                {

                    var copy = new SqlBulkCopy(_configuration.GetConnectionString("Azure-DB"));

                    copy.DestinationTableName = "dbo.Details";
                    copy.ColumnMappings.Add(nameof(Details.seqno), "seqno");
                    copy.ColumnMappings.Add(nameof(Details.FirstName), "FirstName");
                    copy.ColumnMappings.Add(nameof(Details.LastName), "LastName");
                    copy.ColumnMappings.Add(nameof(Details.Email), "Email");
                    copy.ColumnMappings.Add(nameof(Details.Phone), "Phone");


                    copy.WriteToServer(ToDataTable(details));
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                if (prop.Name.ToLower() != "id")
                {
                    //Defining type of data column gives proper data table 
                    var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                    //Setting column names as Property names
                    dataTable.Columns.Add(prop.Name, type);
                }
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length-1];

                int i = 0;
                foreach (var value in Props)
                {
                    if(value.Name != "Id")
                    {
                        values[i++] = value.GetValue(item,null);
                    }
                }

                //for (int i = 1; i < Props.Length; i++)
                //{
                //    //inserting property values to datatable rows
                //    values[i] = Props[i].GetValue(item, null);
                //}
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
        public static async Task UploadAsync(string connectionString, string localFilePath)
        {
            // Get a connection string to our Azure Storage account.  You can
            // obtain your connection string from the Azure Portal (click
            // Access Keys under Settings in the Portal Storage account blade)
            // or using the Azure CLI with:
            //
            //     az storage account show-connection-string --name <account_name> --resource-group <resource_group>
            //
            // And you can provide the connection string to your application
            // using an environment variable.

            // Name of the directory and file we'll create
            string dirName = "csv-files";
            string fileName = "sample-file.csv";

            // Get a reference to a share and then create it
            ShareClient share = new ShareClient(connectionString, "azurefileshare1");
            await share.CreateAsync();

            // Get a reference to a directory and create it
            ShareDirectoryClient directory = share.GetDirectoryClient(dirName);
            await directory.CreateAsync();

            // Get a reference to a file and upload it
            ShareFileClient file = directory.GetFileClient(fileName);
            using (FileStream stream = File.OpenRead(localFilePath))
            {
                await file.CreateAsync(stream.Length);
                await file.UploadRangeAsync(
                    new HttpRange(0, stream.Length),
                    stream);
            }
        }

        //Connect to Azure DB

    }
}
