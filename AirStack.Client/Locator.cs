using AirStack.Client.Services.Navigation;
using AirStack.Client.Services.Settings;
using AirStack.Client.View;
using AirStack.Client.ViewModel;
using Autofac;
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
            SetupMainView(builder);

            builder.RegisterType<ScanCodeView>().AsSelf();
            builder.RegisterType<ScanCodeVM>().AsSelf();

            builder.RegisterType<NavigationService>().As<INavigationService>().SingleInstance();

            builder.RegisterType<SettingsView>().AsSelf();
            builder.RegisterType<SettingsVM>().AsSelf();
            builder.RegisterType<SettingsService>().As<ISettingsProvider>().SingleInstance();

            _ioc = builder.Build();
        }

        static void SetupMainView(ContainerBuilder builder)
        {
            builder.Register<MainView>((ioc) =>
            {
                return new MainView(ioc.Resolve<ISettingsProvider>())
                {
                    DataContext = ioc.Resolve<MainVM>()
                };
            }).AsSelf().SingleInstance();

            builder.Register<MainVM>((ioc) =>
            {
                var nav = ioc.Resolve<INavigationService>();
                nav.PushView(ioc.Resolve<ScanCodeView>());

                return new MainVM(nav);
            }).AsSelf().SingleInstance();
        }

        internal static T Resolve<T>()
        {
            return _ioc.Resolve<T>();
        }
    }
}
