namespace HwCityInfo.API.Servises
{
    public interface IMailService
    {
        void Send(string subject, string message);
    }
}