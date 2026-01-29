namespace EventHub.Service.Interfaces;

public interface IEmailService
{
    Task SendTicketConfirmationAsync(string recipientEmail, string recipientName, string eventName, int quantity, decimal totalPrice, string ticketNumbers);
}
