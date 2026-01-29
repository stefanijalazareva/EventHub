using System.Net;
using System.Net.Mail;
using EventHub.Service.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EventHub.Service.Implementation;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendTicketConfirmationAsync(string recipientEmail, string recipientName, string eventName, int quantity, decimal totalPrice, string ticketNumbers)
    {
        var smtpHost = _configuration["Email:SmtpHost"];
        var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
        var senderEmail = _configuration["Email:SenderEmail"];
        var senderPassword = _configuration["Email:SenderPassword"];
        var senderName = _configuration["Email:SenderName"] ?? "EventHub";

        if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(senderEmail) || string.IsNullOrEmpty(senderPassword))
        {
            _logger.LogWarning("Email configuration is incomplete. Email will not be sent.");
            return;
        }

        _logger.LogInformation("Attempting to send email to {RecipientEmail} for event {EventName}", recipientEmail, eventName);

        var mailMessage = new MailMessage
        {
            From = new MailAddress(senderEmail, senderName),
            Subject = $"Ticket Confirmation - {eventName}",
            Body = GenerateEmailBody(recipientName, eventName, quantity, totalPrice, ticketNumbers),
            IsBodyHtml = true
        };

        mailMessage.To.Add(new MailAddress(recipientEmail, recipientName));

        using var smtpClient = new SmtpClient(smtpHost, smtpPort)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(senderEmail, senderPassword)
        };

        try
        {
            await smtpClient.SendMailAsync(mailMessage);
            _logger.LogInformation("Email sent successfully to {RecipientEmail}", recipientEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {RecipientEmail}. Error: {ErrorMessage}", recipientEmail, ex.Message);
            // Don't fail the booking if email fails
        }
    }

    private string GenerateEmailBody(string recipientName, string eventName, int quantity, decimal totalPrice, string ticketNumbers)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #3F9AAE 0%, #79C9C5 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
        .ticket-info {{ background: white; padding: 20px; border-radius: 8px; margin: 20px 0; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }}
        .ticket-numbers {{ background: #FFE2AF; padding: 15px; border-radius: 5px; margin: 15px 0; font-family: monospace; }}
        .footer {{ text-align: center; margin-top: 30px; color: #666; font-size: 12px; }}
        h1 {{ margin: 0; font-size: 28px; }}
        h2 {{ color: #3F9AAE; }}
        .price {{ font-size: 24px; color: #3F9AAE; font-weight: bold; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🎫 Ticket Confirmation</h1>
        </div>
        <div class='content'>
            <p>Dear <strong>{recipientName}</strong>,</p>
            <p>Thank you for booking tickets with EventHub! Your booking has been confirmed.</p>
            
            <div class='ticket-info'>
                <h2>📅 Event Details</h2>
                <p><strong>Event:</strong> {eventName}</p>
                <p><strong>Quantity:</strong> {quantity} ticket(s)</p>
                <p><strong>Total Price:</strong> <span class='price'>${totalPrice:F2}</span></p>
            </div>

            <div class='ticket-numbers'>
                <strong>🎟️ Your Ticket Numbers:</strong><br/>
                {ticketNumbers}
            </div>

            <p>Please save this email for your records. Show your ticket numbers at the event entrance.</p>
            
            <p>If you have any questions, please don't hesitate to contact us.</p>
            
            <p>See you at the event!</p>
            <p><strong>The EventHub Team</strong></p>
        </div>
        <div class='footer'>
            <p>&copy; 2026 EventHub. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
    }
}
