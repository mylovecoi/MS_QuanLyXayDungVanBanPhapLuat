namespace QuanTriHeThongService.Contracts.Requests.Systems;

public class UpdateSystemInfoApiRequest
{
    public Guid Id { get; set; }
    public string? AppName { get; set; }
    public string? Copyright { get; set; }
    public DateTime MfgDate { get; set; }
    public DateTime ExpDate { get; set; }
    public int LoginLock { get; set; }
    public bool Train { get; set; }
    public bool IsChatBot { get; set; }
    public bool IsOPT { get; set; }
    public string? MenuLayout { get; set; }
}

