using System.Linq;
using Stylelabs.M.Base.Querying;
using Stylelabs.M.Base.Querying.Linq;

using Stylelabs.M.Sdk.Contracts.Base;
using Stylelabs.M.Sdk.WebClient;
using Stylelabs.M.Sdk.WebClient.Authentication;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Stylelabs.M.Framework.Essentials.LoadConfigurations;

namespace ContentHubConsole
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            Uri endpoint = new Uri("https://---.sitecorecontenthub.cloud");
       
            var oauth = new Stylelabs.M.Sdk.WebClient.Authentication.OAuthPasswordGrant()
            {
                ClientId = "----",
                ClientSecret = "---",
                UserName = "---",
                Password = "---"
            };

            IWebMClient client = MClientFactory.CreateMClient(endpoint, oauth);
            //client.TestConnectionAsync().Wait();

            // taxonomy id 194
            //var testResult = await client.Entities.GetAsync(194);

            var query = Query.CreateQuery(entities => from e in entities
                                                      where e.DefinitionName == "M.Asset" &&
                                                      e.Property("CoveoTitle") == string.Empty
                                                      && e.Parent("AssetTypeToAsset") == 34140 //34139 image //34138 doc//'M.AssetType.Documents'
                                                      select e);
            query.Take = 2500;

            var assets = await client.Querying.QueryAsync(query, EntityLoadConfiguration.Default);
            Console.WriteLine(assets.Items.Count);
            int i = 0;
            foreach (var asset in assets.Items)
            {
                var title = await asset.GetPropertyValueAsync("Title");
                var coVeotitle = await asset.GetPropertyValueAsync("CoveoTitle");
                Console.WriteLine(asset.Id + " " + title);
                if (coVeotitle == null)
                {
                    Console.WriteLine(" >>>>>>>>>>>>>      NULL");
                    asset.SetPropertyValue("CoveoTitle", title);
                    await client.Entities.SaveAsync(asset);
                    Console.WriteLine(" =============      Coveo Title updated");
                }
                else
                {
                    Console.WriteLine(" >>>>>>>>>>>>>      " + coVeotitle.ToString().Length + " " + coVeotitle);
                    if (string.IsNullOrEmpty(coVeotitle.ToString()))
                    {
                        asset.SetPropertyValue("CoveoTitle", title);
                        await client.Entities.SaveAsync(asset);
                        Console.WriteLine(" =============      Coveo Title updated");
                    }
                    else
                    {
                        Console.WriteLine(" <<<<<<<<<<<<<      Coveo Title exists");
                    }
                }
                Console.WriteLine(i++);
            }


            Console.ReadLine();
        }
    }
}
