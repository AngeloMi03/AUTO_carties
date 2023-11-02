using AuctionService.Data;
using AuctionService.DtOs;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/auctions")]
public class AuctionsController : ControllerBase
{
    private readonly AuctionDbContext _context;
    private readonly IMapper _mapper;

    public AuctionsController(AuctionDbContext context, IMapper mapper)
   {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<AuctionDTo>>> GetAllAuction()
    {
         var auctions = await _context.Auctions
             .Include(x => x.Item)
             .OrderBy(x => x.Item.Make)
             .ToListAsync();
        
        return _mapper.Map<List<AuctionDTo>>(auctions);
    }
    

    [HttpGet("{id}", Name="GetAuctionById")]
    public async Task<ActionResult<AuctionDTo>> GetAuctionById( Guid id)
    {
        var auction = await _context.Auctions
          .Include(x => x.Item)
          .FirstOrDefaultAsync(x => x.Id == id);

          if(auction is null) return NotFound();

         return _mapper.Map<AuctionDTo>(auction);
    }

    [HttpPost]
    public async Task<ActionResult<AuctionDTo>> CreateAuction(CreateAuctionDTo auctionDTo)
    {
       var auction = _mapper.Map<Auction>(auctionDTo);

       //todo : add current user as seller
       auction.Seller = "test"; 

       _context.Auctions.Add(auction);

       var result = await _context.SaveChangesAsync() > 0;

       if(!result) return BadRequest("Could not save changes to the Db");

       return CreatedAtRoute(nameof(GetAuctionById), new {auction.Id}, _mapper.Map<AuctionDTo>(auction));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAuction( Guid id, UpdateAuctionDto updateAuctionDto)
    {
        var auction = await _context.Auctions.Include(x => x.Item)
             .FirstOrDefaultAsync(x => x.Id ==  id);

       if(auction is null) return NotFound();

       //TODO :check seller == username
       auction.Item.Make = updateAuctionDto.Make ??  auction.Item.Make;
       auction.Item.Model = updateAuctionDto.Model ??  auction.Item.Model;
       auction.Item.Color = updateAuctionDto.Color ??  auction.Item.Color;
       auction.Item.Mileage = updateAuctionDto.Mileage ??  auction.Item.Mileage;
       auction.Item.Year = updateAuctionDto.Year ??  auction.Item.Year;

       var result = await _context.SaveChangesAsync() > 0;
       
       if (result) return Ok();

       return BadRequest("problem saving changes");
       
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAuction(Guid id)
    {
        var auction = await _context.Auctions.FindAsync(id);

        if(auction is null) return NotFound();

        //TODO :check seller == username

        _context.Auctions.Remove(auction);

        var result = await _context.SaveChangesAsync() > 0; 

        if(!result)  return BadRequest("could not update DB");

        return Ok();
        
    }
}
