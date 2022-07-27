namespace BarnameNevis1401.Models;

public class ErrorViewModel:IDisposable
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    public void Dispose()
    {
    }
}