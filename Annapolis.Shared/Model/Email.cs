namespace Annapolis.Shared.Model
{
    public enum EmailNotificationReason
    {
        UserCreateNotification,
        UserCreateVerification
    }

    public class Email
    {
        public string EmailTo { get; set; }
        public string EmailFrom { get; set; }
        public string EmailReceiverName { get; set; }

        public EmailNotificationReason NotificationReadon { get; set; }
    }
}
