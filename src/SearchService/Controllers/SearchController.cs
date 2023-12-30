using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;

namespace SearchService;

[ApiController]
[Route("api/search")]
public class SearchController : ControllerBase
{
     [HttpGet]
     public async Task<ActionResult<List<Item>>> SearchItems([FromQuery] SearchParams searchParams)
     {
            // var query = DB.Find<Item>();
        var query = DB.PagedSearch<Item, Item>();

        //query.Sort(x => x.Ascending(a => a.Make));

        if(!string.IsNullOrEmpty(searchParams.searchTerm))
        {
            query.Match(Search.Full, searchParams.searchTerm).SortByTextScore();
        }

        query = searchParams.OrderBy switch 
        {
            "make" => query.Sort(x => x.Ascending(a => a.Make)),
            "new" => query.Sort(x => x.Descending(a => a.CreatedAt)),
            _ => query.Sort(x => x.Descending(a => a.AuctionEnd))
        };

        query = searchParams.FilterBy switch
        {
            "finished" => query.Match(x => x.AuctionEnd < DateTime.UtcNow),
            "endingSoon" => query.Match(x => x.AuctionEnd < DateTime.UtcNow.AddHours(6) && x.AuctionEnd > DateTime.UtcNow),
             _ => query.Match(x => x.AuctionEnd > DateTime.UtcNow)
        };

        if(!string.IsNullOrEmpty(searchParams.Seller))
        {
            query.Match(x => x.Seller ==searchParams.Seller );
        }

         if(!string.IsNullOrEmpty(searchParams.Winner))
        {
            query.Match(x => x.Winner ==searchParams.Winner );
        }

        query.PageSize(searchParams.PageSize);
        query.PageNumber(searchParams.PageNumber);

        var result = await query.ExecuteAsync();

        //return result;

        return Ok(
            new 
            {
               results = result.Results,
               PageCount = result.PageCount,
               totalCount = result.TotalCount
            }
        );
     }
}
