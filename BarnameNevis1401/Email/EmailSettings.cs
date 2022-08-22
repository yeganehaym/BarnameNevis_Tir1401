namespace BarnameNevis1401.Email;

public class EmailSettings
{
    public string SmtpServer { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public int Port { get; set; }
    public bool SSL { get; set; }
}