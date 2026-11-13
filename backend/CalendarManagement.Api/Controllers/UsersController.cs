using CalendarManagement.Api.DTOs;
using CalendarManagement.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CalendarManagement.Api.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>Gets the current user profile.</summary>
    [HttpGet("profile")]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserProfileDto>> GetProfile(CancellationToken cancellationToken = default)
    {
        var profile = await _userService.GetProfileAsync(1, cancellationToken);
        return Ok(profile);
    }

    /// <summary>Updates the user profile.</summary>
    [HttpPut("profile")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateProfile([FromBody] UpdateProfileDto dto, CancellationToken cancellationToken = default)
    {
        await _userService.UpdateProfileAsync(1, dto, cancellationToken);
        return Ok();
    }

    /// <summary>Changes the user password.</summary>
    [HttpPut("password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordDto dto, CancellationToken cancellationToken = default)
    {
        await _userService.ChangePasswordAsync(1, dto, cancellationToken);
        return Ok();
    }

    /// <summary>Gets user settings.</summary>
    [HttpGet("settings")]
    [ProducesResponseType(typeof(UserSettingsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserSettingsDto>> GetSettings(CancellationToken cancellationToken = default)
    {
        var settings = await _userService.GetSettingsAsync(1, cancellationToken);
        return Ok(settings);
    }

    /// <summary>Updates user settings.</summary>
    [HttpPut("settings")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UpdateSettings([FromBody] UpdateSettingsDto dto, CancellationToken cancellationToken = default)
    {
        await _userService.UpdateSettingsAsync(1, dto, cancellationToken);
        return Ok();
    }

    /// <summary>Deletes the user account.</summary>
    [HttpDelete("account")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteAccount(CancellationToken cancellationToken = default)
    {
        await _userService.DeleteAccountAsync(1, cancellationToken);
        return NoContent();
    }
}
