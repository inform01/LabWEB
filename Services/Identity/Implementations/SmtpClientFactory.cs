using System.Net;
using System.Net.Mail;
using Crypto.Options;
using Microsoft.Extensions.Options;

namespace Crypto.Services.Identity.Implementations;

internal class SmtpClientFactory : ISmtpClientFactory
{
    private readonly MailOptions _mailOptions;

    public string DisplayName => _mailOptions.DisplayName;

    public SmtpClientFactory(IOptions<MailOptions> options)
    {
        _mailOptions = options.Value;
    }

    public SmtpClient CreateClient()
    {
        return new SmtpClient(_mailOptions.Host, _mailOptions.Port)
        {
            EnableSsl = _mailOptions.UseSsl,
            Credentials = new NetworkCredential(_mailOptions.UserName, _mailOptions.Password)
        };
    }
}
