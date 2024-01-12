using AuctionService.Data;
using Contract;
using MassTransit;

namespace AuctionService;

public class AuctionFinishedConsumer : IConsumer<AuctionFinished>
{
    private readonly AuctionDbContext _dbcontext;

    public AuctionFinishedConsumer(AuctionDbContext dbcontext)
    {
        _dbcontext = dbcontext;
    }

    public async Task Consume(ConsumeContext<AuctionFinished> context)
    {

        Console.WriteLine("--> Consuming Auction Finished");

        var auction = await _dbcontext.Auctions.FindAsync(context.Message.AuctionId);

        if(context.Message.ItemSold)
        {
            auction.Winner = context.Message.Winner;
            auction.SoldAmount = context.Message.Amout;
        }

        auction.Status = auction.SoldAmount > auction.ReservePrice 
          ? Status.Finished : Status.ReserveNotMet;

        await _dbcontext.SaveChangesAsync();
    }
}
