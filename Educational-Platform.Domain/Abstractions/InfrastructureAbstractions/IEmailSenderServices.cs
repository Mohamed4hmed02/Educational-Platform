using System.Net.Mail;

namespace Educational_Platform.Domain.Abstractions.InfrastructureAbstractions
{
	public interface IEmailSenderServices
	{
		ValueTask SendMailAsync(string message, string to, bool isHTML = false, Attachment? attachment = null, string? subject = null);
		ValueTask AcceptMailAsync(string message, string From, bool isHTML = false, Attachment? attachment = null, string? subject = null);
	}
}
