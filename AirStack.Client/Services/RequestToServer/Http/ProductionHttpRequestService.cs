using AirStack.Client.Model;
using AirStack.Client.Services.Notification;
using AirStack.Client.Services.Settings;
using AirStack.Core.Model;
using Serilog;
using System;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace AirStack.Client.Services.RequestToServer.Http
{
    public class ProductionHttpRequestService : BaseHttpRequestService
    {
        public ProductionHttpRequestService(ISettingsProvider settings, INotificationProvider notify, ILogger log)
            : base(settings, notify, log)
        { }

        protected override StatusEnum ItemState => StatusEnum.Production;

        public override async Task<RequestResultObject> SendRequestAsync(string code)
        {
            RequestResultObject resultObj = new RequestResultObject(code);

            try
            {
                var req = await _client.PostAsJsonAsync(_url + "/Item", code);
                resultObj = HandleResponse(req, code);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                _notify.NotifyError("Chyba při dotazu na server");
            }

            return resultObj;
        }
    }
}