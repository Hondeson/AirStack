using AirStack.Client.Services.Navigation;
using AirStack.Client.Services.Notification;
using AirStack.Client.Services.RequestToServer;
using AirStack.Client.Services.RequestToServer.Http;
using AirStack.Client.Services.Settings;
using AirStack.Client.Services.UserInput;
using AirStack.Client.View;
using AirStack.Client.ViewModel;
using Autofac;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirStack.Client
{
    internal static class Locator
    {
        static IContainer _ioc;

        internal static void Initialize()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<MainView>().AsSelf().SingleInstance();
            builder.RegisterType<MainVM>().AsSelf().SingleInstance();

            builder.RegisterType<ScanCodeView>().AsSelf();
            builder.RegisterType<ScanCodeVM>().AsSelf();

            builder.RegisterType<NavigationService>().As<INavigationService>().SingleInstance();

            builder.RegisterType<SettingsView>().AsSelf();
            builder.RegisterType<SettingsVM>().AsSelf();
            builder.RegisterType<SettingsService>().As<ISettingsProvider>().SingleInstance();

            builder.RegisterType<COMService>().As<IUserInputProvider>().As<IDisposable>();
            builder.RegisterType<DisplayNotificationService>().As<INotificationProvider>().SingleInstance().As<IDisposable>().SingleInstance();

            SetupServerRequestServices(builder);

            SetupLogger(builder);

            _ioc = builder.Build();

            Locator.Resolve<ISettingsProvider>().Load();
            SetupMainView();
        }

        static void SetupServerRequestServices(ContainerBuilder builder)
        {
            builder.RegisterType<ProductionHttpRequestService>().AsSelf();
            builder.RegisterType<TestsHttpRequestService>().AsSelf();
            builder.RegisterType<DispatchedHttpRequestService>().AsSelf();
            builder.RegisterType<ComplaintHttpRequestService>().AsSelf();
            builder.RegisterType<ComplaintToSupplierHttpRequestService>().AsSelf();
            builder.RegisterType<MockServerRequestService>().AsSelf();

            builder.Register((ioc) =>
            {
                var type = HttpRequestServiceFactory.GetHttpRequestServiceType(ioc.Resolve<ISettingsProvider>());

                //return ioc.Resolve<MockServerRequestService>();

                return (IServerRequestService)ioc.Resolve(type);
            }).SingleInstance();
        }

        static void SetupLogger(ContainerBuilder builder)
        {
            builder.Register<ILogger>((ioc) =>
            {
                var config = new LoggerConfiguration();
                var path = Environment.CurrentDirectory + "/Logs/Log-.txt";
                config.WriteTo.File(path, rollingInterval: RollingInterval.Minute);

                return config.CreateLogger();
            }).SingleInstance();
        }

        static void SetupMainView()
        {
            var view = _ioc.Resolve<MainView>();
            var vm = _ioc.Resolve<MainVM>();

            view.DataContext = vm;
            view.Loaded += (s, e) =>
            {
                vm.Navigation.PushView(_ioc.Resolve<ScanCodeView>());
            };
        }

        internal static T Resolve<T>()
        {
            try
            {
                return _ioc.Resolve<T>();
            }
            catch (Exception ex)
            {
                var log = _ioc.Resolve<ILogger>();
                var notify = _ioc.Resolve<INotificationProvider>();

                log.Error(ex.Message);
                notify.NotifyError(ex.Message);


                throw ex;
            }
        }

        internal static object Resolve(Type type)
        {
            try
            {
                return _ioc.Resolve(type);
            }
            catch (Exception ex)
            {
                var log = _ioc.Resolve<ILogger>();
                var notify = _ioc.Resolve<INotificationProvider>();

                log.Error(ex.Message);
                notify.NotifyError(ex.Message);

                throw ex;
            }
        }
    }
}
