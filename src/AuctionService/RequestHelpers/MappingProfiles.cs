using AuctionService.DtOs;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;

namespace AuctionService.RequestHelpers;

public class MappingProfiles : Profile
{
   public MappingProfiles()
   {
      CreateMap<Auction,AuctionDTo>().IncludeMembers(x => x.Item);
      CreateMap<Item,AuctionDTo>();
      CreateMap<CreateAuctionDTo,Auction>()
        .ForMember(d => d.Item, o => o.MapFrom(s =>s));
      CreateMap<CreateAuctionDTo,Item>();
   }
}
