using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Azure.Documents;

namespace AzureDocumentDbPaging.Controllers
{
    [Route("api/items")]
    public class ItemsController : Controller
    {
        private static readonly Uri Endpoint = new Uri("https://localhost:8081/");
        private static readonly string Key = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        private static readonly string DatabaseId = "ToDoList";
        private static readonly string CollectionId = "Items";
        private static DocumentClient client;

        public ItemsController()
        {
            client = new DocumentClient(Endpoint, Key);
        }

        [HttpGet]
        public async Task<dynamic> Get([FromQuery] string q, [FromQuery] string token = null)
        {
            try
            {
                string rc = null;
                if (!string.IsNullOrEmpty(token))
                {
                    rc = JsonConvert.SerializeObject(new RequestContinuation { Token = token, Range = new RequestContinuationRange { Min = string.Empty, Max = "FF" } });
                }

                var collection = UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId);
                IDocumentQuery<Item> query = client.CreateDocumentQuery<Item>(collection, new FeedOptions { MaxItemCount = 5, RequestContinuation = rc }).AsDocumentQuery();

                var result = await query.ExecuteNextAsync<Item>();
                var itemResult = new ItemResult();
                itemResult.Result = result;
                itemResult.HasMoreResults = query.HasMoreResults;
                itemResult.MaxItemCount = 5;

                if (query.HasMoreResults)
                {
                    itemResult.RequestContinuation = result.ResponseContinuation;
                }

                return itemResult;
            }
            catch (DocumentClientException ex)
            {
                return ex;
            }
   
        }
    }

    public class ItemResult
    {
        public dynamic Result { get; set; }
        public string RequestContinuation { get; set; }
        public int? MaxItemCount { get; set; }
        public bool HasMoreResults { get; set; }
    }

    public class Item
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "isComplete")]
        public bool Completed { get; set; }
    }

    public class RequestContinuation
    {
        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }
        [JsonProperty(PropertyName = "range")]
        public RequestContinuationRange Range { get; set; }
    }
    public class RequestContinuationRange
    {
        [JsonProperty(PropertyName = "min")]
        public string Min { get; set; }
        [JsonProperty(PropertyName = "max")]
        public string Max { get; set; }
    }
}
