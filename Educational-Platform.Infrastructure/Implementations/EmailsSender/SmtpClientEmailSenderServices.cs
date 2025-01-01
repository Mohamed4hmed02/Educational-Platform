using Educational_Platform.Domain.Abstractions.InfrastructureAbstractions;
using Educational_Platform.Infrastructure.Options;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Educational_Platform.Infrastructure.Implementations.EmailsSender
{
	public class SmtpClientEmailSenderServices : IEmailSenderServices
	{
		private readonly SmtpClient _smtpClient;
		private readonly string _address;

		public SmtpClientEmailSenderServices(IOptionsMonitor<SMTPOptions> options)
		{
			_smtpClient = new SmtpClient(options.CurrentValue.Server, 587)
			{
				EnableSsl = true,
				Credentials = new NetworkCredential(options.CurrentValue.Email, options.CurrentValue.Password)
			};
			_address = options.CurrentValue.Email;
		}

		public ValueTask AcceptMailAsync(string message, string from, bool isHTML = false, Attachment? attachment = null, string? subject = null)
		{
			message += "\nFrom " + from;
			var msg = new MailMessage()
			{
				From = new MailAddress(from),
				Body = message,
				IsBodyHtml = isHTML
			};
			if (attachment != null)
				msg.Attachments.Add(attachment);

			if (subject != null)
				msg.Subject = subject;

			msg.To.Add(_address);
			_smtpClient.Send(msg);
			return ValueTask.CompletedTask;
		}

		public ValueTask SendMailAsync(string message, string to, bool isHTML = false, Attachment? attachment = null, string? subject = null)
		{
			var msg = new MailMessage()
			{
				From = new MailAddress(_address),
				Body = message,
				IsBodyHtml = isHTML
			};
			if (attachment != null)
				msg.Attachments.Add(attachment);

			if (subject != null)
				msg.Subject = subject;

			msg.To.Add(to);

			if(attachment?.ContentStream is MemoryStream)
				attachment.ContentStream.Position = 0;

			_smtpClient.Send(msg);

			return ValueTask.CompletedTask;
		}
	}
}
