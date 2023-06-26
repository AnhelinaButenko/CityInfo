namespace HwCityInfo.API.Servises
{
    public class CloudMailService : IMailService
    {
        private readonly string _mailTo = "admin@mycompany.com";
        private readonly string _mailFrom = "noreply@mycompany";

        public CloudMailService(IConfiguration configuration)
        {
            _mailTo = configuration["mailSettings:mailToAddress"];
            _mailFrom = configuration["mailSettings:mailFromAddress"];
        }

        public void Send(string subject, string message)
        {
            Console.WriteLine($"Mail from {_mailFrom} to {_mailTo}," +
                $"with {nameof(CloudMailService)}.");
            Console.WriteLine($"Subject{subject}");
            Console.WriteLine($"Message{message}");
        }
    }
}
