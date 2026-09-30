namespace QuanTriHeThongService.Application.DTOs.Systems;

public class UpdatePermissionRequest
{
    public bool Index { get; set; }
    public bool Create { get; set; }
    public bool Edit { get; set; }
    public bool Delete { get; set; }
    public bool Approve { get; set; }
    public bool Public { get; set; }
}

