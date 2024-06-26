﻿using AuctionService.Data;
using AuctionService.DtOs;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Contract;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/auctions")]
public class AuctionsController : ControllerBase
{
    private readonly AuctionDbContext _context;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;

    public AuctionsController(AuctionDbContext context, IMapper mapper, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
    }

    [HttpGet]
    public async Task<ActionResult<List<AuctionDTo>>> GetAllAuction(string date)
    {
        var query = _context.Auctions.OrderBy(x => x.Item.Make).AsQueryable();

        if (!string.IsNullOrEmpty(date))
        {
            query = query.Where(x => x.UpdatedAt.CompareTo(DateTime.Parse(date).ToUniversalTime()) > 0);
        }

        /*var auctions = await _context.Auctions
             .Include(x => x.Item)
             .OrderBy(x => x.Item.Make)
             .ToListAsync();
        
        return _mapper.Map<List<AuctionDTo>>(auctions);*/

        return await query.ProjectTo<AuctionDTo>(_mapper.ConfigurationProvider).ToListAsync();
    }


    [HttpGet("{id}", Name = "GetAuctionById")]
    public async Task<ActionResult<AuctionDTo>> GetAuctionById(Guid id)
    {
        var auction = await _context.Auctions
          .Include(x => x.Item)
          .FirstOrDefaultAsync(x => x.Id == id);

        if (auction is null) return NotFound();

        return _mapper.Map<AuctionDTo>(auction);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<AuctionDTo>> CreateAuction(CreateAuctionDTo auctionDTo)
    {
        var auction = _mapper.Map<Auction>(auctionDTo);


        auction.Seller = User.Identity.Name;

        _context.Auctions.Add(auction);

        var newAuction = _mapper.Map<AuctionDTo>(auction);

        await _publishEndpoint.Publish(_mapper.Map<AuctionCreated>(newAuction));

        var result = await _context.SaveChangesAsync() > 0;

        if (!result) return BadRequest("Could not save changes to the Db");

        return CreatedAtRoute(nameof(GetAuctionById), new { auction.Id }, newAuction);
    }


    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto updateAuctionDto)
    {
        var auction = await _context.Auctions.Include(x => x.Item)
             .FirstOrDefaultAsync(x => x.Id == id);

        if (auction is null) return NotFound();

        if (auction.Seller != User.Identity.Name) return Forbid();

        auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
        auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
        auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
        auction.Item.Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage;
        auction.Item.Year = updateAuctionDto.Year ?? auction.Item.Year;

        await _publishEndpoint.Publish(_mapper.Map<AuctionUpdated>(auction));

        var result = await _context.SaveChangesAsync() > 0;

        if (result) return Ok();

        return BadRequest("problem saving changes");

    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAuction(Guid id)
    {
        var auction = await _context.Auctions.FindAsync(id);

        if (auction is null) return NotFound();

        if (auction.Seller != User.Identity.Name) return Forbid();

        _context.Auctions.Remove(auction);

        await _publishEndpoint.Publish<AuctionDeleted>(new { Id = auction.Id.ToString() });

        var result = await _context.SaveChangesAsync() > 0;

        if (!result) return BadRequest("could not update DB");

        return Ok();

    }
}
