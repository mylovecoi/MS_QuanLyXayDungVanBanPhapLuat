namespace QuanTriHeThongService.Contracts.Requests.Systems;

public class PermissionUpdateApiRequest
{
    public bool Index { get; set; }
    public bool Create { get; set; }
    public bool Edit { get; set; }
    public bool Delete { get; set; }
    public bool Approve { get; set; }
    public bool Public { get; set; }
}

