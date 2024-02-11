using AutoMapper;
using AuctionService.DTOs;
using AuctionService.Entities;

namespace AuctionService.RequestHelpers;

public class MappingProfile : Profile
{
  public MappingProfile()
  {
    CreateMap<Auction, AuctionDto>().IncludeMembers(a => a.Item);
    CreateMap<Item, AuctionDto>();
    CreateMap<CreateAuctionDto, Auction>().ForMember(d => d.Item, opt => opt.MapFrom(s => s));
    CreateMap<CreateAuctionDto, Item>();
  }
}
