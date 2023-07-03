namespace HwCityInfo.API.Servises
{
    public class LocalMailService : IMailService // derived.. it manages the service lifetime.
    {
        private readonly string _mailTo = "admin@mycompany.com";
        private readonly string _mailFrom = "noreply@mycompany";

        public LocalMailService(IConfiguration configuration) 
        {
            _mailTo = configuration["mailSettings:mailToAddress"];
            _mailFrom = configuration["mailSettings:mailFromAddress"];
        }

        // send a mail info to the console window
        public void Send(string subject, string message)
        {
            Console.WriteLine($"Mail from {_mailFrom} to {_mailTo}," +
                $"with {nameof(LocalMailService)}.");
            Console.WriteLine($"Subject{subject}");
            Console.WriteLine($"Message{message}");
        }
    }
}
