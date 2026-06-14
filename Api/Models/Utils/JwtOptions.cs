namespace Api.Models.Utils;

public class JwtOptions
{
    public string Key { get; set; } = "";
    public int ExpiresInHours { get; set; }
}
