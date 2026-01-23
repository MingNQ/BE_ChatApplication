using Application.Common.Interfaces;
using Domain.Common.Enums;

namespace Application.Dto.Social;

public class PresenceDto : IDto
{
    public long UserId { get; set; }
    public UserPresenceStatusEnum Status { get; set; }
    public DateTimeOffset LastActiveAt { get; set; }
}