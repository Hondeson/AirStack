using AirStack.Client.Model;
using AirStack.Client.Services.Notification;
using AirStack.Client.Services.Settings;
using AirStack.Core.Model;
using AirStack.Core.Model.API;
using Serilog;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
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
        protected const int c_TimeOut = 12;
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
        public virtual async Task<RequestResultObject> SendRequestAsync(string code)
        {
            RequestResultObject resultObj = new RequestResultObject(code);

            try
            {
                var updateItemObj = new UpdateItemDTO(code, ItemState);
                var req = await _client.PutAsJsonAsync(_url + "/Item", updateItemObj);

                resultObj = HandleResponse(req, code);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                _notify.NotifyError("Chyba při dotazu na server");
            }

            return resultObj;
        }

        protected RequestResultObject HandleResponse(HttpResponseMessage req, string input)
        {
            var resultObj = new RequestResultObject(input);

            if (req.IsSuccessStatusCode)
            {
                resultObj.Result = true;
                return resultObj;
            }

            resultObj.Result = false;
            var msg = req.Content.ReadAsStringAsync().Result;

            _log.Error($"RequestMode: {this.ItemState}, input: {input}");
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

            if (req.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                _notify.NotifyError(msg);

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