namespace PumpPalace.Models
{
    public class NotificationViewModel
    {
        public int NotificationId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
        public bool IsRead { get; set; }
    }

    public class NotificationListViewModel
    {
        public List<NotificationViewModel> Notifications { get; set; } // Lista powiadomień
    }
}
