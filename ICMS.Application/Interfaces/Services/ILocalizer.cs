namespace ICMS.Application.Interfaces.Services
{
    public interface ILocalizer
    {
        string this[string key] { get; }
        string this[string key, params object[] args] { get; }
    }
}
