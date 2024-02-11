using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/auctions")]
public class AuctionController : ControllerBase
{
  private readonly AuctionDbContext _context;
  private readonly IMapper _mapper;

  public AuctionController(AuctionDbContext context, IMapper mapper)
  {
    _context = context;
    _mapper = mapper;
  }

  [HttpGet]
  public async Task<ActionResult<IEnumerable<AuctionDto>>> GetAuctions()
  {
    var auctions = await _context.Auctions
      .Include(a => a.Item)
      .OrderBy(a => a.Item.Make)
      .ToListAsync();

    return Ok(_mapper.Map<IEnumerable<AuctionDto>>(auctions));
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
  {
    var auction = await _context.Auctions
      .Include(a => a.Item)
      .FirstOrDefaultAsync(a => a.Id == id);

    if (auction == null)
    {
      return NotFound();
    }

    return Ok(_mapper.Map<AuctionDto>(auction));
  }

  [HttpPost]
  public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto createAuctionDto)
  {
    var auction = _mapper.Map<Auction>(createAuctionDto);
    _context.Auctions.Add(auction);
    await _context.SaveChangesAsync();

    return CreatedAtAction(nameof(GetAuctionById), new { id = auction.Id }, _mapper.Map<AuctionDto>(auction));
  }

  [HttpPut("{id}")]
  public async Task<IActionResult> UpdateAuction(Guid id, UpdateAuctionDto updateAuctionDto)
  {
    var auction = await _context.Auctions
      .Include(a => a.Item)
      .FirstOrDefaultAsync(a => a.Id == id);

    if (auction == null) return NotFound(); 

    auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
    auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
    auction.Item.Year = updateAuctionDto.Year != 0 ? updateAuctionDto.Year : auction.Item.Year;
    auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
    auction.Item.Mileage = updateAuctionDto.Mileage != 0 ? updateAuctionDto.Mileage : auction.Item.Mileage;
    auction.Item.ImageUrl = updateAuctionDto.ImageUrl ?? auction.Item.ImageUrl;

    var result = await _context.SaveChangesAsync() > 0;

    if (result) return Ok();

    return BadRequest("Failed to update auction");
  }

  [HttpDelete("{id}")]
  public async Task<IActionResult> DeleteAuction(Guid id)
  {
    var auction = await _context.Auctions
      .Include(a => a.Item)
      .FirstOrDefaultAsync(a => a.Id == id);

    if (auction == null) return NotFound();

    _context.Auctions.Remove(auction);
    var result = await _context.SaveChangesAsync() > 0;

    if (result) return Ok();

    return BadRequest("Failed to delete auction");
  }
}
