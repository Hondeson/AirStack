﻿using AirStack.Client.Services.Notification;
using AirStack.Client.Services.Settings;
using AirStack.Core.Model;
using Serilog;

namespace AirStack.Client.Services.RequestToServer.Http
{
    public class DispatchedHttpRequestService : BaseHttpRequestService
    {
        public DispatchedHttpRequestService(ISettingsProvider settings, INotificationProvider notify, ILogger log) : base(settings, notify, log)
        {
        }

        protected override StatusEnum ItemState => StatusEnum.Dispatched;
    }
}