using System.Net.Mail;

namespace Crypto.Services.Identity;

public interface ISmtpClientFactory
{
    string DisplayName { get; }

    SmtpClient CreateClient();
}
