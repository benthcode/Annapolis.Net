namespace Annapolis.Web.Client
{
    public enum NotificationType
    {
         Alert, 
         Success, 
         Error,
         Warning, 
         Information, 
         Confirmation
    }


    public class Notification
    {
        public const int LongTime = 4000;
        public const int MediumTime = 2500;
        public const int ShortTime = 1000;
        public const int TransientTime = 500;
        public const int Stick = 0;

        public NotificationType Type {get; set;}
        
        public string Message { get; set; }

        public int TimeOut { get; set; }

        public bool IsModal { get; set; }

        public bool IsVisible { get; set; }

        public Notification() { }

        public Notification(string message, NotificationType type, int timeOut = Notification.ShortTime, 
                                bool isModal = true, bool isVisible = true)
        {
            Message = message;
            Type = type;
            TimeOut = timeOut;
            IsModal = isModal;
            IsVisible = isVisible;
        }
    }
}