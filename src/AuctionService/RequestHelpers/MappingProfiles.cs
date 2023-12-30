using AuctionService.DtOs;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Contract;

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
      CreateMap<AuctionDTo, AuctionCreated>();

      CreateMap<Auction,AuctionUpdated>().IncludeMembers(x => x.Item);
      CreateMap<Item,AuctionUpdated>();
   }
}
