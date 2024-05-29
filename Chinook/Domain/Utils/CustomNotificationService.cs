namespace Chinook.Domain.Utils
{
    public class CustomNotificationService
    {
        public event Func<string, Task> OnSubscribeEvent;

        public async Task NotifyEventAsync(string eventName)
        {
            if (OnSubscribeEvent != null)
            {
                await OnSubscribeEvent.Invoke(eventName);
            }
        }
    }
}
