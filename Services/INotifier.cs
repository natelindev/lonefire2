using System;

namespace lonefire.Services
{
    public interface INotifier
    {
        void Notify(string message);

        void Notify(string message, NotificationLevel level);
    }
}
