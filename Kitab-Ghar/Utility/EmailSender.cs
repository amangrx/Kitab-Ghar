using System.Net;
using System.Net.Mail;

public class EmailSender
{
    public string SmtpServer { get; set; }
    public int Port { get; set; }
    public string SenderEmail { get; set; }
    public string SenderPassword { get; set; }

    public EmailSender(string smtpServer, int port, string senderEmail, string senderPassword)
    {
        SmtpServer = smtpServer;
        Port = port;
        SenderEmail = senderEmail;
        SenderPassword = senderPassword;
    }

    public void SendEmail(string recipientEmail, string subject, string body)
    {
        using (var client = new SmtpClient(SmtpServer, Port))
        {
            client.Credentials = new NetworkCredential(SenderEmail, SenderPassword);
            client.EnableSsl = true;

            var mailMessage = new MailMessage(SenderEmail, recipientEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            client.Send(mailMessage);
        }
    }
}
