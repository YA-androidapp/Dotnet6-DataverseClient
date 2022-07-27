// Copyright (c) 2022 YA-androidapp(https://github.com/YA-androidapp) All rights reserved.


using System;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using Microsoft.Extensions.Configuration;

namespace DataverseClient
{
    internal class Program
    {
        private static string connectionString = "";

        static void Init()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            connectionString = configuration["ConnectionStrings:MyEnv"];
        }

        static void WhoAmI()
        {
            try
            {
                using (HttpClient client = SampleHelpers.GetHttpClient(connectionString, SampleHelpers.clientId, SampleHelpers.redirectUrl))
                {
                    // Use the WhoAmI function
                    var response = client.GetAsync("WhoAmI").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        //Get the response content and parse it.
                        JObject body = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                        Guid userId = (Guid)body["UserId"];
                        Console.WriteLine("Your UserId is {0}", userId);
                    }
                    else
                    {
                        Console.WriteLine("The request failed with a status of '{0}'",
                                    response.ReasonPhrase);
                    }
                    Console.WriteLine("Press any key to exit.");
                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                SampleHelpers.DisplayException(ex);
                Console.WriteLine("Press any key to exit.");
                Console.ReadLine();
            }
        }

        static void GetRecord()
        {
            try
            {
                using (HttpClient client = SampleHelpers.GetHttpClient(connectionString, SampleHelpers.clientId, SampleHelpers.redirectUrl))
                {
                    // Use the WhoAmI function
                    var response = client.GetAsync("systemusers(cffe9069-b13d-eb11-bf68-000d3a51d5f0)").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        //Get the response content and parse it.
                        JObject body = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                        var fullName = body["fullname"];
                        Console.WriteLine("Your FullName is {0}", fullName);
                    }
                    else
                    {
                        Console.WriteLine("The request failed with a status of '{0}'",
                                    response.ReasonPhrase);
                    }
                    Console.WriteLine("Press any key to exit.");
                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                SampleHelpers.DisplayException(ex);
                Console.WriteLine("Press any key to exit.");
                Console.ReadLine();
            }
        }

        static void Main(string[] args)
        {
            Init();

            // WhoAmI();
            GetRecord();
        }
    }
}
