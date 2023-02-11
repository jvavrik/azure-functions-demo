using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

public class MultiResponse{
        [CosmosDBOutput(
            databaseName: "FavoriteGame",
            containerName:"Games", 
            Connection = "CosmosConnection", 
            CreateIfNotExists =true)]
            public FavoriteGameModel FavoriteGame {get;set;}
            public HttpResponseData HttpResponse { get; set; }
    }