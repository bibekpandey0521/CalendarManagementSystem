using CalendarManagement.Api.DTOs;

namespace CalendarManagement.Api.Interfaces;

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default);
}
