using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        { // For development, log the email instead of sending it
          Console.WriteLine($"Sending email to {email} with subject {subject}");
            return Task.CompletedTask;
         }
     }   
}
