using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

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
        public async Task<MultiResponse> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var body = await new StreamReader(req.Body).ReadToEndAsync();
            var bodyModel = Newtonsoft.Json.JsonConvert.DeserializeObject<FavoriteGameModel>(body);
            bodyModel.id = Guid.NewGuid().ToString();
            _logger.LogInformation(body);
            
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString($"Thank you for your submission! {body}");
            return new MultiResponse()
                {
                    FavoriteGame = bodyModel,
                    HttpResponse = response
                };
        }

        [Function("FavoriteGameGet")]
        public async Task<IEnumerable<FavoriteGameModel>> Get([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req,
         [CosmosDBInput(
                databaseName: "FavoriteGame",
                containerName: "Games",
                Connection = "CosmosConnection",
                SqlQuery ="SELECT * FROM c ORDER BY c._ts DESC")] IEnumerable<FavoriteGameModel> games
            )
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            
            return games;
        }
    }
}
