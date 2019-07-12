
/**
 *    Copyright 2019 Amazon.com, Inc. or its affiliates
 *
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

/*
 * The following program.cs file shows sample usage of a HelixHelper.cs class.
 */
namespace Twitch_Helix_Wrapper
{
    class Program
    {
        static void Main(string[] args)
        {
            //Sample Usage
            HelixHelper helper = new HelixHelper("YOUR_CLIENT_ID", "YOUR_TWITCH_USER_ID");

            //Sample list of Twitch ids
            List<string> ids = new List<string>();
            ids.Add("156900877");   //baxter4343
            ids.Add("55706186");    //v0oid
            ids.Add("67650991");    //realkraftyy

            //Sample list of Twitch login names
            List<string> some_login_names = new List<string>();
            some_login_names.Add("ninja");
            some_login_names.Add("drlupo");
            some_login_names.Add("nickmercs");

            //Get some users
            HelixHelper.Twitch_Users users_ById = helper.GetUsers(ids);
            HelixHelper.Twitch_Users users_ByName = helper.GetUsers(null, some_login_names);
            HelixHelper.Twitch_Users users_Both = helper.GetUsers(ids, some_login_names);

            //Log to console
            Console.WriteLine("Got some Twitch data!");
            Console.WriteLine("Users by Twitch ID: ");
            for(int i = 0; i < users_ById.data.Count; i++)
            {
                Console.WriteLine(users_ById.data[i].display_name);
            }
            Console.WriteLine("Users by Twitch Username: ");
            for (int i = 0; i < users_ByName.data.Count; i++)
            {
                Console.WriteLine(users_ByName.data[i].display_name);
            }
            Console.WriteLine("Users for both: ");
            for (int i = 0; i < users_Both.data.Count; i++)
            {
                Console.WriteLine(users_Both.data[i].display_name);
            }
        }
    }

    
}
