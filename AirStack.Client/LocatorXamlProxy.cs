using System;
using System.Windows.Markup;

namespace AirStack.Client.ioc
{
    public class IocProxy : MarkupExtension
    {
        public Type Type { get; set; }
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Locator.Resolve(this.Type);
        }
    }
}
