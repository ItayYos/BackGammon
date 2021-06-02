namespace Server.Hubs
{
    public interface IUserHubNotificationService
    {
        void LogInCompletedNotification(string userName);
    }
}