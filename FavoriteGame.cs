using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Jvavrik.Functions
{
    public class FavoriteGame
    {
        private readonly ILogger _logger;

        public FavoriteGame(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<FavoriteGame>();
        }

        [Function("FavoriteGame")]
        public async Task<MultiResponse> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var body = await new StreamReader(req.Body).ReadToEndAsync();
            _logger.LogInformation(body);
            
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString($"Thank you for your submission! {body}");
            return new MultiResponse()
                {
                    FavoriteGame = new FavoriteGameModel
                    {
                        Name = body,
                        id = Guid.NewGuid().ToString(),
                    },
                    HttpResponse = response
                };
        }
    }

    public class MultiResponse{
        [CosmosDBOutput(
            databaseName: "FavoriteGame",
            containerName:"Games", 
            Connection = "CosmosConnection", 
            CreateIfNotExists =true)]
            public FavoriteGameModel FavoriteGame {get;set;}
            public HttpResponseData HttpResponse { get; set; }
    }

    public class FavoriteGameModel{
        public string Name {get;set;}
        public string id {get;set;}
    }
}
