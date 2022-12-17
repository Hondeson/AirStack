using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
