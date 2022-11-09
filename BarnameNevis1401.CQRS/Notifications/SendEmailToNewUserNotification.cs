using System.Net.Mail;
using BarnameNevis1401.Domains.Users;
using MediatR;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace BarnameNevis1401.CQRS.Notifications;

public class NewUserNotification:INotification
{
    public User User { get; set; }
}

public class SendEmailToNewUserNotificationHandler : INotificationHandler<NewUserNotification>
{
    
    public async Task Handle(NewUserNotification notification, CancellationToken cancellationToken)
    {
        await File.WriteAllTextAsync($"h:\\email{DateTime.Now.ToString("hh-mm-ss")}.txt",
            "Email To : " + notification.User.Email);
    }

   
}

public class SendSMSToNewUserNotificationHandler : INotificationHandler<NewUserNotification>
{
    
    public async Task Handle(NewUserNotification notification, CancellationToken cancellationToken)
    {
        await File.WriteAllTextAsync($"h:\\sms{DateTime.Now.ToString("hh-mm-ss")}.txt",
            "SMS To : " + notification.User.Mobile);
    }

   
}