using System;
using System.Net;
using System.Net.Mail;

public interface IEmailSender
{
    Task SendEmailAsync(string recipientEmail, string subject, string body);
}

public class EmailSender : IEmailSender
{

    public Task SendEmailAsync(string recipientEmail, string subject, string body)
    {
        var emailSender = "gprabal505@gmail.com";
        var emailPassword = "cxvu vcax vvtx mtkl";

        var client = new SmtpClient("smtp.gmail.com", 587)
        {
            Credentials = new NetworkCredential(emailSender, emailPassword),
            EnableSsl = true
        };

        return client.SendMailAsync(new MailMessage
        (
            from: emailSender,
            to: recipientEmail, 
            subject,
            body
        )); 
    }
}
