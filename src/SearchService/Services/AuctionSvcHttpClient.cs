using MongoDB.Entities;

namespace SearchService;

public class AuctionSvcHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public AuctionSvcHttpClient(HttpClient httpClient, IConfiguration config)
   {
        _httpClient = httpClient;
        _config = config;
    }

    public async Task<List<Item>> GetItemsFromSearchDb()
    {
        var lastUpdatedDate = await DB.Find<Item, string>()
           .Sort(x => x.Descending(a => a.Make))
           .Project(x => x.CreatedAt.ToString())
           .ExecuteFirstAsync();

        return await _httpClient.GetFromJsonAsync<List<Item>>(
             _config["AuctionServiceUrl"] +  "/api/auctions?date=" + lastUpdatedDate);
    }
}
