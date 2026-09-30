using QuanTriHeThongService.Domain.Common;

namespace QuanTriHeThongService.Domain.Entities.Systems;

public class SystemInfoEntity : BaseEntity
{
    public string? AppName { get; set; } = "Giải pháp phần mềm";
    public string? Copyright { get; set; } = "LifeSoftware";
    public DateTime MfgDate { get; set; } = DateTime.Now;
    public DateTime ExpDate { get; set; } = DateTime.Now.AddYears(1);
    public int LoginLock { get; set; } = 5;
    public bool Train { get; set; }
    public bool IsChatBot { get; set; }
    public bool IsOPT { get; set; }
    public string MenuLayout { get; set; } = "vertical";
}

