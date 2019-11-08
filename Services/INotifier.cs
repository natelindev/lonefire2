using System;
using lonefire.Models;
using Microsoft.AspNetCore.Mvc;

namespace lonefire.Services
{
    public interface INotifier
    {
        void Notify(string message);

        void Notify(string message, NotificationLevel level);
    }
}
