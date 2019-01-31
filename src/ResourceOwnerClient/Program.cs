// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ResourceOwnerClient
{
    public class Program
    {
        public static void Main(string[] args) => MainAsync().GetAwaiter().GetResult();

        private static async Task MainAsync()
        {
            // discover endpoints from metadata
            var client = new HttpClient();

            var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }

            // using token client to request token
            var clientId = "ro.client";
            var clientSecret = "secret";
            var tokenClient = new TokenClient(disco.TokenEndpoint, clientId, clientSecret);

            //// get token by phone/authcode
            //var extra = new Dictionary<string, string> { { "AuthCode", "123456" } , { "PhoneNumber", "0901111111" } };
            //var tokenClientResponse = await tokenClient.RequestResourceOwnerPasswordAsync("xxx", "xxx", extra: extra);

            // get token by username/password
            var tokenClientResponse = await tokenClient.RequestResourceOwnerPasswordAsync("alice", "password");

            if (tokenClientResponse.IsError)
            {
                Console.WriteLine(tokenClientResponse.Error);
                return;
            }

            Console.WriteLine(tokenClientResponse.Json);
            Console.WriteLine("\n\n");

            // call api
            var apiClient = new HttpClient();
            apiClient.SetBearerToken(tokenClientResponse.AccessToken);

            var response = await apiClient.GetAsync("http://localhost:5001/identity");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine(JArray.Parse(content));
            }

            Console.ReadLine();
        }
    }
}