using System;

namespace api.DTO;

public class PhotoForApprovalDTO
{
    public int PhotoId { get; set; }
    public required string Url { get; set; }
    public required string UserName { get; set; }
    public required bool IsApproved { get; set; }
}
