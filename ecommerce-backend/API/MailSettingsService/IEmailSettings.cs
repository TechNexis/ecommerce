namespace API.MailSettingsService
{
    public interface IEmailSettings
    {
        public Task SendEmailMessage(Email mail);
    }
}
