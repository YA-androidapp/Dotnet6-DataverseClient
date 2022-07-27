// Copyright (c) 2022 YA-androidapp(https://github.com/YA-androidapp) All rights reserved.


using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;

namespace DataverseClient
{
    internal class Program
    {
        /// <summary>
        /// Contains the application's configuration settings. 
        /// </summary>
        IConfiguration Configuration { get; }

        /// <summary>
        /// Constructor. Loads the application configuration settings from a JSON file.
        /// </summary>
        Program()
        {
            // Load the app's configuration settings from the JSON file.
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }

        static void WhoAmI(ServiceClient serviceClient)
        {
            // Send a WhoAmI message request to the Organization service to obtain  
            // information about the logged on user.
            WhoAmIResponse resp = (WhoAmIResponse)serviceClient.Execute(new WhoAmIRequest());
            Console.WriteLine("User ID is {0}.", resp.UserId);
        }

        static void GetRecord(ServiceClient serviceClient)
        {
            // Create an in-memory account named Nightmare Coffee.
            Entity account = new("account");
            account["name"] = "Nightmare Coffee";

            // Now create that account in Dataverse. Note that the Dataverse
            // created account ID is being stored in the in-memory account
            // for later use with the Update() call.
            account.Id = serviceClient.Create(account);

            // In Dataverse, update the account's name and set it's postal code.
            account["name"] = "Fourth Coffee";
            account["address2_postalcode"] = "98052";
            serviceClient.Update(account);

            // Retrieve the updated account from Dataverse.
            Entity retrievedAccount = serviceClient.Retrieve(
                entityName: account.LogicalName,
                id: account.Id,
                columnSet: new ColumnSet("name", "address2_postalcode")
            );

            Console.WriteLine("Retrieved account name: {0}, postal code: {1}",
                retrievedAccount["name"], retrievedAccount["address2_postalcode"]);

            GetRecords(serviceClient);

            // Pause program execution before resource cleanup.
            Console.WriteLine("Press any key to undo environment data changes.");
            Console.ReadKey();

            // In Dataverse, delete the created account, and then dispose the connection.
            serviceClient.Delete(account.LogicalName, account.Id);
        }

        static void GetRecords(ServiceClient serviceClient)
        {
            var accountsCollection = serviceClient.RetrieveMultiple(new QueryExpression("account")
            {
                ColumnSet = new ColumnSet("name"),
                TopCount = 10
            });

            Console.WriteLine("Records:");
            Console.WriteLine(string.Join("\n",
                accountsCollection.Entities
                    .Select(x => $"{x.GetAttributeValue<string>("name")}, {x.Id}")
                    ));
        }

        static void Main(string[] args)
        {
            Program app = new();

            // Create a Dataverse service client using the default connection string.
            var connectionString = app.Configuration.GetConnectionString("MyEnv");
            ServiceClient serviceClient = new(connectionString);

            WhoAmI(serviceClient);
            GetRecord(serviceClient);

            serviceClient.Dispose();
        }
    }
}
