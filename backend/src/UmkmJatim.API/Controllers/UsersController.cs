using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UmkmJatim.Application.Auth.DTOs;
using UmkmJatim.Application.Common.Interfaces;
using UmkmJatim.Application.Users.Commands;
using UmkmJatim.Application.Users.DTOs;
using UmkmJatim.Domain.Entities;
using UmkmJatim.Domain.Exceptions;

namespace UmkmJatim.API.Controllers;

[Authorize]
public class UsersController : BaseApiController
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICurrentUser _currentUser;

    public UsersController(UserManager<ApplicationUser> userManager, ICurrentUser currentUser)
    {
        _userManager = userManager;
        _currentUser = currentUser;
    }

    [HttpGet("me")]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProfile()
    {
        if (_currentUser.UserId == null)
            throw new UnauthorizedException();

        var user = await _userManager.FindByIdAsync(_currentUser.UserId.Value.ToString());
        if (user == null)
            throw new NotFoundException("User", _currentUser.UserId.Value);

        var dto = new UserProfileDto(
            user.Id,
            user.Email ?? string.Empty,
            user.FullName,
            user.Role,
            user.PhoneNumber,
            user.AvatarUrl,
            user.CreatedAt
        );

        return Ok(dto);
    }

    [HttpPut("me")]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("me/addresses")]
    [ProducesResponseType(typeof(List<AddressDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAddresses()
    {
        var addresses = await Mediator.Send(new Application.Users.Queries.GetAddressesQuery());
        return Ok(addresses);
    }

    [HttpPost("me/addresses")]
    [ProducesResponseType(typeof(AddressDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateAddress([FromBody] CreateAddressCommand command)
    {
        var address = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetAddresses), new { id = address.Id }, address);
    }

    [HttpPut("me/addresses/{id:int}")]
    [ProducesResponseType(typeof(AddressDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateAddress(int id, [FromBody] UpdateAddressCommand command)
    {
        if (id != command.Id)
            return BadRequest(new { message = "ID pada route tidak cocok dengan body request." });

        var address = await Mediator.Send(command);
        return Ok(address);
    }

    [HttpDelete("me/addresses/{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAddress(int id)
    {
        await Mediator.Send(new DeleteAddressCommand(id));
        return NoContent();
    }

    [HttpPatch("me/addresses/{id:int}/default")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> SetDefaultAddress(int id)
    {
        await Mediator.Send(new SetDefaultAddressCommand(id));
        return NoContent();
    }
}
