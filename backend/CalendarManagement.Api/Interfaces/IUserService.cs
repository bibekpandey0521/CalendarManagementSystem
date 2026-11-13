using CalendarManagement.Api.DTOs;

namespace CalendarManagement.Api.Interfaces;

public interface IUserService
{
    Task<UserProfileDto> GetProfileAsync(int userId, CancellationToken cancellationToken = default);
    Task UpdateProfileAsync(int userId, UpdateProfileDto dto, CancellationToken cancellationToken = default);
    Task ChangePasswordAsync(int userId, ChangePasswordDto dto, CancellationToken cancellationToken = default);
    Task<UserSettingsDto> GetSettingsAsync(int userId, CancellationToken cancellationToken = default);
    Task UpdateSettingsAsync(int userId, UpdateSettingsDto dto, CancellationToken cancellationToken = default);
    Task DeleteAccountAsync(int userId, CancellationToken cancellationToken = default);
}
