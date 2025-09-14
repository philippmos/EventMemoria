namespace EventMemoria.Web.Services.Interfaces;

public interface ISubscriberService
{
    Task<bool> AddSubscriberAsync(string email);
}
