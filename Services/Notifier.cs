using System;
using lonefire.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace lonefire.Services
{
    public class Notifier : INotifier
    {
        private readonly ILogger _logger;
        private readonly NotificationHub _hub;

        public Notifier(
            ILogger logger,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            NotificationHub hub
            )
        {
            _logger = logger;
            _hub = hub;
            _httpContextAccessor = httpContextAccessor;

        }

        private static IHttpContextAccessor _httpContextAccessor;
        public static HttpContext Current => _httpContextAccessor.HttpContext;

        public NotificationLevel DefaultLevel => MapNotificationLevel(Startup.Configuration.GetValue<string>("Notification.NotificationLevel"));

        // Notify on default level
        public void Notify(string message)
        { 
            Notify(message, DefaultLevel);
        }

        // Notify on specified level
        public async void Notify(string message, NotificationLevel level)
        {
            if (level >= DefaultLevel)
            {
                await _hub.SendMessage("[" + level + "]" + message);
            }
        }

        // Match string to NotificationLevel
        public NotificationLevel MapNotificationLevel(string str)
        {
            var levels = NotificationLevel.GetNotificationLevels();
            foreach (var level in levels)
            {
                if (level == str) return level;
            }

            // fallback level
            return NotificationLevel.Info;
        }

    }


    public class NotificationLevel
    {
        private NotificationLevel(string value) { Value = value; }

        public string Value { get; private set; }

        public static NotificationLevel Debug => new NotificationLevel("Debug");
        public static NotificationLevel Info => new NotificationLevel("Info");
        public static NotificationLevel Success => new NotificationLevel("Success");
        public static NotificationLevel Warn => new NotificationLevel("Warn");
        public static NotificationLevel Error => new NotificationLevel("Error");

        public static bool operator <(NotificationLevel n1, NotificationLevel n2 )
        {
            var array = GetNotificationLevels();
            return Array.IndexOf(array, n1) < Array.IndexOf(array, n2);
        }

        public static bool operator >(NotificationLevel n1, NotificationLevel n2)
        {
            var array = GetNotificationLevels();
            return Array.IndexOf(array, n1) > Array.IndexOf(array, n2);
        }

        public static bool operator <=(NotificationLevel n1, NotificationLevel n2)
        {
            return n1 < n2 || n1 == n2;
        }

        public static bool operator >=(NotificationLevel n1, NotificationLevel n2)
        {
            return n1 > n2 || n1 == n2;
        }

        // return NotificationLevels Array
        public static NotificationLevel[] GetNotificationLevels()
        {
            var levels = new NotificationLevel[5];
            levels[0] = NotificationLevel.Debug;
            levels[1] = NotificationLevel.Info;
            levels[2] = NotificationLevel.Success;
            levels[3] = NotificationLevel.Warn;
            levels[4] = NotificationLevel.Error;
            return levels;
        }

        public override string ToString() { return Value; }

        public static implicit operator string(NotificationLevel level) { return level.Value; }
    }
}
