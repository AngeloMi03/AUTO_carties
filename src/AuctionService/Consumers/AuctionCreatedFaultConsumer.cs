using Contract;
using MassTransit;

namespace AuctionService;

public class AuctionCreatedFaultConsumer : IConsumer<Fault<AuctionCreated>>
{
    public async Task Consume(ConsumeContext<Fault<AuctionCreated>> context)
    {
       Console.WriteLine("--> consuming faulty creation");

       var Exception = context.Message.Exceptions.First();

       if(Exception.ExceptionType == "System.ArgumentException")
       {
          context.Message.Message.Model = "FooBar";

          await context.Publish(context.Message.Message);
       }else
       {
         Console.WriteLine("Not an argument exception- update error argument dashboard somewhere");
       }
    }
}
