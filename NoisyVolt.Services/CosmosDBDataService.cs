using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.Azure.Cosmos;
using NoisyVolt.Models;

namespace NoisyVolt.Services
{
    //
    public class CosmosDBDataService : INoSqlDataService
    {
        //https://noisyvolt.documents.azure.com:443/

        private string _connectionString = @"AccountEndpoint=https://noisyvolt.documents.azure.com:443/;AccountKey=1x9zl0yIK45VsBnyG4kiyDOaQHy2huAe28uA5iJkSAAl9yLKV2PCeH1yZ25qHUyjH9Ag5SkT8Qia2fkDl8gLBA==";
        private CosmosClient _client;
        private Database _database;
        private Container _embedCardContainer;

        public CosmosDBDataService()
        {

            var options = new CosmosClientOptions() { ConnectionMode = ConnectionMode.Gateway };
            _client = new CosmosClient(_connectionString, options);
            _database = _client.GetDatabase("NoisyVolt");
            _embedCardContainer = _database.GetContainer("EmbedCards");
        }
        public async Task<string> SaveEmbedCard(EmbedCard embedCard)
        {
            string rmsg = "";
            try
            {
                ItemResponse<EmbedCard> resp = await _embedCardContainer.CreateItemAsync<EmbedCard>(embedCard);
                //Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", loc_response.Resource, loc_response.RequestCharge);
                rmsg = resp.StatusCode.ToString();
            }
            catch (CosmosException cosmosException)
            {
                // Log the full exception including the stack trace with: cosmosException.ToString()
                var t = cosmosException.Message;
                // The Diagnostics can be logged separately if required with: cosmosException.Diagnostics.ToString()
            }
            catch (Exception ex)
            {
                rmsg = ex.Message;
            }

            return rmsg;

        }

        public async Task<List<EmbedCard>> GetEmbeds()
        {
            var sqlQueryText = "SELECT * FROM EmbedCards";

            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<EmbedCard> queryResultSetIterator = _embedCardContainer.GetItemQueryIterator<EmbedCard>(queryDefinition);

            List<EmbedCard> mylist = new List<EmbedCard>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<EmbedCard> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (EmbedCard x in currentResultSet)
                {
                    mylist.Add(x);
                    // Console.WriteLine("\tRead {0}\n", x);
                }
            }

            return mylist;
        }
    }
}
