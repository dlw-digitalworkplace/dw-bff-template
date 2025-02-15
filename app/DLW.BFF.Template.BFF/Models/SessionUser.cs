namespace DLW.BFF.Template.BFF.Models
{
    public class SessionUser (string? UserName, bool IsAuthenticated)
    {
        public string? UserName { get; set; } = UserName;
        public bool IsAuthenticated { get; set; } = IsAuthenticated;
    }
}
