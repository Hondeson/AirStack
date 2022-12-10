using AirStack.Client.Services.Navigation;
using AirStack.Client.ViewModel.Base;
using AirStack.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirStack.Client.ViewModel
{
    public class MainVM : BaseVM
    {
        public MainVM(INavigationService nav)
        {
            Navigation = nav;
        }

        public INavigationService Navigation { get; }
    }
}
