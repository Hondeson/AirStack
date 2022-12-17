using AirStack.Client.Services.Notification;
using AirStack.Client.Services.Settings;
using AirStack.Core.Model;
using Serilog;

namespace AirStack.Client.Services.RequestToServer.Http
{
    public class ComplaintToSupplierHttpRequestService : BaseHttpRequestService
    {
        public ComplaintToSupplierHttpRequestService(ISettingsProvider settings, INotificationProvider notify, ILogger log) : base(settings, notify, log)
        {
        }

        protected override StatusEnum ItemState => StatusEnum.ComplaintToSupplier;
    }
}