namespace Contract;

public class BidPlaced
{
   public string Id { get; set; }
   public string AuctionId { get; set; }
   public string Bidder { get; set; }
   public DateTime BidTime { get; set; }
   public int Amout { get; set; }
   public string BidStatus { get; set; }

}
