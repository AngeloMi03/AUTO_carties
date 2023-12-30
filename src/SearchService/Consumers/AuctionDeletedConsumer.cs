﻿using Contract;
using MassTransit;
using MongoDB.Entities;

namespace SearchService;

public class AuctionDeletedConsumer : IConsumer<AuctionDeleted>
{
    public async Task Consume(ConsumeContext<AuctionDeleted> context)
    {

         Console.WriteLine("---> Consuming aution Deleted" + context.Message.Id);

         var result =  await DB.DeleteAsync<Item>(context.Message.Id);

        if(!result.IsAcknowledged)
          throw new MessageException(typeof(AuctionDeleted), "Problem deleting auction");
    }
}