using Contract;
using MassTransit;
using MongoDB.Entities;

namespace SearchService;

public class AuctionFinishedConsumer : IConsumer<AuctionFinished>
{
    public async Task Consume(ConsumeContext<AuctionFinished> context)
    {
         Console.WriteLine("---> Consuming bid Placed");

        var auction = await DB.Find<Item>().OneAsync(context.Message.AuctionId);

         if(context.Message.ItemSold)
        {
            auction.Winner = context.Message.Winner;
            auction.SoldAmount = (int)context.Message.Amout;
        }

        auction.Status = "Finished";

        await auction.SaveAsync();
    }
}
