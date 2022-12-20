using AirStack.Client.Model;
using AirStack.Client.Services.Notification;
using AirStack.Client.Services.Settings;
using AirStack.Core.Model;
using AirStack.Core.Model.API;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace AirStack.Client.Services.RequestToServer.Http
{
    public abstract class BaseHttpRequestService : IServerRequestService
    {
        protected readonly ILogger _log;
        protected readonly INotificationProvider _notify;
        protected readonly ISettingsProvider _settings;
        protected readonly HttpClient _client;
        protected string _url;

#if !DEBUG
        protected const int c_TimeOut = 6;
#else
        protected const int c_TimeOut = 180;
#endif
        public BaseHttpRequestService(ISettingsProvider settings, INotificationProvider notify, ILogger log)
        {
            _log = log;
            _notify = notify;
            _settings = settings;
            _settings.SettingsChanged += settings_SettingsChanged;

            _url = GetUrl();

            _client = new HttpClient();
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.Timeout = TimeSpan.FromSeconds(c_TimeOut);
        }

        void settings_SettingsChanged(object sender, EventArgs e)
        {
            _url = GetUrl();
        }

        string GetUrl()
        {
            string httpPrefix = "http://";
            string httpsPrefix = "https://";
            string apiSufix = "/api";

            var a = _settings.Settings.ServerAdress;

            if (!a.StartsWith(httpPrefix) || !a.StartsWith(httpsPrefix))
                a = httpPrefix + a;

            if (!a.EndsWith(apiSufix))
                a = a + apiSufix;

            return a;
        }

        protected abstract StatusEnum ItemState { get; }
        public virtual async Task<RequestResultObject> SendRequestAsync(ItemModel item)
        {
            RequestResultObject resultObj = new RequestResultObject(item.Code);

            try
            {
                var updateItemObj = new UpdateItemDTO(item, ItemState);
                var req = await _client.PutAsJsonAsync(_url + "/Item", updateItemObj);

                resultObj = HandleResponse(req, item);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                _notify.NotifyError("Chyba při dotazu na server");
            }

            return resultObj;
        }

        protected RequestResultObject HandleResponse(HttpResponseMessage req, ItemModel item)
        {
            var resultObj = new RequestResultObject(item.Code);

            if (req.IsSuccessStatusCode)
            {
                resultObj.Result = true;
                return resultObj;
            }

            resultObj.Result = false;

            var msg = req.Content.ReadAsStringAsync().Result;
            _log.Error(msg);

            try
            {
                var jObj = JsonNode.Parse(msg).AsObject().ToDictionary(x => x.Key, x => x.Value.ToString());
                resultObj.ResultMessage = jObj["detail"];
            }
            catch
            {
                resultObj.ResultMessage = msg;
            }

            return resultObj;
        }
    }

    public static class HttpRequestServiceFactory
    {
        public static Type GetHttpRequestServiceType(ISettingsProvider settings)
        {
            switch (settings.Settings.AppMode)
            {
                case StatusEnum.Production:
                    return typeof(ProductionHttpRequestService);

                case StatusEnum.Tests:
                    return typeof(TestsHttpRequestService);

                case StatusEnum.Dispatched:
                    return typeof(DispatchedHttpRequestService);

                case StatusEnum.Complaint:
                    return typeof(ComplaintHttpRequestService);

                case StatusEnum.ComplaintToSupplier:
                    return typeof(ComplaintToSupplierHttpRequestService);

                default:
                    throw new NotImplementedException($"{settings.Settings.AppMode} service is not defined!");
            }
        }
    }
}