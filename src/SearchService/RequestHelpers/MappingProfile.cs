using AutoMapper;
using Contract;

namespace SearchService;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<AuctionCreated,Item>();

         CreateMap<AuctionUpdated,Item>();
    }
}
